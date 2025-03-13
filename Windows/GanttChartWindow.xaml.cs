using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using System.Windows;

namespace MSProj_Analog.Windows
{

    public partial class GanttChartWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] TaskLabels { get; set; }
        public Func<double, string> DateFormatter { get; set; }

        public GanttChartWindow(ICollection<ProjectTask> tasks)
        {
            InitializeComponent();
            LoadGanttChart(tasks);
            DataContext = this;
        }

        private void LoadGanttChart(ICollection<ProjectTask> tasks)
        {
            SeriesCollection = new SeriesCollection();
            TaskLabels = tasks.Select(t => t.Name).ToArray();
            DateFormatter = val => new DateTime((long)val).ToShortDateString();

            int index = 0;
            foreach (var task in tasks)
            {
                var start = task.StartDate.Ticks;
                var end = task.EndDate.Ticks;
                var duration = end - start;

                SeriesCollection.Add(new RowSeries
                {
                    Title = task.Name,
                    Values = new ChartValues<GanttPoint> { new GanttPoint(start, end) },
                    DataLabels = true
                });

                index++;
            }
            GanttChart.Series = SeriesCollection;
        }

        private void ExportToPDFButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = ConfigOptions.Path+"GantChart.pdf";
            ExportChart.ExportChartToPdf(filePath,GanttChart);
        }

        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
