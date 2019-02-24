using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class EnemyManager : MonoBehaviour, Manager
{
    Player _player;

    public List<Enemy> _enemies_test;

    void Start()
    {
        _player = FindObjectOfType<Player>();
        //_enemies = new List<Enemy>(FindObjectsOfType<Enemy>());

        _enemies_test = new List<Enemy>(FindObjectsOfType<Enemy>());

        _player.CurrentHexagonChanged += OnPlayerHexagonChanged;

        
        foreach (Enemy enemy in _enemies_test)
        {
            enemy.state = Enemy.States.WaySearching;
        }
    }

    private void OnPlayerHexagonChanged()
    {
        foreach (Enemy enemy in _enemies_test)
        {
            enemy.state = Enemy.States.WaySearching;
        }
    }

    void Update()
    {
        /*foreach (Enemy enemy in _enemies)
        {
            enemy.FollowPlayer();
        }*/

        foreach (Enemy enemy in _enemies_test)
        {
            if (enemy.way.Count != 0)
            {
                Debug.Log("Way is not empty");
                enemy.state = Enemy.States.Moving;
            }
            if (enemy.currentHex == _player._currentHexagon)
            {
                enemy.state = Enemy.States.Hunting;
            }
        }
    }

    public void StartManager()
    {
        
    }

    public void PauseManager()
    {
        
    }

    public void ResumeManager()
    {
        
    }
}

