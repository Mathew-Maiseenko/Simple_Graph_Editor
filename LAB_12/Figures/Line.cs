using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace LAB_12.Figures
{
    [Serializable]
    internal class LineFigure : FigureBase
    {
        private Point start;
        private Point end;

        public LineFigure(Point s, Point e)
        {
            start = s;
            end = e;
        }

        public override void Draw(Canvas canvas)
        {
            shapeElement = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = StrokeColor,
                StrokeThickness = Thickness
            };
            canvas.Children.Add(shapeElement);
        }
    }

}
