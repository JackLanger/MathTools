using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using MathCanvas.Core.Actions;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace MathCanvas.Controller;

public abstract class CanvasController : BaseController
{
    public CanvasController(Canvas canvas)
    {
        _canvas = canvas;
        _canvas.SizeChanged += UpdateCanvas;
        _canvas.MouseDown += CaptureMouseDown;
        _canvas.MouseWheel += OnZoom;

        ClearCanvas();
    }

    private void OnZoom(object sender, MouseWheelEventArgs e)
    {
        
        var d = e.Delta;

        var zoom = 1+ d / 500f;
        Zoom += zoom;
        Scale = Scale < 5? 5:Scale*zoom;

        RefreshCanvas();
    }


    private void CaptureMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            // move canvas in 2d / 3d rotate around axis...
            // // TODO: refuses to let go of mouse move event
            _canvas.MouseMove += UpdateOffset;
            _canvas.MouseUp += (_, _) => { _canvas.MouseMove -= UpdateOffset; };
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            var pressedPos = e.GetPosition(_canvas);
            // create a new point
            _canvas.MouseUp += (s, evt) =>
            {
                CreateNewPoint(pressedPos, evt.GetPosition(_canvas));
            };
        }
        // do context menu stuff here.
    }

    private void CreateNewPoint(Point pressedPos, Point getPosition)
    {

    }

    private void UpdateOffset(object sender, MouseEventArgs e)
    {

        // TODO position.
        if (_currentCenter is null) return;

        
        var currentMousePos = e.GetPosition(_canvas);
        var offX = currentMousePos.X - _currentCenter?.X;
        var offY = currentMousePos.Y - _currentCenter?.Y;

        _currentCenter = new PointF((float) (_currentCenter?.X + offX)!,
            (float) (_currentCenter?.Y + offY)!);
        RefreshCanvas();

    }

    private PointF Offset(PointF target, PointF offset) =>
        new(target.X + offset.X, target.Y + offset.Y);

    /// <summary>
    ///     Takes A string of coordinates seperated by ';' and adds the points to the points list.
    /// </summary>
    /// <param name="s">the points to parse</param>
    private void ParsePoints()
    {
        if (Equals(PointsString, string.Empty)) return;
        ParsePoints2D(PointsString);
        DrawPointsOnCanvas();

        PointsString = string.Empty;
    }


    public void ParsePoints2D(string s)
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

    /// <summary>
    ///     Process the points in the array Points.
    /// </summary>
    protected abstract void Process();


    private void ClearCanvas()
    {
        if (_canvas.Children.Count == 0) return;
        
        _canvas.Children.Clear();
        GC.Collect();
    }

    private void RefreshCanvas()
    {
            ClearCanvas();
            Draw2DCoords();
            DrawPointsOnCanvas();
    }

    private void UpdateCanvas(object sender, SizeChangedEventArgs args)
    {
        ClearCanvas();
        _canvasSize = args.NewSize;
        _currentCenter = new PointF((float) _canvasSize.Width / 2, (float) _canvasSize.Height / 2);
        if (_offset is not null) Offset(_currentCenter!.Value, _offset!.Value);
        Draw2DCoords();
    }

    private void Draw2DCoords()
    {
        if (_currentCenter is null) return;
        if (_offset is not null)
            _currentCenter = Offset(_currentCenter.Value, _offset.Value);

        Paper.DrawGrid(PaperLayout.FIVE_MM_GRID, _canvas, _canvasSize, Scale, _currentCenter);

        var pointLx = new PointF(CanvasOffsets.OffsetX, (float) _currentCenter?.Y!);
        var pointRx = new PointF((float) (_canvasSize.Width - CanvasOffsets.OffsetX),
            (float) _currentCenter?.Y!);

        var pointTy = new PointF((float) _currentCenter?.X!, CanvasOffsets.OffsetY);
        var pointTb = new PointF((float) _currentCenter?.X!,
            (float) _canvasSize.Height - CanvasOffsets.OffsetY);

        var horizontal = DrawUtils.Line(pointLx, pointRx, ColorConst.PENCIL, 1.1);

        var vertical = DrawUtils.Line(pointTy, pointTb, ColorConst.PENCIL, 1.1);

        _canvas.Children.Add(horizontal);
        _canvas.Children.Add(vertical);
        // Draw indices for beter overview
        DrawIndices();

    }

    private void DrawIndices()
    {

    }

    private void DrawPointsOnCanvas()
    {
        var drp = new DropShadowEffect();
        drp.Opacity = .2f;
        drp.ShadowDepth = 1;
        foreach (var p in Points)
        {
            var point = Point(3, p, ColorConst.DARK_PENCIL);
            point.Stroke = ColorConst.DARK_PENCIL;
            point.Effect = drp;
            point.Fill = ColorConst.DARK_PENCIL;
            point.Focusable = true;
            point.ToolTip = $"P{_points.IndexOf(p)}(x: {p.X} | y: {p.Y})";
            // todo subscribe to events.

            _canvas.Children.Add(point);
        }
    }

    private Ellipse Point(double radius, PointF point, SolidColorBrush? fill = null)
    {
        if (_currentCenter is null) return DrawUtils.Point(radius, point, fill);

        var ps = MultPoint(point, Scale);
        var canvasPoint = new PointF((float) (_currentCenter?.X + ps.X)!,
            (float) (_currentCenter?.Y - ps.Y)!);

        return DrawUtils.Point(radius, canvasPoint, fill);
    }


    private static PointF MultPoint(PointF p, double val) =>
        new((int) (p.X * val), (int) (p.Y * val));

    #region PRIVATE FIELDS

    private readonly Canvas _canvas;

    private ICommand? _addPointCommand;

    private Size _canvasSize;

    private ICommand? _clearListCommand;

    private PointF? _currentCenter;

    private PointF? _offset;

    private ObservableCollection<PointF> _points = new();

    private ICommand? _processCommand;

    private float _scaling = 20;

    private string _pointsString;

    #endregion


    #region PROPERTY GETTERS

    public string PointsString
    {
        get => _pointsString;
        set => SetField(ref _pointsString, value);
    }

    public float Zoom { get; set; } = 1.0F;

    public float Scale
    {
        get => _scaling;
        set => SetField(ref _scaling, value);
    }

    public ObservableCollection<PointF> Points
    {
        get => _points;
        set => SetField(ref _points, value);
    }

    public ICommand AddPointCommand =>
        _addPointCommand ??= new RelayCommand(() => { ParsePoints(); });

    public ICommand ClearListCommand =>
        _clearListCommand ??= new RelayCommand(() =>
        {
            Points.Clear();
            PointsString = string.Empty;
            ClearCanvas();
            Draw2DCoords();
        });

    public ICommand ProcessCommand =>
        _processCommand ??= new RelayCommand(() => Process());

    #endregion
}