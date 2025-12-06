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
    internal class BezierFigure : FigureBase
    {
        private Point start;
        private Point control1;
        private Point control2;
        private Point end;

        public BezierFigure(Point s, Point c1, Point c2, Point e)
        {
            start = s;
            control1 = c1;
            control2 = c2;
            end = e;
        }

        public override void Draw(Canvas canvas)
        {
            // Создаём сегмент Безье
            BezierSegment bezierSegment = new BezierSegment(control1, control2, end, true);

            // Начальная точка
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = start,
                Segments = new PathSegmentCollection { bezierSegment }
            };

            // Геометрия пути
            PathGeometry geometry = new PathGeometry(new[] { pathFigure });

            // Сам элемент Path
            shapeElement = new Path
            {
                Stroke = StrokeColor,
                StrokeThickness = Thickness,
                Data = geometry
            };

            canvas.Children.Add(shapeElement);
        }
    }
}
