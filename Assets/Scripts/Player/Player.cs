using UnityEngine;


public class Player: MonoBehaviour {

    public delegate void PlayerEvents();
    public event PlayerEvents CurrentHexagonChanged;
    public event PlayerEvents WellWasDetected;

    [SerializeField]
    private float _speed;

    private Rigidbody2D _rigidbody;
    private Hexagon _currentHexagon;
    private Hexagon _lastHexagon;
    private Animator _animator;

    private bool _wellIsView;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _wellIsView = false;
    }

    public void SetVelocity(Vector2 direction)
    {
        _rigidbody.velocity = direction * _speed;
    }

    public void SetAngle(float angle)
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Well") && _wellIsView)
        {
            WellWasDetected();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Hexagon"))
        {
            SetCurrentHexagon(collision.gameObject.GetComponent<Hexagon>());
        }
        else
        {
            _currentHexagon = null;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Hexagon"))
        {
            SetLastHexagon(collision.gameObject.GetComponent<Hexagon>());
            Debug.Log("triger exit");
        }
    }

    private void SetLastHexagon(Hexagon hexagon)
    {
        if (_lastHexagon == null || !_lastHexagon.NeighborContains(_currentHexagon.transform))
        {
            if(_lastHexagon != null && _lastHexagon != _currentHexagon) _lastHexagon.ChangeWall();
            _lastHexagon = hexagon;
        }
    }

    private void SetCurrentHexagon(Hexagon hexagon)
    {
        if (_currentHexagon != hexagon)
        {
            _currentHexagon = hexagon;
            CurrentHexagonChanged?.Invoke();
        }
    }

    public void DetectWell()
    {
        _wellIsView = true;
    }

    public void LoseWell()
    {
        _wellIsView = false;
    }

    public GameObject GetCurrentHexagon()
    {
        return _currentHexagon.gameObject;
    }

    public void StartAnimation()
    {
        if(_animator != null)
            _animator.SetBool("PlayerGo", true);
    }

    public void StopAnimation()
    {
        if(_animator != null)
            _animator.SetBool("PlayerGo", false);
    }
}

