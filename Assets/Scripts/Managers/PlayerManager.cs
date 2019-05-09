﻿using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour, Manager
{
    private Player _player;
    private Animator _playerLampStates;
    private KeyController _keyController;
    private bool _isPaused = true;
    
    private Transform _startTransform;
    public int CurrentLampMode { get; private set; }
    public bool playerCanMove = false;

    public static PlayerManager Instance { get; private set; }
    public bool IsLoaded { get; private set; }

    public PlayerManager() : base()
    {
        Instance = this;
    }

    private void Awake()
    {
        _isPaused = true;
    }

    void Update()
    {
        if (_isPaused || _player == null)
            return;
        if (playerCanMove)
        {
            UpdatePlayerMovement();
            UpdateLightMode();
        }
    }

    private void UpdatePlayerMovement()
    {
        Vector2 velocity = _keyController.GetVelocity();
    
        if (velocity == Vector2.zero)
            _player.StopAnimation();
        else
            _player.StartAnimation();

        _player.SetVelocity(velocity);
        _player.SetAngle(_keyController.GetAngle());
    }
    
    
    private static readonly int ChangeUp = Animator.StringToHash("ChangeUp");
    private static readonly int ChangeDown = Animator.StringToHash("ChangeDown");

    private void UpdateLightMode()
    {
        switch (_keyController.GetWheelMovment())
        {
            case KeyController.WheelMovment.Up:
                _playerLampStates.SetTrigger(ChangeUp);
                break;
            case KeyController.WheelMovment.Down:
                _playerLampStates.SetTrigger(ChangeDown);
                break;
        }
    }

    public void StartManager()
    {
        _keyController = GetComponent<KeyController>();
        _player = Player.Instance;
        IsLoaded = true;
        _playerLampStates = _player.transform.GetChild(0).GetComponent<Animator>();
        
        _isPaused = false;
    }
    
    public void PauseManager()
    {
        _isPaused = true;
    }

    public void ResumeManager()
    {
        _isPaused = false;
    }
}