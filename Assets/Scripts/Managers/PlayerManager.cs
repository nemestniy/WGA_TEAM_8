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
    private List<MeshRenderer> _lampsMeshRenderers;
    public int CurrentLampMode { get; private set; }

    [SerializeField] private Material _normalViewMat;
    [SerializeField] private Material _detectiveViewMat;
    
    public static PlayerManager Instance { get; private set; }
    public bool IsLoaded { get; private set; }

    public PlayerManager() : base()
    {
        Instance = this;
    }
    
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
        CurrentLampMode = _keyController.GetLightMode(_playerEnergy.IsPreDeath); //should NOT be called twice by frame
        //setting light mode for main and back lamp
        _fieldOfViews[0].SetLightMode(CurrentLampMode);
        _fieldOfViews[1].SetLightMode(CurrentLampMode);

        if (_fieldOfViews[0]._changingState == 1) //if _changingState == 1 in mane fov than in back too
        {
            switch (CurrentLampMode)
            {
                case 0: //normal mode
                    _lampsMeshRenderers[0].material = _normalViewMat;
                    _lampsMeshRenderers[1].material = _normalViewMat;
                    break;
                case 1: //combat mode
                    //TODO move updating enemies state from FieldOfView to EnemyManager with switching in this string
                    break;
                case 2: //detective mode
                    _lampsMeshRenderers[0].material = _detectiveViewMat;
                    _lampsMeshRenderers[1].material = _detectiveViewMat;
                    break;
            }
        }
    }

    public void StartManager()
    {
        _keyController = GetComponent<KeyController>();
        GameObject playerGO = GameObject.FindWithTag("Player");
        _player = playerGO.GetComponent<Player>();
        _playerEnergy = playerGO.GetComponent<Energy>();
        IsLoaded = true;
        _fieldOfViews = new List<FieldOfView>(playerGO.GetComponentsInChildren<FieldOfView>());
        _lampsMeshRenderers = new List<MeshRenderer>(playerGO.GetComponentsInChildren<MeshRenderer>());
        
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