using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public Hexagon[] hexagons; // !изменить на private
    public Stack<Hexagon> way;
    public Hexagon targetHex, currentHex, xHex; // !убрать и добавить объявление в тело метода
    public Hexagon[] publicWay;

    public Player player;

    public States state;
    public float speed;

    public enum States // Состояния моба
    {
        Moving, // Движение к игроку по лабиринту
        WaySearching, // Поиск пути до игрока
        Waiting, // Ожидание
        Hunting, // Активное преследование
        Escaping, // Бегство
        Paused, // Пауза в игре

    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        switch (state) // Действия моба в зависимости от состояния
        {
            case States.WaySearching:
                FindWayToPlayer();
                break;
            case States.Moving:
                FollowPlayer();
                break;
            case States.Hunting:
                HuntPlayer();
                break;
            case States.Escaping:
                HideFromLight();
                break;
        }
    }
    
    public delegate void OnTriggerAction();
    public static event OnTriggerAction OnTrigger;

   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTrigger?.Invoke();
            Debug.Log("am");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Hexagon"))
        {
            currentHex = collision.gameObject.GetComponent<Hexagon>(); // находим гекс на котором в данный момент находится моб
        }
        else
        {
            currentHex = null;
        }
    }

    private void HuntPlayer() // Активное преследование игрока. Состояние - Hunting
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position,
            speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, player.transform.position) < 1f)
        {
            //Смеееееееееерть
        }
    }

    public void HideFromLight() // Сбежать в ужасе. Состояние - Escaping (Потом переименовать метод в Escape)
    {
        
    }

    private void FollowPlayer() // Движение по вычисленному пути до игрока. Состояние - Moving
    {
        Debug.Log("Enter method FollowPlayer");
        transform.position = Vector2.MoveTowards(transform.position, way.Peek().transform.position,
            speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, way.Peek().transform.position) < 1f)
        {
            way.Pop();
        }
        
    }

    private void FindWayToPlayer() // Поиск пути до игрока. Состояние - WaySearching
    {
        if (player.GetCurrentHexagon().gameObject.GetComponent<Hexagon>() != null)
        {
            targetHex = player.GetCurrentHexagon().gameObject.GetComponent<Hexagon>();
        }
        if (currentHex != null && targetHex != null)
        {
            xHex = currentHex;
            way = new Stack<Hexagon>();
            hexagons = FindObjectsOfType<Hexagon>();
            xHex.GetComponent<Hexagon>().SetVisited(); // и отметить его как посещенный.

            while (xHex != targetHex) // Пока гекс моба не совпадает с гексом игрока
            {
                List<Hexagon> freeUnVisitedNeighbours = ReturnFreeUnVisitedNeighbours(xHex.ReturnFreeNeighbours());

                if (freeUnVisitedNeighbours.Count > 0) // Если текущий гекс имеет непосещенных «соседей»
                {
                    way.Push(xHex); // Протолкнуть текущий гекс в стек
                    xHex = freeUnVisitedNeighbours[0]; // Выбрать случайный доступный гекс из соседних, сделать выбранный гекс текущим
                    xHex.GetComponent<Hexagon>().SetVisited(); // и отметить его как посещенный.
                }
                else
                {
                    if (way.Count > 0) // Иначе если стек не пуст
                    {
                        xHex = way.Pop(); // Выдернуть гекс из стека и Сделать его текущим
                    }
                    else
                    {
                        //Пути нету, пока фиг знает что в этом случае делать
                        break;
                    }
                }

            }
            way.Push(xHex);

            way = ReverseStack(way);

            publicWay = way.ToArray();
            foreach (Hexagon hexagon in hexagons)  // отмечаем все гексы непосещенными для следующих проходов алгоритма поиска пути
            {
                hexagon.SetUnvisited();
            }
        }
    }

    private List<Hexagon> ReturnFreeUnVisitedNeighbours(List<Hexagon> freeNeighbours) // Вернуть все непосещенные соседние гексы (поиск пути)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        List<Hexagon> freeUnVisitedNeighbours = new List<Hexagon>();
        foreach (Hexagon neighbour in freeNeighbours)
        {
            if (!neighbour.GetComponent<Hexagon>().IsVisited())
            {
                freeUnVisitedNeighbours.Add(neighbour);
            }
        }

        GetComponent<CircleCollider2D>().enabled = true;



        return freeUnVisitedNeighbours;
    }

    private Stack<Hexagon> ReverseStack(Stack<Hexagon> stack) // Разворот стэка гексов
    {
        Stack<Hexagon> newStack = new Stack<Hexagon>();
        int x = stack.Count;
        for (int i = 0; i < x; i++)
        {
            newStack.Push(stack.Pop());
        }

        return newStack;
    }
}
