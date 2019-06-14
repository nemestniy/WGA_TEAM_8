using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour, Manager
{
    private Player _player
    {
        get { return Player.Instance; }
        set { _player = value; }
    }

    private Animator _playerLampStates;
    private MoveController _keyManager;
    private bool _isPaused = true;
    
    private Transform _startTransform;
    public int CurrentLampMode { get; private set; }

    public Player Player => _player;
    
    public bool IsLoaded { get; private set; }
    #region Singletone
    public static PlayerManager Instance { get; private set; }
    public PlayerManager() : base()
    {
        Instance = this;
    }
    #endregion

    private void Awake()
    {
        _keyManager = MoveController.Instance;
        
        _isPaused = true;
    }

    void Update()
    {
        if (_isPaused || _player == null)
            return;
        
        UpdateLightMode();
    }

    private void FixedUpdate()
    {
        if (_isPaused || _player == null)
            return;
         
        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
        if (!_player.gameObject.activeInHierarchy)
            _player = FindObjectOfType<Player>();
        
        
        Vector2 velocity = _keyManager.GetVelocity();
    
        if (velocity == Vector2.zero)
            _player.StopAnimation();
        else
            _player.StartAnimation();

        _player.SetVelocity(velocity);
        _player.SetAngle(_keyManager.GetAngle());
    }
    
    
    private static readonly int ChangeUp = Animator.StringToHash("ChangeUp");
    private static readonly int ChangeDown = Animator.StringToHash("ChangeDown");
    private static readonly int ButtonState = Animator.StringToHash("ButtonState");

    private void UpdateLightMode()
    {
        if (!_player.gameObject.activeInHierarchy)
            _player = FindObjectOfType<Player>();
        
        switch (_keyManager.GetButtonState()) //switching by mouse buttons hold
        {
            case MoveController.MoseButtonStates.Released:
                _playerLampStates.SetInteger(ButtonState, 0); //0 means mouse buttons are released
                break;
            
            case MoveController.MoseButtonStates.LeftDown:
                _playerLampStates.SetInteger(ButtonState, 1); //1 means mouse left button is down
                break;
            
            case MoveController.MoseButtonStates.RightDown:
                _playerLampStates.SetInteger(ButtonState, 2); //2 means mouse rigth button is down
                break;
        }
    }

    public void StartManager()
    {
        _playerLampStates = _player.transform.GetChild(0).GetComponent<Animator>();
        IsLoaded = true;
        _isPaused = false;
    }
    
    public void PauseManager()
    {
        _player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        _isPaused = true;
        _player.StopAnimation();
    }

    public void ResumeManager()
    {
        _player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        _isPaused = false;
        _player.StartAnimation();
    }
}