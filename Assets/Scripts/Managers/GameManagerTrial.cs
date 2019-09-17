using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerTrial : MonoBehaviour, Manager
{
    //    [Header("Managers:")]
        [SerializeField]
    private PlayerManager _playerManager;
        [SerializeField]
    private EnemyManager _enemyManager;
        [SerializeField]
    private AudioManager _audioManager;

    public Tutorial tutorial;


    [Header("")]
    [SerializeField]
    private float _energyDeathDelay = 4;

    [SerializeField] private Cutscene _screemerCutsceen;


    public float DistanceToClosestEnemy => _enemyManager.DistanceToClosestEnemy; //returns -1 if there are no enemy in enemy list
    //public BackgroundController.Biome CurrentBiome => _mapManager.Background.GetBiomeByPosition(PlayerManager.Instance.player.transform.position);
    public float LampEnergyLvl => PlayerManager.Instance.player.GetComponent<Energy>().CurrentEnergyLvl;
    public int CurrentLampMode => _playerManager.CurrentLampMode;
    //public float DistanceToWell => _mapManager.GetComponent<ObjectsGenerator>().DistanceToWell; //returns -1 if there are no well
    public bool IsPlayerMoving =>
        PlayerManager.Instance.player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerWalkAnimation");
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
        //StartCoroutine(_winCutscene.ShowFrames(new Action(StartManager)));
    }

    private void OnDeathByEnemy()
    {
        PauseManager();
        StartCoroutine(CutscenesManager.Instance.ShowFrames(_screemerCutsceen));
        StartCoroutine(CallWithDelay(_screemerCutsceen.frames[0].secondsToChange, new Action(LoadMenu)));
        //StartCoroutine(_screemerCutscene.ShowFrames(new Action(ShowDeathCutsceen)));
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDeathByRanoutOfEnergy()
    {
        PauseManager();
        StartCoroutine(CallWithDelay(_energyDeathDelay, new Action(ShowDeathCutsceen)));
    }

    private void ShowDeathCutsceen()
    {
        PauseManager();
        //StartCoroutine(_deathCutscene.ShowFrames(new Action(StartManager)));
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
        if (_audioManager != null)
            _audioManager.PauseManager();
        _playerManager.PauseManager();
        _enemyManager.PauseManager();

        tutorial.daughter.GetComponent<Pathfinding.AIPath>().canMove = false;
        tutorial.daughter.GetComponent<Animator>().Play("Entry");
    }

    public void ResumeManager()
    {
        _audioManager.ResumeManager();
        _playerManager.ResumeManager();
        _enemyManager.ResumeManager();

        tutorial.daughter.GetComponent<Pathfinding.AIPath>().canMove = true;
        tutorial.daughter.GetComponent<Animator>().Play("Walk Animation");

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

