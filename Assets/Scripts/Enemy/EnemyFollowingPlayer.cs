﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowingPlayer : MonoBehaviour
{

    public Player _player;
    public float speed;
    public GameObject currentHexagon, targetHexagon, curHex;
    private Stack<GameObject> mainPath;
    private bool pathCalculated;
    
    // Start is called before the first frame update
    void Start()
    {
        
        pathCalculated = false;
        //StartHunting();

    }

    private void Awake()
    {
        _player.CurrentHexagonChanged += PlayerOnCurrentHexagonChanged;
    }

    private void PlayerOnCurrentHexagonChanged()
    {
        StartHunting();
    }

    // Update is called once per frame
    void Update()
    {
        if (pathCalculated)
        {
            transform.position =
                Vector2.MoveTowards(transform.position, mainPath.Peek().transform.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, mainPath.Peek().transform.position) < 1f)
            {
                mainPath.Pop();
            }
        }
    }

    private void StartHunting()
    {
        targetHexagon = _player.GetCurrentHexagon();
    }

    

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if(collision.CompareTag("Hexagon"))
        {
            currentHexagon = collision.gameObject;
            curHex = GetCurrentHexagon(); //1. Сделать начальный гекс текущим
            mainPath = ReverseStack(CalculatePath());
            
        }
        else
        {
            currentHexagon = null;
        }
    }


    public GameObject GetCurrentHexagon()
    {
        return currentHexagon;
    }

    public Stack<GameObject> CalculatePath()
    {
        Debug.Log("CalclulatePath");
        Stack<GameObject> path = new Stack<GameObject>(); 
        if (curHex != null && targetHexagon != null)
        {
            curHex.GetComponent<Hexagon>().SetVisited(); // и отметить его как посещенный.
            
            while (curHex.transform.position != targetHexagon.transform.position) //Пока гекс моба не совпадает с гексом игрока
            {
                List<GameObject> freeUnVisitedNeighbours =
                    ReturnFreeUnVisitedNeighbours(curHex.GetComponent<Hexagon>().ReturnFreeNeighbours());
                
                if (freeUnVisitedNeighbours.Count > 0) //Если текущий гекс имеет непосещенных «соседей»
                {
                    path.Push(curHex); //Протолкнуть текущий гекс в стек
                    curHex = freeUnVisitedNeighbours[0]; //Выбрать случайный доступный гекс из соседних, сделать выбранный гекс текущим
                    curHex.GetComponent<Hexagon>().SetVisited(); // и отметить его как посещенный.
                }
                else
                {
                    if (path.Count > 0) //Иначе если стек не пуст
                    {
                        curHex = path.Pop(); //Выдернуть гекс из стека и Сделать его текущим
                    }
                    else
                    {
                        //Пути нету, пока фиг знает что в этом случае делать
                        break;
                    }
                }
            }
            path.Push(curHex);
            
            pathCalculated = true;
        }

        

        return path;
    }

    private List<GameObject> ReturnFreeUnVisitedNeighbours(List<GameObject> freeNeighbours)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        List<GameObject> _freeUnVisitedNeighbours = new List<GameObject>();
        foreach (GameObject neighbour in freeNeighbours)
        { 
            if (!neighbour.GetComponent<Hexagon>().IsVisited())
            {
                _freeUnVisitedNeighbours.Add(neighbour);
            }
        }
        GetComponent<CircleCollider2D>().enabled = true;
        return _freeUnVisitedNeighbours;
    }

    private Stack<GameObject> ReverseStack(Stack<GameObject> path)
    {
        Stack<GameObject> _path = new Stack<GameObject>();

        int x = path.Count;

        for (int i = 0; i < x; i++)
        {
            _path.Push(path.Pop());
        }
        Debug.Log(_path.Count);
        
        return _path;
    }

    public void HideFromLight()
    {
        
    }

    
}
