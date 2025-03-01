using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Appka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string? filepath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_DFT(object sender, RoutedEventArgs e)
        {
            Logic.FrequencyData[] frequencyDatas = Logic.DoDFT(filepath??throw new Exception ("Choose a file first"));
        }
        private void Button_Click_FFT(object sender, RoutedEventArgs e)
        {
            float[] frequencyDatas = Logic.DoFFT(filepath ?? throw new Exception("Choose a file first"));
            DrawGrid(grid, grid.ActualWidth);
            wav_button.Content = "NEMĚŇTE VELIKOST OKNA";
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".wav"; // Default file extension
            dialog.Filter = "Audio file format (.wav)|*.wav"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                filepath = dialog.FileName;
            }
        }
        private void DrawGrid(Grid grid, double wh)
        {
            for (int i = 1; i < 21; i++)
            {
                Line line = new Line();
                line.X1 = i * wh/40 - 1;
                line.X2 = i * wh/40 - 1;
                line.Y1 = 1;
                line.Y2 = wh/2;
                line.Stroke = Brushes.Gray;
                line.StrokeThickness = 2;
                grid.Children.Add(line);
            }
            for (int i = 1; i < 21; i++)
            {
                Line line = new Line();
                line.X1 = 1;
                line.X2 = wh / 2;
                line.Y1 = i * wh/40 - 1;
                line.Y2 = i * wh/40 - 1;
                line.Stroke = Brushes.Gray;
                line.StrokeThickness = 2;
                grid.Children.Add(line);
            }
            Line line2 = new Line();
            line2.X1 = wh / 20 - 1;
            line2.X2 = wh / 20 - 1;
            line2.Y1 = 1;
            line2.Y2 = wh / 2;
            line2.Stroke = Brushes.Black;
            line2.StrokeThickness = 2;
            grid.Children.Add(line2);
            Line line3 = new Line();
            line3.Y1 = 18 * wh / 40 - 1;
            line3.Y2 = 18 * wh / 40 - 1;
            line3.X1 = 1;
            line3.X2 = wh / 2;
            line3.Stroke = Brushes.Black;
            line3.StrokeThickness = 2;
            grid.Children.Add(line3);
        }
    }
}