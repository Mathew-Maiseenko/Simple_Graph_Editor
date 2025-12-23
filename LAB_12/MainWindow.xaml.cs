using LAB_12.Figures;
﻿using System;
﻿using System.Collections.Generic;
﻿using System.IO;
﻿using System.Linq;
﻿using System.Runtime.Serialization.Formatters.Binary;
﻿using System.Text;
﻿using System.Threading.Tasks;
﻿using System.Windows;
﻿using System.Windows.Controls;
﻿using System.Windows.Controls.Primitives;
﻿using System.Windows.Data;
﻿using System.Windows.Documents;
﻿using System.Windows.Input;
﻿using System.Windows.Media;
﻿using System.Windows.Media.Imaging;
﻿using System.Windows.Navigation;
﻿using System.Windows.Shapes;
﻿
﻿using WF = System.Windows.Forms;       // псевдоним для WinForms
﻿using SD = System.Drawing;
using Path = System.Windows.Shapes.Path;

namespace LAB_12
﻿{
﻿    /// <summary>
﻿    /// Логика взаимодействия для MainWindow.xaml
﻿    /// </summary>
﻿    public partial class MainWindow : Window
﻿    {
﻿        private List<IFigure> figures = new List<IFigure>();
﻿        private FigureBase curFigure;
﻿        private Brush lineColor = Brushes.Green;
﻿        private Brush fillColor = Brushes.DarkGreen;
﻿        public MainWindow()
﻿        {
﻿            InitializeComponent();
﻿        }
﻿
﻿        private void DrawLine_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            var bezierMaker = new BezierMakerWindow("Line");
﻿            if (bezierMaker.ShowDialog() == true)
﻿            {
﻿
﻿                var line = new LineFigure(bezierMaker.points[0], bezierMaker.points[1]);
﻿                line.SetAttributes(lineColor, fillColor, ThicknessSlider.Value);
﻿                line.Draw(DrawCanvas);
﻿
﻿                AddClickEventListenerToFigure(line);
﻿
﻿                figures.Add(line);
﻿
﻿            }
﻿            //curFigure = line;
﻿        }
﻿
﻿        private void DrawCircle_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            var circle = new CircleFigure(150, 150, 50);
﻿            circle.SetAttributes(lineColor, fillColor, ThicknessSlider.Value);
﻿            circle.Draw(DrawCanvas);
﻿
﻿            AddClickEventListenerToFigure(circle);
﻿
﻿            figures.Add(circle);
﻿            //curFigure = circle;
﻿        }
﻿
﻿
﻿
﻿
﻿        private void AddClickEventListenerToFigure(FigureBase figure)
﻿        {
﻿            if (figure.ShapeElement != null)
﻿            {
﻿                figure.ShapeElement.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
﻿                figure.ShapeElement.Tag = figure;
﻿            }
﻿        }
﻿
﻿        private void DrawPolygon_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            var dialog = new PolygonSettingsDialog();
﻿            if (dialog.ShowDialog() == true)
﻿            {
﻿                int vertices = dialog.NumberOfVertices;
﻿                double radius = 80;
﻿                Point center = new Point(150, 150);
﻿
﻿                var points = PolygonFigure.GenerateRegularPolygonPoints(center, vertices, radius);
﻿
﻿                var polygon = new PolygonFigure(points);
﻿
﻿                polygon.SetAttributes(lineColor, fillColor, ThicknessSlider.Value);
﻿                polygon.Draw(DrawCanvas);
﻿                RemoveAdorners(curFigure);
﻿
﻿                AddClickEventListenerToFigure(polygon);
﻿
﻿                figures.Add(polygon);
﻿                //curFigure = polygon;
﻿            }
﻿        }
﻿
﻿
﻿        private void DrawBezier_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            var bezierMaker = new BezierMakerWindow();
﻿            if (bezierMaker.ShowDialog() == true)
﻿            {
﻿                if (bezierMaker.points.Count >= 2)
﻿                {
﻿                    var bezier = new BezierFigure(bezierMaker.points, true);
﻿
﻿                    bezier.SetAttributes(lineColor, Brushes.Transparent, ThicknessSlider.Value);
﻿                    bezier.Draw(DrawCanvas);
﻿
﻿                    AddClickEventListenerToFigure(bezier);
﻿
﻿                    figures.Add(bezier);
﻿                }
﻿            }
﻿        }
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿        // Обработчик клика по самой фигуре
﻿        private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
﻿        {
﻿            e.Handled = true; // Предотвращаем всплытие события до Canvas
﻿
﻿            if (sender is Shape shape && shape.Tag is FigureBase figure)
﻿            {
﻿                // Снимаем адорнеры со старой фигуры
﻿                RemoveAdorners(curFigure);
﻿
﻿                // Назначаем новую текущую фигуру
﻿                curFigure = figure;
﻿
﻿                // Добавляем адорнеры к новой фигуре
﻿                AddAdorners(curFigure);
﻿            }
﻿        }
﻿
﻿        // Обработчик клика по Canvas (для снятия выделения)
﻿        private void DrawCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
﻿        {
﻿            // Если кликнули не по фигуре, а по пустому месту на Canvas
﻿            if (e.OriginalSource is Canvas)
﻿            {
﻿                RemoveAdorners(curFigure);
﻿                curFigure = null;
﻿            }
﻿        }
﻿
﻿        private void AddAdorners(FigureBase figure)
﻿        {
﻿            if (figure?.ShapeElement == null) return;
﻿
﻿            var adornerLayer = AdornerLayer.GetAdornerLayer(figure.ShapeElement);
﻿            if (adornerLayer != null)
﻿            {
﻿                // Убираем старые адорнеры
﻿                var adorners = adornerLayer.GetAdorners(figure.ShapeElement);
﻿                if (adorners != null)
﻿                {
﻿                    foreach (var ad in adorners)
﻿                        adornerLayer.Remove(ad);
﻿                }
﻿
﻿                // Добавляем новые
﻿                adornerLayer.Add(new MonipulationsAdorner(figure.ShapeElement));
﻿            }
﻿        }
﻿
﻿        private void RemoveAdorners(FigureBase figure)
﻿        {
﻿            if (figure?.ShapeElement == null) return;
﻿
﻿            var adornerLayer = AdornerLayer.GetAdornerLayer(figure.ShapeElement);
﻿            if (adornerLayer != null)
﻿            {
﻿                var adorners = adornerLayer.GetAdorners(figure.ShapeElement);
﻿                if (adorners != null)
﻿                {
﻿                    foreach (var ad in adorners)
﻿                        adornerLayer.Remove(ad);
﻿                }
﻿            }
﻿        }
﻿
﻿        private void ChooseColor(ref Brush colorParam, object sender)
﻿        {
﻿            var colorDialog = new WF.ColorDialog();
﻿            if (colorDialog.ShowDialog() == WF.DialogResult.OK)
﻿            {
﻿                SD.Color selectedColor = colorDialog.Color;
﻿
﻿                colorParam = new SolidColorBrush(System.Windows.Media.Color.FromArgb(
﻿                    selectedColor.A,
﻿                    selectedColor.R,
﻿                    selectedColor.G,
﻿                    selectedColor.B));
﻿                Button button = (Button)sender;
﻿
﻿                button.Background = colorParam;
﻿
﻿            }
﻿        }
﻿
﻿        private void ChangeCurFigureParams()
﻿        {
﻿            if (curFigure == null) return;
﻿
﻿            if (curFigure is BezierFigure bezier)
﻿            {
﻿                bezier.SetAttributes(lineColor, Brushes.Transparent, ThicknessSlider.Value);
﻿            }
﻿            else if (curFigure is LineFigure line)
﻿            {
﻿                line.SetAttributes(lineColor, Brushes.Transparent, ThicknessSlider.Value);
﻿            }
﻿            else
﻿            {
﻿                curFigure.SetAttributes(lineColor, fillColor, ThicknessSlider.Value);
﻿            }
﻿        }
﻿
﻿
﻿
﻿        private void ColorPickkerFill_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            ChooseColor(ref fillColor, sender);
﻿            ChangeCurFigureParams();
﻿        }
﻿
﻿        private void ColorPickkerStroke_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            ChooseColor(ref lineColor, sender);
﻿            ChangeCurFigureParams();
﻿        }
﻿
﻿        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
﻿        {
﻿            ChangeCurFigureParams();
﻿        }
﻿
﻿        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
﻿        {
﻿            SaveFigures();
﻿        }
﻿
﻿        private void Window_Loaded(object sender, RoutedEventArgs e)
﻿        {
﻿            LoadFigures();
﻿        }
﻿
﻿                private void SaveFigures()
﻿                {
﻿                    foreach (var figure in figures)
﻿                    {
﻿                        if (figure is FigureBase figBase)
﻿                        {
﻿                            figBase.UpdateStateFromShape();
﻿                        }
﻿                    }
﻿        
﻿                    BinaryFormatter formatter = new BinaryFormatter();
﻿                    using (FileStream fs = new FileStream("figures.dat", FileMode.OpenOrCreate))
﻿                    {
﻿                        formatter.Serialize(fs, figures);
﻿                    }
﻿                }
﻿        
﻿                private void LoadFigures()
﻿                {
﻿                    if (!File.Exists("figures.dat")) return;
﻿        
﻿                    BinaryFormatter formatter = new BinaryFormatter();
﻿                    using (FileStream fs = new FileStream("figures.dat", FileMode.OpenOrCreate))
﻿                    {
﻿                        if (fs.Length > 0)
﻿                        {
﻿                            figures = (List<IFigure>)formatter.Deserialize(fs);
﻿        
﻿                            foreach (var figure in figures)
﻿                            {
﻿                                if (figure is FigureBase figBase)
﻿                                {
﻿                                    figBase.Draw(DrawCanvas);
﻿        
﻿                                    if (figBase.ShapeElement != null)
﻿                                    {
﻿                                        Canvas.SetLeft(figBase.ShapeElement, figBase.Position.X);
﻿                                        Canvas.SetTop(figBase.ShapeElement, figBase.Position.Y);
﻿                                        
﻿                                        if (figBase.ShapeElement is Path path)
﻿                                        {
﻿                                            path.Data.Transform = new MatrixTransform(figBase.GeometryTransform);
﻿                                        }
﻿                                        AddClickEventListenerToFigure(figBase);
﻿                                    }
﻿                                }
﻿                            }
﻿                        }
﻿                    }
﻿                }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            
            figures.Clear();
            RemoveAdorners(curFigure);
            curFigure = null;
            DrawCanvas.Children.Clear();

        }
    }
﻿}
