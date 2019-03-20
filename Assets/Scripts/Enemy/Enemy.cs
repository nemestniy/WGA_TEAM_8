using UnityEngine;
using System.Collections.Generic;
using System;

public class Enemy : MonoBehaviour
{
    public Player player;
    public AstarPath path;

    public States state;

    private float speed;
    private Pathfinding.AIDestinationSetter destinationSetter;

    public delegate void EnemyEvents();
    public event EnemyEvents Player_death;
    public event EnemyEvents InLight; // Моб в луче фонаря

    public enum States // Состояния моба
    {
        Moving, // Движение к игроку по лабиринту
        WaySearching, // Поиск пути до игрока
        Waiting, // Ожидание
        Hunting, // Активное преследование
        Frying, // Прожарка в луче фонаря
        Escaping, // Бегство
        Paused, // Пауза в игре

    }

    private void Start()
    {
        destinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
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
            case States.Frying:
                Maneur();
                break;
            case States.Paused:
                Pause();
                break;
        }
    }

    

    public delegate void OnTriggerAction();
    public static event OnTriggerAction OnTrigger; // событие для вызова катсцены смерти игрока когда его съели


    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTrigger?.Invoke();
            Debug.Log("am");
        }
    }

    private void HuntPlayer() // Активное преследование игрока. Состояние - Hunting
    {
               
    }

    public void Escape() // Сбежать в ужасе. Состояние - Escaping (Потом переименовать метод в Escape)
    {
        if (destinationSetter.target == player.transform)
        {
            float r = UnityEngine.Random.Range(50f, 100f);
            int alfa = UnityEngine.Random.Range(0, 359);

            float X = player.transform.position.x + (Mathf.Sin(alfa) * r);
            float Y = player.transform.position.y + (Mathf.Cos(alfa) * r);

            Vector2 escapePoint = new Vector2(X, Y);

            GameObject escapeObject = new GameObject("EscapePoint");
            escapeObject.transform.position = escapePoint;

            destinationSetter.target = escapeObject.transform;

            GetComponent<Pathfinding.AIPath>().maxSpeed = 20f;
        }
    }

    private void Maneur()
    {
        throw new NotImplementedException();
    }

    private void FollowPlayer() // Движение по вычисленному пути до игрока. Состояние - Moving
    {
        player = FindObjectOfType<Player>();

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
