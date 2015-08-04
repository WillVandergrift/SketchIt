using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;
using System.Windows.Media;

namespace SketchIt.Tools
{
    public class DrawingToolsHelper
    {
        /// <summary>
        /// Create a new Brush
        /// </summary>
        /// <param name="brushColor">The color of the brush</param>
        /// <param name="brushSize">The width and height of the brush</param>
        /// <returns></returns>
        public DrawingAttributes CreateBrush(Color brushColor, double brushSize)
        {
            DrawingAttributes newDA = new DrawingAttributes();

            newDA.Color = brushColor;
            newDA.IsHighlighter = false;
            newDA.IgnorePressure = true;
            newDA.StylusTip = StylusTip.Ellipse;
            newDA.Height = brushSize;
            newDA.Width = brushSize;

            return newDA;
        }

    }
}
