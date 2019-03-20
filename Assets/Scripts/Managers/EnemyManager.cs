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
    
    public static EnemyManager Instance { get; private set; }

    
    public EnemyManager() : base()
    {
        Instance = this;
    }

    private void Start()
    {
        hexagonsGenerator.MapIsCreate += OnMapCreated;
        foreach (Enemy enemy in _enemies)
        {
            enemy.InLight += OnEnemyInLight;
        }

    }

    private void OnEnemyInLight()
    {
        throw new NotImplementedException();
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

    public void OnEnemyOnLight(Enemy enemy)
    {
        Debug.Log("isOnFire: " + enemy.name);
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

