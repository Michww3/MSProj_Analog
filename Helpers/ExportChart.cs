﻿using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace MSProj_Analog.Helpers
{
    public static class ExportChart
    {
        public static void ExportChartToPdf(string filePath, FrameworkElement chart)
        {
            // Создаем новый PDF документ
            using (PdfDocument document = new PdfDocument())
            {
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
                chart.Measure(renderSize);
                chart.Arrange(new System.Windows.Rect(0, 0, renderSize.Width, renderSize.Height));
                renderTarget.Render(chart);

                // Сохраняем изображение в MemoryStream
                using (var memoryStream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                    encoder.Save(memoryStream);

                    // Сбрасываем позицию потока на начало, чтобы его можно было прочитать
                    memoryStream.Position = 0;

                    // Добавляем изображение в PDF
                    XImage xImage = XImage.FromStream(memoryStream); // Теперь передаем поток напрямую

                    gfx.DrawImage(xImage, 0, 0);
                }

                // Сохраняем PDF файл
                document.Save(filePath);
            }
        }
    }
}
