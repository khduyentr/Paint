using System;
using System.Windows;
using System.Windows.Media;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        string Icon { get; }
        
        SolidColorBrush Brush { get; set; }
        int Thickness { get; set; }

        DoubleCollection StrokeDash { get; set; }

        void HandleStart(double x, double y);
        void HandleEnd(double x, double y);
        IShape Clone();

        UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash);
    }
}
