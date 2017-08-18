using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using JackTiming.MessageInfrastructure;
using JackTiming.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace JackTiming
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DrawParam.StartX = 50;
            DrawParam.StartY = 15;
            DrawParam.UnitX = 10;
            DrawParam.UnitY = 30;
            DrawParam.Margin = 10;
            DrawParam.VerticalSpace = 15;
            DrawParam.MarkHeight = 6;

            _lastPoint = new Point(0, 0);
            _timingDatas = null;

            Messenger.Default.Register<MessageToken>(this, (item) =>
            {
                switch (item.TokenType)
                {
                    case MessageTokenType.UpdateTimingDiagram:
                        DrawTiming(myCanvas);
                        break;
                    case MessageTokenType.CopyToClipboard:
                        CopyUIElementToClipboard(myCanvas);
                        break;
                    case MessageTokenType.SaveBitmap:
                        var dlg = new SaveFileDialog()
                        {
                            Filter = "Png files (*.png)|*.png"
                        };

                        if (dlg.ShowDialog() == true)
                        {
                            CreateSaveBitmap(myCanvas, dlg.FileName);
                        }
                        break;
                }
            });
        }

        private List<TimingMap> _timingDatas;

        private void TxtData_OnKeyUp(object sender, KeyEventArgs e)
        {
            Messenger.Default.Send(new MessageToken()
            {
                TokenType = MessageTokenType.KeyChanged,
                Message = null
            });

            DrawTiming(myCanvas);
        }

        private Point _lastPoint;


        private void DrawTiming(Canvas canvas)
        {
            _timingDatas = TimingMapParser.Parse(txtData.Text);

            var startX = DrawParam.StartX;
            var startY = DrawParam.StartY;
            var unitX = DrawParam.UnitX;
            var unitY = DrawParam.UnitY;
            var verticalSpace = DrawParam.VerticalSpace;
            var margin = DrawParam.Margin;

            var lineIndex = 0;

            canvas.Children.Clear();

            var labelIndex = new List<int>();

            // check the width of symbol 
            var max = 0.0;
            foreach (var timingData in _timingDatas)
            {
                var x = MeasureTextWidth(timingData.Symbol, 14);
                if (max < x)
                    max = x;
            }

            // ensure we draw behind symbol
            startX = (int)max;

            // check the width of diagram 
            max = 0;
            foreach (var timingData in _timingDatas)
            {
                if (max < (int)timingData.Timing.Length)
                    max = (int)timingData.Timing.Length;
            }

            // re-calculate the size of canvas
            canvas.Width = startX + (max * unitX) + (margin * 2);
            canvas.Height = _timingDatas.Count * (unitY + verticalSpace) + startY;

            // draw markline
            var markline = new Line();
            markline.Stroke = Brushes.Blue;
            markline.StrokeThickness = 1;
            markline.X1 = startX;
            markline.X2 = startX + (max * unitX);
            markline.Y1 = 1;
            markline.Y2 = 1;
            canvas.Children.Add(markline);
            // markline's vertical line
            for (int i = 0; i <= max; i++)
            {
                var ll = new Line();
                ll.Stroke = Brushes.Blue;
                ll.StrokeThickness = 1;
                ll.X1 = i * unitX + startX;
                ll.X2 = ll.X1;
                ll.Y1 = 1;
                ll.Y2 = DrawParam.MarkHeight;
                canvas.Children.Add(ll);
            }

            // draw content
            foreach (var timingMap in _timingDatas)
            {
                DrawText(
                    canvas,
                    timingMap.Symbol,
                    new Point(0, startY + (lineIndex * unitY)),
                    14,
                    //HorizontalAlignment.Left,
                    //VerticalAlignment.Center,
                    Brushes.Green);

                var xIndex = 0;

                foreach (var ch in timingMap.Timing)
                {
                    var l = new Line();
                    l.Stroke = Brushes.Black;
                    l.StrokeThickness = 2;

                    if (ch == '~')
                    {
                        l.X1 = startX + (xIndex * unitX);
                        l.X2 = l.X1 + unitX;

                        l.Y1 = startY + (lineIndex * unitY);
                        l.Y2 = l.Y1;
                    }
                    else if (ch == '_')
                    {
                        l.X1 = startX + (xIndex * unitX);
                        l.X2 = l.X1 + unitX;

                        l.Y1 = startY + (lineIndex * unitY) + unitY;
                        l.Y2 = l.Y1;
                    }
                    else if (ch == '|')
                    {
                        labelIndex.Add(xIndex);
                    }

                    // draw connection line
                    if (xIndex != 0)
                        if (_lastPoint != (new Point(l.X1, l.Y1)))
                        {
                            var conjection = new Line()
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,

                                X1 = _lastPoint.X,
                                Y1 = _lastPoint.Y,

                                X2 = l.X1,
                                Y2 = l.Y1
                            };
                            canvas.Children.Add(conjection);
                        }

                    _lastPoint = new Point(l.X2, l.Y2);
                    xIndex++;
                    canvas.Children.Add(l);
                }

                lineIndex++;
                startY += verticalSpace;
            }

            var labelIndexChar = 0;
            foreach (var i in labelIndex)
            {
                var verticalLine = new Line();

                verticalLine.StrokeDashArray = new DoubleCollection(new double[] { 1, 2 });
                verticalLine.StrokeThickness = 2;
                verticalLine.Stroke = Brushes.Green;

                verticalLine.X1 = startX + i * unitX;
                verticalLine.X2 = verticalLine.X1;

                verticalLine.Y1 = DrawParam.StartY;
                verticalLine.Y2 = DrawParam.StartY + (lineIndex * (unitY + verticalSpace));

                var c = Convert.ToChar(labelIndexChar++ + 97);

                DrawText(
                    canvas,
                    c.ToString(),
                    new Point(verticalLine.X1 - 2, verticalLine.Y1 + 5),
                    12,
                    //HorizontalAlignment.Left,
                    //VerticalAlignment.Center,
                    Brushes.Green);

                canvas.Children.Add(verticalLine);
            }
        }

        private double MeasureTextWidth(string text, double font_size)
        {
            var label = new Label();
            label.Content = text;
            label.FontSize = font_size;

            // Position the label.
            label.Measure(new Size(double.MaxValue, double.MaxValue));
            return label.DesiredSize.Width;
        }

        // Position a label at the indicated point.
        private void DrawText(Canvas can, string text, Point location,
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

        /// <summary>
        /// Copies a UI element to the clipboard as an image.
        /// </summary>
        /// <param name="element">The element to copy.</param>
        public void CopyUIElementToClipboard(FrameworkElement element)
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

        private void CreateSaveBitmap(Canvas canvas, string filename)
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

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawTiming(myCanvas);
        }

    }

    public static class DrawParam
    {
        public static int UnitX { get; set; }
        public static int UnitY { get; set; }
        public static int Margin { get; set; }
        public static int VerticalSpace { get; set; }
        public static int StartX { get; set; }
        public static int StartY { get; set; }

        public static int MarkHeight { get; set; }
    }
}
