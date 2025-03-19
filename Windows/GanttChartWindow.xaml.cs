using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using SkiaSharp;
using Svg;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            var saveFileDialog = ConfigOptions.Export("Сохранить диаграмму Ганта как PDF", "PDF файлы (*.pdf)|*.pdf", "pdf", "GanttChart.pdf");

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportChart.ExportChartToPdf(saveFileDialog.FileName, GanttChart);
                MessageBox.Show("Экспорт завершен!", "Экспорт в PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = ConfigOptions.Export("Сохранить диаграмму Ганта как SVG", "SVG файлы (*.svg)|*.svg", "svg", "GanttChart.svg");

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportChartToSvg(saveFileDialog.FileName);
                MessageBox.Show("Экспорт завершен!", "Экспорт в SVG", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportChartToSvg(string filePath)
        {
            if (GanttChart == null || SeriesCollection == null || SeriesCollection.Count == 0)
                return;

            // Размеры диаграммы
            var width = (int)GanttChart.ActualWidth;
            var height = (int)GanttChart.ActualHeight;
            var barHeight = 30; // Высота одной задачи
            var padding = 10;

            var svgDoc = new SvgDocument
            {
                Width = width,
                Height = height
            };

            // Определяем минимальную и максимальную дату
            double minDate = SeriesCollection.Min(series => ((GanttPoint)series.Values[0]).StartPoint);
            double maxDate = SeriesCollection.Max(series => ((GanttPoint)series.Values[0]).EndPoint);
            double dateRange = maxDate - minDate;

            int index = 0;
            foreach (var series in SeriesCollection)
            {
                var ganttPoint = (GanttPoint)series.Values[0];

                double startX = padding + ((ganttPoint.StartPoint - minDate) / dateRange) * (width - 2 * padding);
                double barWidth = ((ganttPoint.EndPoint - ganttPoint.StartPoint) / dateRange) * (width - 2 * padding);
                double y = padding + index * (barHeight + padding);

                var random = new Random();
                var fillColor = System.Drawing.Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

                // Добавляем прямоугольник (задачу)
                var rect = new SvgRectangle
                {
                    X = new SvgUnit((float)startX),
                    Y = new SvgUnit((float)y),
                    Width = new SvgUnit((float)barWidth),
                    Height = new SvgUnit((float)barHeight),
                    Fill = new SvgColourServer(fillColor),
                    Stroke = new SvgColourServer(System.Drawing.Color.Black),
                    StrokeWidth = 1
                };
                svgDoc.Children.Add(rect);

                // Добавляем текст (название задачи)
                var text = new SvgText(series.Title)
                {
                    X = new SvgUnitCollection { new SvgUnit((float)(startX + 5)) }, // Указываем X как коллекцию
                    Y = new SvgUnitCollection { new SvgUnit((float)(y + barHeight / 2)) }, // Указываем Y как коллекцию
                    Fill = new SvgColourServer(System.Drawing.Color.Black),
                    FontSize = new SvgUnit(14)
                };
                svgDoc.Children.Add(text);

                index++;
            }

            // Сохраняем SVG в файл
            svgDoc.Write(filePath);
        }

        //private void ExportChartToSvg(string filePath)
        //{
        //    var width = (int)GanttChart.ActualWidth;
        //    var height = (int)GanttChart.ActualHeight;

        //    var svgCanvas = new SkiaSharp.Extended.Svg.SKSvg();
        //    using (var skSurface = SKSurface.Create(new SKImageInfo(width, height)))
        //    {
        //        var canvas = skSurface.Canvas;
        //        canvas.Clear(SKColors.White);

        //        var renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        //        renderTargetBitmap.Render(GanttChart);

        //        var encoder = new PngBitmapEncoder();
        //        encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            encoder.Save(memoryStream);
        //            var skImage = SKImage.FromEncodedData(memoryStream.ToArray());
        //            canvas.DrawImage(skImage, 0, 0);
        //        }

        //        skSurface.Flush();
        //        using (var fileStream = File.OpenWrite(filePath))
        //        {
        //            skSurface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo(fileStream);
        //        }
        //    }
        //}
    }
}
