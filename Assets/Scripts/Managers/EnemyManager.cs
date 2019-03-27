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
    public bool IsLoaded { get; private set; }
    

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
            }

            if (!enemy.inLight)
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
                    enemy.state = Enemy.States.Maneuring;
                }
            }
        }
    }

    public void StartManager()
    {
        Debug.Log("Enemy Manager Started");

        _player = FindObjectOfType<Player>();

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

}

