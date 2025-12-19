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
    [Serializable]
    internal class PolygonFigure : FigureBase
    {
        private List<Point> points;

        public PolygonFigure(IEnumerable<Point> pts)
        {
            points = new List<Point>(pts);
        }

        static public List<Point> GenerateRegularPolygonPoints(Point center, int vertices, double radius)
        {
            var points = new List<Point>();
            double angleStep = 2 * Math.PI / vertices;

            for (int i = 0; i < vertices; i++)
            {
                double angle = i * angleStep - Math.PI / 2; // Start from top
                double x = center.X + radius * Math.Cos(angle);
                double y = center.Y + radius * Math.Sin(angle);
                points.Add(new Point(x, y));
            }
            return points;
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
