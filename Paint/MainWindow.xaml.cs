using Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


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
        private bool _isEditMode = false;
        private bool _isSaved = false;

        //memory buffers
        private List<IShape> _shapes = new List<IShape>();
        private Stack<IShape> _buffer = new Stack<IShape>();
        private IShape _preview = null;
        private string _selectedShapeName = "";
        private List<IShape> _copyBuffers = new List<IShape>();
        private List<IShape> _chosedShapes = new List<IShape>();

        // Edit more variables
        private double editPreviousX = -1;
        private double editPreviousY = -1;
        private List<controlPoint> _controlPoints = new List<controlPoint>();

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

                //check if shape folder exist

                if (!Directory.Exists(folder + "/shapes"))
                    return;

                var fis = new DirectoryInfo(folder).GetFiles("shapes/*.dll");

                Console.Write(fis.Count());

                foreach (var f in fis)
                {
                    var assembly = Assembly.LoadFile(f.FullName);
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (type.IsClass && typeof(IShape).IsAssignableFrom(type))
                        {
                            IShape shape = (IShape)Activator.CreateInstance(type);
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

            if (this.allShape.Count == 0)
                return;

            _selectedShapeName = allShape[0].Name;
            _preview = _factory.Create(_selectedShapeName);

            //Handle kerydown event for entire application
            var window = GetWindow(this);
            window.KeyDown += HandleKeyPress;
        }

        //keyborad event
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            //can remove
            switch (e.Key)
            {
                case Key.C:
                    {
                        if (Keyboard.IsKeyDown(Key.X))
                            Console.WriteLine("AAA");
                        else
                            Console.WriteLine("A");
                        break;
                    }
            }
            return;
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
            if (this.allShape.Count == 0)
                return;

            if (this._isEditMode)
            {
                if (Mouse.RightButton == MouseButtonState.Pressed)
                {
                    _chosedShapes.Clear();
                    RedrawCanvas();
                    return;
                }
                return;
            }

            _isDrawing = true;
            Point pos = e.GetPosition(drawingArea);

            _preview.HandleStart(pos.X, pos.Y);
        }

        private void drawingArea_MouseMove(object sender, MouseEventArgs e)
        {

            //mouse change
            bool isChange = false;
            if (_chosedShapes.Count == 1)
            {
                CShape shape1 = (CShape)_chosedShapes[0];
                Point currentPos1 = e.GetPosition(drawingArea);
                for (int i = 0; i < _controlPoints.Count; i++)
                {
                    if (_controlPoints[i].isHovering(shape1.getRotateAngle(), currentPos1.X, currentPos1.Y))
                    {
                        switch (_controlPoints[i].getEdge(shape1.getRotateAngle()))
                        {
                            case "topleft" or "bottomright":
                                {
                                    Mouse.OverrideCursor = Cursors.SizeNWSE;
                                    break;
                                }
                            case "topright" or "bottomleft":
                                {
                                    Mouse.OverrideCursor = Cursors.SizeNESW;
                                    break;
                                }
                            case "top" or "bottom":
                                {
                                    Mouse.OverrideCursor = Cursors.SizeNS;
                                    break;
                                }
                            case "left" or "right":
                                {
                                    Mouse.OverrideCursor = Cursors.SizeWE;
                                    break;
                                }
                        }

                        if (_controlPoints[i].type == "move" || _controlPoints[i].type == "rotate")
                            Mouse.OverrideCursor = Cursors.Hand;

                        isChange = true;
                        break;
                    }
                };

                if (!isChange)
                    Mouse.OverrideCursor = null;
            }


            if (this._isEditMode)
            {
                if (_chosedShapes.Count < 1)
                    return;

                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    return;

                Point currentPos = e.GetPosition(drawingArea);

                double dx, dy;

                if (editPreviousX == -1 || editPreviousY == -1)
                {
                    editPreviousX = currentPos.X;
                    editPreviousY = currentPos.Y;
                    return;
                }

                dx = currentPos.X - editPreviousX;
                dy = currentPos.Y - editPreviousY;

                if (_chosedShapes.Count > 1)
                {
                    //handle multiple shapes

                    _chosedShapes.ForEach(E =>
                    {
                        CShape K = (CShape)E;

                        K.LeftTop.X = K.LeftTop.X + dx;
                        K.LeftTop.Y = K.LeftTop.Y + dy;
                        K.RightBottom.X = K.RightBottom.X + dx;
                        K.RightBottom.Y = K.RightBottom.Y + dy;
                    });

                }
                else
                {
                    // handle only one shapes
                    /*
					Console.WriteLine($"dx {dx}| dy {dy}");
					Console.WriteLine($"currentPos {currentPos.X}| {currentPos.Y}");
					Console.WriteLine($"x {editPreviousX}| y {editPreviousY}");
					*/

                    //controlPoint detect part
                    CShape shape = (CShape)_chosedShapes[0];
                    _controlPoints.ForEach(ctrlPoint =>
                    {
                        List<cord> edges = new List<cord>()
                        {
                        new cord(shape.LeftTop),      // 0 xt
                        new cordY(shape.LeftTop),      // 1 yt
                        new cord(shape.RightBottom),  // 2 xb
                        new cordY(shape.RightBottom)   // 3 yb
						};

                        List<int> rotate0 = new List<int>
                        {
                        0, 1, 2, 3
                        };
                        List<int> rotate90 = new List<int>
                        {
                        //xt, yt, xb, xb
                        3, 0, 1, 2
                        };
                        List<int> rotate180 = new List<int>
                        {
                        //xt, yt, xb, xb
                        2, 3, 0, 1
                        };
                        List<int> rotate270 = new List<int>
                        {
                        //xt, yt, xb, xb
                        1, 2, 3, 0
                        };

                        List<List<int>> rotateList = new List<List<int>>()
                        {
                        rotate0,
                        rotate90,
                        rotate180,
                        rotate270
                        };

                        double rot = shape.getRotateAngle();
                        int index = 0;

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

                        if (ctrlPoint.isHovering(shape.getRotateAngle(), currentPos.X, currentPos.Y))
                        {
                            switch (ctrlPoint.type)
                            {
                                case "rotate":
                                    {
                                        const double RotateFactor = 180.0 / 270;
                                        double alpha = Math.Abs(dx + dy);

                                        Point2D v = shape.getCenterPoint();

                                        double xv = editPreviousX - v.X;
                                        double yv = editPreviousY - v.Y;

                                        double angle = Math.Atan2(dx * yv - dy * xv, dx * xv + dy * yv);

                                        if (angle > 0)
                                            shape.setRotateAngle(shape.getRotateAngle() - alpha * RotateFactor);
                                        else
                                            shape.setRotateAngle(shape.getRotateAngle() + alpha * RotateFactor);
                                        break;
                                    }

                                case "move":
                                    {
                                        shape.LeftTop.X = shape.LeftTop.X + dx;
                                        shape.LeftTop.Y = shape.LeftTop.Y + dy;
                                        shape.RightBottom.X = shape.RightBottom.X + dx;
                                        shape.RightBottom.Y = shape.RightBottom.Y + dy;
                                        break;
                                    }

                                case "diag":
                                    {
                                        Point2D handledXY = ctrlPoint.handle(shape.getRotateAngle(), dx, dy);

                                        switch (index)
                                        {
                                            case 1:
                                                handledXY.X *= -1;
                                                break;
                                            case 2:
                                                {
                                                    handledXY.Y *= -1;
                                                    handledXY.X *= -1;
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    handledXY.Y *= -1;
                                                    break;
                                                }
                                        }


                                        switch (ctrlPoint.getEdge(shape.getRotateAngle()))
                                        {
                                            case "topleft":
                                                {
                                                    edges[rotateList[index][0]].setCord(handledXY.X);
                                                    edges[rotateList[index][1]].setCord(handledXY.Y);
                                                    edges[rotateList[index][2]].setCord(-handledXY.X);
                                                    edges[rotateList[index][3]].setCord(-handledXY.Y);
                                                    break;
                                                }
                                            case "topright":
                                                {
                                                    edges[rotateList[index][2]].setCord(handledXY.X);
                                                    edges[rotateList[index][1]].setCord(handledXY.Y);
                                                    edges[rotateList[index][0]].setCord(-handledXY.X);
                                                    edges[rotateList[index][3]].setCord(-handledXY.Y);
                                                    break;
                                                }
                                            case "bottomright":
                                                {
                                                    edges[rotateList[index][2]].setCord(handledXY.X);
                                                    edges[rotateList[index][3]].setCord(handledXY.Y);
                                                    edges[rotateList[index][0]].setCord(-handledXY.X);
                                                    edges[rotateList[index][1]].setCord(-handledXY.Y);
                                                    break;
                                                }
                                            case "bottomleft":
                                                {
                                                    edges[rotateList[index][0]].setCord(handledXY.X);
                                                    edges[rotateList[index][3]].setCord(handledXY.Y);
                                                    edges[rotateList[index][2]].setCord(-handledXY.X);
                                                    edges[rotateList[index][1]].setCord(-handledXY.Y);
                                                    break;
                                                }
                                            case "right":
                                                {
                                                    edges[rotateList[index][2]].setCord(handledXY.X);
                                                    edges[rotateList[index][0]].setCord(-handledXY.X);
                                                    break;
                                                }
                                            case "left":
                                                {
                                                    edges[rotateList[index][0]].setCord(handledXY.X);
                                                    edges[rotateList[index][2]].setCord(-handledXY.X);
                                                    break;
                                                }
                                            case "top":
                                                {
                                                    edges[rotateList[index][1]].setCord(handledXY.Y);
                                                    edges[rotateList[index][3]].setCord(-handledXY.Y);
                                                    break;
                                                }
                                            case "bottom":
                                                {
                                                    edges[rotateList[index][3]].setCord(handledXY.Y);
                                                    edges[rotateList[index][1]].setCord(-handledXY.Y);
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }
                        }

                    });
                }


                editPreviousX = currentPos.X;
                editPreviousY = currentPos.Y;

                RedrawCanvas();
                return;
            }

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
            if (this.allShape.Count == 0)
                return;

            _isDrawing = false;

            if (this._isEditMode)
            {

                if (e.ChangedButton != MouseButton.Left)
                    return;

                Point currentPos = e.GetPosition(drawingArea);
                for (int i = this._shapes.Count - 1; i >= 0; i--)
                {
                    CShape temp = (CShape)_shapes[i];
                    if (temp.isHovering(currentPos.X, currentPos.Y))
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            if (!_chosedShapes.Contains(_shapes[i]))
                                this._chosedShapes.Add(_shapes[i]);
                            else
                                this._chosedShapes.Remove(_shapes[i]);
                        }
                        else
                        {
                            _chosedShapes.Clear();
                            this._chosedShapes.Add(_shapes[i]);
                        }

                        RedrawCanvas();
                        break;
                    }
                }

                this.editPreviousX = -1;
                this.editPreviousY = -1;

                return;
            }

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

        private void drawingArea_MouseLeave(object sender, MouseEventArgs e)
        {
            //this._isDrawing = false;
        }
        private void drawingArea_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.allShape.Count == 0)
                return;

            if (this._isEditMode)
                return;

            if (Mouse.LeftButton != MouseButtonState.Pressed && this._isDrawing)
            {
                //wish there is a better solution like
                // this.drawingArea_MouseUp(sender, e)
                // but e is not MouseButtonEventArgs (;-;)
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

        #region color button

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

        #endregion

        private void iconListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.allShape.Count == 0)
                return;

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
            if (this.allShape.Count == 0)
                return;

            _isSaved = false;
            _isDrawing = false;
            _isEditMode = false;

            _chosedShapes.Clear();

            _shapes.Clear();

            _selectedShapeName = allShape[0].Name;
            _preview = _factory.Create(_selectedShapeName);

            _currentThickness = 1;
            _currentColor = new SolidColorBrush(Colors.Red);
            _currentDash = null;

            _backgroundImagePath = "";

            dashComboBox.SelectedIndex = 0;
            sizeComboBox.SelectedIndex = 0;

            EditMode.Header = "Draw Mode";
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
            Console.WriteLine(_shapes.Count);
            foreach (var shape in _shapes)
            {
                var element = shape.Draw(shape.Brush, shape.Thickness, shape.StrokeDash);
                drawingArea.Children.Add(element);
            }

            //control Point display ontop
            //rework
            if (_isEditMode && _chosedShapes.Count > 0)
            {
                _chosedShapes.ForEach(shape =>
                {
                    CShape chosedShape = (CShape)shape;
                    drawingArea.Children.Add(chosedShape.controlOutline());

                    //if only chose one shape
                    if (_chosedShapes.Count == 1)
                    {
                        List<controlPoint> ctrlPoints = chosedShape.GetControlPoints();
                        this._controlPoints = ctrlPoints;
                        ctrlPoints.ForEach(K =>
                        {
                            drawingArea.Children.Add(K.drawPoint(chosedShape.getRotateAngle(), chosedShape.getCenterPoint()));
                        });
                    }
                });
            }
        }

        //Tools tab event

        private void EditMode_Click(object sender, RoutedEventArgs e)
        {
            this._isEditMode = !this._isEditMode;
            if (_isEditMode)
                EditMode.Header = "Edit Mode";
            else EditMode.Header = "Draw Mode";

            if (!this._isEditMode)
                this._chosedShapes.Clear();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (this._isEditMode)
            {

                _chosedShapes.ForEach(k =>
               {
                   _shapes.Remove(k);
               });

                _chosedShapes.Clear();
                RedrawCanvas();
            }
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            if (this._isEditMode)
            {
                _chosedShapes.ForEach(K =>
                {
                    _copyBuffers.Add(K);
                });
            }
        }

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this._isEditMode)
            {
                _copyBuffers.ForEach(K =>
                {
                    CShape temp = (CShape)K;
                    _shapes.Add((IShape)temp.deepCopy());
                });
                RedrawCanvas();
            }

        }

        private void Group_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
