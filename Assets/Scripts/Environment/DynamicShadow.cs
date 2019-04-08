using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicShadow : MonoBehaviour
{
    private float _dynamicShadowOffset;
    private GameObject _shadow;
    Transform _transform;

    private void Start()
    {
        _dynamicShadowOffset = MapManager.Instance.GetComponent<ObjectsGenerator>()._dynamicShadowOffset;
        _transform = transform;
    }

    private void LateUpdate()
    {
        _transform.localPosition = _dynamicShadowOffset * (_transform.parent.position - Player.Instance.transform.position).normalized;
    }
}
