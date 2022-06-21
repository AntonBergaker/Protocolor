namespace Protocolor.Util;

public readonly struct RawColor {
    public readonly uint Value;
    public RawColor(uint value) {
        Value = value;
    }

    public static bool operator==(RawColor lhs, RawColor rhs) {
        return lhs.Value == rhs.Value;
    }

    public static bool operator !=(RawColor lhs, RawColor rhs) {
        return lhs.Value != rhs.Value;
    }

    public bool Equals(RawColor other) {
        return Value == other.Value;
    }

    public override bool Equals(object? obj) {
        return obj is RawColor other && Equals(other);
    }

    public override int GetHashCode() {
        return (int)Value;
    }

    public override string ToString() {
        return "#"+Value.ToString("X8");
    }
}
