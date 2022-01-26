using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contract
{
	public class CShape
	{
		protected Point2D _leftTop = new Point2D();
        protected Point2D _rightBottom = new Point2D();
        protected double _rotateAngle = 0;
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

        public void setRotateAngle(double angle)
		{
            this._rotateAngle = angle;
		}

        public double getRotateAngle()
		{
            return this._rotateAngle;
		}

        virtual public Point2D getCenterPoint()
		{
            Point2D centerPoint = new Point2D();
            var left = Math.Min(_rightBottom.X, _leftTop.X);
            var top = Math.Min(_rightBottom.Y, _leftTop.Y);

            centerPoint.X = ((_leftTop.X + _rightBottom.X) / 2);
            centerPoint.Y = ((_leftTop.Y + _rightBottom.Y) / 2);
            return centerPoint; 
		}

        virtual public bool isHovering(double x, double y)
		{
            return util.isBetween(x, this._rightBottom.X, this._leftTop.X)
                && util.isBetween(y, this._rightBottom.Y, this._leftTop.Y);
		}

        virtual public List<controlPoint> GetControlPoints()
		{
            List<controlPoint> controlPoints = new List<controlPoint>();
            
            controlPoint diagPointTopLeft = new controlPoint();
            diagPointTopLeft.setPoint(_leftTop.X, _leftTop.Y);
            diagPointTopLeft.edge = "topleft";

            controlPoint diagPointBottomLeft = new controlPoint();
            diagPointBottomLeft.setPoint(_leftTop.X, RightBottom.Y);
            diagPointBottomLeft.edge = "bottomleft";

            controlPoint diagPointTopRight = new controlPoint();
            diagPointTopRight.setPoint(_rightBottom.X, _leftTop.Y);
            diagPointTopRight.edge = "topright";

            controlPoint diagPointBottomRight = new controlPoint();
            diagPointBottomRight.setPoint(_rightBottom.X, _rightBottom.Y);
            diagPointBottomRight.edge = "bottomright";

            //one way control Point

            controlPoint diagPointRight = new controlPoint();
            diagPointRight.setPoint(_rightBottom.X, (_rightBottom.Y + _leftTop.Y) / 2);
            diagPointRight.edge = "right";

            controlPoint diagPointLeft = new controlPoint();
            diagPointLeft.setPoint(_leftTop.X, (_rightBottom.Y + _leftTop.Y) / 2);
            diagPointLeft.edge = "left";
    
            controlPoint diagPointTop = new controlPoint();
            diagPointTop.setPoint((_leftTop.X + _rightBottom.X) / 2, _leftTop.Y);
            diagPointTop.edge = "top";

            controlPoint diagPointBottom = new controlPoint();
            diagPointBottom.setPoint((_leftTop.X + _rightBottom.X) / 2, _rightBottom.Y);
            diagPointBottom.edge = "bottom";


            controlPoint angleControlPoint = new rotatePoint();
            angleControlPoint.setPoint((_rightBottom.X + _leftTop.X)/2, Math.Min(_rightBottom.Y, _leftTop.Y) - 50);
            angleControlPoint.edge = "rotate";

            controlPoint moveControlPoint = new controlPoint();
            moveControlPoint.setPoint((_leftTop.X + _rightBottom.X) / 2, (_leftTop.Y + _rightBottom.Y) / 2);
            moveControlPoint.edge = "center";

            controlPoints.Add(diagPointTopLeft);
            controlPoints.Add(diagPointTopRight);
            controlPoints.Add(diagPointBottomLeft);
            controlPoints.Add(diagPointBottomRight);

            controlPoints.Add(diagPointRight);
            controlPoints.Add(diagPointLeft);
            controlPoints.Add(diagPointBottom);
            controlPoints.Add(diagPointTop);

            controlPoints.Add(angleControlPoint);
            controlPoints.Add(moveControlPoint);

            return controlPoints;
		}

        virtual public UIElement controlOutline()
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
                StrokeThickness = 2,
                Stroke = Brushes.Black,
                StrokeDashArray = { 4, 2, 4 }
            };

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);

            RotateTransform transform = new RotateTransform(this._rotateAngle);
            transform.CenterX = width * 1.0 / 2;
            transform.CenterY = height * 1.0 / 2;

            rect.RenderTransform = transform;

            return rect;
		}
	}
}
