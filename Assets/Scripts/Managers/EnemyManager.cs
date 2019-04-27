using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class EnemyManager : MonoBehaviour, Manager
{
    public Player _player;
    public AstarPath path;

    public List<IEnemy> enemies;
    public List<EnemyDeepWaterer> enemyDeepWaterers;
    public List<EnemyStatue> enemyStatues;

    public HexagonsGenerator hexagonsGenerator;

    public List<Transform>visibleEnemiesList;

    private int framecount = 0;

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
        hexagonsGenerator.MapIsCreate += OnMapCreated;
    }

    private void OnMapCreated()
    {
        path.Scan();
        foreach (EnemyDeepWaterer enemy in enemyDeepWaterers)
        {
            enemy.GetComponent<Pathfinding.AIDestinationSetter>().target = FindObjectOfType<Player>().transform;
            enemy.state = State.Moving;
            Debug.Log("onMapCreate");
        }

        IsLoaded = true;

    }

    void Update()
    {
        foreach (EnemyDeepWaterer enemy in enemyDeepWaterers)
        {
            if (Vector2.Distance(enemy.transform.position, enemy.destinationSetter.target.position) <0.5f)
            {
                enemy.state = State.Moving;
                enemy.escapePointCreated = false;
                enemy.maneurPointCreated = false;
                enemy.aiPath.maxSpeed = enemy.speed;
            }

            if (enemy.inLight)
            {
                if (_player.transform.GetChild(0).GetComponent<Lamp>()._isFrying)
                {
                    if(Time.time - enemy.time > 3.0f)
                    {
                        enemy.state = State.Escaping;
                    }
                    else
                    {
                        if (enemy.distanceToPlayer > enemy.eventHorizon)
                        {
                            enemy.state = State.Maneuring;
                        }
                    }
                }
                else
                {
                    enemy.time = Time.time;
                }
            }
            else
            {
                enemy.time = Time.time;
            }


        }

        UpdateEnemiesState();
        framecount++;
        if (framecount > 60)
        {
            path.Scan();
            framecount = 0;
        }
    }

    public void StartManager()
    {
        _player = Player.Instance;
        enemyDeepWaterers = new List<EnemyDeepWaterer>(FindObjectsOfType<EnemyDeepWaterer>());
        enemyStatues = new List<EnemyStatue>(FindObjectsOfType<EnemyStatue>());
        enemies = new List<IEnemy>();
        enemies.AddRange(FindObjectsOfType<EnemyDeepWaterer>());
        enemies.AddRange(FindObjectsOfType<EnemyStatue>());

        foreach (IEnemy enemy in enemyDeepWaterers)
        {
            path.Scan();
            switch (enemy.GetEnemyType())
            {
                case EnemyType.DeepWaterer:
                    enemy.SetState(State.Moving);
                    break;

                case EnemyType.Statue:
                    enemy.SetState(State.Waiting);
                    break;
            }
        }
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
        
            foreach (var enemy in visibleEnemiesList)
            {
                enemy.GetComponent<EnemyDeepWaterer>().inLight = true;
            }

            foreach (EnemyDeepWaterer enemy in enemyDeepWaterers)
            {
                if (!visibleEnemiesList.Contains(enemy.transform))
                {
                    enemy.GetComponent<EnemyDeepWaterer>().inLight = false;
                }
            }
        
    }
}

