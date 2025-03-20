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

                // Берем цвет из коллекции, используя индекс по модулю
                var taskColor = ConfigOptions.chartColors[index % ConfigOptions.chartColors.Count];

                SeriesCollection.Add(new RowSeries
                {
                    Title = task.Name,
                    Values = new ChartValues<GanttPoint> { new GanttPoint(start, end) },
                    DataLabels = true,
                    Fill = new SolidColorBrush(taskColor) // Применяем цвет
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
                ConfigOptions.MessageBoxExport("Pdf export");
            }
        }

        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = ConfigOptions.Export("Сохранить диаграмму Ганта как SVG", "SVG файлы (*.svg)|*.svg", "svg", "GanttChart.svg");

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportChartToSvg(saveFileDialog.FileName);
                ConfigOptions.MessageBoxExport("Svg export");
            }
        }

        private void ExportChartToSvg(string filePath)
        {
            if (GanttChart == null || SeriesCollection == null || SeriesCollection.Count == 0)
                return;

            // Размеры диаграммы
            var width = (int)GanttChart.ActualWidth;
            var height = (int)GanttChart.ActualHeight;
            var barHeight = 60; // Высота одной задачи
            var padding = 5;
            var axisHeight = 40; // Высота оси времени
            var gridLinesCount = 10; // Количество линий сетки на оси времени

            var svgDoc = new SvgDocument
            {
                Width = width,
                Height = height + axisHeight
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

                // Используем цвет из коллекции, связанный с текущим индексом
                var wpfColor = ConfigOptions.chartColors[index % ConfigOptions.chartColors.Count];
                var fillColor = System.Drawing.Color.FromArgb(wpfColor.R, wpfColor.G, wpfColor.B);


                // Добавляем прямоугольник (задачу)
                var rect = new SvgRectangle
                {
                    X = new SvgUnit((float)startX),
                    Y = new SvgUnit((float)y),
                    Width = new SvgUnit((float)barWidth),
                    Height = new SvgUnit((float)barHeight),
                    Fill = new SvgColourServer(fillColor),  // Применяем цвет задачи
                    Stroke = new SvgColourServer(System.Drawing.Color.Black),
                    StrokeWidth = 1
                };
                svgDoc.Children.Add(rect);

                // Добавляем текст (название задачи)
                var text = new SvgText(series.Title)
                {
                    X = new SvgUnitCollection { new SvgUnit((float)(startX + 5)) },
                    Y = new SvgUnitCollection { new SvgUnit((float)(y + barHeight / 2)) },
                    Fill = new SvgColourServer(System.Drawing.Color.Black),
                    FontSize = new SvgUnit(14)
                };
                svgDoc.Children.Add(text);

                index++;
            }

            // Добавляем ось времени внизу диаграммы
            double axisY = height + padding; // Позиция оси X

            // Линия оси
            var axisLine = new SvgLine
            {
                StartX = new SvgUnit(padding),
                StartY = new SvgUnit((float)axisY),
                EndX = new SvgUnit(width - padding),
                EndY = new SvgUnit((float)axisY),
                Stroke = new SvgColourServer(System.Drawing.Color.Black),
                StrokeWidth = 2
            };
            svgDoc.Children.Add(axisLine);

            // Добавляем метки дат и вертикальные линии сетки
            for (int i = 0; i <= gridLinesCount; i++)
            {
                double xPos = padding + (i / (double)gridLinesCount) * (width - 2 * padding);
                double dateValue = minDate + (i / (double)gridLinesCount) * dateRange;
                DateTime date = new DateTime((long)dateValue);

                // Вертикальная линия сетки
                var gridLine = new SvgLine
                {
                    StartX = new SvgUnit((float)xPos),
                    StartY = new SvgUnit(padding),
                    EndX = new SvgUnit((float)xPos),
                    EndY = new SvgUnit((float)axisY),
                    Stroke = new SvgColourServer(System.Drawing.Color.Gray),
                    StrokeWidth = 1,
                    StrokeDashArray = new SvgUnitCollection { new SvgUnit(4), new SvgUnit(4) } // Пунктирная линия
                };
                svgDoc.Children.Add(gridLine);

                // Текст с датой
                var dateText = new SvgText(date.ToString("dd.MM.yyyy"))
                {
                    X = new SvgUnitCollection { new SvgUnit((float)xPos) },
                    Y = new SvgUnitCollection { new SvgUnit((float)(axisY + 20)) },
                    Fill = new SvgColourServer(System.Drawing.Color.Black),
                    FontSize = new SvgUnit(12),
                    TextAnchor = SvgTextAnchor.Middle
                };
                svgDoc.Children.Add(dateText);
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