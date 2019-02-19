using UnityEngine;


public class PlayerManager : MonoBehaviour, Manager
{
    private Player _player;
    private MoveController _moveController;
    private bool _isPaused;
    private Transform _startTransform;
    private FieldOfView _fieldOfView;

    private void Awake()
    {
        _moveController = GetComponent<MoveController>();
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        _player = playerGO.GetComponent<Player>();
        _fieldOfView = playerGO.GetComponent<FieldOfView>();
    }

    private void Start()
    {
        _startTransform = _player.transform;
    }

    void Update()
    {
        if (!_isPaused)
        {
            UpdatePlayerMovement();
            if (Input.GetButtonDown("Fire1"))
            {
                _fieldOfView.ChangeLightMode();
            }
        }
    }

    private void UpdatePlayerMovement()
    {
        Vector2 velocity = _moveController.GetVelocity();
    
        if (velocity == Vector2.zero)
            _player.StopAnimation();
        else
            _player.StartAnimation();

        _player.SetVelocity(velocity);

        _player.SetAngle(_moveController.GetAngle());
    }

    public void StartManager()
    {
        _player.transform.position = _startTransform.position;
        _player.transform.eulerAngles = _startTransform.eulerAngles;
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