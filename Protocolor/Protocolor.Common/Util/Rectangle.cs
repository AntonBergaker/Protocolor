namespace Protocolor.Util; 

public readonly struct Rectangle {
    public readonly Point Point0;
    public readonly Point Point1;

    public int X0 => Point0.X;
    public int Y0 => Point0.Y;
    public int X1 => Point1.X;
    public int Y1 => Point1.Y;
    
    public int Width => X1 - X0 + 1;
    public int Height => Y1 - Y0 + 1;

    public static Rectangle Zero => new Rectangle(0, 0, 0, 0);


    public Rectangle(Point point0, Point point1) {
        Point0 = point0;
        Point1 = point1;

        if (point1.Y < point0.Y) {
            throw new ArgumentException("Y1 can not be less than Y0");
        }

        if (point1.X < point0.X) {
            throw new ArgumentException("X1 can not be less than X0");
        }
    }

    public Rectangle(int x0, int y0, int x1, int y1) : this(new Point(x0, y0), new Point(x1, y1)) { }

    public static Rectangle FromUnorderedPositions(int x0, int y0, int x1, int y1) {
        if (x1 < x0) {
            (x1, x0) = (x0, x1);
        }

        if (y1 < y0) {
            (y1, y0) = (y0, y1);
        }

        return new Rectangle(x0, y0, x1, y1);
    }

    public static Rectangle Union(Rectangle a, Rectangle b) {
        int x0 = Math.Min(a.X0, b.X0);
        int y0 = Math.Min(a.Y0, b.Y0);
        
        int x1 = Math.Max(a.X1, b.X1);
        int y1 = Math.Max(a.Y1, b.Y1);

        return new Rectangle(x0, y0, x1, y1);
    }

    public static Rectangle Union(Rectangle a, Rectangle b, Rectangle c) {
        int x0 = Math.Min(a.X0, Math.Min(b.X0, c.X0));
        int y0 = Math.Min(a.Y0, Math.Min(b.Y0, c.Y0));

        int x1 = Math.Max(a.X1, Math.Max(b.X1, c.X1));
        int y1 = Math.Max(a.Y1, Math.Max(b.Y1, c.Y1));

        return new Rectangle(x0, y0, x1, y1);
    }

    public static Rectangle Union(Rectangle a, Rectangle b, Rectangle c, Rectangle d) {
        int x0 = Math.Min(Math.Min(a.X0, b.X0), Math.Min(c.X0, d.X0));
        int y0 = Math.Min(Math.Min(a.Y0, b.X0), Math.Min(c.Y0, d.Y0));

        int x1 = Math.Max(Math.Max(a.X1, b.X1), Math.Max(c.X1, d.X1));
        int y1 = Math.Max(Math.Max(a.Y1, b.Y1), Math.Max(c.Y1, d.Y1));

        return new Rectangle(x0, y0, x1, y1);
    }

    public bool Contains(Point point) {
        return point.X >= X0 && point.X <= X1 && point.Y >= Y0 && point.Y <= Y1;
    }

    public override string ToString() {
        return $"[{Point0} - {Point1}]";
    }

    public void Deconstruct(out int x0, out int y0, out int x1, out int y1) {
        x0 = X0;
        y0 = Y0;
        x1 = X1;
        y1 = Y1;
    }
}