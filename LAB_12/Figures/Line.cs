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
            var minX = Math.Min(start.X, end.X);
            var minY = Math.Min(start.Y, end.Y);

            var relativeStart = new Point(start.X - minX, start.Y - minY);
            var relativeEnd = new Point(end.X - minX, end.Y - minY);

            var lineGeometry = new LineGeometry(relativeStart, relativeEnd);

            shapeElement = new Path
            {
                Stroke = StrokeColor,
                StrokeThickness = Thickness,
                Data = lineGeometry
            };

            Canvas.SetLeft(shapeElement, minX);
            Canvas.SetTop(shapeElement, minY);
            canvas.Children.Add(shapeElement);
        }

        public override void UpdateStateFromShape()
        {
            if (shapeElement is Path path)
            {
                Position = new Point(Canvas.GetLeft(path), Canvas.GetTop(path));
                GeometryTransform = path.Data.Transform.Value;
            }
        }
    }
}
