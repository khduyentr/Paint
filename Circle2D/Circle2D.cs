using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Circle2D
{
    public class Circle2D : IShape
    {

        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public DoubleCollection StrokeDash { get; set; }

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
        public string Name => "Circle";
        public string Icon => "Images/circle.png";
        public int Thickness { get; set; }

        public void HandleStart(double x, double y)
        {
            _leftTop.X = x;
            _leftTop.Y = y;
        }
        public void HandleEnd(double x, double y)
        {
            _rightBottom.X = x;
            _rightBottom.Y = y;

            double width = Math.Abs(_rightBottom.X - _leftTop.X);
            double height = Math.Abs(_rightBottom.Y - _leftTop.Y);
            if (width < height)
            {
                if (_rightBottom.Y < _leftTop.Y)
                    _rightBottom.Y = _leftTop.Y - width;
                else
                    _rightBottom.Y = _leftTop.Y + width;
            }
            else
            if (width > height)
            {
                if (_rightBottom.X < _leftTop.X)
                    _rightBottom.X = _leftTop.X - height;
                else _rightBottom.X = _leftTop.X + height;
            }
        }

        public UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash)
        {
            double width = Math.Abs(_rightBottom.X - _leftTop.X);
            double height = Math.Abs(_rightBottom.Y - _leftTop.Y);

            var circle = new Ellipse()
            {
                Width = width,
                Height = height,
                StrokeThickness = thickness,
                Stroke = brush,
                StrokeDashArray = dash

            };

            if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
            {
                Canvas.SetLeft(circle, _leftTop.X);
                Canvas.SetTop(circle, _leftTop.Y);
            }
            else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
            {
                Canvas.SetLeft(circle, _rightBottom.X);
                Canvas.SetTop(circle, _leftTop.Y);
            }
            else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
            {
                Canvas.SetLeft(circle, _leftTop.X);
                Canvas.SetTop(circle, _rightBottom.Y);
            }
            else
            {
                Canvas.SetLeft(circle, _rightBottom.X);
                Canvas.SetTop(circle, _rightBottom.Y);
            }

            return circle;
        }

        public IShape Clone()
        {
            return new Circle2D();
        }
    }
}
