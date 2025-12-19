using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;


namespace LAB_12.Figures
{
    [Serializable]
    internal class CircleFigure : FigureBase
    {
        private double x, y, radius;

        public CircleFigure(double cx, double cy, double r)
        {
            x = cx; y = cy; radius = r;
        }

        public override void Draw(Canvas canvas)
        {
            shapeElement = new Path
            {
                Data = new EllipseGeometry(new Point(radius, radius), radius, radius),
                Stroke = StrokeColor,
                Fill = FillColor,
                StrokeThickness = Thickness
            };
             
            Canvas.SetLeft(shapeElement, x - radius);
            Canvas.SetTop(shapeElement, y - radius);
            canvas.Children.Add(shapeElement);
        }
    }

}
