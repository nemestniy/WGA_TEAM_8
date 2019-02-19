using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class EnemyManager : MonoBehaviour, Manager
{
    public List<Player> _player;
    public List<Enemy> _enemies;

    void Start()
    {
        _player = new List<Player>(FindObjectsOfType<Player>());
        _enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
    }

    
    void Update()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.FollowPlayer();
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

