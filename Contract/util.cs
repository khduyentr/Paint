using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
	public class util
	{
		public static bool isBetween(double x, double x1, double x2) {
			if (x1 > x2)
				return x < x1 && x > x2;
			else
				return x < x2 && x > x1;
		}
	}
}
