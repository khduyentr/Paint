namespace Contract
{
	public class util
	{
		public static bool isBetween(double x, double x1, double x2)
		{
			if (x1 > x2)
				return x < x1 && x > x2;
			else
				return x < x2 && x > x1;
		}
	}

	//to select reference value of X or Y of point2D cord 
	public class cord
	{
		protected Point2D point;

		public cord(Point2D temp)
		{
			point = temp;
		}

		virtual public double getCord()
		{
			return point.X;
		}
		virtual public void setCord(double x)
		{
			point.X += x;
		}
	}

	public class cordY : cord
	{
		public cordY(Point2D temp) : base(temp) { }

		public override double getCord()
		{
			return point.Y;
		}

		public override void setCord(double x)
		{
			point.Y += x;
		}
	}
}
