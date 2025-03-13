using PdfSharp.Pdf;
using PdfSharp.Drawing;
using LiveCharts;
using LiveCharts.Wpf;
using MSProj_Analog.DTOs;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MSProj_Analog.Config;
using MSProj_Analog.Helpers;

namespace MSProj_Analog.Windows
{
    public partial class PieChartWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }

        public PieChartWindow(ICollection<ProjectTask> tasks)
        {
            InitializeComponent();
            LoadPieChart(tasks);
            DataContext = this;
        }

        private void LoadPieChart(ICollection<ProjectTask> tasks)
        {
            SeriesCollection = new SeriesCollection();

            foreach (var task in tasks)
            {
                double duration = (task.EndDate - task.StartDate).TotalDays;

                SeriesCollection.Add(new PieSeries
                {
                    Title = task.Name,
                    Values = new ChartValues<double> { duration },
                    DataLabels = true
                });
            }

            PieChart.Series = SeriesCollection;
        }

        private void ExportToPDFButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = $"{ConfigOptions.Path}PieChart.pdf";
            ExportChart.ExportChartToPdf(filePath, PieChart);
        }

        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
