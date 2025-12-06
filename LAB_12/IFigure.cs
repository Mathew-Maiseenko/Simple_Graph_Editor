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
    internal interface IFigure
    {
        void Draw(Canvas canvas);              // Отрисовка на холсте
        void Move(double dx, double dy);       // Сдвиг
        void Scale(double factor);             // Масштабирование
        void Rotate(double angle);             // Поворот
        void SetAttributes(Brush stroke, Brush fill, double thickness); // Настройка атрибутов

        

    }
}
