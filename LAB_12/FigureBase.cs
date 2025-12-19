using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace LAB_12
{
    [Serializable]
    internal abstract class FigureBase : IFigure
    {
        [NonSerialized]
        protected Shape shapeElement; // WPF Shape (Line, Ellipse, Polygon, Path)
        public string StrokeColorString { get; set; } = "Black";
        public string FillColorString { get; set; } = "Transparent";
        
        protected Brush StrokeColor
        {
            get => (Brush)new BrushConverter().ConvertFromString(StrokeColorString);
            set => StrokeColorString = new BrushConverter().ConvertToString(value);
        }
        protected Brush FillColor
        {
            get => (Brush)new BrushConverter().ConvertFromString(FillColorString);
            set => FillColorString = new BrushConverter().ConvertToString(value);
        }
        
        protected double Thickness = 2;
        
        public Point Position { get; set; }
        public Matrix GeometryTransform { get; set; }

        public Shape ShapeElement => shapeElement;
        
        public abstract void Draw(Canvas canvas);

        public abstract void UpdateStateFromShape();

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
