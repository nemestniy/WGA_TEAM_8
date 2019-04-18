using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour, Manager
{
    [Header("Managers:")]
    [SerializeField]
    private PlayerManager _playerManager;
    [SerializeField]
    private EnemyManager _enemyManager;
    [SerializeField]
    private MapManager _mapManager;
    [SerializeField]
    private AudioManager _audioManager;
    
    [Header("Cutscenes:")]
    [SerializeField]
    private Cutscene _startCutscene;
    [SerializeField]
    private Cutscene _screemerCutscene;
    [SerializeField]
    private Cutscene _deathCutscene;
    [SerializeField]
    private Cutscene _winCutscene;

    [Header("")]
    [SerializeField]
    private float _energyDeathDelay = 4;

    
    public float DistanceToClosestEnemy => _enemyManager.DistanceToClosestEnemy; //returns -1 if there are no enemy in enemy list
    public BackgroundController.Biome CurrentBiome => _mapManager.Background.GetBiomeByPosition(Player.Instance.transform.position);
    public float LampEnergyLvl => Player.Instance.GetComponent<Energy>().CurrentEnergyLvl;
    public int CurrentLampMode => _playerManager.CurrentLampMode;
    public float DistanceToWell => _mapManager.GetComponent<ObjectsGenerator>().DistanceToWell; //returns -1 if there are no well
//    public bool IsEnemyInCurrZone =>;
//    public Zone CurrentZone =>;
//    public HexType CurrentHexType =>;
    
    
    public static GameManager Instance { get; private set; }
    public PlayerManager PlayerManager => _playerManager;
    public EnemyManager EnemyManager => _enemyManager;
    public MapManager MapManager => _mapManager;
    public AudioManager AudioManager => _audioManager;

    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        Well.OnTrigger += OnWin;
        Enemy.OnTrigger += OnDeathByEnemy;
        Energy.OnRanoutOfEnergy += OnDeathByRanoutOfEnergy;
    }

    public GameManager() : base()
    {
        Instance = this;
    }

    private void Start()
    {
        StartManager();
        StartCoroutine(_startCutscene.Show( new Action(StartManager))); //show method gets delegate what to do after showing
    }

    private void OnWin()
    {
        PauseManager();
        StartCoroutine(_winCutscene.Show(new Action(StartManager)));
    }

    private void OnDeathByEnemy()
    {
        PauseManager();
        StartCoroutine(_screemerCutscene.Show(new Action(ShowDeathCutsceen)));
    }

    private void OnDeathByRanoutOfEnergy()
    {
        PauseManager();
        StartCoroutine(CallWithDelay(_energyDeathDelay, new Action(ShowDeathCutsceen)));
    }

    private void ShowDeathCutsceen()
    {
        PauseManager();
        StartCoroutine(_deathCutscene.Show(new Action(StartManager)));
    }


    public void StartManager()
    {
        _mapManager.StartManager();
        _audioManager.StartManager();
        _playerManager.StartManager();
        _enemyManager.StartManager();
        IsLoaded = true;
    }

    public void PauseManager()
    {
        _audioManager.PauseManager();
        _playerManager.PauseManager();
        _enemyManager.PauseManager();
//      _mapManager.PauseManager();
    }

    public void ResumeManager()
    {
        _audioManager.ResumeManager();
        _playerManager.ResumeManager();
//      _enemyManager.ResumeEnemies();
//      _mapManager.ResumeMap();
    }
    
    private IEnumerator CallWithDelay(float delay, Action method)
    {
        yield return new WaitForSeconds(delay);
        method?.Invoke();
        yield return null;
    }
}

