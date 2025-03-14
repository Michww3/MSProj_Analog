using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
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
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить диаграмму Ганта как PDF",
                Filter = "PDF файлы (*.pdf)|*.pdf", // Ограничение выбора PDF-файлами
                DefaultExt = "pdf",
                FileName = "GanttChart.pdf", // Стандартное имя файла
                InitialDirectory = ConfigOptions.Path // Начальная папка для сохранения
            };

            if (saveFileDialog.ShowDialog() == true) // Открываем диалоговое окно
            {
                ExportChart.ExportChartToPdf(saveFileDialog.FileName, GanttChart);
                MessageBox.Show("Экспорт завершен!", "Экспорт в PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
