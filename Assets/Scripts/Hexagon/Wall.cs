using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private BoxCollider2D _collider;
    private bool _isActive;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _isActive = true;
    }

    public Vector2 GetDirection()
    {
        return transform.TransformDirection(Vector2.right);
    }

    public Transform GetNeighbor(float distance)
    {
        Debug.DrawLine(transform.GetChild(0).position, GetDirection() * distance + (Vector2)transform.parent.position);
        var hit = Physics2D.Linecast(transform.GetChild(0).position, GetDirection()*distance + (Vector2)transform.parent.position);
        if(hit != null)
        {
            return hit.transform;
        }
        return null;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void Disable()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        _collider.isTrigger = true;
        _isActive = false;
    }

    public void Enable()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        _collider.isTrigger = false;
        _isActive = true;
    }

    public bool IsActive()
    {
        return _isActive;
    }
}
