using UnityEngine;


public class PlayerManager : MonoBehaviour, Manager
{

    [SerializeField]
    private Player _player;

    private MoveController _moveController;

    private bool _isPaused;
    private Transform _startTransform;

    private void Awake()
    {
        _moveController = GetComponent<MoveController>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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