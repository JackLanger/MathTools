using System;
using System.Drawing;
using System.Windows.Controls;
using Size = System.Windows.Size;

namespace MathCanvas.Controller;

public class Paper
{
    public static void DrawGrid(PaperLayout layout, Canvas canvas, Size canvasSize, int scale,
        PointF? currentCenter)
    {

        if (currentCenter is null) return;
        switch (layout)
        {
            case PaperLayout.FIVE_MM_GRID:
                DrawFiveMMGrid(canvas, scale, canvasSize, currentCenter!.Value);
                break;
            case PaperLayout.ONE_MM_GRID:
                DrawOneMMGrid(canvas, scale, canvasSize, currentCenter!.Value);
                break;
            case PaperLayout.LINES:
                DrawLines(canvas, scale, canvasSize, currentCenter!.Value);
                break;
        }
    }

    private static void DrawLines(Canvas canvas, int scale, Size canvasSize, PointF currentCenter)
    {
        var height = (int) canvasSize.Height / 2;

        var cl = DrawUtils.Line(new PointF(0f, currentCenter.Y),
            new PointF((float) canvasSize.Width, currentCenter.Y), ColorConst.GRID_LINE);
        canvas.Children.Add(cl);
        var h = currentCenter.Y;
        while (h > 0)
        {
            var line = DrawUtils.Line(new PointF(0f, h),
                new PointF((float) canvasSize.Width, h),
                ColorConst.GRID_LINE
            );
            h -= scale;
            canvas.Children.Add(line);
        }

        h = currentCenter.Y + scale;
        while (h < canvasSize.Height)
        {
            var line = DrawUtils.Line(new PointF(0f, h),
                new PointF((float) canvasSize.Width, h),
                ColorConst.GRID_LINE
            );
            h += scale;
            canvas.Children.Add(line);
        }

    }

    private static void DrawOneMMGrid(Canvas canvas, int scale, Size canvasSize,
        PointF currentCenter)
    {

        throw new NotImplementedException();
    }

    private static void DrawFiveMMGrid(Canvas canvas, int scale, Size canvasSize,
        PointF currentCenter)
    {
        DrawLines(canvas, scale, canvasSize, currentCenter);

        var w = currentCenter.X;
        while (w > 0)
        {
            var line = DrawUtils.Line(new PointF(w, 0f),
                new PointF(w, (float) canvasSize.Height),
                ColorConst.GRID_LINE
            );
            w -= scale;
            canvas.Children.Add(line);
        }

        w = currentCenter.X + scale;
        while (w < canvasSize.Width)
        {
            var line = DrawUtils.Line(new PointF(w, 0f),
                new PointF(w, (float) canvasSize.Height),
                ColorConst.GRID_LINE
            );
            w += scale;
            canvas.Children.Add(line);
        }
    }
}