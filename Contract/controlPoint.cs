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

		virtual public string type { get; set; }

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

		virtual public string getEdge(double angle)
		{
			string[] edge = { "topleft", "topright", "bottomright", "bottomleft" };
			int index;
			if (point.X > centrePoint.X)
				if (point.Y > centrePoint.Y)
					index = 2;
				else
					index = 1;
			else
				if (point.Y > centrePoint.Y)
				index = 3;
			else
				index = 0;

			double rot = angle;

			if (rot > 0)
				while (true)
				{
					rot -= 90;
					if (rot < 0)
						break;
					index++;

					if (index == 4)
						index = 0;
				}
			else
				while (true)
				{
					rot += 90;
					if (rot > 0)
						break;
					index--;
					if (index == -1)
						index = 3;
				};

			return edge[index];
		}

		virtual public Point2D handle(double angle, double x, double y)
		{
			Point2D result = new Point2D();

			//result.X = Math.Cos(angle) * x + Math.Sin(angle) * y;
			//result.Y = Math.Cos(angle) * y + Math.Sin(angle) * x;

			result.X = x;
			result.Y = y;

			return result;
		}
	}
}
