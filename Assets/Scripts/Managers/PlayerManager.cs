using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour, Manager
{
    private Player _player;
    private KeyController _keyController;
    private bool _isPaused = true;
    private Transform _startTransform;
    private List<FieldOfView> _fieldOfViews;
    private Energy _playerEnergy;

    public bool IsLoaded { get; private set; }
    

    void Update()
    {
        if (!_isPaused && _player != null)
        {
            UpdatePlayerMovement();
        }

        if (_fieldOfViews != null)
        {
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

    private void UpdateLightMode()
    {
        int lm = _keyController.GetLightMode(_playerEnergy.IsPreDeath); //should NOT be called twice by frame
        
        //setting light mode for main and back lamp
        _fieldOfViews[0].SetLightMode(lm);
        _fieldOfViews[1].SetLightMode(lm);
    }

    public void StartManager()
    {
        _keyController = GetComponent<KeyController>();
        GameObject playerGO = GameObject.FindWithTag("Player");
        _player = playerGO.GetComponent<Player>();
        _playerEnergy = playerGO.GetComponent<Energy>();
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