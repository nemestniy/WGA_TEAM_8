using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Player _player;
    public Enemy[] _enemies;
    void Start()
    {
        _player = FindObjectOfType<Player>();
        _enemies = FindObjectsOfType<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.FollowPlayer();
        }
    }
}
