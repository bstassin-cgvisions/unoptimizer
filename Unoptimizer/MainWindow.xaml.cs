using System.Windows;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Unoptimizer
{
    public partial class MainWindow : Window, IProgress<string>
    {
        private const string optimedValueFalse = "<Optimize>false</Optimize>";
        private const string optimedValueTrue = "<Optimize>true</Optimize>";
        private readonly string[] toScan = new string[] { "*.csproj", "*.vbproj" };
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Report(string value)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtBoxResults.Text += value;
                this.progressBar1.Value++;
                lblCurrent.Content = value;
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            txtBoxResults.Text = "Starting...";
            var path = this.textBox1.Text;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Value = 0;
            this.progressBar1.Maximum = Directory.GetFiles(path, toScan[0], SearchOption.AllDirectories).Length + Directory.GetFiles(path, toScan[1], SearchOption.AllDirectories).Length;
            await doOverwriting(path, this, (bool)doOptimize.IsChecked);
            MessageBox.Show("Done!");
        }
        private async Task doOverwriting(string path, IProgress<string> progress, bool optimize)
        {
            await Task.Run(() =>
            {
                foreach (var filetype in toScan)
                {
                    foreach (var file in Directory.GetFiles(path, filetype, SearchOption.AllDirectories))
                    {
                        try
                        {
                            var content = File.ReadAllText(file);

                            if (optimize)
                                content = content.Replace(optimedValueFalse, optimedValueTrue);
                            else
                                content = content.Replace(optimedValueTrue, optimedValueFalse);

                            File.WriteAllText(file, content);
                            if (progress != null)
                                progress.Report(Path.GetFileName(file) + " overwritten\n");
                        }
                        catch (Exception)
                        {
                            progress.Report("ERROR - " + Path.GetFileName(file) + " couldn't be overwritten\n");
                        }
                    }
                }
            });
        }
    }
}
