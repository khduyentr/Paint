using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
	public class CShape
	{
		protected Point2D _leftTop = new Point2D();
        protected Point2D _rightBottom = new Point2D();
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
        virtual public bool isHovering(double x, double y)
		{
            return util.isBetween(x, this._rightBottom.X, this._leftTop.X)
                && util.isBetween(y, this._rightBottom.Y, this._leftTop.Y);
		}
	}
}
