using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MarkItDownDesktop.Models;

namespace MarkItDownDesktop.Services;

public static class AppDataService
{
    private static readonly string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MarkItDownDesktop");
    private static readonly string SettingsPath = Path.Combine(DataDirectory, "settings.json");
    private static readonly string HistoryPath = Path.Combine(DataDirectory, "history.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static AppSettings LoadSettings() => Load(SettingsPath, new AppSettings());
    public static List<ConversionRecord> LoadHistory() => Load(HistoryPath, new List<ConversionRecord>());
    public static void SaveSettings(AppSettings settings) => Save(SettingsPath, settings);
    public static void SaveHistory(List<ConversionRecord> history) => Save(HistoryPath, history);

    private static T Load<T>(string path, T fallback)
    {
        try { return File.Exists(path) ? JsonSerializer.Deserialize<T>(File.ReadAllText(path)) ?? fallback : fallback; }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException) { return fallback; }
    }

    private static void Save<T>(string path, T value)
    {
        Directory.CreateDirectory(DataDirectory);
        var temp = path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(value, JsonOptions));
        File.Move(temp, path, true);
    }
}
