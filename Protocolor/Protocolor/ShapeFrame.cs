using System.Diagnostics.CodeAnalysis;
using System.Text;
using Protocolor.Util;

namespace Protocolor;
public class ShapeFrame : IEquatable<ShapeFrame> {
    private readonly Grid<bool> grid;

    public ShapeFrame(Grid<bool> grid) {
        this.grid = grid;
    }

    public static ShapeFrame ParseFromRaw(Rectangle position, Grid<RawColor> image) {
        Grid<bool> shape = new Grid<bool>(position.Width, position.Height);

        for (int x = position.X0; x <= position.X1; x++) {
            for (int y = position.Y0; y <= position.Y1; y++) {
                if (image[x, y] != PaletteColor.White.Color) {
                    shape[x - position.X0, y - position.Y0] = true;
                }
            }
        }

        return new ShapeFrame(shape);
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

    public bool Equals(ShapeFrame? other) {
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
        if (obj is ShapeFrame frame == false) {
            return false;
        }

        return Equals(frame);
    }

    private string? cachedString;

    public override string ToString() {
        if (cachedString != null) {
            return cachedString;
        }

        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < grid.Height; y++) {
            for (int x = 0; x < grid.Width; x++) {
                sb.Append(grid[x, y] ? 'X' : ' ');
            }
            sb.Append('\n');
        }

        cachedString = sb.ToString();
        return cachedString;
    }
}
