using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemyManager : MonoBehaviour, Manager
{
    public Player _player => Player.Instance;
    public AstarPath path;

    [SerializeField] [ShowOnly] private List<IEnemy> enemies;
    [SerializeField] [ShowOnly] private List<EnemyDeepWaterer> enemyDeepWaterers;
    [SerializeField] [ShowOnly] private List<EnemyStatue> enemyStatues;

    public HexagonsGenerator hexagonsGenerator;

    public List<Transform> visibleEnemiesList;

    private bool _wasFried;
    private int framecount = 0;
    public float timeToEnemyEscape;
    
    #region Singletone
    public static EnemyManager Instance { get; private set; }
    public EnemyManager() : base()
    {
        Instance = this;
    }
    #endregion

    public float DistanceToClosestEnemy
    {
        get
        {
            if (!IsLoaded || enemies.Count == 0)
                return -1; //in case of error
            
            float minDist = Vector2.Distance(enemies[0].GetTransform().position, Player.Instance.transform.position);
            for(int i = 1; i < enemies.Count; i++)
            {
                var curEnemyDist = Vector2.Distance(enemies[i].GetTransform().position, Player.Instance.transform.position);
                if (curEnemyDist < minDist)
                {
                    minDist = curEnemyDist;
                }
            }
            return minDist;
        }
    }
    
    
    
    public bool IsLoaded { get; private set; }

    
    
    private void Start()
    {
        if (hexagonsGenerator == null)
        {
            OnMapCreated();
        }
        else
        {
            hexagonsGenerator.MapIsCreate += OnMapCreated; //если разкомментируешь эту строчку, не забудь разкомментить отписывание в OnDestroy
        }
    }

    [SerializeField] 
    private EnemyDeepWaterer _enemyDeepWaterer;
    
    private void OnMapCreated()
    {
        Debug.Log("onMapCreate");
        if (SceneManager.GetActiveScene().name == "TutorialTestScene")//TODO:КасТЫЫЫЫЫЛЬ
        {
            enemyDeepWaterers = new List<EnemyDeepWaterer>();
            enemyDeepWaterers.Add(_enemyDeepWaterer);
        }
        else
        {
            enemyDeepWaterers = new List<EnemyDeepWaterer>(FindObjectsOfType<EnemyDeepWaterer>());
        }
        
        
        enemyStatues = new List<EnemyStatue>(FindObjectsOfType<EnemyStatue>());
        enemies = new List<IEnemy>();
        enemies.AddRange(FindObjectsOfType<EnemyDeepWaterer>());
        enemies.AddRange(FindObjectsOfType<EnemyStatue>());

        //_player = Player.Instance;
        if (MapManager.Instance != null)
            hexagonsGenerator = MapManager.Instance.GetComponent<HexagonsGenerator>();

        path.Scan();
        foreach (EnemyDeepWaterer enemy in enemyDeepWaterers)
        {
            enemy.GetComponent<Pathfinding.AIDestinationSetter>().target = Player.Instance.transform;
            enemy.SetState(State.Moving);
            
        }

        IsLoaded = true;

    }

    private void InitEnemies()
    {
        if (SceneManager.GetActiveScene().name == "TutorialTestScene")//TODO:КасТЫЫЫЫЫЛЬ
        {
            enemyDeepWaterers = new List<EnemyDeepWaterer>();
            enemyDeepWaterers.Add(_enemyDeepWaterer);
        }
        else
        {
            enemyDeepWaterers = new List<EnemyDeepWaterer>(FindObjectsOfType<EnemyDeepWaterer>());
        }
        enemyStatues = new List<EnemyStatue>(FindObjectsOfType<EnemyStatue>());
        enemies = new List<IEnemy>();
        enemies.AddRange(FindObjectsOfType<EnemyDeepWaterer>());
        enemies.AddRange(FindObjectsOfType<EnemyStatue>());
        foreach (EnemyDeepWaterer enemy in enemyDeepWaterers)
        {
            enemy.GetComponent<Pathfinding.AIDestinationSetter>().target = Player.Instance.transform;
            
            if (SceneManager.GetActiveScene().name != "TutorialTestScene")//TODO:КасТЫЫЫЫЫЛЬ
            {
                enemy.SetState(State.Moving);
            }
        }
    }

    void Update()
    {
        if (enemies.Count == 0)//TODO:КАСТЫЫЫЛЬ
        {
            InitEnemies();
        }
        
        if (!IsLoaded) return;
        foreach (EnemyDeepWaterer deepWaterer in enemyDeepWaterers)
        {
            if (deepWaterer.GetDestinationSetter().target != null && Vector2.Distance(deepWaterer.transform.position, deepWaterer.GetDestinationSetter().target.position) < 0.5f)
            {
                deepWaterer.SetState(State.Moving);
                deepWaterer.escapePointCreated = false;
                deepWaterer.maneurPointCreated = false;
                deepWaterer.aiPath.maxSpeed = deepWaterer.speed;
                _wasFried = false;
            }

            if (deepWaterer.inLight)
            {
                if (_player.transform.GetChild(0).GetComponent<Lamp>()._isFrying)
                {
                    Debug.Log("Frying time - "+ (Time.time - deepWaterer.time));
                    if (Time.time - deepWaterer.time > timeToEnemyEscape)
                    {
                        if (!_wasFried)
                        {
                            AudioManager.Instance.TriggerSoundEvent("Sucsessfully fried");
                            _wasFried = true;
                        }
                        deepWaterer.SetState(State.Escaping);
                    }
                    else
                    {
                        if (deepWaterer.distanceToPlayer > deepWaterer.eventHorizon)
                        {
                            deepWaterer.SetState(State.Maneuring);
                        }
                    }
                }
                else
                {
                    deepWaterer.time = Time.time;
                }
            }
            else
            {
                deepWaterer.time = Time.time;
            }

        }

        foreach (EnemyStatue statue in enemyStatues)
        {
            if (statue.GetState() == State.Waiting && !statue.inLight)
            {
                GameObject statueHex = hexagonsGenerator.GetHexagonByPoint(statue.transform.position);
                GameObject playerHex = hexagonsGenerator.GetHexagonByPoint(_player.transform.position);
//                Debug.Log("playerHex" + playerHex);
                if (statueHex == playerHex && statueHex != null && playerHex != null)
                {
                    statue.GetDestinationSetter().target = _player.transform;
                    statue.SetState(State.Moving);
                } 
            }

            if (statue.inLight)
            {
                statue.SetState(State.Waiting);
            }
        }

        UpdateEnemiesState();
        framecount++;
        if (framecount > 60)
        {
            path.Scan();
            Debug.Log("Scanning...");
            framecount = 0;
        }
    }

    public void StartManager()
    {
        path.Scan();

        if (enemies != null)
        {
            foreach (IEnemy enemy in enemies)
            {
                path.Scan();
                switch (enemy.GetEnemyType())
                {
                    case EnemyType.DeepWaterer:
                        enemy.GetDestinationSetter().target = Player.Instance.transform;
                        enemy.SetState(State.Moving);
                        break;

                    case EnemyType.Statue:
                        enemy.SetState(State.Waiting);
                        break;
                }
            } 
        }
        IsLoaded = true;
    }

    public void PauseManager()
    {
        foreach (IEnemy enemy in enemies)
        {
            enemy.Pause();
        }
    }

    public void ResumeManager()
    {
        foreach (IEnemy enemy in enemies)
        {
            enemy.Resume();
        }
    }

    public void SaveManagerState() // для последующей возможности сохранить игру
    {
        foreach (IEnemy enemy in enemies)
        {
            enemy.SaveState();
        }
    }

    private void UpdateEnemiesState()
    {
        Debug.Log("UpdateEnemiesState");
        
        foreach (Transform enemy in visibleEnemiesList)
        {
            enemy.gameObject.GetComponent<IEnemy>().SetOnLight();
            Debug.Log(enemy.gameObject.name + " is on light");
        }

        foreach (IEnemy enemy in enemies)
        {
            if (!visibleEnemiesList.Contains(enemy.GetTransform()))
            {
                enemy.SetOutOfLight();
            }
        }
        
    }

    private void OnDestroy()
    {
        hexagonsGenerator.MapIsCreate -= OnMapCreated;
    }
}

