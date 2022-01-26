﻿using Contract;
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
        private bool _isEditMode = false;
        private bool _isSaved = false;

        private List<IShape> _shapes = new List<IShape>();
        private Stack<IShape> _buffer = new Stack<IShape>();
        private IShape _preview = null;
        private string _selectedShapeName = "";

        // Edit more variables
        private int _chosedShape = -1;
        private double lastClickPosX = -1;
        private double lastClickPosY = -1;
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
                    _chosedShape = -1;
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
            if (_chosedShape != -1 && _chosedShape < _shapes.Count)
            {

                CShape shape1 = (CShape)_shapes[_chosedShape];
                Point currentPos1 = e.GetPosition(drawingArea);
                for (int i = 0; i < _controlPoints.Count; i++)
                {
                    if (_controlPoints[i].isHovering(shape1.getRotateAngle(), currentPos1.X, currentPos1.Y))
                    {
                        switch (_controlPoints[i].edge)
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
                            case "rotate" or "center":
                                {
                                    Mouse.OverrideCursor = Cursors.Hand;
                                    break;
                                }
                        }
                        isChange = true;
                        break;
                    }
                };

                if (!isChange)
                    Mouse.OverrideCursor = null;

            }


            if (this._isEditMode)
            {
                if (_chosedShape == -1 || _chosedShape >= _shapes.Count)
                    return;

                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    return;

                CShape shape = (CShape)_shapes[_chosedShape];

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

                /*
				Console.WriteLine($"dx {dx}| dy {dy}");
				Console.WriteLine($"currentPos {currentPos.X}| {currentPos.Y}");
				Console.WriteLine($"x {editPreviousX}| y {editPreviousY}");
                */

                //controlPoint detect part
                _controlPoints.ForEach(ctrlPoint =>
                {
                    if (ctrlPoint.isHovering(shape.getRotateAngle(), currentPos.X, currentPos.Y))
                    {
                        switch (ctrlPoint.type)
                        {
                            case "rotate":
                                {
                                    const double RotateFactor = 180.0 / (45);
                                    double alpha = dx + dy;

                                    shape.setRotateAngle(shape.getRotateAngle() + alpha * RotateFactor);
                                    break;
                                }

                            case "diag":
                                {
                                    Point2D handledXY = ctrlPoint.handle(shape.getRotateAngle(), dx, dy);

                                    switch (ctrlPoint.edge)
                                    {
                                        case "topleft":
                                            {
                                                shape.LeftTop.X += handledXY.X;
                                                shape.LeftTop.Y += handledXY.Y;
                                                shape.RightBottom.X -= handledXY.X;
                                                shape.RightBottom.Y -= handledXY.Y;
                                                break;
                                            }
                                        case "topright":
                                            {
                                                shape.RightBottom.X += handledXY.X;
                                                shape.LeftTop.Y += handledXY.Y;
                                                shape.LeftTop.X -= handledXY.X;
                                                shape.RightBottom.Y -= handledXY.Y;
                                                break;
                                            }
                                        case "bottomright":
                                            {
                                                shape.RightBottom.X += handledXY.X;
                                                shape.RightBottom.Y += handledXY.Y;
                                                shape.LeftTop.X -= handledXY.X;
                                                shape.LeftTop.Y -= handledXY.Y;
                                                break;
                                            }
                                        case "bottomleft":
                                            {
                                                shape.LeftTop.X += handledXY.X;
                                                shape.RightBottom.Y += handledXY.Y;
                                                shape.RightBottom.X -= handledXY.X;
                                                shape.LeftTop.Y -= handledXY.Y;
                                                break;
                                            }
                                        case "right":
                                            {
                                                shape.RightBottom.X += handledXY.X;
                                                shape.LeftTop.X -= handledXY.X;
                                                break;
                                            }
                                        case "left":
                                            {
                                                shape.RightBottom.X -= handledXY.X;
                                                shape.LeftTop.X += handledXY.X;
                                                break;
                                            }
                                        case "top":
                                            {
                                                shape.RightBottom.Y -= handledXY.Y;
                                                shape.LeftTop.Y += handledXY.Y;
                                                break;
                                            }
                                        case "bottom":
                                            {
                                                shape.RightBottom.Y += handledXY.Y;
                                                shape.LeftTop.Y -= handledXY.Y;
                                                break;
                                            }
                                        case "center":
                                            {
                                                shape.LeftTop.X = shape.LeftTop.X + dx;
                                                shape.LeftTop.Y = shape.LeftTop.Y + dy;
                                                shape.RightBottom.X = shape.RightBottom.X + dx;
                                                shape.RightBottom.Y = shape.RightBottom.Y + dy;
                                                break;
                                            };
                                    }
                                    break;
                                }
                        }
                    }

                });


                /*
                if (!shape.isHovering(currentPos.X, currentPos.Y))
				{
                    //rotate factor
                    const double RotateFactor = 180.0 / (360 * 2);
                    double alpha = dx + dy;

                    shape.setRotateAngle(shape.getRotateAngle() + alpha * RotateFactor);
				}
                else
				{
                    //shape translate
                    shape.LeftTop.X = shape.LeftTop.X + dx;
                    shape.LeftTop.Y = shape.LeftTop.Y + dy;
                    shape.RightBottom.X = shape.RightBottom.X + dx;
                    shape.RightBottom.Y = shape.RightBottom.Y + dy;
				}
                */

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
                Point currentPos = e.GetPosition(drawingArea);
                for (int i = this._shapes.Count - 1; i >= 0; i--)
                {
                    CShape temp = (CShape)_shapes[i];
                    if (temp.isHovering(currentPos.X, currentPos.Y))
                    {
                        this._chosedShape = i;
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

            _chosedShape = -1;

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

            //control Point display ontop
            if (_isEditMode && _chosedShape != -1 && _chosedShape < _shapes.Count)
            {
                CShape chosedShape = (CShape)this._shapes[_chosedShape];

                drawingArea.Children.Add(chosedShape.controlOutline());

                List<controlPoint> ctrlPoints = chosedShape.GetControlPoints();
                this._controlPoints = ctrlPoints;

                ctrlPoints.ForEach(K =>
                {
                    drawingArea.Children.Add(K.drawPoint(chosedShape.getRotateAngle(), chosedShape.getCenterPoint()));
                });

                //temp draw, can be comment
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
                this._chosedShape = -1;
        }
    }
}
