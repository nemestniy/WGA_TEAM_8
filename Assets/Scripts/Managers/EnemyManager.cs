using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class EnemyManager : MonoBehaviour
{
<<<<<<< HEAD
  public class EnemyManager : MonoBehaviour
  {
    
    public List<Player.Player> _player;
    public List<Enemy.Enemy> _enemies;
=======
  
    public List<Player> _player;
    public List<Enemy> _enemies;
>>>>>>> f524c74c6a05ca880974305279e52bf1b0dbbfe1
    void Start()
    {
        _player = new List<Player.Player>(FindObjectsOfType<Player.Player>());
        _enemies = new List<Enemy.Enemy>(FindObjectsOfType<Enemy.Enemy>());
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

