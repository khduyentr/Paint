using Contract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ellipse2D
{
    public class Ellipse2D : IShape
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
        public string Name => "Ellipse";
        public string Icon => "Images/ellipse.png";

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

            return ellipse;
        }

        public IShape Clone()
        {
            return new Ellipse2D();
        }
    }
}
