using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, Manager
{
    [SerializeField]
    private float _energyDeathDelay = 4;
    
    private PlayerManager _playerManager;
    private EnemyManager _enemyManager;
    private MapManager _mapManager;
    private AudioManager _audioManager;
    private CutscenesManager _cutscenesManager;
    
    public float DistanceToClosestEnemy => _enemyManager.DistanceToClosestEnemy; //returns -1 if there are no enemy in enemy list
    public BackgroundController.Biome CurrentBiome => _mapManager.Background.GetBiomeByPosition(Player.Instance.transform.position);
    public float LampEnergyLvl => Player.Instance.GetComponent<Energy>().CurrentEnergyLvl;
    public int CurrentLampMode => _playerManager.CurrentLampMode;
    public float DistanceToWell => _mapManager.GetComponent<ObjectsGenerator>().DistanceToWell; //returns -1 if there are no well
    public bool IsPlayerMoving =>
        Player.Instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerWalkAnimation");
//    public bool IsEnemyInCurrZone =>;
//    public Zone CurrentZone =>;
//    public HexType CurrentHexType =>;


    public static GameManager Instance { get; private set; }

    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        Well.OnTrigger += OnWin;
        Enemy.OnTrigger += OnDeathByEnemy;
        EnemyDeepWaterer.OnTrigger += OnDeathByEnemy;
        EnemyStatue.OnTrigger += OnDeathByEnemy;
        Energy.OnRanoutOfEnergy += OnDeathByRanoutOfEnergy;
        ExitTrigger.OnExit += Exit;
    }

    public GameManager() : base()
    {
        Instance = this;
    }

    private void Start()
    {
        ConnectManagers();
        _mapManager.StartManager();
        StartCoroutine(_cutscenesManager.Show("Start", new Action(StartManager)));//show method gets delegate what to do after showing
        IsLoaded = true;
    }

    private void OnWin()
    {
        PauseManager();
        StartCoroutine(_cutscenesManager.Show("Win", new Action(FinishGame)));
    }

    private void OnDeathByEnemy()
    {
        PauseManager();
        StartCoroutine(_cutscenesManager.Show("Screemer", new Action(ShowDeathCutscene)));
    }

    private void OnDeathByRanoutOfEnergy()
    {
        PauseManager();
        StartCoroutine(CallWithDelay(_energyDeathDelay, new Action(ShowDeathCutscene)));
    }

    private void ShowDeathCutscene()
    {
        PauseManager();
        StartCoroutine(_cutscenesManager.Show("Death", new Action(FinishGame)));
    }

    private void FinishGame()
    {
        SceneManager.LoadScene(0);
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
        _mapManager = MapManager.Instance;
        _audioManager = AudioManager.Instance;
        _cutscenesManager = CutscenesManager.Instance;
    }

    private void Exit()
    {
        StartCoroutine(_cutscenesManager.Show("BossFight", new Action(ShowChoiceCutscene)));
//        FinishGame();
    }

    private void ShowChoiceCutscene()
    {
        StartCoroutine(_cutscenesManager.Show("Choice", null));
    }

    public void Run()
    {
        StartCoroutine(_cutscenesManager.Show("Run", new Action(FinishGame)));
    }

    public void Kill()
    {
        StartCoroutine(_cutscenesManager.Show("Kill", new Action(FinishGame)));
    }
    

    private void OnDestroy()
    {
        Well.OnTrigger -= OnWin;
        Enemy.OnTrigger -= OnDeathByEnemy;
        EnemyDeepWaterer.OnTrigger -= OnDeathByEnemy;
        EnemyStatue.OnTrigger -= OnDeathByEnemy;
        Energy.OnRanoutOfEnergy -= OnDeathByRanoutOfEnergy;
        ExitTrigger.OnExit -= Exit;
    }
}

