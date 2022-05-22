using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolor;
using Protocolor.Util;

namespace UnitTests;
public static partial class TestingUtil {


    private static readonly TwoWayDictionary<PaletteColor, byte> paletteToByte = new() {
        { PaletteColor.Gray, 0 },
        { PaletteColor.DarkRed, 1 },
        { PaletteColor.Lavender, 2 },
        { PaletteColor.Orange, 3 },
        { PaletteColor.Yellow, 4 },
        { PaletteColor.Green, 5 },
        { PaletteColor.LightBlue, 6 },
        { PaletteColor.Blue, 7 },
        { PaletteColor.Purple, 8 },
        { PaletteColor.LightGray, 9 },
        { PaletteColor.Brown, 10 },
        { PaletteColor.Pink, 11 },
        { PaletteColor.Gold, 12 },
        { PaletteColor.LightYellow, 13 },
        { PaletteColor.Lime, 14 },
        { PaletteColor.LightCyan, 15 },
    };



    /// <summary>
    /// Creates a frame based on the words in the string using a the image as a binary encoding of sorts
    /// </summary>
    /// <returns></returns>
    public static IdentifierFrame StringToBinaryFrame(string @str) {
        int height = 4;
        int width = (1+str.Length)/2;

        Grid<PaletteColor> grid = new(width, height);

        int i = 0;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y+=2) {
                if (i >= str.Length) {
                    grid[x, y] = PaletteColor.White;
                    grid[x, y+1] = PaletteColor.White;
                    break;
                }
                byte c = (byte)str[i++];
                byte b0 = (byte)(c & 0xF);
                byte b1 = (byte)((c>>4) & 0XF);

                grid[x, y] = paletteToByte[b0];
                grid[x, y+1] = paletteToByte[b1];
            }
        }

        return new IdentifierFrame(grid);
    }

    /// <summary>
    /// Reads the contents of the frame as a string using the colors as bytes
    /// </summary>
    /// <param name="frame"></param>
    /// <returns></returns>
    public static string BinaryFrameToString(IdentifierFrame frame) {
        StringBuilder sb = new StringBuilder();

        for (int x = 0; x < frame.Width; x++) {
            for (int y = 0; y < frame.Height; y+=2) {
                if (frame[x, y] == PaletteColor.White) {
                    continue;
                }

                byte b0 = paletteToByte[frame[x, y]];
                byte b1 = paletteToByte[frame[x, y+1]];

                char c = (char)(b0 | (b1<<4) );
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

}
