using System.Windows.Controls;

namespace MathCanvas.Controller;

public class ProcessingCanvasController : CanvasController
{
    public ProcessingCanvasController(Canvas canvas) : base(canvas)
    {
    }

    protected override void Process()
    {
        // Your implementation goes here
        // Points are being hold in the field Points
    }
}