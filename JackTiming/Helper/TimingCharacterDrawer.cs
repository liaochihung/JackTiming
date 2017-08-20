using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JackTiming.ViewModel;

namespace JackTiming
{
    public class TimingCharacterDrawer
    {
        public static Canvas Canvas { get; set; }
        public string TimingCharacters { get; set; }

        private TimingMap _timingMap;
        private int _lineIndex;

        public bool IsMarker()
        {
            for (var index = 0; index < _timingMap.Timing.Length; index++)
            {
                if (_timingMap.Timing[index] == '|')
                {
                    return true;
                }
            }

            return false;
        }

        public TimingCharacterDrawer(int lineIndex, TimingMap tm)
        {
            _lineIndex = lineIndex;
            _timingMap = tm;

            // todo: check if current line is marker line
        }

        public void DrawMarker(int _lineIndex)
        {
            _labelIndex = new List<int>();

            for (var index = 0; index < _timingMap.Timing.Length; index++)
            {
                if (_timingMap.Timing[index] == '|')
                {
                    _labelIndex.Add(index);
                }
            }

            var xIndex = 0;
            var labelIndexChar = 0;

            foreach (var i in _labelIndex)
            {
                var verticalLine = new Line();

                verticalLine.StrokeDashArray = new DoubleCollection(new double[] { 1, 2 });
                verticalLine.StrokeThickness = 2;
                verticalLine.Stroke = Brushes.Green;

                verticalLine.X1 = DrawParam.StartX + i * DrawParam.UnitX;
                verticalLine.X2 = verticalLine.X1;

                verticalLine.Y1 = 10;
                verticalLine.Y2 = 10 + (_lineIndex * (DrawParam.UnitY + DrawParam.VerticalSpace));

                var c = Convert.ToChar(labelIndexChar++ + 97);

                GraphicHelper.DrawText(
                    Canvas,
                    c.ToString(),
                    new Point(verticalLine.X1 - 2, verticalLine.Y1 + 5),
                    12,
                    Brushes.Green);

                Canvas.Children.Add(verticalLine);
            }
        }

        private List<int> _labelIndex;

        public void Draw()
        {
            GraphicHelper.DrawText(
                Canvas,
                _timingMap.Symbol,
                new Point(0, DrawParam.StartY + (_lineIndex * DrawParam.UnitY)),
                14,
                Brushes.Green);

            var xIndex = 0;
            foreach (var element in _timingMap.Elements)
            {
                if (element == null)
                    continue;

                element.TopLeftPoint = new Point(
                    DrawParam.StartX + (xIndex * DrawParam.UnitX),
                    DrawParam.StartY + (_lineIndex * DrawParam.UnitY));

                // get line data and draw out
                foreach (var myLine in element.Lines)
                {
                    var line = new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,

                        X1 = myLine.StartPoint.X,
                        Y1 = myLine.StartPoint.Y,

                        X2 = myLine.EndPoint.X,
                        Y2 = myLine.EndPoint.Y
                    };

                    Canvas.Children.Add(line);
                }
                xIndex++;
            }

            // 畫出連接線
            for (int i = 1; i < _timingMap.Timing.Length; i++)
            {
                var ch1 = _timingMap.Timing[i - 1];
                var ch2 = _timingMap.Timing[i];

                if (ch1 == ch2)
                    continue;

                if (ch1 == '*' || ch2 == '*')
                    continue;
                if (ch1 == ':' || ch2 == ':')
                    continue;
                if (ch1 == '<' || ch2 == '<')
                    continue;

                if (_timingMap.Elements[i-1] == null ||
                    _timingMap.Elements[i] == null )
                    continue;

                if (_timingMap.Elements[i - 1].Lines == null)
                    continue;

                if (_timingMap.Elements[i].Lines == null)
                    continue;

                var line = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,

                    X1 = _timingMap.Elements[i - 1].Lines[0].EndPoint.X,
                    Y1 = _timingMap.Elements[i - 1].Lines[0].EndPoint.Y,

                    X2 = _timingMap.Elements[i].Lines[0].StartPoint.X,
                    Y2 = _timingMap.Elements[i].Lines[0].StartPoint.Y
                };

                Canvas.Children.Add(line);
            }

        }
    }
}