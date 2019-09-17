using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour, Manager
{
    public Player player
    {
        get
        {
            if (_player == null)
                _player = FindObjectOfType<Player>();
            return _player;
        }
        private set{ _player = value; }
    }

    private Animator _playerLampStates;
    private InputController _keyManager;
    private bool _isPaused = true;
    
    private Transform _startTransform;
    public int CurrentLampMode { get; private set; }

    private Player _player;
    
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
        _keyManager = InputController.Instance;
        _isPaused = true;
    }

    private void Start()//TODO:КАСТЫЫЫЫЛЬ
    {
        StartManager();
    }

    void Update()
    {
        if (_isPaused || player == null)
            return;
        
        UpdateLightMode();
    }

    private void FixedUpdate()
    {
        if (_isPaused || player == null)
            return;
         
        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
//        if (!player.gameObject.activeInHierarchy)
//            player = FindObjectOfType<Player>();
        
        
        Vector2 movingDirection = _keyManager.GetMovingDirection();
    
        if (movingDirection == Vector2.zero)
            player.StopAnimation();
        else
            player.StartAnimation();

        player.SetVelocity(movingDirection);
        player.SetAngularVelocity(_keyManager.GetAimingDirection());
    }
    
    
    private static readonly int ChangeUp = Animator.StringToHash("ChangeUp");
    private static readonly int ChangeDown = Animator.StringToHash("ChangeDown");
    private static readonly int ButtonState = Animator.StringToHash("ButtonState");

    private void UpdateLightMode()
    {
//        if (!player.gameObject.activeInHierarchy)
//            player = FindObjectOfType<Player>();
        
        switch (_keyManager.GetButtonState()) //switching by mouse buttons hold
        {
            case InputController.MoseButtonStates.Released:
                _playerLampStates.SetInteger(ButtonState, 0); //0 means mouse buttons are released
                break;
            
            case InputController.MoseButtonStates.LeftDown:
                _playerLampStates.SetInteger(ButtonState, 1); //1 means mouse left button is down
                break;
            
            case InputController.MoseButtonStates.RightDown:
                _playerLampStates.SetInteger(ButtonState, 2); //2 means mouse rigth button is down
                break;
        }
    }

    public void StartManager()
    {
        _playerLampStates = player.transform.GetChild(0).GetComponent<Animator>();
        IsLoaded = true;
        _isPaused = false;
    }
    
    public void PauseManager()
    {
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        _isPaused = true;
        player.StopAnimation();
    }

    public void ResumeManager()
    {
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        _isPaused = false;
        player.StartAnimation();
    }
}