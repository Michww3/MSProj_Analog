using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MSProj_Analog.DTOs;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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
            string filePath = "C:\\Users\\User\\Desktop\\Ganttchart.pdf";
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
            GanttChart.Measure(renderSize);
            GanttChart.Arrange(new System.Windows.Rect(0, 0, renderSize.Width, renderSize.Height));
            renderTarget.Render(GanttChart);

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
