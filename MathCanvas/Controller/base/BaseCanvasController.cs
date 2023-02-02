using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MathCanvas.Controller.Helpers;
using Point = System.Drawing.Point;
using Size = System.Windows.Size;

namespace MathCanvas.Controller;

public abstract class BaseCanvasController : BaseController, IDrawOnCanvas
{
    public Ellipse Point(double radius, Point point, SolidColorBrush? fill = null,
        Size? canvasSize = null)
    {
        var size = canvasSize ?? new Size(0, 0);

        var ellipse = new Ellipse {Width = radius, Height = radius, Fill = fill ?? null};
        var left = point.X - radius / 2;
        var top = point.Y - radius / 2;

        ellipse.Margin = new Thickness(left + CanvasOffsets.OffsetX,
            size.Height - CanvasOffsets.OffsetY - top, 0, 0);
        return ellipse;
    }

    public Line Line(Point p1, Point p2, SolidColorBrush? colorBrush = null,
        double thickness = 1) =>
        new()
        {
            X1 = p1.X, X2 = p2.X, Y1 = p1.Y, Y2 = p2.Y,
            StrokeThickness = thickness,
            SnapsToDevicePixels = true,
            Stroke = colorBrush ?? null
        };
}