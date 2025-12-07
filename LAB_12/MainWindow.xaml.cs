using LAB_12.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LAB_12
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IFigure> figures = new List<IFigure>();
        private FigureBase curFigure;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawLine_Click(object sender, RoutedEventArgs e)
        {
            var line = new LineFigure(new Point(50, 50), new Point(200, 200));
            line.SetAttributes(Brushes.Black, Brushes.Transparent, ThicknessSlider.Value);
            line.Draw(DrawCanvas);
            figures.Add(line);
            curFigure = line;
        }

        private void DrawCircle_Click(object sender, RoutedEventArgs e)
        {
            var circle = new CircleFigure(150, 150, 50);
            circle.SetAttributes(Brushes.Blue, Brushes.LightBlue, ThicknessSlider.Value);
            circle.Draw(DrawCanvas);
            figures.Add(circle);
            curFigure = circle;
        }

        private void DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PolygonSettingsDialog();
            if (dialog.ShowDialog() == true)
            {
                int vertices = dialog.NumberOfVertices;
                double radius = 80; // or some other default size
                Point center = new Point(150, 150); // or some other default position

                var points = GenerateRegularPolygonPoints(center, vertices, radius);

                var polygon = new PolygonFigure(points);

                polygon.SetAttributes(Brushes.Green, Brushes.LightGreen, ThicknessSlider.Value);
                polygon.Draw(DrawCanvas);
                figures.Add(polygon);
                curFigure = polygon;
            }
        }


        private void DrawBezier_Click(object sender, RoutedEventArgs e)
        {
            var bezier = new BezierFigure(
                new Point(50, 300),    // старт
                new Point(150, 100),   // контрольная точка 1
                new Point(250, 500),   // контрольная точка 2
                new Point(400, 300)    // конец
            );

            bezier.SetAttributes(Brushes.Purple, Brushes.Transparent, ThicknessSlider.Value);
            bezier.Draw(DrawCanvas);
            figures.Add(bezier);
            curFigure = bezier;
        }


        private void DrawCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (curFigure != null)
            {
                Shape shape = curFigure.ShapeElement;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(shape);
                if (adornerLayer != null)
                {
                    // убрать старые адорнеры
                    var adorners = adornerLayer.GetAdorners(shape);
                    if (adorners != null)
                    {
                        foreach (var ad in adorners)
                            adornerLayer.Remove(ad);
                    }

                    // добавить новый только на curFigure
                    adornerLayer.Add(new MonipulationsAdorner(shape));
                }
            }
        }

        private List<Point> GenerateRegularPolygonPoints(Point center, int vertices, double radius)
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



    }












}
