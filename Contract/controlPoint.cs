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
	public class controlPoint
	{
		protected const int size = 12;
		protected Point2D point;
		protected Point2D centrePoint;

		virtual public string type => "diag";

		virtual public string edge { get; set; }
		public controlPoint()
		{
			point = new Point2D();
		}

		public void setPoint(double x, double y)
		{
			point.X = x;
			point.Y = y;
		}

		public Point2D getPoint()
		{
			return point;
		}

		virtual public UIElement drawPoint(double angle, Point2D centrePoint)
		{
			UIElement element = new Ellipse()
			{
				Width = size,
				Height = size,
				Fill = Brushes.White,
				Stroke = Brushes.Black,
				StrokeThickness = size / 5,
			};

			this.centrePoint = centrePoint;

			//element.RenderTransform = rotateTransform;
			Point pos = new Point(point.X, point.Y);
			Point centre = new Point(centrePoint.X, centrePoint.Y);

			Point afterTransform = VectorTranform.Rotate(pos, angle, centre);

			Canvas.SetLeft(element, afterTransform.X - size / 2);
			Canvas.SetTop(element, afterTransform.Y - size / 2);
			
			return element;
		}
		virtual public bool isHovering(double angle, double x, double y)
		{
			Point pos = new Point(point.X, point.Y);
			Point centre = new Point(centrePoint.X, centrePoint.Y);

			Point afterTransform = VectorTranform.Rotate(pos, angle, centre);

			return util.isBetween(x, afterTransform.X + 15, afterTransform.X - 15)
				&& util.isBetween(y, afterTransform.Y + 15, afterTransform.Y - 15);
		}

		virtual public Point2D handle(double angle, double x, double y)
		{
			Point2D result = new Point2D();

			result.X = x;
			result.Y = y;

			return result;
		}
	}
}
