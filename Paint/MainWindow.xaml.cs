using Contract;
using Newtonsoft.Json;
using System;
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

        private bool _isDrawing = false;
        private bool _isSaved = false;

        private List<IShape> _shapes = new List<IShape>();
        private Stack<IShape> _buffer = new Stack<IShape>();
        private IShape _preview = null;
        private string _selectedShapeName = "";

        // Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        private List<IShape> allShape = new List<IShape>();
        private ShapeFactory _factory = ShapeFactory.Instance;

        // Shapes properties
        private static int _currentThickness = 1;
        private static SolidColorBrush _currentColor = new SolidColorBrush(Colors.Red);
        private static DoubleCollection _currentDash = null;

        private string _backgroundImagePath = "";

        /// <summary>
        /// Implement interface and child class
        /// </summary>


        class ShapeFactory
        {
            Dictionary<string, IShape> _prototypes;
            private static ShapeFactory _instance = null;
            private ShapeFactory()
            {
                _prototypes = new Dictionary<string, IShape>();

                /*IShape line = new Line2D();
                IShape rect = new Rectangle2D();
                IShape ellipse = new Ellipse2D();
                IShape circle = new Circle2D();
                IShape square = new Square2D();

                _prototypes.Add(line.Name, line);
                _prototypes.Add(rect.Name, rect);
                _prototypes.Add(ellipse.Name, ellipse);
                _prototypes.Add(circle.Name, circle);
                _prototypes.Add(square.Name, square);*/



                // Uncomment this block of code later to load dll file

                string exePath = Assembly.GetExecutingAssembly().Location;
                string folder = System.IO.Path.GetDirectoryName(exePath);
                var fis = new DirectoryInfo(folder).GetFiles("*.dll");

                foreach (var f in fis)
                {
                    var assembly = Assembly.LoadFile(f.FullName);
                    var types = assembly.GetTypes();

                    foreach (var typr in types)
                    {
                        if (typr.IsClass && typeof(IShape).IsAssignableFrom(typr))
                        {
                            IShape shape = (IShape)Activator.CreateInstance(typr);
                            _prototypes.Add(shape.Name, shape);
                        }
                    }
                }

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
            if (_backgroundImagePath.Length > 0 && _shapes.Count == 0)
            {
                _backgroundImagePath = "";
                drawingArea.Background = new SolidColorBrush(Colors.White);
            }
            if (_shapes.Count == 0)
            {
                // MessageBox.Show("This canvas is empty");
                return;
            }


            if (_isSaved)
            {
                ResetToDefault();
                return;
            }

            var result = MessageBox.Show("Do you want to save current file?", "Unsaved changes detected", MessageBoxButton.YesNoCancel);

            if (MessageBoxResult.Yes == result)
            {
                // save then reset

                // save 
                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                var serializedShapeList = JsonConvert.SerializeObject(_shapes, settings);

                // experience 
                StringBuilder builder = new StringBuilder();
                builder.Append(serializedShapeList).Append("\n").Append($"{_backgroundImagePath}");
                string content = builder.ToString();


                var dialog = new System.Windows.Forms.SaveFileDialog();

                dialog.Filter = "JSON (*.json)|*.json";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dialog.FileName;
                    File.WriteAllText(path, content);
                }

                // reset
                ResetToDefault();
                _isSaved = true;
            }
            else if (MessageBoxResult.No == result)
            {
                //reset
                ResetToDefault();
                return;
            }
            else if (MessageBoxResult.Cancel == result)
            {
                return;
            }


        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new System.Windows.Forms.OpenFileDialog();

            dialog.Filter = "JSON (*.json)|*.json";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                string[] content = File.ReadAllLines(path);

                string background = "";
                string json = content[0];
                if (content.Length > 1)
                    background = content[1];
                //string json = File.ReadAllText(path);


                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                _shapes.Clear();
                _backgroundImagePath = background;
                drawingArea.Children.Clear();

                List<IShape> containers = JsonConvert.DeserializeObject<List<IShape>>(json, settings);

                foreach (var item in containers)
                    _shapes.Add(item);

                if (_backgroundImagePath.Length != 0)
                {
                    ImageBrush brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(_backgroundImagePath, UriKind.Absolute));
                    drawingArea.Background = brush;
                }

                //MessageBox.Show($"{background}");
            }

            foreach (var shape in _shapes)
            {
                var element = shape.Draw(shape.Brush, shape.Thickness, shape.StrokeDash);
                drawingArea.Children.Add(element);
            }


        }

        private void saveFileButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            var serializedShapeList = JsonConvert.SerializeObject(_shapes, settings);

            // experience 
            StringBuilder builder = new StringBuilder();
            builder.Append(serializedShapeList).Append("\n").Append($"{_backgroundImagePath}");
            string content = builder.ToString();


            var dialog = new System.Windows.Forms.SaveFileDialog();

            dialog.Filter = "JSON (*.json)|*.json";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                File.WriteAllText(path, content);
                _isSaved = true;
            }
        }

        private void SaveCanvasToImage(Canvas canvas, string filename, string extension = "png")
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.Width, (int)canvas.Height,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();




            switch (extension)
            {
                case "png":
                    PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        pngEncoder.Save(file);
                    }
                    break;
                case "jpeg":
                    JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
                    jpegEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        jpegEncoder.Save(file);
                    }
                    break;
                case "tiff":
                    TiffBitmapEncoder tiffEncoder = new TiffBitmapEncoder();
                    tiffEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        tiffEncoder.Save(file);
                    }
                    break;
                case "bmp":

                    BmpBitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        bitmapEncoder.Save(file);
                    }
                    break;
                default:
                    break;
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
                    UIElement element = shape.Draw(shape.Brush, shape.Thickness, shape.StrokeDash);
                    drawingArea.Children.Add(element);
                }

                // lastly, draw preview object 
                drawingArea.Children.Add(_preview.Draw(_currentColor, _currentThickness, _currentDash));
            }
        }

        private void drawingArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;

            Point pos = e.GetPosition(drawingArea);
            _preview.HandleEnd(pos.X, pos.Y);

            // Ddd to shapes list & save it color + thickness
            _shapes.Add(_preview);
            _preview.Brush = _currentColor;
            _preview.Thickness = _currentThickness;
            _preview.StrokeDash = _currentDash;

            // Draw new thing -> isSaved = false
            _isSaved = false;

            // Move to new preview 
            _preview = _factory.Create(_selectedShapeName);

            // Re-draw the canvas
            RedrawCanvas();
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
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp | TIFF (*.tiff)|*.tiff";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                string extension = path.Substring(path.LastIndexOf('\\') + 1).Split('.')[1];

                SaveCanvasToImage(drawingArea, path, extension);
            }
            _isSaved = true;
        }

        private void dashComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = dashComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    _currentDash = null;
                    break;
                case 1:
                    _currentDash = new DoubleCollection() { 4, 1, 1, 1, 1, 1 };
                    break;
                case 2:
                    _currentDash = new DoubleCollection() { 1, 1 };
                    break;
                case 3:
                    _currentDash = new DoubleCollection() { 6, 1 };
                    break;
                default:
                    break;
            }
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp | TIFF (*.tiff)|*.tiff";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                _backgroundImagePath = path;

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(path, UriKind.Absolute));
                drawingArea.Background = brush;
            }
        }

        private void ResetToDefault()
        {
            _isSaved = false;
            _isDrawing = false;

            _shapes.Clear();

            _selectedShapeName = allShape[0].Name;
            _preview = _factory.Create(_selectedShapeName);

            _currentThickness = 1;
            _currentColor = new SolidColorBrush(Colors.Red);
            _currentDash = null;

            _backgroundImagePath = "";

            dashComboBox.SelectedIndex = 0;
            sizeComboBox.SelectedIndex = 0;

            drawingArea.Children.Clear();
            drawingArea.Background = new SolidColorBrush(Colors.White);
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shapes.Count == 0)
                return;
            if (_shapes.Count == 0 && _buffer.Count == 0)
                return;

            // Push last shape into buffer and remove it from final list, then re-draw canvas
            int lastIndex = _shapes.Count - 1;
            _buffer.Push(_shapes[lastIndex]);
            _shapes.RemoveAt(lastIndex);

            RedrawCanvas();
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_buffer.Count == 0)
                return;
            if (_shapes.Count == 0 && _buffer.Count == 0)
                return;

            // Pop the last shape from buffer and add it to final list, then re-draw canvas
            _shapes.Add(_buffer.Pop());
            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            drawingArea.Children.Clear();
            foreach (var shape in _shapes)
            {
                var element = shape.Draw(shape.Brush, shape.Thickness, shape.StrokeDash);
                drawingArea.Children.Add(element);
            }
        }
    }
}
