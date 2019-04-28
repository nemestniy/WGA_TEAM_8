﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class EnemyManager : MonoBehaviour, Manager
{
    public Player _player;
    public AstarPath path;

    [SerializeField] [ShowOnly] private List<IEnemy> enemies;
    [SerializeField] [ShowOnly] private List<EnemyDeepWaterer> enemyDeepWaterers;
    [SerializeField] [ShowOnly] private List<EnemyStatue> enemyStatues;

    public HexagonsGenerator hexagonsGenerator;

    public List<Transform> visibleEnemiesList;

    private int framecount = 0;
    private CommonUtils commonUtils;

    public float DistanceToClosestEnemy
    {
        get
        {
            if (enemies.Count == 0)
                return -1; //in case of error
            
            float minDist = Vector2.Distance(enemies[0].GetTransform().position, Player.Instance.transform.position);
            for(int i = 1; i < enemies.Count; i++)
            {
                var curEnemyDist = Vector2.Distance(enemies[i].GetTransform().position, Player.Instance.transform.position);
                if (curEnemyDist < minDist)
                {
                    minDist = curEnemyDist;
                }
            }
            return minDist;
        }
    }
    
    
    public static EnemyManager Instance { get; private set; }
    public bool IsLoaded { get; private set; }
    
    public EnemyManager() : base()
    {
        Instance = this;
    }
    
    private void Start()
    {
        //hexagonsGenerator.MapIsCreate += OnMapCreated;
        enemyDeepWaterers = new List<EnemyDeepWaterer>(FindObjectsOfType<EnemyDeepWaterer>());
        enemyStatues = new List<EnemyStatue>(FindObjectsOfType<EnemyStatue>());
        enemies = new List<IEnemy>();
        enemies.AddRange(FindObjectsOfType<EnemyDeepWaterer>());
        enemies.AddRange(FindObjectsOfType<EnemyStatue>());

        commonUtils = CommonUtils.Instance;
        _player = Player.Instance;
    }

    /*private void OnMapCreated()
    {
        Debug.Log("onMapCreate");
        path.Scan();
        foreach (EnemyDeepWaterer enemy in enemyDeepWaterers)
        {
            enemy.GetComponent<Pathfinding.AIDestinationSetter>().target = Player.Instance.transform;
            enemy.SetState(State.Moving);
            
        }

        IsLoaded = true;

    }*/

    void Update()
    {
        foreach (EnemyDeepWaterer deepWaterer in enemyDeepWaterers)
        {
            if (deepWaterer.GetDestinationSetter().target != null && Vector2.Distance(deepWaterer.transform.position, deepWaterer.GetDestinationSetter().target.position) < 0.5f)
            {
                deepWaterer.SetState(State.Moving);
                deepWaterer.escapePointCreated = false;
                deepWaterer.maneurPointCreated = false;
                deepWaterer.aiPath.maxSpeed = deepWaterer.speed;
            }

            if (deepWaterer.inLight)
            {
                if (_player.transform.GetChild(0).GetComponent<Lamp>()._isFrying)
                {
                    if (Time.time - deepWaterer.time > 3.0f)
                    {
                        deepWaterer.SetState(State.Escaping);
                    }
                    else
                    {
                        if (deepWaterer.distanceToPlayer > deepWaterer.eventHorizon)
                        {
                            deepWaterer.SetState(State.Maneuring);
                        }
                    }
                }
                else
                {
                    deepWaterer.time = Time.time;
                }
            }
            else
            {
                deepWaterer.time = Time.time;
            }

        }

        foreach (EnemyStatue statue in enemyStatues)
        {
            if (statue.GetState() == State.Waiting && !statue.inLight)
            {
                GameObject statueHex = commonUtils.GetHexagonByPoint(statue.transform.position);
                GameObject playerHex = commonUtils.GetHexagonByPoint(_player.transform.position);
                if (statueHex == playerHex && statueHex != null && playerHex != null)
                {
                    Debug.Log("Player near statue");
                    statue.GetDestinationSetter().target = _player.transform;
                    statue.SetState(State.Moving);
                } 
            }

            if (statue.inLight)
            {
                statue.SetState(State.Waiting);
            }
        }

        UpdateEnemiesState();
        framecount++;
        if (framecount > 60)
        {
            path.Scan();
            Debug.Log("Scanning...");
            framecount = 0;
        }
    }

    public void StartManager()
    {
        path.Scan();

        CommonUtils.Instance.InitializeCommonUtils();

        foreach (IEnemy enemy in enemies)
        {
            path.Scan();
            switch (enemy.GetEnemyType())
            {
                case EnemyType.DeepWaterer:
                    enemy.GetDestinationSetter().target = Player.Instance.transform;
                    enemy.SetState(State.Moving);
                    break;

                case EnemyType.Statue:
                    enemy.SetState(State.Waiting);
                    break;
            }
        }
        IsLoaded = true;
    }

    public void PauseManager()
    {
        foreach (IEnemy enemy in enemies)
        {
            //enemy.SetState(State.Paused);
        }
    }

    public void ResumeManager()
    {
        
    }

    private void UpdateEnemiesState()
    {
        
        foreach (Transform enemy in visibleEnemiesList)
        {
            enemy.gameObject.GetComponent<IEnemy>().SetOnLight();
        }

        foreach (IEnemy enemy in enemies)
        {
            if (!visibleEnemiesList.Contains(enemy.GetTransform()))
            {
                enemy.SetOutOfLight();
            }
        }
        
    }
}

