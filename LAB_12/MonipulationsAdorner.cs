using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LAB_12
{
    internal class MonipulationsAdorner : Adorner
    {
        private VisualCollection visualChildren;
        private Thumb moveThumb, resizeThumb, rotateThumb;
        private Shape adornedShape;

        public MonipulationsAdorner(UIElement adornedElement) : base(adornedElement)
        {
            adornedShape = adornedElement as Shape;
            visualChildren = new VisualCollection(this);

            // Перемещение
            moveThumb = BuildThumb(Brushes.LightBlue);
            moveThumb.DragDelta += MoveThumb_DragDelta;
            visualChildren.Add(moveThumb);

            // Масштабирование
            resizeThumb = BuildThumb(Brushes.LightGreen);
            resizeThumb.DragDelta += ResizeThumb_DragDelta;
            visualChildren.Add(resizeThumb);

            // Поворот
            rotateThumb = BuildThumb(Brushes.Orange);
            rotateThumb.DragDelta += RotateThumb_DragDelta;
            visualChildren.Add(rotateThumb);
        }

        private Thumb BuildThumb(Brush color)
        {
            return new Thumb
            {
                Width = 10,
                Height = 10,
                Background = color,
                Cursor = System.Windows.Input.Cursors.SizeAll
            };
        }

        // Сдвиг
        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // для всех фигур (Ellipse, Line, Polygon и т.д.)
            double left = Canvas.GetLeft(adornedShape);
            double top = Canvas.GetTop(adornedShape);

            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            Canvas.SetLeft(adornedShape, left + e.HorizontalChange);
            Canvas.SetTop(adornedShape, top + e.VerticalChange);
        }

        // Масштабирование
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            adornedShape.RenderTransform = new ScaleTransform(
                1 + e.HorizontalChange / 100.0,
                1 + e.VerticalChange / 100.0,
                adornedShape.RenderSize.Width / 2,
                adornedShape.RenderSize.Height / 2);
        }

        // Поворот
        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            adornedShape.RenderTransform = new RotateTransform(
                e.HorizontalChange, // угол поворота
                adornedShape.RenderSize.Width / 2,
                adornedShape.RenderSize.Height / 2);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double w = adornedShape.RenderSize.Width;
            double h = adornedShape.RenderSize.Height;

            moveThumb.Arrange(new Rect(w / 2 - 5, h / 2 - 5, 10, 10));
            resizeThumb.Arrange(new Rect(w - 5, h - 5, 10, 10));
            rotateThumb.Arrange(new Rect(w / 2 - 5, -20, 10, 10));

            return finalSize;
        }

        protected override int VisualChildrenCount => visualChildren.Count;
        protected override Visual GetVisualChild(int index) => visualChildren[index];

    }
}
