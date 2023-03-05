using System.Windows.Controls;
using MathTools;

namespace MathCanvas.Controller;

public class ProcessingCanvasController : CanvasController
{
    public ProcessingCanvasController(Canvas canvas) : base(canvas)
    {
    }

    protected override double[][] InterpolateFunction()
    {
        var funcs = new double[Points.Count - 4][];
        if (Points.Count < 3) return funcs;
        var m = new Matrix(Points.Count - 3, Points.Count - 3);
        for (var i = 1; i < Points.Count - 2; i++)
        {
            m[i - 1, i - 1] = 2 * (Points[i + 1].X - Points[i - 1].X);

            if (i > 1)
                m[i - 1, i - 2] = Points[i].X - Points[i - 1].X;
            if (i < Points.Count - 3)
                m[i - 1, i] = Points[i + 1].X - Points[i].X;
        }

        for (var i = 0; i < funcs.Length; i++)
        {
            funcs[i] = new double[4];

            funcs[i][0] = (m[i + 1, i] - m[i, i]) / 3 * (Points[i + 1].X - Points[i].X);
            funcs[i][1] = m[i, i];
            funcs[i][2] = (Points[i + 1].Y - Points[i].Y) / (Points[i + 1].X - Points[i].X)
                          - 1 / 3.0 * (m[i + 1, i] - m[i, i]) * (Points[i + 1].X - Points[i].X)
                          - m[i, i] * (Points[i + 1].X - Points[i].X);
            funcs[i][3] = Points[i].Y;

        }

        return funcs;
    }
}