using UnityEngine;


public class PlayerManager : MonoBehaviour, Manager
{
    [SerializeField]
    private GameObject _playerPref;
    private Player _player;
    private MoveController _moveController;
    private bool _isPaused;
    private Transform _startTransform;
    private FieldOfView _fieldOfView;

    private void Awake()
    {
        _moveController = GetComponent<MoveController>();
    }

    void Update()
    {
        if (!_isPaused && _fieldOfView != null)
        {
            UpdatePlayerMovement();
            
            if (Input.GetButtonDown("Fire1"))
            {
                _fieldOfView.ChangeLightMode(1);
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                _fieldOfView.ChangeLightMode(2);
            }
            else if (Input.GetButtonDown("Fire3"))
            {
                _fieldOfView.ChangeLightMode(0);
            }
        }
    }

    private void UpdatePlayerMovement()
    {
        if (_player != null)
        {
            Vector2 velocity = _moveController.GetVelocity();
    
            if (velocity == Vector2.zero)
                _player.StopAnimation();
            else
                _player.StartAnimation();

            _player.SetVelocity(velocity);

            _player.SetAngle(_moveController.GetAngle());
        }
    }

    public void StartManager()
    {
        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO)
        {
            Destroy(playerGO);
        }
        playerGO = Instantiate(_playerPref) as GameObject; 
        _player = playerGO.GetComponent<Player>();
        _fieldOfView = playerGO.GetComponent<FieldOfView>();
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