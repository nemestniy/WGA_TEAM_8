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
        _keyManager = GetComponent<MoveController>();
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
//        switch (_keyManager.GetWheelMovment()) //switching by mouse movement
//        {
//            case KeyManager.WheelMovment.Up:
//                _playerLampStates.SetTrigger(ChangeUp);
//                break;
//            case KeyManager.WheelMovment.Down:
//                _playerLampStates.SetTrigger(ChangeDown);
//                break;
//        }

        if (!_player.gameObject.activeInHierarchy)
            _player = FindObjectOfType<Player>();
        
        switch (_keyManager.GetButtonState()) //switching by mouse buttons hold
        {
            case KeyManager.MoseButtonStates.Released:
                _playerLampStates.SetInteger(ButtonState, 0); //0 means mouse buttons are released
                break;
            
            case KeyManager.MoseButtonStates.LeftDown:
                _playerLampStates.SetInteger(ButtonState, 1); //1 means mouse left button is down
                break;
            
            case KeyManager.MoseButtonStates.RightDown:
                _playerLampStates.SetInteger(ButtonState, 2); //2 means mouse rigth button is down
                break;
        }
    }

    public void StartManager()
    {
        _keyManager = KeyManager.Instance;
        //_player = Player.Instance;
        IsLoaded = true;
        _playerLampStates = _player.transform.GetChild(0).GetComponent<Animator>();
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