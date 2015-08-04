using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Ink;
using SketchIt.Tools;
using System.Windows.Controls;
using System.Windows.Input;

namespace SketchIt
{
    public static class Session
    {
        //public delegates
        public delegate void BackgroundColorChangedHandler(Brush background);
        public delegate void DrawingStateChangedHandler(bool drawingState);
        public delegate void CurrentWidthChangedHandler(double currentWidth);
        public delegate void CurrentHeightChangedHandler(double currentHeight);
        public delegate void CurrentTopChangedHandler(double currentTop);
        public delegate void CurrentLeftChangedHandler(double currentLeft);
        public delegate void DrawingVisibilityChangedHandler(bool visibility);
        public delegate void DrawingAttributesChangedHandler(DrawingAttributes drawingAttributes);
        public delegate void EditingModeChangedHandler(InkCanvasEditingMode mode);
        public delegate void AddImageHandler(string ImagePath);
        public delegate void ClearScreenHandler();

        //public events
        public static event BackgroundColorChangedHandler BackgroundColorChanged;
        public static event DrawingStateChangedHandler DrawingStateChanged;
        public static event CurrentWidthChangedHandler CurrentWidthChanged;
        public static event CurrentHeightChangedHandler CurrentHeightChanged;
        public static event CurrentTopChangedHandler CurrentTopChanged;
        public static event CurrentLeftChangedHandler CurrentLeftChanged;
        public static event DrawingVisibilityChangedHandler DrawingVisibilityChanged;
        public static event DrawingAttributesChangedHandler DrawingAttributesChanged;
        public static event EditingModeChangedHandler EditingModeChanged;
        public static event AddImageHandler AddImage;
        public static event ClearScreenHandler ClearScreen;

        //Private fields
        private static string _WorkingDirectory;
        private static Cursor _EraserCursor;
        private static DrawingToolsHelper _DrawingTools = new DrawingToolsHelper();
        private static bool _DrawingEnabled = false;
        private static bool _DrawingVisible = true;
        private static Toolbar _ToolbarWindow = new Toolbar();
        private static ResizeWindow _ResizeScreen = new ResizeWindow();
        private static Brush _BackgroundBrush;
        private static DrawingAttributes _CurrentDrawingSettings;
        private static InkCanvasEditingMode _CurrentEditingMode = InkCanvasEditingMode.Ink;
        private static double _CurrentWidth;
        private static double _CurrentHeight;
        private static double _CurrentTop;
        private static double _CurrentLeft;
        private static bool _Resizing = false;

        //Public Properties

        /// <summary>
        /// Returns the path to the working directory for the application
        /// </summary>
        public static string WorkingDirectory
        {
            get { return System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath); }
        }

        /// <summary>
        /// DrawingTools allows you to easily create different DrawingAttributes
        /// </summary>
        public static DrawingToolsHelper DrawingTools
        {
            get { return Session._DrawingTools; }
            set { Session._DrawingTools = value; }
        }

        /// <summary>
        /// Is drawing currently enabled
        /// </summary>
        public static bool DrawingEnabled
        {
            get { return Session._DrawingEnabled; }
            set 
            { 
                Session._DrawingEnabled = value;
                //Raise the DrawingStateChanged event
                DrawingStateChanged(value);
            }
        }

        public static ResizeWindow ResizeScreen
        {
            get { return Session._ResizeScreen; }
            set { Session._ResizeScreen = value; }
        }

        /// <summary>
        /// Is the drawing visible on the screen
        /// </summary>
        public static bool DrawingVisible
        {
            get { return Session._DrawingVisible; }
            set 
            { 
                Session._DrawingVisible = value; 
                //Raise the DrawingVisibilityChanged event
                DrawingVisibilityChanged(value);
            }
        }

        /// <summary>
        /// The Window that contains toolbars
        /// </summary>
        public static Toolbar ToolbarWindow
        {
            get { return _ToolbarWindow; }
            set { _ToolbarWindow = value; }
        }

        /// <summary>
        /// The brush to use for the canvas' background
        /// </summary>
        public static Brush BackgroundBrush
        {
            get { return Session._BackgroundBrush; }
            set 
            { 
                Session._BackgroundBrush = value; 
                //Raise the BackgroundColorChanged event
                BackgroundColorChanged(value);                
            }
        }

        /// <summary>
        /// The current drawing attributes
        /// </summary>
        public static DrawingAttributes CurrentDrawingSettings
        {
            get { return Session._CurrentDrawingSettings; }
            set 
            { 
                Session._CurrentDrawingSettings = value; 
                //Raise the DrawingAttributesChanged event
                DrawingAttributesChanged(value);
            }
        }

        /// <summary>
        /// The current editing mode being used (Ink, Erase, Select)
        /// </summary>
        public static InkCanvasEditingMode CurrentEditingMode
        {
            get { return Session._CurrentEditingMode; }
            set 
            { 
                Session._CurrentEditingMode = value; 
                //Fire the EditingModeChanged event
                EditingModeChanged(value);
            }
        }

        /// <summary>
        /// Gets/Sets the width of the drawing window
        /// </summary>
        public static double CurrentWidth
        {
            get { return Session._CurrentWidth; }
            set 
            { 
                Session._CurrentWidth = value; 
                //Fire the CurrentWidthChanged event
                CurrentWidthChanged(value);
            }
        }

        /// <summary>
        /// Gets/Sets the height of the drawing form
        /// </summary>
        public static double CurrentHeight
        {
            get { return Session._CurrentHeight; }
            set 
            { 
                Session._CurrentHeight = value;
                //Fire the CurrentHeightChanged event
                CurrentHeightChanged(value);
            }
        }

        /// <summary>
        /// Gets/Sets the current top position of the drawing window
        /// </summary>
        public static double CurrentTop
        {
            get { return Session._CurrentTop; }
            set 
            { 
                Session._CurrentTop = value;
                //Fire the CurrentTopChanged event
                CurrentTopChanged(value);
            }
        }

        /// <summary>
        /// Gets/Sets the current left position of the drawing window
        /// </summary>
        public static double CurrentLeft
        {
            get { return Session._CurrentLeft; }
            set 
            { 
                Session._CurrentLeft = value;
                //Fire the CurrentLeftChanged event
                CurrentLeftChanged(value);
            }
        }

        /// <summary>
        /// Gets/Sets the current resize state
        /// </summary>
        public static bool Resizing
        {
            get { return Session._Resizing; }
            set { Session._Resizing = value; }
        }

        /// <summary>
        /// The eraser cursor used when the canvas is in eraser mode
        /// </summary>
        public static Cursor EraserCursor
        {
            get { return Session._EraserCursor; }
            set { Session._EraserCursor = value; }
        }

        /// <summary>
        /// Loads the graphical cursors used when editing the ink canvas
        /// </summary>
        public static void LoadCursors()
        {
            EraserCursor = new System.Windows.Input.Cursor(System.IO.Path.Combine(WorkingDirectory, @"Images\Eraser-Cursor.cur"));
        }

        /// <summary>
        /// Add an Image to the canvas
        /// </summary>
        public static void AddImageToCanvas(string imagePath)
        {
            //Raise the AddImage event
            AddImage(imagePath);
        }

        /// <summary>
        /// Clear all items added to the drawing screen
        /// </summary>
        public static void ClearDrawingScreen()
        {
            ClearScreen();
        }

    }
}
