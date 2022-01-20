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

        /// <summary>
        /// for global variables
        /// </summary>

        bool _isDrawing = false;
        double _lastX = -1;
        double _lastY = -1;
        int _choice = ShapeType.Line;
        List<IShape> _shapes = new List<IShape>();
        IShape _preview = new Line2D();

        // property of a shape
        static int _currentThickness = 1;
        static SolidColorBrush _currentColor = new SolidColorBrush(Colors.Red);
        class ShapeType
        {
            public const int Line = 1;
            public const int Rectangle = 2;
            public const int Ellipse = 3;
            public const int Square = 4;
            public const int Circle = 5;
        }

        /// <summary>
        /// Implement interface and child class
        /// </summary>

        interface IShape
        {
            string Name { get; }

            SolidColorBrush Brush { get; set; }
            int Thickness { get; set; }

            void HandleStart(double x, double y);
            void HandleEnd(double x, double y);

            UIElement Draw();
            UIElement Draw(SolidColorBrush brush);

            UIElement Draw(SolidColorBrush brush, int thickness);
        }

        class Point2D : IShape
        {
            public double X { get; set; }
            public double Y { get; set; }

            public SolidColorBrush Brush { get; set; }
            public string Name => "Point";

            public int Thickness { get ; set; }

            public void HandleStart(double x, double y)
            {
                X = x;
                Y = y;
            }

            public void HandleEnd(double x, double y)
            {
                X = x;
                Y = y;
            }

            public UIElement Draw()
            {
                Line line = new Line()
                {
                    X1 = X,
                    Y1 = Y,
                    X2 = X,
                    Y2 = Y,
                    StrokeThickness = _currentThickness,
                    Stroke = new SolidColorBrush(Colors.Red)
                };
                return line;
            }

            public UIElement Draw(SolidColorBrush brush)
            {
                Line line = new Line()
                {
                    X1 = X,
                    Y1 = Y,
                    X2 = X,
                    Y2 = Y,
                    StrokeThickness = _currentThickness,
                    Stroke = brush
                };
                return line;
            }

            public UIElement Draw(SolidColorBrush brush, int thickness)
            {
                Line line = new Line()
                {
                    X1 = X,
                    Y1 = Y,
                    X2 = X,
                    Y2 = Y,
                    StrokeThickness = thickness,
                    Stroke = brush
                };
                return line;
            }
        }

        class Line2D : IShape
        {

            private Point2D _start = new Point2D();
            private Point2D _end = new Point2D();

            public SolidColorBrush Brush { get; set; }
            public string Name => "Line";

            public int Thickness { get; set; }

            public void HandleStart(double x, double y)
            {
                _start = new Point2D() { X = x, Y = y };
            }

            public void HandleEnd(double x, double y)
            {
                _end = new Point2D() { X = x, Y = y };
            }

            public UIElement Draw()
            {
                Line line = new Line()
                {
                    X1 = _start.X,
                    Y1 = _start.Y,
                    X2 = _end.X,
                    Y2 = _end.Y,
                    StrokeThickness = _currentThickness,
                    Stroke = new SolidColorBrush(Colors.Red)

                };

                return line;
            }

            public UIElement Draw(SolidColorBrush brush)
            {
                Line line = new Line()
                {
                    X1 = _start.X,
                    Y1 = _start.Y,
                    X2 = _end.X,
                    Y2 = _end.Y,
                    StrokeThickness = _currentThickness,
                    //Stroke = new SolidColorBrush(Colors.Red)
                    Stroke = brush

                };

                return line;
            }

            public UIElement Draw(SolidColorBrush brush, int thickness)
            {
                Line line = new Line()
                {
                    X1 = _start.X,
                    Y1 = _start.Y,
                    X2 = _end.X,
                    Y2 = _end.Y,
                    StrokeThickness = thickness,
                    //Stroke = new SolidColorBrush(Colors.Red)
                    Stroke = brush

                };

                return line;
            }
        }

        class Rectangle2D : IShape
        {
            private Point2D _leftTop = new Point2D();
            private Point2D _rightBottom = new Point2D();
            public SolidColorBrush Brush { get; set; }
            public string Name => "Rectangle";

            public int Thickness { get; set; }

            public void HandleStart(double x, double y)
            {
                _leftTop = new Point2D() { X = x, Y = y };
            }

            public void HandleEnd(double x, double y)
            {
                _rightBottom = new Point2D() { X = x, Y = y };
            }

            public UIElement Draw()
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
                    StrokeThickness = _currentThickness,
                    Stroke = new SolidColorBrush(Colors.Red),

                    

                };

                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);

                return rect;
            }

            public UIElement Draw(SolidColorBrush brush)
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

                    StrokeThickness = _currentThickness,
                    Stroke = brush

                };

                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);

                return rect;
            }

            public UIElement Draw(SolidColorBrush brush, int thickness)
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

                    StrokeThickness = thickness,
                    Stroke = brush

                };

                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);

                return rect;
            }
        }

        class Ellipse2D : IShape
        {
            private Point2D _leftTop = new Point2D();
            private Point2D _rightBottom = new Point2D();
            public SolidColorBrush Brush { get; set; }
            public string Name => "Ellipse";

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
            public UIElement Draw()
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
                    Stroke = new SolidColorBrush(Colors.Red),
                    
                    StrokeThickness = _currentThickness,

                };

                Canvas.SetLeft(ellipse, left);
                Canvas.SetTop(ellipse, top);

                return ellipse;
            }

            public UIElement Draw(SolidColorBrush brush)
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

                    StrokeThickness = _currentThickness,

                };

                Canvas.SetLeft(ellipse, left);
                Canvas.SetTop(ellipse, top);

                return ellipse;
            }

            public UIElement Draw(SolidColorBrush brush, int thickness)
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

                };

                Canvas.SetLeft(ellipse, left);
                Canvas.SetTop(ellipse, top);

                return ellipse;
            }
        }


        class Circle2D : IShape
        {

            private Point2D _leftTop = new Point2D();
            private Point2D _rightBottom = new Point2D();
            public SolidColorBrush Brush { get; set; }
            public string Name => "Circle";

            public int Thickness { get; set; }

            public void HandleStart(double x, double y)
            {
                _leftTop.X = x;
                _leftTop.Y = y;
            }
            public void HandleEnd(double x, double y)
            {
                _rightBottom.X = x;

                var width = _rightBottom.X - _leftTop.X;
                
                _rightBottom.Y = _leftTop.Y + width; // width = height 
            }


            public UIElement Draw()
            {
                double width = Math.Abs(_rightBottom.X - _leftTop.X);
                double height = Math.Abs(_rightBottom.Y - _leftTop.Y);

                var circle = new Ellipse()
                {
                    Width = width,
                    Height = height,
                    Stroke = new SolidColorBrush(Colors.Red),
                    StrokeThickness = 1
                };

                if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _leftTop.X);
                    Canvas.SetTop(circle, _leftTop.Y);
                }
                else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _rightBottom.X);
                    Canvas.SetTop(circle, _leftTop.Y);
                }
                else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _leftTop.X);
                    Canvas.SetTop(circle, _rightBottom.Y);
                }
                else
                {
                    Canvas.SetLeft(circle, _rightBottom.X);
                    Canvas.SetTop(circle, _rightBottom.Y);
                }

                return circle;
            }


            public UIElement Draw(SolidColorBrush brush)
            {
                double width = Math.Abs(_rightBottom.X - _leftTop.X);
                double height = Math.Abs(_rightBottom.Y - _leftTop.Y);

                var circle = new Ellipse()
                {
                    Width = width,
                    Height = height,
                    Stroke = brush,
                    StrokeThickness = 1
                };

                if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _leftTop.X);
                    Canvas.SetTop(circle, _leftTop.Y);
                }
                else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _rightBottom.X);
                    Canvas.SetTop(circle, _leftTop.Y);
                }
                else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _leftTop.X);
                    Canvas.SetTop(circle, _rightBottom.Y);
                }
                else
                {
                    Canvas.SetLeft(circle, _rightBottom.X);
                    Canvas.SetTop(circle, _rightBottom.Y);
                }

                return circle;
            }

            public UIElement Draw(SolidColorBrush brush, int thickness)
            {
                double width = Math.Abs(_rightBottom.X - _leftTop.X);
                double height = Math.Abs(_rightBottom.Y - _leftTop.Y);

                var circle = new Ellipse()
                {
                    Width = width,
                    Height = height,
                    Stroke = brush,
                    StrokeThickness = thickness
                };

                if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _leftTop.X);
                    Canvas.SetTop(circle, _leftTop.Y);
                }
                else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _rightBottom.X);
                    Canvas.SetTop(circle, _leftTop.Y);
                }
                else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
                {
                    Canvas.SetLeft(circle, _leftTop.X);
                    Canvas.SetTop(circle, _rightBottom.Y);
                }
                else
                {
                    Canvas.SetLeft(circle, _rightBottom.X);
                    Canvas.SetTop(circle, _rightBottom.Y);
                }

                return circle;
            }
        }

 
        class Square2D : IShape
        {
            private Point2D _leftTop = new Point2D();
            private Point2D _rightBottom = new Point2D();
            public string Name => "Square";

            public SolidColorBrush Brush { get ; set ; }
            public int Thickness { get; set; }

            public UIElement Draw()
            {

                double width = Math.Abs(_rightBottom.X - _leftTop.X);
                double height = Math.Abs(_rightBottom.Y - _leftTop.Y);

                var square = new Rectangle()
                {
                    Width = width,
                    Height = height,
                    Stroke = new SolidColorBrush(Colors.Red),
                    StrokeThickness = 1
                };

                if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(square, _leftTop.X);
                    Canvas.SetTop(square, _leftTop.Y);
                }
                else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(square, _rightBottom.X);
                    Canvas.SetTop(square, _leftTop.Y);
                }
                else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
                {
                    Canvas.SetLeft(square, _leftTop.X);
                    Canvas.SetTop(square, _rightBottom.Y);
                }
                else
                {
                    Canvas.SetLeft(square, _rightBottom.X);
                    Canvas.SetTop(square, _rightBottom.Y);
                }

                return square;
            }
            public void HandleStart(double x, double y)
            {
                _leftTop.X = x;
                _leftTop.Y = y;
            }
            public void HandleEnd(double x, double y)
            {
                _rightBottom.X = x;
                _rightBottom.Y = y;


                double width = Math.Abs(_rightBottom.X - _leftTop.X);
                double height = Math.Abs(_rightBottom.Y - _leftTop.Y);
                if (width < height)
                {
                    if (_rightBottom.Y < _leftTop.Y)
                        _rightBottom.Y = _leftTop.Y - width;
                    else
                        _rightBottom.Y = _leftTop.Y + width;
                }
                else
                if (width > height)
                {
                    if (_rightBottom.X < _leftTop.X)
                        _rightBottom.X = _leftTop.X - height;
                    else _rightBottom.X = _leftTop.X + height;
                }

            }

            public UIElement Draw(SolidColorBrush brush)
            {

                double width = Math.Abs(_rightBottom.X - _leftTop.X);
                double height = Math.Abs(_rightBottom.Y - _leftTop.Y);

                var square = new Rectangle()
                {
                    Width = width,
                    Height = height,
                    Stroke = brush,
                    StrokeThickness = 1
                };

                if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(square, _leftTop.X);
                    Canvas.SetTop(square, _leftTop.Y);
                }
                else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(square, _rightBottom.X);
                    Canvas.SetTop(square, _leftTop.Y);
                }
                else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
                {
                    Canvas.SetLeft(square, _leftTop.X);
                    Canvas.SetTop(square, _rightBottom.Y);
                }
                else
                {
                    Canvas.SetLeft(square, _rightBottom.X);
                    Canvas.SetTop(square, _rightBottom.Y);
                }

                return square;
            }

            public UIElement Draw(SolidColorBrush brush, int thickness)
            {
                double width = Math.Abs(_rightBottom.X - _leftTop.X);
                double height = Math.Abs(_rightBottom.Y - _leftTop.Y);

                var square = new Rectangle()
                {
                    Width = width,
                    Height = height,
                    Stroke = brush,
                    StrokeThickness = thickness
                };

                if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(square, _leftTop.X);
                    Canvas.SetTop(square, _leftTop.Y);
                }
                else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
                {
                    Canvas.SetLeft(square, _rightBottom.X);
                    Canvas.SetTop(square, _leftTop.Y);
                }
                else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
                {
                    Canvas.SetLeft(square, _leftTop.X);
                    Canvas.SetTop(square, _rightBottom.Y);
                }
                else
                {
                    Canvas.SetLeft(square, _rightBottom.X);
                    Canvas.SetTop(square, _rightBottom.Y);
                }

                return square;
            }
        }
        /// <summary>
        /// End of implement interface
        /// </summary>

        private void editColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorPicker = new System.Windows.Forms.ColorDialog();

            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _currentColor = new SolidColorBrush(Color.FromRgb(colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B));
            }
        }


        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void createNewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveAsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lineButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = ShapeType.Line;
            _preview = new Line2D();
        }

        private void rectangleButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = ShapeType.Rectangle;
            _preview = new Rectangle2D();
        }

        private void ellipseButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = ShapeType.Ellipse;
            _preview = new Ellipse2D();
        }

        private void circleButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = ShapeType.Circle;
            _preview = new Circle2D();
        }

        private void squareButton_Click(object sender, RoutedEventArgs e)
        {
            _choice = ShapeType.Square;
            _preview = new Square2D();
        }

        private void drawingArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
            Point pos = e.GetPosition(drawingArea);

            _lastX = pos.X;
            _lastY = pos.Y;

            _preview.HandleStart(pos.X, pos.Y);
        }

        private void drawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                Point pos = e.GetPosition(drawingArea);

                _preview.HandleEnd(pos.X, pos.Y);

                // delete old shapes
                drawingArea.Children.Clear();

                // redraw all shapes
                foreach (var shape in _shapes)
                {
                    UIElement element = shape.Draw(shape.Brush, shape.Thickness);
                    drawingArea.Children.Add(element);
                }

                // lastly, draw preview object 
                drawingArea.Children.Add(_preview.Draw(_currentColor, _currentThickness));
            }
        }

        private void drawingArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;

            Point pos = e.GetPosition(drawingArea);
            _preview.HandleEnd(pos.X, pos.Y);

            // add to shapes list & save it color + thickness

            _shapes.Add(_preview);
            _preview.Brush = _currentColor;
            _preview.Thickness = _currentThickness;

            if (_choice == ShapeType.Line)
                _preview = new Line2D();
            else if (_choice == ShapeType.Rectangle)
                _preview = new Rectangle2D();
            else if (_choice == ShapeType.Ellipse)
                _preview = new Ellipse2D();
            else if (_choice == ShapeType.Circle)
                _preview = new Circle2D();
            else if (_choice == ShapeType.Square)
                _preview = new Square2D();

            // re draw everything

            // del all first
            drawingArea.Children.Clear();

            // re draw all shape in shape list
            foreach (var shape in _shapes)
            {
                var element = shape.Draw(shape.Brush, shape.Thickness);
                drawingArea.Children.Add(element);
            }
        }

        private void sizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sizeComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    _currentThickness = 1;
                    break;
                case 1:
                    _currentThickness = 2;
                    break;
                case 2:
                    _currentThickness = 3;
                    break;
                case 3:
                    _currentThickness = 5;
                    break;
                default:
                    break;
            }
        }

        private void btnBasicBlack_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        private void btnBasicGray_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(192, 192, 192));
        }

        private void btnBasicRed_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        private void btnBasicOrange_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 165, 0));
        }

        private void btnBasicBlue_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 0, 255));
        }

        private void btnBasicGreen_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }

        private void btnBasicPurple_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(191, 64, 191));
        }

        private void btnBasicPink_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 182, 193));
        }
    }
}
