using LAB_12.Figures;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LAB_12
{
    public partial class BezierPointsSelectionDialog : Window
    {
        public List<Point> Points { get; private set; }
        private Shape previewShape;

        public BezierPointsSelectionDialog()
        {
            InitializeComponent();
            Points = new List<Point>();
        }

        private void SelectionCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(SelectionCanvas);
            Points.Add(p);

            // Add a visual marker for the point
            Ellipse pointMarker = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(pointMarker, p.X - pointMarker.Width / 2);
            Canvas.SetTop(pointMarker, p.Y - pointMarker.Height / 2);
            SelectionCanvas.Children.Add(pointMarker);

            UpdatePreview();

            if (Points.Count >= 2)
            {
                CreateButton.IsEnabled = true;
            }
        }

        private void UpdatePreview()
        {
            // Remove the old preview shape
            if (previewShape != null)
            {
                SelectionCanvas.Children.Remove(previewShape);
            }

            if (Points.Count < 2)
            {
                return;
            }

            // Create a new BezierFigure for the preview
            var bezierPreview = new BezierFigure(new List<Point>(Points), false);
            bezierPreview.SetAttributes(Brushes.Purple, Brushes.Transparent, 2);
            
            // Draw it on the canvas
            bezierPreview.Draw(SelectionCanvas);
            
            // Keep a reference to the shape to remove it later
            previewShape = bezierPreview.ShapeElement;
            // Ensure the preview is behind the point markers
            Canvas.SetZIndex(previewShape, -1);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
