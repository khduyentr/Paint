using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rectangle2D
{
    public class Rectangle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public Point2D LeftTop   // property
        {
            get { return _leftTop; }   // get method
            set { _leftTop = value; }  // set method
        }

        public Point2D RightBottom
        {
            get { return _rightBottom; }   // get method
            set { _rightBottom = value; }  // set method
        }
        public SolidColorBrush Brush { get; set; }

        public DoubleCollection StrokeDash { get; set; }
        public string Icon => "Images/rectangle.png";
        public string Name => "Rectangle";

        public int Thickness { get; set; }

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
        }

        public UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash)
        {
            var left = Math.Min(_rightBottom.X, _leftTop.X);
            var top = Math.Min(_rightBottom.Y, _leftTop.Y);

            var right = Math.Max(_rightBottom.X, _leftTop.X);
            var bottom = Math.Max(_rightBottom.Y, _leftTop.Y);

            var width = right - left;
            var height = bottom - top;

            var rect = new Rectangle()
            {
                Width = width,
                Height = height,

                StrokeThickness = thickness,
                Stroke = brush,
                StrokeDashArray = dash
            };

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);

            return rect;
        }

        public IShape Clone()
        {
            return new Rectangle2D();
        }
    }
}
