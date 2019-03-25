using UnityEngine;
using System.Collections.Generic;
using System;

public class Enemy : MonoBehaviour, IEnemy
{
    public Player player;
    public AstarPath path;

    public States state;

    public float speed;
    
    public float maneurAngle;
    public float speedDecreaseCoefficient;

    [HideInInspector] public delegate void EnemyEvents();
    [HideInInspector] public event EnemyEvents Player_death;

    public Pathfinding.AIDestinationSetter destinationSetter;
    public Pathfinding.AIPath aiPath;

    public float time;
    public bool inLight;
    [HideInInspector] public bool escapePointCreated;
    
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
        Debug.Log(player.transform.rotation.eulerAngles.z);
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
            float alfa = UnityEngine.Random.Range(0, 359) * Mathf.PI / 180;

            float X = player.transform.position.x - (Mathf.Sin(alfa) * r);
            float Y = player.transform.position.y + (Mathf.Cos(alfa) * r);

            Vector2 escapePoint = new Vector2(X, Y);

            GameObject escapeObject = new GameObject("EscapePoint");
            escapeObject.transform.position = escapePoint;

            destinationSetter.target = escapeObject.transform;

            aiPath.maxSpeed = 20f;

            escapePointCreated = true;
        }
        
    }

    private void Maneur() // Маневрирование(уклонение) от луча. Состояние - Frying
    {
        while(destinationSetter.target == player.transform)// Доделать проверку на свободное место для маневра
        {
           
            aiPath.maxSpeed = speed * speedDecreaseCoefficient;
            float r = Vector2.Distance(player.transform.position, transform.position);
            float alpha = (player.transform.eulerAngles.z + maneurAngle * (UnityEngine.Random.Range(-1, 1) > 0 ? 1 : -1)) * Mathf.PI / 180;

            Debug.Log("x - " + player.transform.position.x + ", y - " + player.transform.position.y + ", angle - " + player.transform.eulerAngles.z + ", alpha - "+ alpha + ", distance - " + r);
            Debug.Log("X = " + player.transform.position.x + " + " + "(Sin(" + alpha + ") * " + r +")");

            float X = player.transform.position.x - (Mathf.Sin(alpha) * r);
            float Y = player.transform.position.y + (Mathf.Cos(alpha) * r);
            

            Vector2 maneurPoint = new Vector2(X, Y);

            GameObject maneurObject = new GameObject("ManeurPoint");
            maneurObject.transform.position = maneurPoint;

            destinationSetter.target = maneurObject.transform; 
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
