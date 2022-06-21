namespace Protocolor.Util; 

public struct Point {
    public readonly int X;
    public readonly int Y;
    public Point(int x, int y) {
        X = x;
        Y = y;
    }

    public static Point operator +(Point lhs, Point rhs) {
        return new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);
    }

    public static Point operator -(Point lhs, Point rhs) {
        return new Point(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static Point operator *(Point lhs, int rhs) {
        return new Point(lhs.X * rhs, lhs.Y * rhs);
    }

    public static Point operator *(int lhs, Point rhs) {
        return new Point(rhs.X * lhs, rhs.Y * lhs);
    }


    public override string ToString() {
        return $"({X}, {Y})";
    }
}