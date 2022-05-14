using System.Diagnostics.CodeAnalysis;
using System.Text;
using Protocolor.Util;

namespace Protocolor;
public class IdentifierFrame : IEquatable<IdentifierFrame> {
    private readonly Grid<PaletteColor> grid;

    public IdentifierFrame(Grid<PaletteColor> grid) {
        this.grid = grid;
    }

    public static bool TryParseFromRaw(Rectangle position, Grid<RawColor> image, [NotNullWhen(true)] out IdentifierFrame? identifier) {
        Grid<PaletteColor> colors = new Grid<PaletteColor>(position.Width, position.Height);

        for (int x = position.X0; x <= position.X1; x++) {
            for (int y = position.Y0; y <= position.Y1; y++) {

                if (PaletteColor.TryFromRaw(image[x, y], out PaletteColor color) == false) {
                    identifier = null;
                    return false;
                }

                colors[x - position.X0, y - position.Y0] = color;
            }
        }

        identifier = new IdentifierFrame(colors);
        return true;
    }

    public static IdentifierFrame ParseFromRaw(Rectangle position, Grid<RawColor> image) {
        if (TryParseFromRaw(position, image, out var frame) == false) {
            throw new Exception("Failed to parse frame");
        }

        return frame;
    }

    private int? cachedHash;

    public override int GetHashCode() {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        if (cachedHash != null) {
            return cachedHash.Value;
        }

        HashCode code = new HashCode();
        code.Add(grid.Width);
        code.Add(grid.Height);
        
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                code.Add(grid[x, y]);
            }
        }

        cachedHash = code.ToHashCode();
        return cachedHash.Value;
        // ReSharper enable NonReadonlyMemberInGetHashCode
    }

    public bool Equals(IdentifierFrame? other) {
        if (other == null) {
            return false;
        }

        if (other.grid.Width != grid.Width || other.grid.Height != grid.Height) {
            return false;
        }

        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                if (other.grid[x, y] != grid[x, y]) {
                    return false;
                }
            }
        }

        return true;
    }

    public override bool Equals(object? obj) {
        if (obj is IdentifierFrame frame == false) {
            return false;
        }

        return Equals(frame);
    }

    public override string ToString() {
        // Most places this is visible is in debug screens, so replace newline with a | for better readability in tests.
        return Utils.FrameToString(grid, "|");
    }

    public string ToString(string newlineChar) {
        return Utils.FrameToString(grid, newlineChar);
    }
}
