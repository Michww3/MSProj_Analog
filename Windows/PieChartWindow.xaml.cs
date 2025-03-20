using LiveCharts;
using LiveCharts.Wpf;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using SkiaSharp;
using Svg.Pathing;
using Svg;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Globalization;




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

            int index = 0;
            foreach (var task in tasks)
            {
                double duration = (task.EndDate - task.StartDate).TotalDays;

                // For cycle color pick
                var taskColor = ConfigOptions.chartColors[index % ConfigOptions.chartColors.Count];

                SeriesCollection.Add(new PieSeries
                {
                    Title = task.Name,
                    Values = new ChartValues<double> { duration },
                    DataLabels = true,
                    Fill = new SolidColorBrush(taskColor) // Применяем цвет
                });

                index++;
            }
            PieChart.Series = SeriesCollection;
        }

        private void ExportToPDFButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = ConfigOptions.Export("Сохранить круговую диаграмму как PDF", "PDF файлы (*.pdf)|*.pdf", "pdf", "PieChart.pdf");

            if (saveFileDialog.ShowDialog() == true) // Открываем диалоговое окно
            {
                ExportChart.ExportChartToPdf(saveFileDialog.FileName, PieChart);
                ConfigOptions.MessageBoxExport("Pdf export");
            }
        }

        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = ConfigOptions.Export("Сохранить круговую диаграмму как SVG", "SVG файлы (*.svg)|*.svg", "svg", "PieChart.svg");

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportChartToSvg(saveFileDialog.FileName);
                ConfigOptions.MessageBoxExport("Svg export");
            }
        }

        private void ExportChartToSvg(string filePath)
        {
            if (PieChart == null || SeriesCollection == null || SeriesCollection.Count == 0)
                return;

            var width = (int)PieChart.ActualWidth;
            var height = (int)PieChart.ActualHeight;
            var centerX = width / 2;
            var centerY = height / 2;
            var radius = Math.Min(centerX, centerY) - 10;
            int legendWidth = 200; // Отступ для легенды

            var svgDoc = new SvgDocument
            {
                Width = width + legendWidth,
                Height = height
            };

            double totalValue = SeriesCollection.Sum(series => series.Values.Count > 0 ? (double)series.Values[0] : 0);
            double startAngle = 0;
            int legendY = 20; // Начальная позиция легенды
            int index = 0;

            foreach (var series in SeriesCollection)
            {
                if (series.Values.Count == 0) continue;

                double value = (double)series.Values[0];
                double percentage = (value / totalValue) * 100;
                double sweepAngle = (value / totalValue) * 360;
                double endAngle = startAngle + sweepAngle;

                // Получаем цвет из `chartColors`
                var wpfColor = ConfigOptions.chartColors[index % ConfigOptions.chartColors.Count]; // Берем цвет по индексу
                var fillColor = System.Drawing.Color.FromArgb(wpfColor.R, wpfColor.G, wpfColor.B);

                // Вычисляем координаты для дуги
                double startX = centerX + radius * Math.Cos(startAngle * Math.PI / 180);
                double startY = centerY + radius * Math.Sin(startAngle * Math.PI / 180);
                double endX = centerX + radius * Math.Cos(endAngle * Math.PI / 180);
                double endY = centerY + radius * Math.Sin(endAngle * Math.PI / 180);

                int largeArcFlag = sweepAngle > 180 ? 1 : 0;

                // Создаем сектор диаграммы
                var path = new SvgPath
                {
                    Fill = new SvgColourServer(fillColor),
                    Stroke = new SvgColourServer(System.Drawing.Color.Black),
                    StrokeWidth = 1
                };

                string pathData = string.Format(CultureInfo.InvariantCulture,
                    "M {0},{1} L {2},{3} A {4},{4} 0 {5},1 {6},{7} Z",
                    centerX, centerY, startX, startY, radius, largeArcFlag, endX, endY);

                path.PathData = SvgPathBuilder.Parse(pathData);
                svgDoc.Children.Add(path);

                // Добавляем текст в центр сектора
                double midAngle = startAngle + sweepAngle / 2;
                double textX = centerX + (radius * 0.6) * Math.Cos(midAngle * Math.PI / 180);
                double textY = centerY + (radius * 0.6) * Math.Sin(midAngle * Math.PI / 180);

                var text = new SvgText(series.Title)
                {
                    X = new SvgUnitCollection { new SvgUnit((float)textX) },
                    Y = new SvgUnitCollection { new SvgUnit((float)textY) },
                    Fill = new SvgColourServer(System.Drawing.Color.Black),
                    FontSize = new SvgUnit(14),
                    TextAnchor = SvgTextAnchor.Middle
                };
                svgDoc.Children.Add(text);

                // Добавляем легенду справа
                int legendX = width + 20;

                var legendRect = new SvgRectangle
                {
                    X = new SvgUnit(legendX),
                    Y = new SvgUnit(legendY),
                    Width = new SvgUnit(20),
                    Height = new SvgUnit(20),
                    Fill = new SvgColourServer(fillColor),
                    Stroke = new SvgColourServer(System.Drawing.Color.Black),
                    StrokeWidth = 1
                };
                svgDoc.Children.Add(legendRect);

                var legendText = new SvgText($"{series.Title} ({percentage:F1}%)")
                {
                    X = new SvgUnitCollection { new SvgUnit(legendX + 30) },
                    Y = new SvgUnitCollection { new SvgUnit(legendY + 15) },
                    Fill = new SvgColourServer(System.Drawing.Color.Black),
                    FontSize = new SvgUnit(14),
                    TextAnchor = SvgTextAnchor.Start
                };
                svgDoc.Children.Add(legendText);

                legendY += 30; // Сдвигаем вниз для следующей задачи
                startAngle = endAngle;
                index++; // Переход к следующему цвету
            }

            // Сохраняем SVG в файл
            svgDoc.Write(filePath);
        }

        //private void ExportChartToSvg(string filePath)
        //{
        //    var width = (int)PieChart.ActualWidth;
        //    var height = Convert.ToInt32(1.1 * (int)PieChart.ActualHeight);

        //    var svgCanvas = new SkiaSharp.Extended.Svg.SKSvg();
        //    using (var skSurface = SKSurface.Create(new SKImageInfo(width, height)))
        //    {
        //        var canvas = skSurface.Canvas;
        //        canvas.Clear(SKColors.White);

        //        var renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        //        renderTargetBitmap.Render(PieChart);

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