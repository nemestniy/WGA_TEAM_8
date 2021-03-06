﻿using UnityEngine;


public class Wall : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private bool _isActive;
    public bool _isBorder;

    public Transform underWall;

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
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
        _collider.enabled = false;
        var hit = Physics2D.Linecast(transform.position, GetDirection() + (Vector2)transform.parent.position);
        _collider.enabled = true;
        if (hit.transform != null && hit.transform != transform)
        {
            underWall = hit.transform;
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

    public Hexagon GetHexagon()
    {
        return transform.parent.GetComponent<Hexagon>();
    }

    public Zone GetZone()
    {
        return GetHexagon().GetZone();
    }

    public Hexagon GetHexagonNeighbor()
    {
        return GetHexagon().GetWallNeighbor(this);
    }

    public Zone GetNeighborZone()
    {
        var neighborHex = GetHexagonNeighbor();
        if (neighborHex != null)
            return neighborHex.GetZone();
        else return null;
    }
}

