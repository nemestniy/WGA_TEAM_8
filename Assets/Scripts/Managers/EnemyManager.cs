using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class EnemyManager : MonoBehaviour
    {
        public Player.Player _player;
        public List<Enemy.Enemy> _enemies;
        void Start()
        {
            _player = FindObjectOfType<Player.Player>();
            _enemies = FindObjectsOfType<Enemy.Enemy>().ToList();
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
