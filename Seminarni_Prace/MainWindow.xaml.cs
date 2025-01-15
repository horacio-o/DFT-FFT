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
            testLabel.Content = string.Join(": ", frequencyDatas);
        }
        private void Button_Click_FFT(object sender, RoutedEventArgs e)
        {
            float[] frequencyDatas = Logic.DoFFT(filepath ?? throw new Exception("Choose a file first"));
            testLabel.Content = string.Join(": " , frequencyDatas);
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
    }
}