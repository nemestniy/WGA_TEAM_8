using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour {

    public delegate void PlayerEvents();
    public event PlayerEvents CurrentHexagonChanged;

    [SerializeField]
    private float _speed;

	private Rigidbody2D _rigidbody;
    public Hexagon _currentHexagon;
    public Hexagon _lastHexagon;
    private Animator _animator;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public void SetVelocity(Vector2 direction)
	{
        _rigidbody.velocity = direction * _speed;
	}

    public void SetAngle(float angle)
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
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
            CurrentHexagonChanged();
        }
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
