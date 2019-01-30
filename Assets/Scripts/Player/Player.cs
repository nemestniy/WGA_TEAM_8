using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour {

    [SerializeField]
    private float _speed;

	private Rigidbody2D _rigidbody;
    private GameObject _currentHexagon;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 direction)
	{
        if (direction.x != 0 || direction.y != 0)
            direction *= Mathf.Sqrt(2) / 2;
        _rigidbody.velocity = direction * _speed;
	}

    public void SetAngle(float angle)
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Hexagon")
        {
            _currentHexagon = collision.gameObject;
        }
        else
        {
            _currentHexagon = null;
        }
    }

    public GameObject GetCurrentHexagon()
    {
        return _currentHexagon;
    }
}
