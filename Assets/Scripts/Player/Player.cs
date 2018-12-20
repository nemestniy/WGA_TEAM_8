using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour {

    [SerializeField]
    private float _speed;

	private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 direction)
	{
        _rigidbody.velocity = direction * _speed;
	}

    public void SetAngle(float angle)
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
