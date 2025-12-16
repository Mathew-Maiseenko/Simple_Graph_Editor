using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LAB_12.Figures
{
    internal class BezierFigure : FigureBase
    {
        private List<Point> points;
        private bool usePolyBezier;

        // Конструктор для обратной совместимости (4 точки)
        public BezierFigure(Point start, Point control1, Point control2, Point end)
        {
            points = new List<Point> { start, control1, control2, end };
            usePolyBezier = false;
        }

        // Конструктор для произвольного количества точек
        public BezierFigure(List<Point> bezierPoints, bool autoCreateCurve = false)
        {
            if (bezierPoints == null || bezierPoints.Count < 2)
                throw new ArgumentException("Для кривой нужно минимум 2 точки");

            points = new List<Point>(bezierPoints);

            // Если autoCreateCurve = true, то автоматически создаем плавную кривую
            // Если false, то используем PolyBezierSegment (требует 1 + 3n точек)
            usePolyBezier = !autoCreateCurve;

            if (!autoCreateCurve && (points.Count - 1) % 3 != 0)
            {
                // Автоматически дополняем точки до нужного формата
                AutoCompletePoints();
            }
        }

        private void AutoCompletePoints()
        {
            // Дополняем точки до формата 1 + 3n
            int neededPoints = 1 + ((points.Count + 2) / 3) * 3;
            int pointsToAdd = neededPoints - points.Count;

            // Копируем последнюю точку для дополнения
            Point lastPoint = points[points.Count - 1];
            for (int i = 0; i < pointsToAdd; i++)
            {
                points.Add(new Point(lastPoint.X + (i + 1) * 10, lastPoint.Y + (i + 1) * 10));
            }
        }

        public override void Draw(Canvas canvas)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = points[0],
                IsClosed = false
            };

            if (points.Count == 2)
            {
                // Если всего 2 точки - рисуем линию
                LineSegment line = new LineSegment(points[1], true);
                pathFigure.Segments.Add(line);
            }
            else if (points.Count == 3)
            {
                // Если 3 точки - квадратичная Безье
                QuadraticBezierSegment quad = new QuadraticBezierSegment(points[1], points[2], true);
                pathFigure.Segments.Add(quad);
            }
            else if (points.Count == 4)
            {
                // Если 4 точки - кубическая Безье
                BezierSegment segment = new BezierSegment(points[1], points[2], points[3], true);
                pathFigure.Segments.Add(segment);
            }
            else if (usePolyBezier)
            {
                // Используем PolyBezierSegment (формат 1 + 3n)
                PolyBezierSegment polySegment = new PolyBezierSegment();
                for (int i = 1; i < points.Count; i++)
                {
                    polySegment.Points.Add(points[i]);
                }
                pathFigure.Segments.Add(polySegment);
            }
            else
            {
                // Автоматически создаем плавную кривую из любого количества точек
                CreateSmoothCurve(pathFigure);
            }

            geometry.Figures.Add(pathFigure);

            shapeElement = new Path
            {
                Stroke = StrokeColor,
                Fill = FillColor,
                StrokeThickness = Thickness,
                Data = geometry
            };

            canvas.Children.Add(shapeElement);
        }

        private void CreateSmoothCurve(PathFigure pathFigure)
        {
            // Метод Catmull-Rom для создания плавной кривой
            for (int i = 1; i < points.Count; i++)
            {
                Point p0, p1, p2, p3;

                // Определяем точки для сегмента
                if (i == 1)
                {
                    p0 = points[0];
                    p1 = points[0];
                    p2 = points[1];
                    p3 = (points.Count > 2) ? points[2] : points[1];
                }
                else if (i == points.Count - 1)
                {
                    p0 = points[i - 2];
                    p1 = points[i - 1];
                    p2 = points[i];
                    p3 = points[i];
                }
                else
                {
                    p0 = points[i - 2];
                    p1 = points[i - 1];
                    p2 = points[i];
                    p3 = points[i + 1];
                }

                // Вычисляем контрольные точки
                double tension = 0.5;
                Point cp1 = new Point(
                    p1.X + (p2.X - p0.X) * tension / 6,
                    p1.Y + (p2.Y - p0.Y) * tension / 6
                );

                Point cp2 = new Point(
                    p2.X - (p3.X - p1.X) * tension / 6,
                    p2.Y - (p3.Y - p1.Y) * tension / 6
                );

                // Создаем сегмент Безье
                BezierSegment segment = new BezierSegment(cp1, cp2, p2, true);
                pathFigure.Segments.Add(segment);
            }
        }

        // Свойство для доступа к точкам
        public IReadOnlyList<Point> Points => points.AsReadOnly();

        // Метод для добавления точки
        public void AddPoint(Point point)
        {
            points.Add(point);
        }
    }
}