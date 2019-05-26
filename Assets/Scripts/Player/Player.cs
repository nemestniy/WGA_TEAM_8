﻿using UnityEngine;


public class Player: MonoBehaviour {

    public delegate void PlayerEvents();
    public event PlayerEvents CurrentHexagonChanged;

    public Zone CurrentZone;

    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _rotationSpeed; //degrees per fixed update

    [SerializeField]
    private float _angelOffset;
    
    [SerializeField]
    private float _normalAnimationSpeed;

    private Rigidbody2D _rigidbody;
    public Hexagon _currentHexagon;
    private Hexagon _lastHexagon;
    private Animator _animator;
    private static readonly int PlayerGo = Animator.StringToHash("PlayerGo");

    public static Player Instance { get; private set; }
    
    public Player() : base()
    {
        Instance = this;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public void SetVelocity(Vector2 direction)
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
        _rigidbody.velocity = direction * _speed;
    }

    public void SetAngle(float angle)
    {
        int angelDirection = 0;
        if (angle < -_angelOffset)
        {
            angelDirection = 1;
        }
        else if (angle > _angelOffset)
        {
            angelDirection = -1;
        }
        _rigidbody.angularVelocity = angelDirection * _rotationSpeed;
    }

    public void SetLastHexagon(Hexagon hexagon)
    {
        if (_lastHexagon == null || _currentHexagon && !_lastHexagon.NeighborContains(_currentHexagon.transform))
        {
            _lastHexagon = hexagon;
        }
    }

    public void SetCurrentHexagon(Hexagon hexagon)
    {
        if (_currentHexagon != hexagon)
        {
            _currentHexagon = hexagon;
            CurrentHexagonChanged?.Invoke();
            CurrentZone = hexagon.GetZone();
        }
    }

    public Lamp GetLamp()
    {
        return GetComponentInChildren<Lamp>();
    }

    public bool IsVisible(Collider2D collider)
    {
        return GetLamp().IsVisible(collider);
    }


    public GameObject GetCurrentHexagon()
    {
        return _currentHexagon.gameObject;
    }

    public Zone GetCurrentZone()
    {
        return _currentHexagon.GetZone();
    }

    public void StartAnimation()
    {
        if(_animator != null)
            _animator.SetBool(PlayerGo, true);
    }

    public void StopAnimation()
    {
        if(_animator != null)
            _animator.SetBool(PlayerGo, false);
    }

    public void Update()
    {
        _animator.speed = _speed / _normalAnimationSpeed;
        Debug.LogWarning(BackgroundController.Instance.GetBiomeByPosition(gameObject.transform.position));
    }
}

