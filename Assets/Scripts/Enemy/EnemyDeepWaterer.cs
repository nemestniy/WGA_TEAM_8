﻿using UnityEngine;
using System.Collections.Generic;
using System;
using Pathfinding;

public class EnemyDeepWaterer : MonoBehaviour, IEnemy
{
    public Player player;
    public AstarPath path;


    [SerializeField] [ShowOnly] private State state;
    EnemySavedState savedState;

    public float speed;
    public float _normalAnimationSpeed;
    
    public float maneurAngle;
    public float speedIncreaseCoefficient;

    public float distanceToPlayer;
    public float eventHorizon;// расстояние на котором враг уже не будет пытаться увернуться от луча фонаря, а просто пойдет напролом (як горизонт событий черной дыры)

    [HideInInspector] public delegate void EnemyEvents();
    [HideInInspector] public event EnemyEvents Player_death;

    public AIDestinationSetter destinationSetter;
    public AIPath aiPath;

    public float time;
    public bool inLight;
    [HideInInspector] public bool escapePointCreated;
    [HideInInspector] public bool maneurPointCreated;
    
    
    private Animator animator;
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int GotDamage = Animator.StringToHash("GotDamage");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Start()
    {
        player = Player.Instance;
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
            case State.WaySearching:
                //
                break;
            case State.Moving:
                Move();
                break;
            case State.Hunting:
                //
                break;
            case State.Escaping:
                Escape();
                break;
            case State.Maneuring:
                Maneur();
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
            other.transform.GetChild(0).GetComponent<Animator>().SetTrigger(GotDamage);
//            OnTrigger?.Invoke();
        }
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

    public void StartAnimation()
    {
        if(animator != null)
            animator.SetBool(IsRunning, true);
    }

    public void StopAnimation()
    {
        if(animator != null)
            animator.SetBool(IsRunning, false);
    }

    private void Move() // Движение по вычисленному пути до игрока. Состояние - Moving
    {
        //player = FindObjectOfType<Player>();
        StartAnimation();
        aiPath.maxSpeed = speed;

        //destinationSetter.target = player.transform;
        GetComponent<AIPath>().canMove = true;
    }

    public void Pause()
    {
        StopAnimation();
        Debug.Log("Enemy Paused");
        GetComponent<AIPath>().canMove = false;
        
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public AIDestinationSetter GetDestinationSetter()
    {
        return destinationSetter;
    }

    public void SetState(State state)
    {
        this.state = state;
    }

    public EnemyType GetEnemyType()
    {
        return EnemyType.DeepWaterer;
    }

    public void SetOnLight()
    {
        inLight = true;
    }

    public void SetOutOfLight()
    {
        inLight = false;
    }

    public State GetState()
    {
        return state;
    }
}
