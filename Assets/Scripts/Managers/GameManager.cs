using Cutscenes;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers:")]
        [SerializeField]
        private PlayerManager _playerManager;
        [SerializeField]
        private EnemyManager _enemyManager;
        [SerializeField]
        private MapManager _mapManager;
    
        [Header("Cutscenes:")]
        [SerializeField]
        private Cutscene _startCutscene;

        private void Start()
        {
            PauseGame();
            _startCutscene.Show();
            ResumeGame();
        }

        private void PauseGame()
        {
            _playerManager.PausePlayer();
//        _enemyManager.PauseEnemies();
//        _mapManager.PauseMap();
        }

        private void ResumeGame()
        {
            _playerManager.ResumePlayer();
//        _enemyManager.ResumeEnemies();
//        _mapManager.ResumeMap();
        }

        private void RestartGame()
        {
            _playerManager.RestartPlayer();
//        _enemyManager.RestartEnemies();
//        _mapManager.RestartMap();
        }
    }
}
