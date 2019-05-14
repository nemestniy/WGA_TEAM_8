using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, Manager
{
    private PlayerManager _playerManager;
    private EnemyManager _enemyManager;
    private MapManager _mapManager;
    private AudioManager _audioManager;
    private CutscenesManager _cutscenesManager;
    private static readonly int Next = Animator.StringToHash("Next");

    public float DistanceToClosestEnemy => _enemyManager.DistanceToClosestEnemy; //returns -1 if there are no enemy in enemy list
    public BackgroundController.Biome CurrentBiome => _mapManager.Background.GetBiomeByPosition(Player.Instance.transform.position);
    public float LampEnergyLvl => Player.Instance.GetComponent<Energy>().CurrentEnergyLvl;
    public int CurrentLampMode => _playerManager.CurrentLampMode;
    public float DistanceToWell => _mapManager.GetComponent<ObjectsGenerator>().DistanceToWell; //returns -1 if there are no well
    public bool IsPlayerMoving =>
        Player.Instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerWalkAnimation");

    #region Singletone
    public static GameManager Instance { get; private set; }
    public GameManager() : base()
    {
        Instance = this;
    }
    #endregion

    public bool IsLoaded { get; private set; }

    private void Start()
    {
        ConnectManagers();
        IsLoaded = true;
    }
    private void ConnectManagers()
    {
        _playerManager = PlayerManager.Instance;
        _enemyManager = EnemyManager.Instance;
        _mapManager = MapManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    public void StartManager()
    {
        _audioManager.StartManager();
        _playerManager.StartManager();
        _enemyManager.StartManager();
    }

    public void PauseManager()
    {
        _audioManager.PauseManager();
        _playerManager.PauseManager();
        _enemyManager.PauseManager();
    }

    public void ResumeManager()
    {
        _audioManager.ResumeManager();
        _playerManager.ResumeManager();
//      _enemyManager.ResumeEnemies();
    }

    public void CallCoroutine(IEnumerator cd)
    {
        StartCoroutine(cd);
    }
}

