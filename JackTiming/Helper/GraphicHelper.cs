using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JackTiming
{
    public static class GraphicHelper
    {
        /// <summary>
        /// Copies a UI element to the clipboard as an image.
        /// </summary>
        /// <param name="element">The element to copy.</param>
        public static void CopyUiElementToClipboard(FrameworkElement element)
        {
            //double width = element.ActualWidth;
            //double height = element.ActualHeight;
            double width = element.Width;
            double height = element.Height;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }

        public static void CreateSaveBitmap(Canvas canvas, string filename)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)canvas.Width, (int)canvas.Height,
                96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }

        public static double MeasureTextWidth(string text, double font_size)
        {
            var label = new Label();
            label.Content = text;
            label.FontSize = font_size;

            // Position the label.
            label.Measure(new Size(double.MaxValue, double.MaxValue));
            return label.DesiredSize.Width;
        }


        // Position a label at the indicated point.
        public static void DrawText(Canvas can, string text, Point location,
            double font_size,
            //HorizontalAlignment halign, VerticalAlignment valign, 
            Brush color)
        {
            // Make the label.
            var label = new Label();
            label.Content = text;
            label.FontSize = font_size;
            label.Foreground = color;
            can.Children.Add(label);

            // Position the label.
            label.Measure(new Size(double.MaxValue, double.MaxValue));

            var x = location.X;
            //if (halign == HorizontalAlignment.Center)
            //    x -= label.DesiredSize.Width / 2;
            //else if (halign == HorizontalAlignment.Right)
            //    x -= label.DesiredSize.Width;
            Canvas.SetLeft(label, x);

            var y = location.Y;
            //if (valign == VerticalAlignment.Center)
            //    y -= label.DesiredSize.Height / 2;
            //else if (valign == VerticalAlignment.Bottom)
            //    y -= label.DesiredSize.Height;
            Canvas.SetTop(label, y);
        }
    }
}