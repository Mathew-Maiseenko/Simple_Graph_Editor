using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LAB_12
{
    internal abstract class FigureBase : IFigure
    {
        protected Shape shapeElement; // WPF Shape (Line, Ellipse, Polygon, Path)
        protected Brush StrokeColor = Brushes.Black;
        protected Brush FillColor = Brushes.Transparent;
        protected double Thickness = 2;
        public Shape ShapeElement => shapeElement;


        public abstract void Draw(Canvas canvas);

        public virtual void Move(double dx, double dy)
        {
            if (shapeElement != null)
            {
                Canvas.SetLeft(shapeElement, Canvas.GetLeft(shapeElement) + dx);
                Canvas.SetTop(shapeElement, Canvas.GetTop(shapeElement) + dy);
            }
        }

        public virtual void Scale(double factor)
        {
            if (shapeElement != null)
            {
                shapeElement.RenderTransform = new ScaleTransform(factor, factor);
            }
        }

        public virtual void Rotate(double angle)
        {
            if (shapeElement != null)
            {
                shapeElement.RenderTransform = new RotateTransform(angle);
            }
        }

        public virtual void SetAttributes(Brush stroke, Brush fill, double thickness)
        {
            StrokeColor = stroke;
            FillColor = fill;
            Thickness = thickness;
            if (shapeElement != null)
            {
                shapeElement.Stroke = StrokeColor;
                shapeElement.Fill = FillColor;
                shapeElement.StrokeThickness = Thickness;
            }
        }
    }

}
