using Contract;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Line2D
{
    public class Line2D : IShape
    {

        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();

        public Point2D Start   // property
        {
            get { return _start; }   // get method
            set { _start = value; }  // set method
        }

        public Point2D End   // property
        {
            get { return _end; }   // get method
            set { _end = value; }  // set method
        }

        public DoubleCollection StrokeDash { get; set; }

        public SolidColorBrush Brush { get; set; }
        public string Name => "Line";
        public string Icon => "Images/line.png";

        public int Thickness { get; set; }

        public void HandleStart(double x, double y)
        {
            _start = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _end = new Point2D() { X = x, Y = y };
        }

        public UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash)
        {
            Line line = new Line()
            {
                X1 = _start.X,
                Y1 = _start.Y,
                X2 = _end.X,
                Y2 = _end.Y,
                StrokeThickness = thickness,
                Stroke = brush,
                StrokeDashArray = dash

            };
            return line;
        }

        public IShape Clone()
        {
            return new Line2D();
        }
    }
}
