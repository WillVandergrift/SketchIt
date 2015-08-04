using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Windows.Ink;
using System.Runtime.InteropServices;
using SketchIt.CanvasObjects;

namespace SketchIt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        # region Transparent Code

        //Constants for setting the window's style
        //These are used to allow users to click through the window when ink is present
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = (-20);

        // Get this window's handle
        private static IntPtr hwnd;

        // Change the extended window style to include WS_EX_TRANSPARENT
        private static int extendedStyle;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            hwnd = new WindowInteropHelper(this).Handle;
            extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        }

        #endregion

        bool _DrawingShape = false;
        Point _ShapeStart;
        Point _ShapeEnd;
        Shape _CurrentShape;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Subscribe to various evens
            Session.DrawingStateChanged += new Session.DrawingStateChangedHandler(Session_DrawingStateChanged);
            Session.BackgroundColorChanged += new Session.BackgroundColorChangedHandler(Session_BackgroundColorChanged);
            Session.DrawingVisibilityChanged += new Session.DrawingVisibilityChangedHandler(Session_DrawingVisibilityChanged);
            Session.DrawingAttributesChanged += new Session.DrawingAttributesChangedHandler(Session_DrawingAttributesChanged);
            Session.EditingModeChanged += new Session.EditingModeChangedHandler(Session_EditingModeChanged);
            Session.AddImage += new Session.AddImageHandler(Session_AddImage);
            Session.CurrentWidthChanged += new Session.CurrentWidthChangedHandler(Session_CurrentWidthChanged);
            Session.CurrentHeightChanged += new Session.CurrentHeightChangedHandler(Session_CurrentHeightChanged);
            Session.CurrentTopChanged += new Session.CurrentTopChangedHandler(Session_CurrentTopChanged);
            Session.CurrentLeftChanged += new Session.CurrentLeftChangedHandler(Session_CurrentLeftChanged);
            Session.ClearScreen += new Session.ClearScreenHandler(Session_ClearScreen);

            //Load all required cursor files
            Session.LoadCursors();            

            //Set the main window as the owner of the toolbar window, so the toolbar will always stay on top
            Session.ToolbarWindow.Owner = this;
            Session.ToolbarWindow.Show();

            //Set the main window as the owner of the resize window
            Session.ResizeScreen.Owner = this;

            //Set the position of the drawing window
            SetWindowSize();
        }

        /// <summary>
        /// Clear the screen of any drawn content
        /// </summary>
        void Session_ClearScreen()
        {
            drawingCanvas.Children.Clear();
            drawingCanvas.Strokes.Clear();
        }

        /// <summary>
        /// Add an image to the canvas
        /// </summary>
        /// <param name="ImagePath">The full path to the image file</param>
        void Session_AddImage(string ImagePath)
        {
            AddImage(ImagePath);
        }

        /// <summary>
        /// The editing mode changed
        /// </summary>
        /// <param name="mode">Ink, Eraser, Select</param>
        void Session_EditingModeChanged(InkCanvasEditingMode mode)
        {
            drawingCanvas.EditingMode = mode;
            //drawingCanvas.EditingMode = InkCanvasEditingMode.None;

            if (mode == InkCanvasEditingMode.Ink)
            {
                drawingCanvas.Cursor = Cursors.Pen;
            }
            else if (mode == InkCanvasEditingMode.Select)
            {
                drawingCanvas.Cursor = Cursors.Hand;
            }
            else if (mode == InkCanvasEditingMode.EraseByPoint || mode == InkCanvasEditingMode.EraseByStroke)
            {
                drawingCanvas.Cursor = Session.EraserCursor;
            }
            else
            {
                drawingCanvas.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Enable/Disable Drawing
        /// </summary>
        /// <param name="drawingState"></param>
        void Session_DrawingStateChanged(bool drawingState)
        {
            if (drawingState)
            {
                //Allow the form to accept mouse input
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle);
            }
            else
            {
                //Make the form and all children transparent so mouse events pass through
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
        }

        /// <summary>
        /// Update the current drawing attributes
        /// </summary>
        /// <param name="drawingAttributes"></param>
        void Session_DrawingAttributesChanged(DrawingAttributes drawingAttributes)
        {
            drawingCanvas.DefaultDrawingAttributes = drawingAttributes;
        }

        /// <summary>
        /// Show/Hide the drawing window
        /// </summary>
        /// <param name="visibility"></param>
        void Session_DrawingVisibilityChanged(bool visibility)
        {
            if (visibility == true)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// Update the background color
        /// </summary>
        /// <param name="background"></param>
        void Session_BackgroundColorChanged(Brush background)
        {
            drawingCanvas.Background = background;
            this.Background = background;
        }

        /// <summary>
        /// Add Image Context Menu Item Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuAddImage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult imgSelectorResult = dlg.ShowDialog();

            if (imgSelectorResult == System.Windows.Forms.DialogResult.OK)
            {
                Session.AddImageToCanvas(dlg.FileName);
            }  
        }

        /// <summary>
        /// Add an image to the canvas
        /// </summary>
        void AddImage(string ImagePath)
        {
            DraggableImage moveableImage = new DraggableImage();
            moveableImage.Source = new BitmapImage(new Uri(ImagePath, UriKind.Absolute));
            moveableImage.Width = 250;
            moveableImage.Height = 250;
            drawingCanvas.Children.Add(moveableImage);
            drawingCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private void drawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (drawingCanvas.EditingMode == InkCanvasEditingMode.Select)
            {
                _DrawingShape = true;
                _ShapeStart = e.GetPosition(drawingCanvas);
            }
        }

        private void drawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_DrawingShape)
            {
                return;
            }

            Line _CurLine = new Line();

            _ShapeEnd = e.GetPosition(drawingCanvas);

            _CurLine.X1 = _ShapeStart.X;
            _CurLine.Y1 = _ShapeStart.Y;
            _CurLine.X2 = _ShapeEnd.X;
            _CurLine.Y2 = _ShapeEnd.Y;

            _CurLine.StrokeThickness = 10;
            _CurrentShape = _CurLine;            
        }

        private void drawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_DrawingShape)
            {
                return;
            }

            drawingCanvas.Children.Add(_CurrentShape);
            _DrawingShape = false;
        }

        /// <summary>
        /// Load the window size from settings
        /// </summary>
        private void SetWindowSize()
        {
            double settingWidth = Properties.Settings.Default.Width;
            double settingHeight = Properties.Settings.Default.Height;

            //Check to see if we have values already saved in settings
            if (settingWidth == 0 || settingHeight == 0)
            {
                //Settings doesn't contain any values, store defaults
                Session.CurrentTop = Top;
                Session.CurrentLeft = Left;
                Session.CurrentWidth = Width;
                Session.CurrentHeight = Height;              
            }
            else
            {
                //Saved settings were found, load them
                Session.CurrentTop = Properties.Settings.Default.Top;
                Session.CurrentLeft = Properties.Settings.Default.Left;
                Session.CurrentWidth = Properties.Settings.Default.Width;
                Session.CurrentHeight = Properties.Settings.Default.Height;
            }
        }

        /// <summary>
        /// Update the width of the window
        /// </summary>
        /// <param name="currentWidth"></param>
        void Session_CurrentWidthChanged(double currentWidth)
        {
            Width = currentWidth;

            //Save the value to settings
            Properties.Settings.Default.Width = currentWidth;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Update the height of the window
        /// </summary>
        /// <param name="currentHeight"></param>
        void Session_CurrentHeightChanged(double currentHeight)
        {
            Height = currentHeight;

            //Save the value to settings
            Properties.Settings.Default.Height = currentHeight;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Update the current top position of the window
        /// </summary>
        /// <param name="currentTop"></param>
        void Session_CurrentTopChanged(double currentTop)
        {
            Top = currentTop;

            //Save the value to settings
            Properties.Settings.Default.Top = currentTop;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Update the current left position of the window
        /// </summary>
        /// <param name="currentLeft"></param>
        void Session_CurrentLeftChanged(double currentLeft)
        {
            Left = currentLeft;

            //Save the value to settings
            Properties.Settings.Default.Left = currentLeft;
            Properties.Settings.Default.Save();
        }

    }
}
