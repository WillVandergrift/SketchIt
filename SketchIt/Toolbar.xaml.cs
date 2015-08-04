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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing.Imaging;

namespace SketchIt
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : Window
    {
        private DispatcherTimer tmrResize = new DispatcherTimer();
        private string windowAction;
        private static int _MinWidth = 164;
        private static int _MinHeight = 70;
        private static int _MaxWidth = 468;
        private static int _MaxHeight = 390;

        public Toolbar()
        {
            InitializeComponent();
        }

        private void EnableDrawing()
        {
            Session.DrawingEnabled = true;
            imgEnable.Source = new BitmapImage(new Uri(@"pack://application:,,,/SketchIt;component/Images/Cancel-32.png"));            
            Session.BackgroundBrush = SetBackgroundColor("#01000000");

            //Display the full toolbar
            windowAction = "expandingHorizontal";
            tmrResize.Start();
        }

        private void DisableDrawing()
        {
            Session.DrawingEnabled = false;
            imgEnable.Source = new BitmapImage(new Uri(@"pack://application:,,,/SketchIt;component/Images/Check-32.png"));
            Session.BackgroundBrush = SetBackgroundColor("#00000000");

            //Shrink the toolbar
            windowAction = "collapsingBoth";
            tmrResize.Start();
        }

        /// <summary>
        /// Return a brush for the given hex value
        /// </summary>
        /// <param name="hexValue"></param>
        /// <returns></returns>
        private Brush SetBackgroundColor(string hexValue)
        {
            BrushConverter bc = new BrushConverter();
            return (Brush)bc.ConvertFrom(hexValue);
        }

        /// <summary>
        /// Minimize the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            windowAction = "collapsingVertical";
            tmrResize.Start();
        }

        /// <summary>
        /// Enable/Disale drawing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Session.DrawingEnabled)
            {
                //Disable drawing
                DisableDrawing();
            }
            else
            {
                //Enable drawing
                EnableDrawing();
                //Make sure the drawing form is visible
                Session.DrawingVisible = true;
                btnShowHide.IsChecked = true;
            }
        }

        /// <summary>
        /// Close the program when the toolbar is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Setup the timer used to expand and collapse the window
            tmrResize.Interval = new TimeSpan(0, 0, 0, 0, 10);
            tmrResize.Tick += new EventHandler(tmrResize_Tick);

            //Disable drawing on startup
            DisableDrawing();
        }

        /// <summary>
        /// Update the window's size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmrResize_Tick(object sender, EventArgs e)
        {
            switch (windowAction)
            {
                case "expandingHorizontal":
                    if (this.Width < _MaxWidth)
                    {
                        this.Width += 25;
                    }
                    else
                    {
                        this.Width = _MaxWidth;
                        windowAction = "idle";
                        tmrResize.Stop();
                    }
                    break;
                case "expandingVertical":
                    if (this.Height < _MaxHeight)
                    {
                        this.Height += 25;
                    }
                    else
                    {
                        this.Height = _MaxHeight;
                        windowAction = "idle";
                        tmrResize.Stop();
                    }
                    break;
                case "collapsingHorizontal":
                    if (this.Width > _MinWidth)
                    {
                        this.Width -= 25;
                    }
                    else
                    {
                        this.Width = _MinWidth;
                        windowAction = "idle";
                        tmrResize.Stop();
                    }
                    break;
                case "collapsingVertical":
                    if (this.Height > _MinHeight)
                    {
                        this.Height -= 25;
                    }
                    else
                    {
                        this.Height = _MinHeight;
                        windowAction = "idle";
                        tmrResize.Stop();
                    }
                    break;
                case "collapsingBoth":
                    if (this.Width == _MinWidth && this.Height == _MinHeight)
                    {
                        windowAction = "idle";
                        tmrResize.Stop();
                    }

                    //Collapse Horizontal
                    if (this.Width > _MinWidth)
                    {
                        this.Width -= 25;
                    }
                    else
                    {
                        this.Width = _MinWidth;                        
                    }
                    //Collapse Vertical
                    if (this.Height > _MinHeight)
                    {
                        this.Height -= 25;
                    }
                    else
                    {
                        this.Height = _MinHeight;
                    }
                    break;
            }
        }

        /// <summary>
        /// Show/Hide the drawing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowHide_Click(object sender, RoutedEventArgs e)
        {
            Session.DrawingVisible = btnShowHide.IsChecked.Value;
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            tabTools.SelectedItem = tabProperties;
            //Display the full toolbar
            windowAction = "expandingVertical";
            tmrResize.Start();

        }

        /// <summary>
        /// Update the drawing properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorSelector_SelectedColorChanged(object sender, Telerik.Windows.Controls.ColorEditor.ColorChangeEventArgs e)
        {
            if (IsLoaded)
            {
                Session.CurrentDrawingSettings = Session.DrawingTools.CreateBrush(colorSelector.SelectedColor, sliderSize.Value);
            }            
        }

        /// <summary>
        /// Update the drawing properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                Session.CurrentDrawingSettings = Session.DrawingTools.CreateBrush(colorSelector.SelectedColor, sliderSize.Value);
            }            
        }

        /// <summary>
        /// Set the editing mode to ink
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToolPen_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                Session.CurrentEditingMode = InkCanvasEditingMode.Ink;
                //Uncheck the other buttons
                UncheckInactiveItems("Pen");
            }
        }

        /// <summary>
        /// Set the current editing mode to eraser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToolEraser_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                Session.CurrentEditingMode = InkCanvasEditingMode.EraseByPoint;
                //Uncheck the other buttons
                UncheckInactiveItems("Eraser");
            }
        }

        /// <summary>
        /// Set the editing mode to select
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToolSelect_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                Session.CurrentEditingMode = InkCanvasEditingMode.Select;
                //Uncheck the other buttons
                UncheckInactiveItems("Select");
            }
        }

        /// <summary>
        /// Uncheck inactive toolbar buttons
        /// </summary>
        /// <param name="activeItem"></param>
        private void UncheckInactiveItems(string activeItem)
        {
            switch (activeItem)
            {
                case "Pen":
                    btnToolPen.IsChecked = true;
                    btnToolEraser.IsChecked = false;
                    btnToolSelect.IsChecked = false;
                    break;
                case "Eraser":
                    btnToolEraser.IsChecked = true;
                    btnToolPen.IsChecked = false;
                    btnToolSelect.IsChecked = false;
                    break;
                case "Select":
                    btnToolSelect.IsChecked = true;
                    btnToolPen.IsChecked = false;
                    btnToolEraser.IsChecked = false;
                    break;
            }
        }

        private void btnInsertPicture_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Supported Image Formats|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            System.Windows.Forms.DialogResult imgSelectorResult = dlg.ShowDialog();

            if (imgSelectorResult == System.Windows.Forms.DialogResult.OK)
            {
                Session.AddImageToCanvas(dlg.FileName);
            }            
        }

        /// <summary>
        /// Display the Settings tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Reset the drawing area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetArea_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DisableDrawing();

            //Check to see if another resize window is already open
            if (Session.Resizing)
            {
                if (Session.ResizeScreen != null)
                {
                    Session.ResizeScreen.Close();
                }
            }
            
            //Get the working area for the primary monitor
            System.Drawing.Rectangle drawingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

            Session.CurrentTop = drawingArea.Top;
            Session.CurrentLeft = drawingArea.Left;
            Session.CurrentWidth = drawingArea.Width;
            Session.CurrentHeight = drawingArea.Height;

            Session.ResizeScreen = new ResizeWindow();
            Session.ResizeScreen.Show();
            Session.Resizing = true;
        }

        /// <summary>
        /// Display the resize window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResize_Click(object sender, RoutedEventArgs e)
        {
            DisableDrawing();

            //Check to see if another resize window is already open
            if (Session.Resizing)
            {
                if (Session.ResizeScreen != null)
                {
                    Session.ResizeScreen.Close();
                }
            }

            Session.ResizeScreen = new ResizeWindow();
            Session.ResizeScreen.Show();
            Session.Resizing = true;
        }

        /// <summary>
        /// Send a message to clear the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToolClear_Click(object sender, RoutedEventArgs e)
        {
            Session.ClearDrawingScreen();
        }

        /// <summary>
        /// Save a screenshot of the current drawing area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToolSave_Click(object sender, RoutedEventArgs e)
        {
            //Get the size and location of the drawing area converted to integers
            int left = (int)Session.ResizeScreen.Left;
            int top = (int)Session.ResizeScreen.Top;
            int width = (int)Session.ResizeScreen.Width;
            int height = (int)Session.ResizeScreen.Height;
            
            //Create a rectangle with the same dimensions and location as the drawing area
            System.Drawing.Rectangle bounds = new System.Drawing.Rectangle();
            bounds.Width = width;
            bounds.Height = height;
            bounds.Location = new System.Drawing.Point(left, top);

            //Create a bitmap of the screen at the location of the drawing area and with the same size
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new System.Drawing.Point(left, top), System.Drawing.Point.Empty, bounds.Size);
                }
                
                //Ask the user where they want to save the screenshot to
                try
                {
                    System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();

                    saveDialog.Title = "Save Screenshot";
                    saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    saveDialog.Filter = "Bitmap Image (*.bmp)|*.bmp|Jpeg Image (*.jpg)|*.jpg|png Image (*.png)|*.png|gif Image (*.gif)|*.gif";

                    System.Windows.Forms.DialogResult result = saveDialog.ShowDialog();

                    if (result != System.Windows.Forms.DialogResult.Cancel)
                    {
                        //Check to see what the file extension is. If it's supported save the image
                        switch (saveDialog.FileName.Substring(saveDialog.FileName.LastIndexOf(".")))
                        {
                            case ".bmp":
                                bitmap.Save(saveDialog.FileName, ImageFormat.Bmp);
                                break;
                            case ".jpg":
                                bitmap.Save(saveDialog.FileName, ImageFormat.Jpeg);
                                break;
                            case ".jpeg":
                                bitmap.Save(saveDialog.FileName, ImageFormat.Jpeg);
                                break;
                            case ".png":
                                bitmap.Save(saveDialog.FileName, ImageFormat.Png);
                                break;
                            case ".gif":
                                bitmap.Save(saveDialog.FileName, ImageFormat.Gif);
                                break;
                            default:
                                MessageBox.Show("Invalid File Extension. Supported image types are: bmp, jpg, jpeg, png, gif");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save the screenshot. " + ex.ToString(), "Error");
                }
                
            }
        }
    }
}
