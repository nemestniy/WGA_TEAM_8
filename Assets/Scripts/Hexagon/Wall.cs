using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private BoxCollider2D _collider;
    private bool _isActive;
    private bool _isBorder;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _isActive = true;
        _isBorder = false;
    }

    // Getting current direction forward of wall 
    public Vector2 GetDirection()
    {
        return transform.TransformDirection(Vector2.right);
    }

    public Transform GetNeighbor(float distance)
    {
        //Off colider, that ray won`t return himself
        _collider.enabled = false;

        var hit = Physics2D.Linecast(transform.position, GetDirection()*distance + (Vector2)transform.parent.position);
        _collider.enabled = true;
        if(hit.transform != null)
        {
            return hit.transform;
        }
        return null;
    }

    public Transform GetWallUnderMe()
    {
        //Make little Ray and check, that ray won`t return himself, after that it return wall under him
        var hit = Physics2D.Linecast(transform.position, GetDirection() + (Vector2)transform.parent.position);
        if (hit.transform != null && hit.transform != transform)
            return hit.transform;
        return null;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void Disable()
    {
        _isActive = false;
        gameObject.SetActive(IsActive());
    }

    public void Enable()
    {
        _isActive = true;
        gameObject.SetActive(IsActive());
    }

    public bool IsActive()
    {
        return _isActive;
    }

    public void SetBorder()
    {
        _isBorder = true;
    }

    public bool IsBorder()
    {
        return _isBorder;
    }
}
