using System;
using System.Collections.Generic;
using System.Windows;

namespace JackTiming.ViewModel
{
    public static class TimingMapParser
    {
        public static List<TimingMap> Parse(string datas)
        {
            var lines = new List<string>(datas.Split(
                new string[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries));

            var data = new List<TimingMap>();

            foreach (var line in lines)
            {
                var l = line.Split('=').Length;
                // ignore if couldn't parse
                if (l <= 1)
                    continue;

                var map = new TimingMap()
                {
                    Symbol = line.Split('=')[0],
                    Timing = line.Split('=')[1],
                    Elements = new List<TimingCharacterElement>()
                };

                var hasSpecChar = false;
                for (var index = 0; index < map.Timing.Length; index++)
                {
                    var ch = map.Timing[index];
                    var item = CreateCharElement(ch, hasSpecChar);
                    map.Elements.Add(item);

                    if (ch == '[' || ch == '*' || ch == '<')
                        hasSpecChar = true;
                    else if (ch == ']' || ch == '>')
                        hasSpecChar = false;
                }

                data.Add(map);
            }

            return data;
        }

        private static TimingCharacterElement CreateCharElement(char ch, bool hasSpecialCharacter)
        {
            MyLine myLine1 = null;
            MyLine myLine2 = null;
            MyLine myLine3 = null;

            switch (ch)
            {
                case '_':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY)
                    };
                    break;
                case '~':
                    myLine1 = new MyLine
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(DrawParam.UnitX, 0)
                    };
                    break;
                case '-':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY / 2.0),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY / 2.0)
                    };
                    break;
                case '*':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY)
                    };
                    myLine2 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX, 0)
                    };
                    break;
                case '/':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX, 0)
                    };
                    break;
                case '\\':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY)
                    };
                    break;
                case '[':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(DrawParam.UnitX, 0)
                    };
                    myLine2 = new MyLine()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(0, DrawParam.UnitY)
                    };
                    myLine3 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY)
                    };
                    break;
                case ']':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(DrawParam.UnitX, 0)
                    };
                    myLine2 = new MyLine()
                    {
                        StartPoint = new Point(DrawParam.UnitX, 0),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY)
                    };
                    myLine3 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY)
                    };
                    break;
                case ':':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX / 2.0, 0)
                    };
                    myLine2 = new MyLine()
                    {
                        StartPoint = new Point(DrawParam.UnitX / 2.0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX, 0)
                    };
                    break;
                case '>':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY / 2.0)
                    };
                    myLine2 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY / 2.0)
                    };
                    break;
                case '<':
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY / 2.0),
                        EndPoint = new Point(DrawParam.UnitX, 0)
                    };
                    myLine2 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY / 2.0),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY)
                    };
                    break;
            }

            if (hasSpecialCharacter)
            {
                // we won't actually draw letter but lines here
                if (ch == ' ' || Char.IsLetterOrDigit(ch))
                {
                    myLine1 = new MyLine()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(DrawParam.UnitX, 0)
                    };
                    myLine2 = new MyLine()
                    {
                        StartPoint = new Point(0, DrawParam.UnitY),
                        EndPoint = new Point(DrawParam.UnitX, DrawParam.UnitY)
                    };
                }
            }

            TimingCharacterElement item = null;
            if (myLine1 != null)
            {
                item =
                    new TimingCharacterElement
                    {
                        Lines = new List<MyLine>()
                    };

                item.Lines.Add(myLine1);
            }

            if (myLine2 != null)
                item.Lines.Add(myLine2);

            if (myLine3 != null)
                item.Lines.Add(myLine3);

            return item;
        }
    }
}