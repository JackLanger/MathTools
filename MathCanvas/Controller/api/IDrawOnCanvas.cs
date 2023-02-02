using System.Drawing;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace MathCanvas.Controller.Helpers;

public interface IDrawOnCanvas
{
    /// <summary>
    ///     Draws a point at the desired position of a given size and color.
    /// </summary>
    /// <param name="radius"> the radius of the point</param>
    /// <param name="point">the Location of the point</param>
    /// <param name="fill">the fill color of the point</param>
    /// <param name="canvasSize"></param>
    /// <returns>a Point at a given location</returns>
    Ellipse Point(double radius, Point point, SolidColorBrush? fill = null,
        Size? canvasSize = null);

    /// <summary>
    ///     Shorthand method to draw a line from P1 to P2 with a given thickness and a given color.
    /// </summary>
    /// <param name="p1">Point A</param>
    /// <param name="p2">Point B</param>
    /// <param name="colorBrush">The color of the line</param>
    /// <param name="thickness">Thickness of the Line</param>
    /// <returns>A line from Point A to Point B</returns>
    Line Line(Point p1, Point p2,
        SolidColorBrush? colorBrush = null, double thickness = 1.0);
}