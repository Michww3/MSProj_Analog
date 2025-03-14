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
using Microsoft.Win32;

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
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить круговую диаграмму как PDF",
                Filter = "PDF файлы (*.pdf)|*.pdf", // Ограничение выбора PDF-файлами
                DefaultExt = "pdf",
                FileName = "PieChart.pdf", // Стандартное имя файла
                InitialDirectory = ConfigOptions.Path // Начальная папка для сохранения
            };

            if (saveFileDialog.ShowDialog() == true) // Открываем диалоговое окно
            {
                ExportChart.ExportChartToPdf(saveFileDialog.FileName, PieChart);
                MessageBox.Show("Экспорт завершен!", "Экспорт в PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
