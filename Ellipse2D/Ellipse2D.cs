using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ellipse2D
{
    public class Ellipse2D : CShape, IShape
    {
        public int Thickness { get; set; }
        public DoubleCollection StrokeDash { get; set; }

        public SolidColorBrush Brush { get; set; }
        public string Name => "Ellipse";
        public string Icon => "Images/ellipse.png";


        public void HandleStart(double x, double y)
        {
            _leftTop.X = x;
            _leftTop.Y = y;
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom.X = x;
            _rightBottom.Y = y;
        }

        public UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash)
        {
            var left = Math.Min(_rightBottom.X, _leftTop.X);
            var top = Math.Min(_rightBottom.Y, _leftTop.Y);

            var right = Math.Max(_rightBottom.X, _leftTop.X);
            var bottom = Math.Max(_rightBottom.Y, _leftTop.Y);

            var width = right - left;
            var height = bottom - top;

            var ellipse = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = brush,

                StrokeThickness = thickness,
                StrokeDashArray = dash

            };

            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);

            RotateTransform transform = new RotateTransform(this._rotateAngle);
            transform.CenterX = width * 1.0 / 2;
            transform.CenterY = height * 1.0 / 2;

            ellipse.RenderTransform = transform;
            return ellipse;
        }

        public IShape Clone()
        {
            return new Ellipse2D();
        }
        override public CShape deepCopy()
        {
            Ellipse2D temp = new Ellipse2D();

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
