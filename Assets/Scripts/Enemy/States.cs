using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum State // Состояния моба
{
    Moving, // Движение к игроку по лабиринту
    WaySearching, // Поиск пути до игрока
    Waiting, // Ожидание
    Hunting, // Активное преследование
    Maneuring, // Прожарка в луче фонаря
    Escaping// Бегство
}

