using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contract
{
    public class Point2D : IShape
    {
        public double X { get; set; }
        public double Y { get; set; }

        public string Icon { get; }

        public SolidColorBrush Brush { get; set; }
        public DoubleCollection StrokeDash { get; set; }
        public string Name => "Point";
        public int Thickness { get; set; }

        public bool isHovering(double x, double y)
		{
            return false;
		}
      

        public void HandleStart(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void HandleEnd(double x, double y)
        {
            X = x;
            Y = y;
        }


        public UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash)
        {
            Line line = new Line()
            {
                X1 = X,
                Y1 = Y,
                X2 = X,
                Y2 = Y,
                StrokeThickness = thickness,
                Stroke = brush,
                StrokeDashArray = dash
            };

			return line;
		}


        public IShape Clone()
        {
            return new Point2D();
        }
        public Point2D deepCopy()
		{
            Point2D temp = new Point2D();
            temp.Y = this.Y;
            temp.X = this.X;
            return temp;
		}
    }
}
