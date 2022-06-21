namespace Protocolor.Util;

public struct PaletteColor {
    public static PaletteColor White = new(Palette.White);
    public static PaletteColor Gray = new(Palette.Gray);
    public static PaletteColor DarkRed = new(Palette.DarkRed);
    public static PaletteColor Red = new(Palette.Red);
    public static PaletteColor Orange = new(Palette.Orange);
    public static PaletteColor Yellow = new(Palette.Yellow);
    public static PaletteColor Green = new(Palette.Green);
    public static PaletteColor Cyan = new(Palette.Cyan);
    public static PaletteColor Blue = new(Palette.Blue);
    public static PaletteColor Purple = new(Palette.Purple);
    public static PaletteColor Black = new(Palette.Black);
    public static PaletteColor LightGray = new(Palette.LightGray);
    public static PaletteColor Brown = new(Palette.Brown);
    public static PaletteColor Pink = new(Palette.Pink);
    public static PaletteColor Gold = new(Palette.Gold);
    public static PaletteColor LightYellow = new(Palette.LightYellow);
    public static PaletteColor Lime = new(Palette.GreenYellow);
    public static PaletteColor LightCyan = new(Palette.LightCyan);
    public static PaletteColor LightBlue = new(Palette.LightBlue);
    public static PaletteColor Lavender = new(Palette.Lavender);

    private enum Palette : byte {
        White,
        Gray,
        DarkRed,
        Red,
        Orange,
        Yellow,
        Green,
        Cyan,
        Blue,
        Purple,
        Black,
        LightGray,
        Brown,
        Pink,
        Gold,
        LightYellow,
        GreenYellow,
        LightCyan,
        LightBlue,
        Lavender,
    }

    static PaletteColor() {
        PaletteMap = new() {
            { 0xFF000000, Palette.Black },
            { 0xFF7F7F7F, Palette.Gray },
            { 0xFF880015, Palette.DarkRed },
            { 0xFFED1C24, Palette.Red },
            { 0xFFFF7F27, Palette.Orange },
            { 0xFFFFF200, Palette.Yellow },
            { 0xFF22B14C, Palette.Green },
            { 0xFF00A2E8, Palette.Cyan },
            { 0xFF3F48CC, Palette.Blue },
            { 0xFFA349A4, Palette.Purple },
            { 0xFFFFFFFF, Palette.White },
            { 0xFFC3C3C3, Palette.LightGray },
            { 0xFFB97A57, Palette.Brown },
            { 0xFFFFAEC9, Palette.Pink },
            { 0xFFFFC90E, Palette.Gold },
            { 0xFFEFE4B0, Palette.LightYellow },
            { 0xFFB5E61D, Palette.GreenYellow },
            { 0xFF99D9EA, Palette.LightCyan },
            { 0xFF7092BE, Palette.LightBlue },
            { 0xFFC8BFE7, Palette.Lavender },
        };
    }

    private static readonly TwoWayDictionary<uint, Palette> PaletteMap;

    private readonly Palette paletteEntry;

    public RawColor Color => new RawColor(PaletteMap[paletteEntry]);

    private PaletteColor(Palette paletteEntry) {
        this.paletteEntry = paletteEntry;
    }

    public static bool TryFromRaw(RawColor rawColor, out PaletteColor color) {
        if (PaletteMap.TryGetValue(rawColor.Value, out var palette)) {
            color = new PaletteColor(palette);
            return true;
        }

        color = default;
        return false;
    }

    public static bool operator ==(PaletteColor lhs, PaletteColor rhs) {
        return lhs.paletteEntry == rhs.paletteEntry;
    }

    public static bool operator !=(PaletteColor lhs, PaletteColor rhs) {
        return lhs.paletteEntry != rhs.paletteEntry;
    }

    public bool Equals(PaletteColor other) {
        return paletteEntry == other.paletteEntry;
    }

    public override bool Equals(object? obj) {
        return obj is PaletteColor other && Equals(other);
    }

    public override int GetHashCode() {
        return paletteEntry.GetHashCode();
    }

    public override string ToString() {
        return paletteEntry.ToString();
    }
}
