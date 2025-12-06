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

            // Используем старый способ через DragDelta для простоты
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

        // После завершения вращения или масштабирования
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

                // Применяем трансформацию к геометрии фигуры
                ApplyTransformationToShapeGeometry(transformMatrix);

                // Сбрасываем трансформации RenderTransform
                ResetTransformations();
            }
        }

        // Применяет трансформацию к геометрии фигуры
        private void ApplyTransformationToShapeGeometry(Matrix matrix)
        {
            // Получаем размеры фигуры
            double width = adornedShape.ActualWidth;
            double height = adornedShape.ActualHeight;

            // Центр фигуры в локальных координатах
            Point localCenter = new Point(width / 2, height / 2);

            // Создаем матрицу трансформации относительно центра фигуры
            Matrix centeredMatrix = matrix;

            // Трансформируем относительно центра (смещаем в центр, применяем, смещаем обратно)
            Matrix translateToCenter = Matrix.Identity;
            translateToCenter.Translate(-localCenter.X, -localCenter.Y);

            Matrix translateBack = Matrix.Identity;
            translateBack.Translate(localCenter.X, localCenter.Y);

            centeredMatrix = Matrix.Multiply(translateToCenter, centeredMatrix);
            centeredMatrix = Matrix.Multiply(centeredMatrix, translateBack);

            // Применяем в зависимости от типа фигуры
            if (adornedShape is Path path && path.Data is Geometry geometry)
            {
                // Для Path с Geometry
                ApplyTransformationToGeometry(geometry, centeredMatrix);
            }
            else if (adornedShape is Ellipse ellipse)
            {
                // Для Ellipse создаем Path с EllipseGeometry
                ConvertToPathAndApplyTransformation(ellipse, centeredMatrix);
            }
            else if (adornedShape is Rectangle rectangle)
            {
                // Для Rectangle создаем Path с RectangleGeometry
                ConvertToPathAndApplyTransformation(rectangle, centeredMatrix);
            }
            else if (adornedShape is Polygon polygon)
            {
                // Для Polygon трансформируем точки относительно локального центра
                TransformPolygonPoints(polygon, centeredMatrix);
            }
            else if (adornedShape is Polyline polyline)
            {
                // Для Polyline трансформируем точки относительно локального центра
                TransformPolylinePoints(polyline, centeredMatrix);
            }
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

        // Конвертирует простую фигуру в Path и применяет трансформацию
        private void ConvertToPathAndApplyTransformation(Shape originalShape, Matrix matrix)
        {
            Geometry geometry = null;

            if (originalShape is Ellipse ellipse)
            {
                // Создаем EllipseGeometry
                geometry = new EllipseGeometry(new Rect(0, 0, ellipse.ActualWidth, ellipse.ActualHeight));
            }
            else if (originalShape is Rectangle rectangle)
            {
                // Создаем RectangleGeometry
                geometry = new RectangleGeometry(new Rect(0, 0, rectangle.ActualWidth, rectangle.ActualHeight));
            }

            if (geometry != null)
            {
                // Применяем трансформацию
                geometry.Transform = new MatrixTransform(matrix);

                // Создаем новую Path
                Path newPath = new Path
                {
                    Data = geometry,
                    Fill = originalShape.Fill,
                    Stroke = originalShape.Stroke,
                    StrokeThickness = originalShape.StrokeThickness,
                    StrokeDashArray = originalShape.StrokeDashArray
                };

                // Заменяем в родительском контейнере
                if (originalShape.Parent is Panel parentPanel)
                {
                    // Сохраняем позицию
                    double left = Canvas.GetLeft(originalShape);
                    double top = Canvas.GetTop(originalShape);

                    int index = parentPanel.Children.IndexOf(originalShape);
                    parentPanel.Children.Remove(originalShape);
                    parentPanel.Children.Insert(index, newPath);

                    // Восстанавливаем позицию
                    if (!double.IsNaN(left))
                        Canvas.SetLeft(newPath, left);
                    if (!double.IsNaN(top))
                        Canvas.SetTop(newPath, top);

                    // Обновляем ссылку на фигуру
                    adornedShape = newPath;
                }
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

        // Трансформирует точки Polyline
        private void TransformPolylinePoints(Polyline polyline, Matrix matrix)
        {
            if (polyline.Points == null || polyline.Points.Count == 0) return;

            PointCollection transformedPoints = new PointCollection();
            foreach (Point point in polyline.Points)
            {
                Point transformedPoint = matrix.Transform(point);
                transformedPoints.Add(transformedPoint);
            }

            // Заменяем исходную коллекцию
            polyline.Points = transformedPoints;
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