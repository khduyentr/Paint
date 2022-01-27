using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Circle2D
{
    public class Circle2D : CShape, IShape
    {

        public DoubleCollection StrokeDash { get; set; }


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
                StrokeDashArray = dash,
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
        override public CShape deepCopy()
        {
            Circle2D temp = new Circle2D();

            temp.LeftTop = this._leftTop.deepCopy();
            temp.RightBottom = this._rightBottom.deepCopy();
            temp._rotateAngle = this._rotateAngle;
            temp.Thickness = this.Thickness;

            if (this.Brush != null)
                temp.Brush = this.Brush.Clone();

            if (this.StrokeDash != null)
                temp.StrokeDash = this.StrokeDash.Clone();

            return temp;
        }
    }
}
