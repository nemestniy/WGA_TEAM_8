﻿using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public Player player;
    public AstarPath path;

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
    public static event OnTriggerAction OnTrigger; // событие для вызова катсцены смерти игрока когда его съели


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
        player = FindObjectOfType<Player>();
        GetComponent<Pathfinding.AIPath>().canMove = true;

    }

    private void FindWayToPlayer() // Поиск пути до игрока. Состояние - WaySearching
    {
        //path.Scan();
        
    }
}
