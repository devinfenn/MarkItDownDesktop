"""Generate the MarkItDownDesktop icon and WinUI image assets."""

from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "MarkItDownDesktop" / "Assets"
INK = "#181D2B"
WHITE = "#EFEFFF"
ACCENT = "#8269F4"


def icon(size: int, background: bool = True) -> Image.Image:
    scale = 4
    canvas = Image.new("RGBA", (size * scale, size * scale), (0, 0, 0, 0))
    draw = ImageDraw.Draw(canvas)

    def point(x, y):
        return (round(x * size * scale / 256), round(y * size * scale / 256))

    def box(x1, y1, x2, y2):
        return (*point(x1, y1), *point(x2, y2))

    if background:
        draw.rounded_rectangle(box(6, 6, 250, 250), radius=round(51 * size * scale / 256), fill=INK)
    draw.rounded_rectangle(box(56, 74, 163, 181), radius=round(26 * size * scale / 256), fill=ACCENT)
    draw.rounded_rectangle(box(93, 74, 200, 181), radius=round(26 * size * scale / 256), fill=WHITE)
    draw.rounded_rectangle(box(120, 101, 173, 154), radius=round(15 * size * scale / 256), fill=INK)
    return canvas.resize((size, size), Image.Resampling.LANCZOS)


def centered(width: int, height: int, icon_size: int) -> Image.Image:
    image = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    image.alpha_composite(icon(icon_size), ((width - icon_size) // 2, (height - icon_size) // 2))
    return image


ASSETS.mkdir(parents=True, exist_ok=True)
icon(256).save(ASSETS / "AppIcon.ico", format="ICO", sizes=[(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)])
icon(256).save(ASSETS / "AppIcon-preview.png")
icon(300).save(ASSETS / "Square150x150Logo.scale-200.png")
icon(88).save(ASSETS / "Square44x44Logo.scale-200.png")
icon(24).save(ASSETS / "Square44x44Logo.targetsize-24_altform-unplated.png")
icon(50).save(ASSETS / "StoreLogo.png")
icon(48).save(ASSETS / "LockScreenLogo.scale-200.png")
centered(620, 300, 190).save(ASSETS / "Wide310x150Logo.scale-200.png")
centered(1240, 600, 260).save(ASSETS / "SplashScreen.scale-200.png")
