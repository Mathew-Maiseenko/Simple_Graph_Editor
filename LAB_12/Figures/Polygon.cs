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
            if (points == null || points.Count < 2) return;

            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);

            var pathFigure = new PathFigure
            {
                StartPoint = new Point(points[0].X - minX, points[0].Y - minY),
                IsClosed = true
            };

            var polyLineSegment = new PolyLineSegment();
            for (int i = 1; i < points.Count; i++)
            {
                polyLineSegment.Points.Add(new Point(points[i].X - minX, points[i].Y - minY));
            }

            pathFigure.Segments.Add(polyLineSegment);

            var geometry = new PathGeometry();
            geometry.Figures.Add(pathFigure);

            shapeElement = new Path
            {
                Stroke = StrokeColor,
                Fill = FillColor ?? Brushes.Transparent,
                StrokeThickness = Thickness,
                Data = geometry
            };

            Canvas.SetLeft(shapeElement, minX);
            Canvas.SetTop(shapeElement, minY);
            canvas.Children.Add(shapeElement);
        }

        public override void UpdateStateFromShape()
        {
            if (shapeElement is Path path && path.Data is PathGeometry geometry)
            {
                Position = new Point(Canvas.GetLeft(path), Canvas.GetTop(path));
                GeometryTransform = geometry.Transform.Value;
            }
        }
    }
}