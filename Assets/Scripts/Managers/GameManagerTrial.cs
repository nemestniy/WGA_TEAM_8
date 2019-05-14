using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class GameManagerTrial : MonoBehaviour, Manager
{
    private PlayerManager _playerManager;
    private EnemyManager _enemyManager;
    private AudioManager _audioManager;


    [Header("")]
    [SerializeField]
    private float _energyDeathDelay = 4;


    public float DistanceToClosestEnemy => _enemyManager.DistanceToClosestEnemy; //returns -1 if there are no enemy in enemy list
    //public BackgroundController.Biome CurrentBiome => _mapManager.Background.GetBiomeByPosition(Player.Instance.transform.position);
    public float LampEnergyLvl => Player.Instance.GetComponent<Energy>().CurrentEnergyLvl;
    public int CurrentLampMode => _playerManager.CurrentLampMode;
    //public float DistanceToWell => _mapManager.GetComponent<ObjectsGenerator>().DistanceToWell; //returns -1 if there are no well
    public bool IsPlayerMoving =>
        Player.Instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerWalkAnimation");
    //    public bool IsEnemyInCurrZone =>;
    //    public Zone CurrentZone =>;
    //    public HexType CurrentHexType =>;

    #region Singletone
    public static GameManagerTrial Instance { get; private set; }
    public GameManagerTrial() : base()
    {
        Instance = this;
    }
    #endregion
    
    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        Well.OnTrigger += OnWin;
        Enemy.OnTrigger += OnDeathByEnemy;
        EnemyDeepWaterer.OnTrigger += OnDeathByEnemy;
        EnemyStatue.OnTrigger += OnDeathByEnemy;
        Energy.OnRanoutOfEnergy += OnDeathByRanoutOfEnergy;
    }

    

    private void Start()
    {
        ConnectManagers();

        StartManager();

        IsLoaded = true;
    }

    private void OnWin()
    {
        PauseManager();
        //StartCoroutine(_winCutscene.Show(new Action(StartManager)));
    }

    private void OnDeathByEnemy()
    {
        PauseManager();
        //StartCoroutine(_screemerCutscene.Show(new Action(ShowDeathCutsceen)));
    }

    private void OnDeathByRanoutOfEnergy()
    {
        PauseManager();
        StartCoroutine(CallWithDelay(_energyDeathDelay, new Action(ShowDeathCutsceen)));
    }

    private void ShowDeathCutsceen()
    {
        PauseManager();
        //StartCoroutine(_deathCutscene.Show(new Action(StartManager)));
    }


    public void StartManager()
    {
        if (_audioManager != null)
            _audioManager.StartManager();
        if (_playerManager != null)
            _playerManager.StartManager();
        if (_enemyManager != null)
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

    private IEnumerator CallWithDelay(float delay, Action method)
    {
        yield return new WaitForSeconds(delay);
        method?.Invoke();
        yield return null;
    }

    private void ConnectManagers()
    {
        _playerManager = PlayerManager.Instance;
        _enemyManager = EnemyManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    private void OnDestroy()
    {
        Well.OnTrigger -= OnWin;
        Enemy.OnTrigger -= OnDeathByEnemy;
        EnemyDeepWaterer.OnTrigger -= OnDeathByEnemy;
        EnemyStatue.OnTrigger -= OnDeathByEnemy;
        Energy.OnRanoutOfEnergy -= OnDeathByRanoutOfEnergy;
    }
}

