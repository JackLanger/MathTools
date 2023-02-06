using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MathCanvas.Controller;

namespace MathCanvas;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new ProcessingCanvasController(MathCanvas);
    }

    private void TbPoints_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var tb = sender as TextBox;
            var cont = DataContext as ProcessingCanvasController;
            var text = tb.Text;
            cont.PointsString = text;
            cont.AddPointCommand.Execute(null);
        }
    }

    private void LbPoints_OnDrop(object sender, DragEventArgs e)
    {
        var lb = sender as TextBox;
        var cont = DataContext as ProcessingCanvasController;
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {

            var files = (string[]) e.Data.GetData(DataFormats.FileDrop)!;
            foreach (var file in files)
            {
                var fileContent = File.ReadLines(file);

                // todo: remove artifacts such as comments

                cont!.ParsePoints2D(string.Join(" ", fileContent));
                cont!.RefreshCanvas();
            }


        }
    }
}