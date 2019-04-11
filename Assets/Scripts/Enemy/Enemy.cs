using UnityEngine;
using System.Collections.Generic;
using System;
using Pathfinding;

public class Enemy : MonoBehaviour, IEnemy
{
    public Player player;
    public AstarPath path;

    public States state;

    public float speed;
    
    public float maneurAngle;
    public float speedIncreaseCoefficient;

    public float distanceToPlayer;
    public float eventHorizon;// расстояние на котором враг уже не будет пытаться увернуться от луча фонаря, а просто пойдет напролом (як горизонт событий черной дыры)

    [HideInInspector] public delegate void EnemyEvents();
    [HideInInspector] public event EnemyEvents Player_death;

    public Pathfinding.AIDestinationSetter destinationSetter;
    public Pathfinding.AIPath aiPath;

    public float time;
    public bool inLight;
    [HideInInspector] public bool escapePointCreated;
    [HideInInspector] public bool maneurPointCreated;

    public enum States // Состояния моба
    {
        Moving, // Движение к игроку по лабиринту
        WaySearching, // Поиск пути до игрока
        Waiting, // Ожидание
        Hunting, // Активное преследование
        Maneuring, // Прожарка в луче фонаря
        Escaping, // Бегство
        Paused, // Пауза в игре

    }

    private void Start()
    {
        destinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        aiPath = GetComponent<Pathfinding.AIPath>();
        inLight = false;
        escapePointCreated = false;
        maneurPointCreated = false;
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
                Escape();
                break;
            case States.Maneuring:
                Maneur();
                break;
            case States.Paused:
                Pause();
                break;
        }
        distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
    }

    

    public delegate void OnTriggerAction();
    public static event OnTriggerAction OnTrigger; // событие для вызова катсцены смерти игрока когда его съели


    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTrigger?.Invoke();
        }
    }

    private void HuntPlayer() // Активное преследование игрока. Состояние - Hunting
    {
               
    }

    public void Escape() // Сбежать в ужасе. Состояние - Escaping (Потом переименовать метод в Escape)
    {
        if (!escapePointCreated)
        {
            float r = UnityEngine.Random.Range(50f, 100f);
            float alpha = UnityEngine.Random.Range(0, 359) * Mathf.PI / 180;

            float X = player.transform.position.x - (Mathf.Sin(alpha) * r);
            float Y = player.transform.position.y + (Mathf.Cos(alpha) * r);

            GraphNode escapePoint = AstarPath.active.GetNearest(new Vector2(X, Y)).node;

            if (escapePoint.Walkable)
            {
                GameObject escapeObject = new GameObject("EscapePoint");
                escapeObject.transform.position = (Vector3)escapePoint.position;
                destinationSetter.target = escapeObject.transform;
                aiPath.maxSpeed = 20f;
                escapePointCreated = true; 
            }
        }
        
    }

    private void Maneur() // Маневрирование(уклонение) от луча. Состояние - Frying
    {
        if(!maneurPointCreated)// Доделать проверку на свободное место для маневра
        {
            float r = Vector2.Distance(player.transform.position, transform.position) - 5f;
            float side = UnityEngine.Random.Range(-10, 10) > 0 ? -1 : 1;
            Debug.LogWarning("side = " + side);
            float alpha = (player.transform.eulerAngles.z + maneurAngle * side) * Mathf.PI / 180;

            float X = player.transform.position.x - (Mathf.Sin(alpha) * r);
            float Y = player.transform.position.y + (Mathf.Cos(alpha) * r);

            GraphNode maneurPoint = AstarPath.active.GetNearest(new Vector2(X, Y)).node;

            if (maneurPoint.Walkable)
            {
                GameObject maneurObject = new GameObject("ManeurPoint");
                maneurObject.transform.position = (Vector3)maneurPoint.position;
                destinationSetter.target = maneurObject.transform;
                aiPath.maxSpeed = speed * speedIncreaseCoefficient;
                maneurPointCreated = true;
            }
        }
    }

    

    private void FollowPlayer() // Движение по вычисленному пути до игрока. Состояние - Moving
    {
        player = FindObjectOfType<Player>();

        aiPath.maxSpeed = speed;

        destinationSetter.target = player.transform;
        GetComponent<Pathfinding.AIPath>().canMove = true;
    }

    private void FindWayToPlayer() // Поиск пути до игрока. Состояние - WaySearching
    {
        //path.Scan();
    }

    private void Pause()
    {
        Debug.Log("Enemy Paused");
        GetComponent<Pathfinding.AIPath>().canMove = false;
    }
}
