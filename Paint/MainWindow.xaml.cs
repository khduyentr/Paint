using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void editColorButton_Click(object sender, RoutedEventArgs e)
        {
           System.Windows.Forms.ColorDialog colorPicker = new System.Windows.Forms.ColorDialog();
        
            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // to get color in general: color = colorPicker.Color;
                // or
                // 
                // new SolidColorBrush(FromArgb(colorPicker.Color.A, colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B))
            }
        }


        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPicker.SelectedColor.HasValue)
            {
                //Color C = colorPicker.SelectedColor.Value;
                //int Red = C.R;
                //int Green = C.G;
                //int Blue = C.B;
                //long colorVal = Convert.ToInt64(Blue * (Math.Pow(256, 0)) + Green * (Math.Pow(256, 1)) + Red * (Math.Pow(256, 2)));
            }
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
