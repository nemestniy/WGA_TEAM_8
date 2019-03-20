using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour, Manager
{
    private PlayerManager _playerManager;
    private EnemyManager _enemyManager;
    private MapManager _mapManager;
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

    public static GameManager Instance { get; private set; }
    public PlayerManager PlayerManager => _playerManager;
    public EnemyManager EnemyManager => _enemyManager;
    public MapManager MapManager => _mapManager;
    public AudioManager AudioManager => _audioManager;

    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        _playerManager = PlayerManager.Instance;
        _enemyManager = EnemyManager.Instance;
        _mapManager = MapManager.Instance;
        _audioManager = AudioManager.Instance;
        
        Well.OnTrigger += OnWin;
        Enemy.OnTrigger += OnDie;
    }

    public GameManager() : base()
    {
        Instance = this;
    }

    private void Start()
    {   
        StartCoroutine(_startCutscene.Show( new Action(StartManager))); //show method gets delegate what to do after showing
    }

    private void OnWin()
    {
        PauseManager();
        StartCoroutine(_winCutscene.Show(new Action(StartManager)));
    }

    private void OnDie()
    {
        PauseManager();
        StartCoroutine(_screemerCutscene.Show(new Action(ShowDeadCutsceen)));
    }

    private void ShowDeadCutsceen()
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
}

