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
            ExportPieChartToPdf(filePath);
        }

        // Метод для экспорта диаграммы в PDF
        public void ExportPieChartToPdf(string filePath)
        {
            // Создаем новый PDF документ
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            // Создаем объект графики для рисования
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Устанавливаем размер для рендеринга диаграммы
            var renderSize = new System.Windows.Size(page.Width, page.Height);

            // Создаем RenderTarget для рисования диаграммы
            var renderTarget = new RenderTargetBitmap(
                (int)renderSize.Width, (int)renderSize.Height,
                96, 96, PixelFormats.Pbgra32);

            // Рисуем диаграмму в RenderTarget
            PieChart.Measure(renderSize);
            PieChart.Arrange(new System.Windows.Rect(0, 0, renderSize.Width, renderSize.Height));
            renderTarget.Render(PieChart);

            // Сохраняем изображение в MemoryStream
            var memoryStream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
            encoder.Save(memoryStream);

            // Сбрасываем позицию потока на начало, чтобы его можно было прочитать
            memoryStream.Position = 0;

            // Добавляем изображение в PDF
            XImage xImage = XImage.FromStream(memoryStream); // Теперь передаем поток напрямую
            gfx.DrawImage(xImage, 0, 0);

            // Сохраняем PDF файл
            document.Save(filePath);
        }

        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
