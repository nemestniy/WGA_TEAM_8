using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour, Manager
{
    [SerializeField]
    private GameObject _playerPref;
    private Player _player;
    private KeyController _keyController;
    private bool _isPaused = true;
    private Transform _startTransform;
    private List<FieldOfView> _fieldOfViews;

    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        _keyController = GetComponent<KeyController>();
    }

    void Update()
    {
        if (!_isPaused)
            UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
        if (_player != null)
        {
            Vector2 velocity = _keyController.GetVelocity();
    
            if (velocity == Vector2.zero)
                _player.StopAnimation();
            else
                _player.StartAnimation();

            _player.SetVelocity(velocity);
            _player.SetAngle(_keyController.GetAngle());

            
            if (_fieldOfViews != null) //setting light mode for all players light sources
            {
                int lm = _keyController.GetLightMode(); //should NOT be called twice by frame
                foreach (var fov in _fieldOfViews)
                {
                    fov.SetLightMode(lm);
                }
            }
        }
    }

    public void StartManager()
    {
        GameObject playerGO = GameObject.FindWithTag("Player");
        _player = playerGO.GetComponent<Player>();
        IsLoaded = true;
        _fieldOfViews = new List<FieldOfView>(playerGO.GetComponentsInChildren<FieldOfView>());
        
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