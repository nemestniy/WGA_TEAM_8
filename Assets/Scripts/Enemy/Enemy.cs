using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Player _player;
    public float speed;
    public GameObject currentHexagon, targetHexagon, curHex;
    private Stack<GameObject> _mainPath;
    private bool _pathCalculated;

    public Hexagon[] hexagons;

    public delegate void EnemyEvents();
    public event EnemyEvents CurrentHexagonChanged;
    
    // Start is called before the first frame update
    void Start()
    {
        _pathCalculated = false;

        
    }

    private void Awake()
    {
        _player.CurrentHexagonChanged += PlayerOnCurrentHexagonChanged;
        CurrentHexagonChanged += OnCurrentHexagonChanged;
        
    }

    private void OnCurrentHexagonChanged()
    {
        _pathCalculated = false;
        StartHunting();
    }

    private void PlayerOnCurrentHexagonChanged()
    {
        _pathCalculated = false;
        StartHunting();
    }

    

    private void StartHunting()
    {
        targetHexagon = _player.GetCurrentHexagon();
    }

    

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if(collision.CompareTag("Hexagon") && !_pathCalculated)
        {
            currentHexagon = collision.gameObject;
            _mainPath = CalculatePath();
            
        }
        else
        {
            currentHexagon = null;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Hexagon") && !_pathCalculated)
        {
            if (currentHexagon != collision.gameObject && CurrentHexagonChanged != null)
            {
                CurrentHexagonChanged();
            }
            _mainPath = CalculatePath();
            
        }
        else
        {
            currentHexagon = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        throw new System.NotImplementedException();
    }


    public GameObject GetCurrentHexagon()
    {
        return currentHexagon;
    }

    public Stack<GameObject> CalculatePath()
    {
        Debug.Log("Enter method CalclulatePath");

        hexagons = FindObjectsOfType<Hexagon>();
        
        curHex = GetCurrentHexagon();                                                                                    // Сделать начальный гекс текущим
        Stack<GameObject> path = new Stack<GameObject>(); 
        
        Debug.Log("Stack created");
        
        if (curHex != null && targetHexagon != null)
        {
            curHex.GetComponent<Hexagon>().SetVisited();                                                                 // и отметить его как посещенный.
            
            while (curHex.transform.position != targetHexagon.transform.position)                                        // Пока гекс моба не совпадает с гексом игрока
            {
                List<GameObject> freeUnVisitedNeighbours =
                    ReturnFreeUnVisitedNeighbours(curHex.GetComponent<Hexagon>().ReturnFreeNeighbours());
                
                Debug.Log("freeUnVisitedNeighbours.Count - " + freeUnVisitedNeighbours.Count);
                
                if (freeUnVisitedNeighbours.Count > 0)                                                                   // Если текущий гекс имеет непосещенных «соседей»
                {
                    path.Push(curHex);                                                                                   // Протолкнуть текущий гекс в стек
                    Debug.Log(path.Count);
                    curHex = freeUnVisitedNeighbours[0];                                                                 // Выбрать случайный доступный гекс из соседних, сделать выбранный гекс текущим
                    curHex.GetComponent<Hexagon>().SetVisited();                                                         // и отметить его как посещенный.
                }
                else
                {
                    if (path.Count > 0)                                                                                  // Иначе если стек не пуст
                    {
                        curHex = path.Pop();                                                                             // Выдернуть гекс из стека и Сделать его текущим
                    }
                    else
                    {
                        //Пути нету, пока фиг знает что в этом случае делать
                        break;
                    }
                }
            }
            path.Push(curHex);

            foreach (GameObject o in path)
            {
                Debug.Log("!!!! - " + o.transform.position + " " + path.Count);
            }

            path = ReverseStack(path);

            foreach (Hexagon hexagon in hexagons)
            {
                hexagon.SetUnvisited();
            }
            
            _pathCalculated = true;
           
        }

        return path;
    }

    private List<GameObject> ReturnFreeUnVisitedNeighbours(List<GameObject> freeNeighbours)
    {
        Debug.Log("Enter method ReturnFreeUnVisitedNeighbours()");
        GetComponent<CircleCollider2D>().enabled = false;
        List<GameObject> freeUnVisitedNeighbours = new List<GameObject>();
        foreach (GameObject neighbour in freeNeighbours)
        { 
            if (!neighbour.GetComponent<Hexagon>().IsVisited())
            {
                freeUnVisitedNeighbours.Add(neighbour);
                Debug.Log("ReturnFreeUnVisitedNeighbours: Added " + neighbour.transform.position);
            }
        }
        GetComponent<CircleCollider2D>().enabled = true;
        
        
        
        return freeUnVisitedNeighbours;
    }

    private Stack<GameObject> ReverseStack(Stack<GameObject> path)
    {
        Debug.Log("Enter method ReverseStack");
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

    public void FollowPlayer()
    {
        
        
        if (_pathCalculated)
        {
            transform.position = Vector2.MoveTowards(transform.position, _mainPath.Peek().transform.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, _mainPath.Peek().transform.position) < 1f)
            {
                _mainPath.Pop();
            }
        }
        else
        {
            //_mainPath = CalculatePath();
        }
    }

    
}
