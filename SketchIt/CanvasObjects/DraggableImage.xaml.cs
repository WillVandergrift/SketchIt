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

namespace SketchIt.CanvasObjects
{
    /// <summary>
    /// Interaction logic for DraggableImage.xaml
    /// </summary>
    public partial class DraggableImage : Image
    {

        bool _isDragging = false;
        Point _offset;

        public DraggableImage()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Get access to the framework element
            FrameworkElement element = sender as FrameworkElement;

            //Exit if we couldn't get access to the frameworkelement
            if (element == null)
            {
                return;
            }

            //Set dragging = true
            _isDragging = true;

            //Set the initial mouse offset
            _offset = e.GetPosition(element);

            //Set the cursor to a drag cursor
            this.Cursor = Cursors.ScrollAll;
         }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            //Return if we're not currently dragging
            if (!_isDragging)
            {
                return;
            }

            //Get access to the framework element
            FrameworkElement element = sender as FrameworkElement;

            //Exit if we couldn't get access to the frameworkelement
            if (element == null)
            {
                return;
            }

            InkCanvas canvas = element.Parent as InkCanvas;

            //Exit if we couldn't find the controls parent inkcanvas
            if (canvas == null)
            {
                return;
            }

            //Get the position of the mouse relative to the parent inkcanvas
            Point mousePoint = e.GetPosition(canvas);

            //Set the offset set when mousedown was called
            mousePoint.Offset(-_offset.X, -_offset.Y);

            //Move the element on the canvas
            element.SetValue(InkCanvas.LeftProperty, mousePoint.X);
            element.SetValue(InkCanvas.TopProperty, mousePoint.Y);

        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Stop Dragging and reset the cursor
            _isDragging = false;
            this.Cursor = Cursors.Arrow;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            //Stop dragging and reset the cursor
            _isDragging = false;
            this.Cursor = Cursors.Arrow;
        }
    }
}
