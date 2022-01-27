using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Square2D
{
    public class Square2D : CShape, IShape
    {

        public string Name => "Square";

        public string Icon => "Images/square.png";

        public DoubleCollection StrokeDash { get; set; }
        public SolidColorBrush Brush { get; set; }
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

            var square = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = brush,
                StrokeThickness = thickness,
                StrokeDashArray = dash
            };

            if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
            {
                Canvas.SetLeft(square, _leftTop.X);
                Canvas.SetTop(square, _leftTop.Y);
            }
            else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
            {
                Canvas.SetLeft(square, _rightBottom.X);
                Canvas.SetTop(square, _leftTop.Y);
            }
            else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
            {
                Canvas.SetLeft(square, _leftTop.X);
                Canvas.SetTop(square, _rightBottom.Y);
            }
            else
            {
                Canvas.SetLeft(square, _rightBottom.X);
                Canvas.SetTop(square, _rightBottom.Y);
            }

            RotateTransform transform = new RotateTransform(this._rotateAngle);
            transform.CenterX = width * 1.0 / 2;
            transform.CenterY = height * 1.0 / 2;

            square.RenderTransform = transform;

            return square;
        }

        public IShape Clone()
        {
            return new Square2D();
        }
        override public CShape deepCopy()
        {
            Square2D temp = new Square2D();

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
