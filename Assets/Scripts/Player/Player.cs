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

    public void SetAngle(Vector2 direction)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(direction.x, direction.y, transform.position.z)).normalized;
        float angle = (360 * Mathf.Atan2(position.x, position.y)) / (Mathf.PI * 2);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -angle);
    }
}
