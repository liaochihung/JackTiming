using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JackTiming
{
    public class MarkerDrawer
    {
        public static Canvas Canvas { get; set; }

        public void Draw(int max)
        {
            // draw markline
            var markline = new Line();

            markline.Stroke = Brushes.Blue;
            markline.StrokeThickness = 1;
            markline.X1 = DrawParam.StartX;
            markline.X2 = DrawParam.StartX + (max * DrawParam.UnitX);
            markline.Y1 = 1;
            markline.Y2 = 1;
            Canvas.Children.Add(markline);

            // markline's vertical line
            for (int i = 0; i <= max; i++)
            {
                var line = new Line();

                line.Stroke = Brushes.Blue;
                line.StrokeThickness = 1;
                line.X1 = i * DrawParam.UnitX + DrawParam.StartX;
                line.X2 = line.X1;
                line.Y1 = 1;
                line.Y2 = DrawParam.MarkHeight;

                Canvas.Children.Add(line);
            }
        }
    }
}