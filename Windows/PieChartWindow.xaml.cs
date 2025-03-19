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
            var saveFileDialog = ConfigOptions.Export("Сохранить круговую диаграмму как PDF", "PDF файлы (*.pdf)|*.pdf", "pdf", "PieChart.pdf");

            if (saveFileDialog.ShowDialog() == true) // Открываем диалоговое окно
            {
                ExportChart.ExportChartToPdf(saveFileDialog.FileName, PieChart);
                MessageBox.Show("Экспорт завершен!", "Экспорт в PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void ExportToSVGButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = ConfigOptions.Export("Сохранить круговую диаграмму как SVG", "SVG файлы (*.svg)|*.svg", "svg", "PieChart.svg");

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportChartToSvg(saveFileDialog.FileName);
                MessageBox.Show("Экспорт завершен!", "Экспорт в SVG", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportChartToSvg(string filePath)
        {
            if (PieChart == null || SeriesCollection == null || SeriesCollection.Count == 0)
                return;

            // Размеры диаграммы
            var width = (int)PieChart.ActualWidth;
            var height = (int)PieChart.ActualHeight;
            var centerX = width / 2;
            var centerY = height / 2;
            var radius = Math.Min(centerX, centerY) - 10;

            // Создаем SVG-документ
            var svgDoc = new SvgDocument
            {
                Width = width,
                Height = height
            };

            double totalValue = SeriesCollection.Sum(series => series.Values.Count > 0 ? (double)series.Values[0] : 0);
            double startAngle = 0;

            Random random = new Random();

            foreach (var series in SeriesCollection)
            {
                if (series.Values.Count == 0) continue;

                double value = (double)series.Values[0];
                double sweepAngle = (value / totalValue) * 360;
                double endAngle = startAngle + sweepAngle;

                // Вычисляем координаты для дуги
                double startX = centerX + radius * Math.Cos(startAngle * Math.PI / 180);
                double startY = centerY + radius * Math.Sin(startAngle * Math.PI / 180);
                double endX = centerX + radius * Math.Cos(endAngle * Math.PI / 180);
                double endY = centerY + radius * Math.Sin(endAngle * Math.PI / 180);

                // Определяем, является ли угол больше 180 градусов
                int largeArcFlag = sweepAngle > 180 ? 1 : 0;

                // Генерируем случайный цвет
                var fillColor = System.Drawing.Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

                // Создаем путь (сектор круга)
                var path = new SvgPath
                {
                    Fill = new SvgColourServer(fillColor),
                    Stroke = new SvgColourServer(System.Drawing.Color.Black),
                    StrokeWidth = 1
                };

                // Определяем команду для рисования сектора
                string pathData = string.Format(CultureInfo.InvariantCulture,
                    "M {0},{1} L {2},{3} A {4},{4} 0 {5},1 {6},{7} Z",
                    centerX, centerY,   // Начало от центра круга
                    startX, startY,      // Линия от центра до начала дуги
                    radius,              // Радиус дуги
                    largeArcFlag,        // Флаг для больших углов
                    endX, endY           // Завершение дуги
                );

                path.PathData = SvgPathBuilder.Parse(pathData);
                svgDoc.Children.Add(path);

                startAngle = endAngle;
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
