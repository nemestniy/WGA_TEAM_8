using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] 
    private Material _visibleMaterial;
    private Material _defaultMaterial;

    private bool _isVisible;
    private SpriteRenderer _spriteRenderer;
    

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultMaterial = _spriteRenderer.material;
    }

    public void BecomeVisible() //called by FieldOfView script for each obstacle which are seen by player
    {
        _isVisible = true;
    }

    private void LateUpdate() //make the obstacle visible if it is seen
    {
        if (_isVisible)
        {
            _spriteRenderer.material = _visibleMaterial;
            _isVisible = false;
        }
        else
        {
            _spriteRenderer.material = _defaultMaterial;
        }
    }
}
