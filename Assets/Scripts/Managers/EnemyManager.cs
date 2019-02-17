using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
  public class EnemyManager : MonoBehaviour
  {
    
    public List<Player.Player> _player;
    public List<Enemy.Enemy> _enemies;
    void Start()
    {
        _player = new List<Player.Player>(FindObjectsOfType<Player.Player>());
        _enemies = new List<Enemy.Enemy>(FindObjectsOfType<Enemy.Enemy>());
    }

        // Update is called once per frame
        void Update()
        {
            foreach (Enemy.Enemy enemy in _enemies)
            {
                enemy.FollowPlayer();
            }
        }
    }
}
