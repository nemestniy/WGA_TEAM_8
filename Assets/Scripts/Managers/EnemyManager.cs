using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class EnemyManager : MonoBehaviour, Manager
{
    public Player _player;
    public AstarPath path;

    public List<Enemy> _enemies;

    public HexagonsGenerator hexagonsGenerator;

    public List<Transform>visibleEnemiesList;

    public float DistanceToClosestEnemy
    {
        get
        {
            if (_enemies.Count == 0)
                return -1; //in case of error
            
            float minDist = Vector2.Distance(_enemies[0].transform.position,Player.Instance.transform.position);
            for(int i = 1; i < _enemies.Count; i++)
            {
                var curEnemyDist = Vector2.Distance(_enemies[i].transform.position,Player.Instance.transform.position);
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
        foreach (Enemy enemy in _enemies)
        {
            enemy.GetComponent<Pathfinding.AIDestinationSetter>().target = FindObjectOfType<Player>().transform;
            enemy.state = Enemy.States.Moving;
            Debug.Log("onMapCreate");
        }

        IsLoaded = true;

    }

    void Update()
    {
        foreach (var enemy in _enemies)
        {
            if (Vector2.Distance(enemy.transform.position, enemy.destinationSetter.target.position) <0.5f)
            {
                enemy.state = Enemy.States.Moving;
                enemy.escapePointCreated = false;
                enemy.maneurPointCreated = false;
                enemy.aiPath.maxSpeed = enemy.speed;
            }

            /*if (!enemy.inLight)
            {
                enemy.time = Time.time;
            }
            else
            {
                if(Time.time - enemy.time > 3.0f)
                {
                    enemy.state = Enemy.States.Escaping;
                }
                else
                {
                    if (enemy.distanceToPlayer > enemy.eventHorizon)
                    {
                        enemy.state = Enemy.States.Maneuring;
                    }
                }
            }*/

            if (enemy.inLight)
            {
                if (PlayerManager.Instance.CurrentLampMode == 1)
                {
                    if(Time.time - enemy.time > 3.0f)
                    {
                        enemy.state = Enemy.States.Escaping;
                    }
                    else
                    {
                        if (enemy.distanceToPlayer > enemy.eventHorizon)
                        {
                            enemy.state = Enemy.States.Maneuring;
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
    }

    public void StartManager()
    {
        Debug.Log("Enemy Manager Started");

        _player = Player.Instance;

        _enemies = new List<Enemy>(FindObjectsOfType<Enemy>());

        foreach (Enemy enemy in _enemies)
        {
            path.Scan();
            enemy.state = Enemy.States.Moving;
        }
        
    }

    public void PauseManager()
    {
        Debug.Log("Pause EnemyManager");
        foreach (Enemy enemy in _enemies)
        {
            enemy.state = Enemy.States.Paused;
        }
    }

    public void ResumeManager()
    {
        
    }

    private void UpdateEnemiesState()
    {
        if (PlayerManager.Instance.ChangingState == 1)
        {
            foreach (var enemy in visibleEnemiesList)
            {
                enemy.GetComponent<Enemy>().inLight = true;
            }

            foreach (var enemy in _enemies)
            {
                if (!visibleEnemiesList.Contains(enemy.transform))
                {
                    enemy.GetComponent<Enemy>().inLight = false;
                }
            }
        }
    }
}

