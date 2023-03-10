using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using MathCanvas.Core.Actions;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace MathCanvas.Controller;

public abstract class CanvasController : BaseController
{
    private double IndexFontSize = 10;

    public CanvasController(Canvas canvas)
    {
        _canvas = canvas;
        _canvas.SizeChanged += UpdateCanvas;
        _canvas.MouseDown += CaptureMouseDown;
        _canvas.MouseWheel += OnZoom;

        ClearCanvas();
    }

    private void CreateNewPoint(Point pressedPos, Point getPosition)
    {
        if (Math.Abs(pressedPos.Y - getPosition.Y) < 1
            && Math.Abs(pressedPos.X - getPosition.X) < 1)
        {
            var y = (float) (_currentCenter?.Y - getPosition.Y)!;
            var x = (float) (getPosition.X - _currentCenter?.X)!;
            y /= Scale;
            x /= Scale;
            Points.Add(new PointF(x, y));
            SortPoints();
            RefreshCanvas();
        }
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

        SortPoints();
    }

    private void SortPoints()
    {
        Points = new ObservableCollection<PointF>(Points.OrderBy(f => f.X));

    }

    /**
     * Interpolate a function and return the coefficients in an array.
     */
    protected abstract double[][] InterpolateFunction();

    /// <summary>
    ///     Process the points in the array Points.
    /// </summary>
    private void Process()
    {

        var arr = InterpolateFunction();

        ICollection<PointF> draw = new List<PointF>();


        if (_points.Count % 2 != 0)
        {
            var last = Points[Points.Count - 1];
            var sndLast = Points[Points.Count - 2];

            var mirror = new PointF(last.X - sndLast.X, sndLast.Y);
            Points.Add(mirror);
        }

        for (var i = 0; i < Points.Count; i += 2)
        {
            var p1 = Points[i];
            var p2 = Points[i + 2];
            var func = arr[i / 2];


            var x = p1.X;
            var y1 = p1.Y;
            while (x < p2.X)
            {
                var y2 = (float) (func[0] * x * x * x + func[1] * x * x + func[2] * x + func[3]);
                var line = DrawUtils.Line(
                    new PointF(x * Scale, y1 * Scale),
                    new PointF((x + 0.1f) * Scale, y2 * Scale),
                    new SolidColorBrush(Color.FromRgb(36, 134, 211))
                );
                y1 = y2;
                x += 0.1f;
                _canvas.Children.Add(line);
            }

        }
    }


    private void ClearCanvas()
    {
        if (_canvas.Children.Count == 0) return;

        _canvas.Children.Clear();
        GC.Collect();
    }

    public void RefreshCanvas()
    {
        ClearCanvas();
        Draw2DCoords();
        DrawPointsOnCanvas();
        DrawGraphs();
    }

    private void DrawGraphs()
    {
        foreach (var line in _lines) _canvas.Children.Add(line);
    }


    private void UpdateCanvas(object sender, SizeChangedEventArgs args)
    {
        _canvasSize = args.NewSize;
        _currentCenter = new PointF((float) _canvasSize.Width / 2, (float) _canvasSize.Height / 2);
        if (_offset is not null) Offset(_currentCenter!.Value, _offset!.Value);
        RefreshCanvas();
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
        var i = 0;
        var x = (double) _currentCenter?.X!;
        var y = (double) _currentCenter?.Y!;
        while (x < _canvasSize.Width)
        {
            if (x > _currentCenter?.X)
            {
                if (Scale <= 15)
                    if (i % 5 != 0)
                    {
                        x += Scale;
                        i++;
                        continue;
                    }

                var pos = new TextBlock();
                pos.Text = $"{i}";
                pos.FontSize = IndexFontSize;
                pos.Foreground = ColorConst.PENCIL;
                var neg = new TextBlock();
                neg.Text = $"-{i}";
                neg.FontSize = IndexFontSize;
                neg.Foreground = ColorConst.PENCIL;

                pos.Margin = new Thickness(x - 1, y + 5, 0, 0);
                neg.Margin = new Thickness((double) (_currentCenter?.X - Scale * i)! - 2, y + 5, 0,
                    0);
                _canvas.Children.Add(pos);
                _canvas.Children.Add(neg);
            }

            x += Scale;
            i++;
        }

        i = 0;
        while (y < _canvasSize.Height)
        {
            if (y > _currentCenter?.Y)
            {
                if (Scale <= 15)
                    if (i % 5 != 0)
                    {
                        y += Scale;
                        i++;
                        continue;
                    }

                var pos = new TextBlock();
                pos.Text = $"{i}";
                pos.FontSize = IndexFontSize;
                pos.Foreground = ColorConst.PENCIL;
                var neg = new TextBlock();
                neg.Text = $"-{i}";
                neg.FontSize = IndexFontSize;
                neg.Foreground = ColorConst.PENCIL;

                pos.Margin = new Thickness((double) (_currentCenter?.X - 5)!, y - 5, 0, 0);
                neg.Margin = new Thickness((double) (_currentCenter?.X - 5)!,
                    (double) (_currentCenter?.Y - i * Scale)! - 5, 0, 0);
                _canvas.Children.Add(pos);
                _canvas.Children.Add(neg);
            }

            y += Scale;
            i++;
        }
    }

    private void DrawPointsOnCanvas()
    {
        var drp = new DropShadowEffect();
        drp.Opacity = .2f;
        drp.ShadowDepth = 1;
        foreach (var p in Points)
        {
            var point = Point(5, p, ColorConst.DARK_PENCIL);
            point.Stroke = ColorConst.DARK_PENCIL;
            point.Effect = drp;
            point.Fill = ColorConst.DARK_PENCIL;
            point.Focusable = true;
            point.ToolTip = $"P{_points.IndexOf(p)}(x: {p.X} | y: {p.Y})";
            _canvas.Children.Add(point);
            // todo subscribe to events.

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

    #region Event FUNC

    private void OnZoom(object sender, MouseWheelEventArgs e)
    {

        var d = e.Delta;

        var zoom = 1 + d / 500f;
        Zoom += zoom;
        IndexFontSize += d / 500f;
        IndexFontSize = IndexFontSize < 10 ? 10 : IndexFontSize > 20 ? 20 : IndexFontSize;
        Scale = Scale < 5 ? 5 : Scale * zoom;

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

    #endregion

    #region PRIVATE FIELDS

    protected ObservableCollection<Line> _lines = new();

    private ICommand? _processCommand;

    protected Canvas _canvas;

    private ICommand? _addPointCommand;

    private Size _canvasSize;

    private PointF? _currentCenter;

    private PointF? _offset;

    private ICommand? _clearListCommand;


    private ObservableCollection<PointF> _points = new();


    private float _scaling = 20;

    private string _pointsString;

    private ICommand _removePointCmd;

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

    #endregion

    #region COMMANDS

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


    public ICommand RemovePointCmd => _removePointCmd ??= new ParameterizedCommand<int>(index =>
    {
        Points.RemoveAt(index!);
        RefreshCanvas();
    });

    public ICommand ProcessCommand =>
        _processCommand ??= new RelayCommand(() => Process());

    #endregion
}