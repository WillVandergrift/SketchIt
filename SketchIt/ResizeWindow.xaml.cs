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

namespace SketchIt
{
    /// <summary>
    /// Interaction logic for ResizeWindow.xaml
    /// </summary>
    public partial class ResizeWindow : Window
    {
        public ResizeWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Session.Resizing = false;
            Close();
        }

        /// <summary>
        /// Store the new position and size of the drawing screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentWidth = ActualWidth;
            Session.CurrentHeight = ActualHeight;
            Session.CurrentTop = GetWindowTop(this);
            Session.CurrentLeft = GetWindowLeft(this);

            Session.Resizing = false;
            Close();
        }

        /// <summary>
        /// Get the actual Left position of the window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private double GetWindowLeft(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                var leftField = typeof(Window).GetField("_actualLeft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (double)leftField.GetValue(window);
            }
            else
            {
                return window.Left;
            }
        }

        /// <summary>
        /// Get the actual top position of the window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private double GetWindowTop(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                var topField = typeof(Window).GetField("_actualTop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (double)topField.GetValue(window);
            }
            else
            {
                return window.Top;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Set the screen position and size to the current drawing window's position and size
            Width = Session.CurrentWidth;
            Height = Session.CurrentHeight;
            Top = Session.CurrentTop;
            Left = Session.CurrentLeft;
        }
    }
}
