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

            AddClickEventListenerToFigure(line);

            figures.Add(line);
            //curFigure = line;
        }

        private void DrawCircle_Click(object sender, RoutedEventArgs e)
        {
            var circle = new CircleFigure(150, 150, 50);
            circle.SetAttributes(Brushes.Blue, Brushes.LightBlue, ThicknessSlider.Value);
            circle.Draw(DrawCanvas);

            AddClickEventListenerToFigure(circle);

            figures.Add(circle);
            //curFigure = circle;
        }

        


        private void AddClickEventListenerToFigure(FigureBase figure)
        {
            if (figure.ShapeElement != null)
            {
                figure.ShapeElement.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
                figure.ShapeElement.Tag = figure;
            }
        }

        private void DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PolygonSettingsDialog();
            if (dialog.ShowDialog() == true)
            {
                int vertices = dialog.NumberOfVertices;
                double radius = 80; 
                Point center = new Point(150, 150); 

                var points = PolygonFigure.GenerateRegularPolygonPoints(center, vertices, radius);

                var polygon = new PolygonFigure(points);

                polygon.SetAttributes(Brushes.Green, Brushes.LightGreen, ThicknessSlider.Value);
                polygon.Draw(DrawCanvas);

                AddClickEventListenerToFigure(polygon);

                figures.Add(polygon);
                //curFigure = polygon;
            }
        }


        private void DrawBezier_Click(object sender, RoutedEventArgs e)
        {
            var bezier = new BezierFigure(
                new List<Point> {

                new Point(50, 300),    // старт
               
                new Point(10, 300),  // конец
                new Point(50, 300),    // старт
            }
            );

            bezier.SetAttributes(Brushes.Purple, Brushes.Transparent, ThicknessSlider.Value);
            bezier.Draw(DrawCanvas);

            AddClickEventListenerToFigure(bezier);

            figures.Add(bezier);
            //curFigure = bezier;
        }








        // Обработчик клика по самой фигуре
        private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Предотвращаем всплытие события до Canvas

            if (sender is Shape shape && shape.Tag is FigureBase figure)
            {
                // Снимаем адорнеры со старой фигуры
                RemoveAdorners(curFigure);

                // Назначаем новую текущую фигуру
                curFigure = figure;

                // Добавляем адорнеры к новой фигуре
                AddAdorners(curFigure);
            }
        }

        // Обработчик клика по Canvas (для снятия выделения)
        private void DrawCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Если кликнули не по фигуре, а по пустому месту на Canvas
            if (e.OriginalSource is Canvas)
            {
                RemoveAdorners(curFigure);
                curFigure = null;
            }
        }

        private void AddAdorners(FigureBase figure)
        {
            if (figure?.ShapeElement == null) return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(figure.ShapeElement);
            if (adornerLayer != null)
            {
                // Убираем старые адорнеры
                var adorners = adornerLayer.GetAdorners(figure.ShapeElement);
                if (adorners != null)
                {
                    foreach (var ad in adorners)
                        adornerLayer.Remove(ad);
                }

                // Добавляем новые
                adornerLayer.Add(new MonipulationsAdorner(figure.ShapeElement));
            }
        }

        private void RemoveAdorners(FigureBase figure)
        {
            if (figure?.ShapeElement == null) return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(figure.ShapeElement);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(figure.ShapeElement);
                if (adorners != null)
                {
                    foreach (var ad in adorners)
                        adornerLayer.Remove(ad);
                }
            }
        }





    }












}
