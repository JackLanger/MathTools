using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MathCanvas.Core.Actions;

namespace MathCanvas.Controller;

public class ProcessingCanvasController : CanvasController
{
    private ICommand? _processCommand;

    public ProcessingCanvasController(Canvas canvas) : base(canvas)
    {
    }

    public ICommand ProcessCommand =>
        _processCommand ??= new RelayCommand(() => Process(ref _canvas));

    protected override void Process(ref Canvas canv)
    {
        // Your implementation goes here
        // Points are being hold in the field Points

        for (var i = 1; i < Points.Count; i++)
        {

            var p1 = Points[i - 1];
            var p2 = Points[i];

            //#2486d5
            var line = DrawUtils.Line(p1, p2, new SolidColorBrush(Colors.Blue));
            canv.Children.Add(line);
        }

        RefreshCanvas();
    }
}