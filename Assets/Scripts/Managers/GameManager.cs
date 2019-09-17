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
    public BackgroundController.Biome CurrentBiome => BackgroundController.Instance.GetBiomeByPosition(PlayerManager.Instance.player.transform.position);
    public float LampEnergyLvl
    {
        get
        {
            if(PlayerManager.Instance.player.GetComponent<Energy>().CurrentEnergyLvl != null)
                return PlayerManager.Instance.player.GetComponent<Energy>().CurrentEnergyLvl;

            return 1;//TODO:КАСТЫЫЫЛЬ
        }
    }

    public int CurrentLampMode => _playerManager.CurrentLampMode;
    public float DistanceToWell => _mapManager.GetComponent<ObjectsGenerator>().DistanceToWell; //returns -1 if there are no well
    public bool IsPlayerMoving =>
        PlayerManager.Instance.player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerWalkAnimation");

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

    public MapManager GetMapManager()
    {
        return _mapManager;
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
        _enemyManager.ResumeManager();
    }

    public void CallCoroutine(IEnumerator cd)
    {
        StartCoroutine(cd);
    }
}

