using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Protocolor.Util;
public static class Utils {


    private static Dictionary<char, PaletteColor> charToColor = new() {
        { 'X', PaletteColor.Black },
        { 'G', PaletteColor.Gray },
        { 'r', PaletteColor.DarkRed },
        { 'R', PaletteColor.Red },
        { 'O', PaletteColor.Orange },
        { 'Y', PaletteColor.Yellow },
        { 'g', PaletteColor.Green },
        { 'C', PaletteColor.Cyan },
        { 'B', PaletteColor.Blue },
        { 'P', PaletteColor.Purple },
        { ' ', PaletteColor.White },
        { 'w', PaletteColor.LightGray }, //light white, clearly
        { 'S', PaletteColor.Brown }, // For sepia
        { 'p', PaletteColor.Pink },
        { 'a', PaletteColor.Gold }, // for Au, bear with me
        { 'y', PaletteColor.LightYellow },
        { 'l', PaletteColor.Lime },
        { 'c', PaletteColor.LightCyan },
        { 'b', PaletteColor.LightBlue },
        { 'L', PaletteColor.Lavender },
    };

    public static IdentifierFrame StringToFrame(params string[] strings) {
        int width = strings.Select(x => x.Length).Max();
        int height = strings.Length;

        Grid<PaletteColor> grid = new(width, height);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (x >= strings[y].Length) {
                    grid[x, y] = PaletteColor.White;
                }
                else {
                    grid[x, y] = charToColor[strings[y][x]];
                }

            }
        }

        return new IdentifierFrame(grid);
    }
    
    public static IdentifierFrame StringToFrame(string input) {
        return StringToFrame(input.Split("\n", StringSplitOptions.RemoveEmptyEntries));
    }

    public static Grid<RawColor> ImageToArray(Image<Bgra32> image) {
        int width = image.Width;
        int height = image.Height;
        Grid<RawColor> pixels = new(width, height);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                pixels[x, y] = new RawColor(image[x, y].Bgra);
            }
        }
        return pixels;
    }
}
