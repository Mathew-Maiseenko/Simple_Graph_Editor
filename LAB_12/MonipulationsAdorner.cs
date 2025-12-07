using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LAB_12
{
    internal class MonipulationsAdorner : Adorner
    {
        private VisualCollection visualChildren;
        private Thumb moveThumb, resizeThumb, rotateThumb;
        private Shape adornedShape;
        private TransformGroup transformGroup;
        private RotateTransform rotateTransform;
        private ScaleTransform scaleTransform;

        // Для вращения мышью
        private Point startMousePosition;
        private double startAngle;
        private bool isRotating = false;

        public MonipulationsAdorner(UIElement adornedElement) : base(adornedElement)
        {
            adornedShape = adornedElement as Shape;
            visualChildren = new VisualCollection(this);

            rotateTransform = new RotateTransform();
            scaleTransform = new ScaleTransform();
            transformGroup = new TransformGroup();

            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(rotateTransform);

            adornedShape.RenderTransform = transformGroup;
            adornedShape.RenderTransformOrigin = new Point(0.5, 0.5); // Устанавливаем центр трансформации

            // Перемещение
            moveThumb = BuildThumb(Brushes.LightBlue);
            moveThumb.DragDelta += MoveThumb_DragDelta;
            visualChildren.Add(moveThumb);

            // Масштабирование
            resizeThumb = BuildThumb(Brushes.LightGreen);
            resizeThumb.DragDelta += ResizeThumb_DragDelta;
            resizeThumb.DragCompleted += Transform_DragCompleted;
            visualChildren.Add(resizeThumb);

            // Поворот
            rotateThumb = BuildThumb(Brushes.Orange);
            rotateThumb.DragDelta += RotateThumb_DragDelta;
            rotateThumb.DragCompleted += Transform_DragCompleted;

            rotateThumb.Cursor = Cursors.Hand;
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
            double newScaleX = scaleTransform.ScaleX + e.HorizontalChange / 100.0;
            double newScaleY = scaleTransform.ScaleY + e.VerticalChange / 100.0;

            if (newScaleX > 0)
                scaleTransform.ScaleX = newScaleX;

            if (newScaleY > 0)
                scaleTransform.ScaleY = newScaleY;
        }

        // Вращение
        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            rotateTransform.Angle += e.HorizontalChange;
            // Центр вращения уже установлен через RenderTransformOrigin
        }

       
        private void Transform_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ApplyTransformationsAndReset();
        }

        // Метод для применения трансформаций к фигуре и сброса состояния
        private void ApplyTransformationsAndReset()
        {
            // Получаем матрицу трансформации
            Matrix transformMatrix = adornedShape.RenderTransform.Value;

            // Если есть трансформации (вращение/масштабирование)
            if (!transformMatrix.IsIdentity)
            {
                // Получаем текущую позицию на Canvas
                double currentLeft = Canvas.GetLeft(adornedShape);
                double currentTop = Canvas.GetTop(adornedShape);

                if (double.IsNaN(currentLeft)) currentLeft = 0;
                if (double.IsNaN(currentTop)) currentTop = 0;

                
                ApplyTransformationToShapeGeometry(transformMatrix);

                
                ResetTransformations();
            }
        }

        // Применяет трансформацию к геометрии фигуры
        private void ApplyTransformationToShapeGeometry(Matrix matrix)
        {
            
            double width = adornedShape.ActualWidth;
            double height = adornedShape.ActualHeight;

            Point localCenter = new Point(width / 2, height / 2);
            Matrix centeredMatrix = matrix;

            // Трансформируем относительно центра (смещаем в центр, применяем, смещаем обратно)
            Matrix translateToCenter = Matrix.Identity;
            translateToCenter.Translate(-localCenter.X, -localCenter.Y);

            Matrix translateBack = Matrix.Identity;
            translateBack.Translate(localCenter.X, localCenter.Y);

            centeredMatrix = Matrix.Multiply(translateToCenter, centeredMatrix);
            centeredMatrix = Matrix.Multiply(centeredMatrix, translateBack);

            // Применяем в зависимости от типа фигуры
            if (adornedShape is Path path && path.Data is Geometry geometry) // да
            {
                MessageBox.Show("Path");
                // Для Path с Geometry
                ApplyTransformationToGeometry(geometry, centeredMatrix);
            }
            
            else if (adornedShape is Line line) // нет
            {
                MessageBox.Show("Line");

                // Для Ellipse создаем Path с EllipseGeometry
                //ApplyTransformationToGeometry(line, centeredMatrix);
                TransformLine(line, centeredMatrix);
            }
            else if (adornedShape is Polygon polygon) // да
            {
                MessageBox.Show("Polygon");
                // Для Polygon трансформируем точки относительно локального центра
                TransformPolygonPoints(polygon, centeredMatrix);
            }
            
        }





        private void TransformLine(Line line, Matrix matrix)
        {
            // Трансформируем начальную и конечную точки
            Point startPoint = new Point(line.X1, line.Y1);
            Point endPoint = new Point(line.X2, line.Y2);

            startPoint = matrix.Transform(startPoint);
            endPoint = matrix.Transform(endPoint);

            line.X1 = startPoint.X;
            line.Y1 = startPoint.Y;
            line.X2 = endPoint.X;
            line.Y2 = endPoint.Y;
        }

        // Применяет трансформацию к Geometry
        private void ApplyTransformationToGeometry(Geometry geometry, Matrix matrix)
        {
            if (geometry.Transform is TransformGroup transformGroup)
            {
                // Добавляем новую трансформацию к существующим
                MatrixTransform matrixTransform = new MatrixTransform(matrix);
                transformGroup.Children.Add(matrixTransform);
            }
            else if (geometry.Transform is MatrixTransform existingTransform)
            {
                // Комбинируем с существующей трансформацией
                Matrix combinedMatrix = Matrix.Multiply(existingTransform.Matrix, matrix);
                geometry.Transform = new MatrixTransform(combinedMatrix);
            }
            else
            {
                // Создаем новую трансформацию
                geometry.Transform = new MatrixTransform(matrix);
            }
        }



        // Трансформирует точки Polygon
        private void TransformPolygonPoints(Polygon polygon, Matrix matrix)
        {
            if (polygon.Points == null || polygon.Points.Count == 0) return;

            PointCollection transformedPoints = new PointCollection();
            foreach (Point point in polygon.Points)
            {
                Point transformedPoint = matrix.Transform(point);
                transformedPoints.Add(transformedPoint);
            }

            // Заменяем исходную коллекцию
            polygon.Points = transformedPoints;
        }

   

        // Сбрасывает трансформации RenderTransform
        private void ResetTransformations()
        {
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            rotateTransform.Angle = 0;
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