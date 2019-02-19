using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private void Start()
    {
        _audioManager.StartManager();
        PauseManager();
        StartCoroutine(_startCutscene.Show(new Action(ResumeManager))); //show method gets delegate what to do after showing
    }


    public void StartManager()
    {
//      _playerManager.StartPlayer();
//      _enemyManager.StartEnemies();
//      _mapManager.StartMap();
    }

    public void PauseManager()
    {
        _audioManager.PauseManager();
        _playerManager.PauseManager();
//      _enemyManager.PauseEnemies();
//      _mapManager.PauseMap();
    }

    public void ResumeManager()
    {
        _audioManager.ResumeManager();
        _playerManager.ResumeManager();
//      _enemyManager.ResumeEnemies();
//      _mapManager.ResumeMap();
    }
}

