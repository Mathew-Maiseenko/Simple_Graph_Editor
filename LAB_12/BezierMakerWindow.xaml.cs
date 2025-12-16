using LAB_12.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LAB_12
{
    /// <summary>
    /// Логика взаимодействия для BezierMakerWindow.xaml
    /// </summary>
    public partial class BezierMakerWindow : Window
    {
        public List<Point> points { get; private set; } = new List<Point>();
        private BezierFigure bezierFigure;
        private string modalType = "";

        public BezierMakerWindow()
        {
            InitializeComponent();
        }

        public BezierMakerWindow(string type)
        {
            InitializeComponent();
            modalType = type;   
        }

        private void BezierCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (modalType == "Line" && points.Count >= 2)
            {
                MessageBox.Show("Линия не может содердать более 2 точек!");
                return;
            }

            Point currentPoint = e.GetPosition(BezierCanvas);
            points.Add(currentPoint);

            // Визуализируем точку
            Ellipse pointVisual = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(pointVisual, currentPoint.X - pointVisual.Width / 2);
            Canvas.SetTop(pointVisual, currentPoint.Y - pointVisual.Height / 2);
            BezierCanvas.Children.Add(pointVisual);

            RedrawBezier();
        }

        private void RedrawBezier()
        {
            // Удаляем предыдущую кривую, если она есть
            if (bezierFigure?.ShapeElement != null && BezierCanvas.Children.Contains(bezierFigure.ShapeElement))
            {
                BezierCanvas.Children.Remove(bezierFigure.ShapeElement);
            }

            if (points.Count < 2) return;

            // Создаем и рисуем новую кривую
            bezierFigure = new BezierFigure(new List<Point>(points), true);
            bezierFigure.SetAttributes(Brushes.Black, Brushes.Transparent, 2);
            bezierFigure.Draw(BezierCanvas);
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
