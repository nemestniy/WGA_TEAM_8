using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicShadow : MonoBehaviour
{
    private float _dynamicShadowOffset;
    private GameObject _shadow;
    private Player _player => PlayerManager.Instance.player;
    Transform _transform;

    private void Start()
    {
        _dynamicShadowOffset = MapManager.Instance.GetComponent<ObjectsGenerator>()._dynamicShadowOffset;
        _transform = transform;
       //_player = PlayerManager.Instance.player;
    }

    private void LateUpdate()
    {
        var position = _transform.parent.position;
        _transform.position = position + _dynamicShadowOffset * (position - _player.transform.position).normalized;
    }
}
