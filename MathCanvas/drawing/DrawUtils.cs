using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MathCanvas.Controller;

public struct DrawUtils
{
    /// <summary>
    ///     Return a point to draw on the Canvas at a given x and y location
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="point"></param>
    /// <param name="fill"></param>
    /// <returns></returns>
    public static Ellipse Point(double radius, PointF point, SolidColorBrush? fill = null)
    {
        var ellipse = new Ellipse {Width = radius, Height = radius, Fill = fill ?? null};
        var left = point.X - radius / 2;
        var top = point.Y - radius / 2;

        ellipse.Margin = new Thickness(left, top, 0, 0);
        return ellipse;
    }

    /// <summary>
    ///     Draw a line From A to B.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="colorBrush"></param>
    /// <param name="thickness"></param>
    /// <returns></returns>
    public static Line Line(PointF p1, PointF p2,
        SolidColorBrush? colorBrush = null,
        double thickness = 1)
    {

        var line = new Line
        {
            X1 = p1.X, X2 = p2.X, Y1 = p1.Y, Y2 = p2.Y,
            StrokeThickness = thickness,
            SnapsToDevicePixels = true,
            Stroke = colorBrush ?? null
        };
        line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

        return line;
    }
}