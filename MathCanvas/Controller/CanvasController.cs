using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MathCanvas.Core.Actions;
using Point = System.Drawing.Point;
using Size = System.Windows.Size;

namespace MathCanvas.Controller;

public class CanvasController : BaseCanvasController
{
    private readonly Canvas _canvas;

    private ICommand? _addPointCommand;

    private Size _canvasSize;

    private ICommand? _clearListCommand;

    private ObservableCollection<PointF> _points = new();

    private ICommand? _processCommand;

    private int _scaling = 20;

    private VisualContext _visualContext = VisualContext.D2;

    public CanvasController(Canvas canvas)
    {
        _canvas = canvas;
        _canvas.SizeChanged += UpdateCanvas;


        ClearCanvas();

    }

    public int Scale
    {
        get => _scaling;
        set => SetField(ref _scaling, value);
    }

    public VisualContext VisualContext
    {
        get => _visualContext;
        set => SetField(ref _visualContext, value);
    }

    public ObservableCollection<PointF> Points
    {
        get => _points;
        set => SetField(ref _points, value);
    }

    public ICommand AddPointCommand =>
        _addPointCommand ??= new ParameterizedCommand<string>(p => ParsePoints(p));

    public ICommand ClearListCommand =>
        _clearListCommand ??= new RelayCommand(() => Points.Clear());

    public ICommand ProcessCommand =>
        _processCommand ??= new RelayCommand(() => Process());

    /// <summary>
    ///     Takes A string of coordinates seperated by ';' and adds the points to the points list.
    /// </summary>
    /// <param name="s">the points to parse</param>
    private void ParsePoints(string s)
    {

        if (Equals(s, string.Empty)) return;
        switch (VisualContext)
        {
            case VisualContext.D2:
                ParsePoints2D(s);
                break;
            case VisualContext.D3:
                ParsePoints3D(s);
                break;
        }

        DrawPointsOnCanvas();
    }

    private void ParsePoints3D(string s)
    {
        throw new NotImplementedException();
    }

    private void ParsePoints2D(string s)
    {
        var pArr = s.Split(' ');
        foreach (var p in pArr)
        {
            var xy = p.Trim().Split(';');
            float x, y;
            float.TryParse(xy[0], out x);
            float.TryParse(xy[1], out y);
            Points.Add(new PointF(x, y));
        }
    }

    protected virtual void Process()
    {
        // Process the values
    }


    private void ClearCanvas()
    {
        _canvas.Children.Clear();
    }


    private void UpdateCanvas(object sender, SizeChangedEventArgs args)
    {
        ClearCanvas();
        _canvasSize = args.NewSize;
        Draw2DGrid(args.NewSize);
    }

    private void Draw2DGrid(Size size)
    {
        var pencil = new SolidColorBrush(Colors.SlateGray);
        var bottom = size.Height - CanvasOffsets.OffsetY;

        var left = CanvasOffsets.OffsetX;
        var right = size.Width - CanvasOffsets.OffsetX;
        var horizontal = Line(new Point(5, (int) bottom),
            new Point((int) right, (int) bottom),
            pencil);
        horizontal.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

        var vertical = Line(new Point(left, CanvasOffsets.OffsetY),
            new Point(left, (int) bottom + CanvasOffsets.OffsetX), pencil);
        vertical.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

        _canvas.Children.Add(horizontal);
        _canvas.Children.Add(vertical);
        // Draw indices for beter overview
        DrawIndices(size);

    }

    private void DrawIndices(Size size)
    {
        //todo implement
    }

    private void DrawPointsOnCanvas()
    {
        var drp = new DropShadowEffect();
        drp.Opacity = .2f;
        drp.ShadowDepth = 1;
        var pencil = new SolidColorBrush(Colors.DarkSlateGray);
        foreach (var p in Points)
        {

            var point = Point(5, MultPoint(p, _scaling), pencil, _canvasSize);
            point.Stroke = pencil;
            point.Effect = drp;
            point.Fill = pencil;
            point.Focusable = true;
            point.ToolTip = $"P{_points.IndexOf(p)}(x: {p.X} | y: {p.Y})";
            // todo subscribe to events.

            _canvas.Children.Add(point);
        }
    }


    private static Point MultPoint(PointF p, double val) =>
        new((int) (p.X * val), (int) (p.Y * val));
}