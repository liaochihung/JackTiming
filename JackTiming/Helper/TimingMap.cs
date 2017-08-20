using System.Collections.Generic;
using System.Windows;

namespace JackTiming.ViewModel
{
    public class TimingMap
    {
        public string Symbol { get; set; }
        public string Timing { get; set; }

        public List<TimingCharacterElement> Elements { get; set; }
    }

    public class TimingCharacterElement
    {
        public List<MyLine> Lines { get; set; }

        private Point _topLeftPoint;
        public Point TopLeftPoint
        {
            get { return _topLeftPoint; }
            set
            {
                _topLeftPoint = value;

                if (Lines == null)
                    return;

                foreach (var myLine in Lines)
                {
                    myLine.ReCalcPoints(_topLeftPoint);
                }
            }
        }

        public TimingCharacterElement()
        {
            TopLeftPoint = new Point(0, 0);
            Lines = new List<MyLine>();
        }
    }

    public class MyLine
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public MyLine()
        {
            StartPoint = new Point(0, 0);
            EndPoint = new Point(0, 0);
        }

        public void ReCalcPoints(Point refPt)
        {
            StartPoint = new Point(StartPoint.X + refPt.X, StartPoint.Y + refPt.Y);
            EndPoint=new Point(EndPoint.X + refPt.X, EndPoint.Y+ refPt.Y);
            //StartPoint.Offset(refPt.X, refPt.Y);
            //EndPoint.Offset(refPt.X, refPt.Y);
        }
    }
}