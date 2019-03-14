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
        foreach (Enemy enemy in _enemies)
        {
            path.Scan();
            enemy.GetComponent<Pathfinding.AIDestinationSetter>().target = FindObjectOfType<Player>().transform;
            enemy.state = Enemy.States.Moving;
            Debug.Log("onMapCreate");
        }

        IsLoaded = true;

    }

    void Update()
    {
                
    }

    public void StartManager()
    {
        Debug.Log("Enemy Manager Started");

        

        _player = FindObjectOfType<Player>();

        _enemies = new List<Enemy>(FindObjectsOfType<Enemy>());

        foreach (Enemy enemy in _enemies)
        {
            path.Scan();
            enemy.GetComponent<Pathfinding.AIDestinationSetter>().target = FindObjectOfType<Player>().transform;
            enemy.state = Enemy.States.Moving;
        }
        
    }

    public void PauseManager()
    {
        
    }

    public void ResumeManager()
    {
        
    }

}

