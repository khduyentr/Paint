﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        List<IShape> _shapes = new List<IShape>();
        IShape _preview = null;
        string _selectedShapeName = "";

        //Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        List<IShape> allShape = new List<IShape>();
        ShapeFactory _factory = ShapeFactory.Instance;

        // property of a shape
        static int _currentThickness = 1;
        static SolidColorBrush _currentColor = new SolidColorBrush(Colors.Red);

        /// <summary>
        /// Implement interface and child class
        /// </summary>

        interface IShape
        {
            string Name { get; }
            string Icon { get; }

            SolidColorBrush Brush { get; set; }
            int Thickness { get; set; }

            void HandleStart(double x, double y);
            void HandleEnd(double x, double y);
            IShape Clone();

            UIElement Draw(SolidColorBrush brush, int thickness);
        }

        class ShapeFactory
        {
            Dictionary<string, IShape> _prototypes;
            private static ShapeFactory _instance = null;
            private ShapeFactory()
            {
                _prototypes = new Dictionary<string, IShape>();

                IShape line = new Line2D();
                IShape rect = new Rectangle2D();
                IShape ellipse = new Ellipse2D();
                IShape circle = new Circle2D();
                IShape square = new Square2D();

                _prototypes.Add(line.Name, line);
                _prototypes.Add(rect.Name, rect);
                _prototypes.Add(ellipse.Name, ellipse);
                _prototypes.Add(circle.Name, circle);
                _prototypes.Add(square.Name, square);



                // Uncomment this block of code later to load dll file

                /*string exePath = Assembly.GetExecutingAssembly().Location;
                string folder = System.IO.Path.GetDirectoryName(exePath);
                var fis = new DirectoryInfo(folder).GetFiles("*.dll");

                foreach (var f in fis) // Lần lượt duyệt qua các file dll
                {
                    var assembly = Assembly.LoadFile(f.FullName);
                    var types = assembly.GetTypes();

                    foreach (var t in types)
                    {
                        if (t.IsClass && typeof(IShape).IsAssignableFrom(t))
                        {
                            IShape shape = (IShape)Activator.CreateInstance(t);
                            _prototypes.Add(shape.Name, shape);
                        }
                    }
                }*/

            }


            public static ShapeFactory Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new ShapeFactory();
                    return _instance;
                }
            }

            public Dictionary<string, IShape> GetDictionary()
            {
                return _prototypes;
            }

            public IShape Create(string shapeName)
            {
                IShape shape = null;

                if (_prototypes.ContainsKey(shapeName))
                    shape = _prototypes[shapeName].Clone();
                return shape;
            }


        }

        class Point2D : IShape
        {
            public double X { get; set; }
            public double Y { get; set; }

            public string Icon { get; }

            public SolidColorBrush Brush { get; set; }
            public string Name => "Point";

            public int Thickness { get; set; }

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

            public IShape Clone()
            {
                return new Point2D();
            }
        }

        class Line2D : IShape
        {

            private Point2D _start = new Point2D();
            private Point2D _end = new Point2D();

            public SolidColorBrush Brush { get; set; }
            public string Name => "Line";
            public string Icon => "Images/line.png";

            public int Thickness { get; set; }

            public void HandleStart(double x, double y)
            {
                _start = new Point2D() { X = x, Y = y };
            }

            public void HandleEnd(double x, double y)
            {
                _end = new Point2D() { X = x, Y = y };
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

            public IShape Clone()
            {
                return new Line2D();
            }
        }

        class Rectangle2D : IShape
        {
            private Point2D _leftTop = new Point2D();
            private Point2D _rightBottom = new Point2D();
            public SolidColorBrush Brush { get; set; }
            public string Icon => "Images/rectangle.png";
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

            public IShape Clone()
            {
                return new Rectangle2D();
            }
        }

        class Ellipse2D : IShape
        {
            private Point2D _leftTop = new Point2D();
            private Point2D _rightBottom = new Point2D();
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

            public IShape Clone()
            {
                return new Ellipse2D();
            }
        }

        class Circle2D : IShape
        {

            private Point2D _leftTop = new Point2D();
            private Point2D _rightBottom = new Point2D();
            public SolidColorBrush Brush { get; set; }
            public string Name => "Circle";
            public string Icon => "Images/circle.png";
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

            public UIElement Draw(SolidColorBrush brush, int thickness)
            {
                double width = Math.Abs(_rightBottom.X - _leftTop.X);
                double height = Math.Abs(_rightBottom.Y - _leftTop.Y);

                var circle = new Ellipse()
                {
                    Width = width,
                    Height = height,
                    StrokeThickness = thickness,
                    Stroke = brush,
                    
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

            public IShape Clone()
            {
                return new Circle2D();
            }
        }

        class Square2D : IShape
        {
            private Point2D _leftTop = new Point2D();
            private Point2D _rightBottom = new Point2D();
            public string Name => "Square";

            public string Icon => "Images/square.png";

            public SolidColorBrush Brush { get; set; }
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

            public IShape Clone()
            {
                return new Square2D();
            }
        }


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

            foreach (var item in _factory.GetDictionary())
            {
                var shape = item.Value as IShape;
                allShape.Add(shape);
            }

            iconListView.ItemsSource = allShape;

            _selectedShapeName = allShape[0].Name;

            _preview = _factory.Create(_selectedShapeName);

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
            MessageBox.Show("Hi");
            CreateSaveBitmap(drawingArea, @"D:/out.png");

            
        }

        private void CreateSaveBitmap(Canvas canvas, string filename)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.Width, (int)canvas.Height,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }

        private void drawingArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
            Point pos = e.GetPosition(drawingArea);

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


            // move to new preview 


            _preview = _factory.Create(_selectedShapeName);

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

        private void btnBasicYellow_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        }

        private void btnBasicBrown_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(160, 82, 45));
        }

        private void iconListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = iconListView.SelectedIndex;

            _selectedShapeName = allShape[index].Name;

            _preview = _factory.Create(_selectedShapeName);
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
