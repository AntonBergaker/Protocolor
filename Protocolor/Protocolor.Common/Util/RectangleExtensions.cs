using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolor.Util;
public static class RectangleExtensions {

    public static Rectangle UnionAll(this IEnumerable<Rectangle> rectangles) {
        using var enumerator = rectangles.GetEnumerator();

        if (enumerator.MoveNext() == false) {
            throw new Exception("Enumerable must be at least one element");
        }

        (int x0, int y0, int x1, int y1) = enumerator.Current;

        while (enumerator.MoveNext()) {
            Rectangle current = enumerator.Current;
            x0 = Math.Min(x0, current.X0);
            y0 = Math.Min(y0, current.Y0);

            x1 = Math.Max(x1, current.X1);
            y1 = Math.Max(y1, current.Y1);
        }

        return new Rectangle(x0, y0, x1, y1);
    }

}
