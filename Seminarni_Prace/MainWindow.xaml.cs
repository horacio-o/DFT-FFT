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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
                // Open document
                string filename = dialog.FileName;
                Logic.FrequencyData[] frequencyDatas = Logic.GetInfo(filename);
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            int minimum = int.Parse(lowerBoundTxtBox.Text);
            int maximum = int.Parse(upperBoundTxtBox.Text);
            if (minimum > maximum)
            {
                SadFace.Visibility = Visibility.Visible;
            }
            else
            {
                SadFace.Visibility = Visibility.Collapsed;
            }
        }
    }
}