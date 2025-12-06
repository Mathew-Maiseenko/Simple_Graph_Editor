using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LAB_12.Figures
{
    internal class PolygonFigure : FigureBase
    {
        private List<Point> points;

        public PolygonFigure(IEnumerable<Point> pts)
        {
            points = new List<Point>(pts);
        }



        public override void Draw(Canvas canvas)
        {
            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);

            var relativePoints = new PointCollection(
                points.Select(p => new Point(p.X - minX, p.Y - minY))
            );

            shapeElement = new Polygon
            {
                Stroke = StrokeColor,
                Fill = FillColor ?? Brushes.Transparent,
                StrokeThickness = Thickness,
                Points = relativePoints,
            };

            Canvas.SetLeft(shapeElement, minX);
            Canvas.SetTop(shapeElement, minY);
            canvas.Children.Add(shapeElement);
        }
    }
}
