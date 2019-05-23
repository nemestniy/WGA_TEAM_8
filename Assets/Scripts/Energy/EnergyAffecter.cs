using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyAffecter : MonoBehaviour
{
    [SerializeField]
    private float _changeValue = 50;
    [SerializeField] 
    private float _timeToAffect = 1;
    [SerializeField] 
    private float _reloadingTime;

    private bool _isReloading = false;
    private float _timePast;

    private void Update()
    {
        if (_isReloading && _reloadingTime >= 0)
        {
            if (_timePast < _reloadingTime)
            {
                _timePast += Time.deltaTime;
            }
            else
            {
                _isReloading = false;
                _timePast = 0;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !_isReloading)
        {
            other.GetComponent<Energy>().ChangeEnergyLvl(_changeValue, _timeToAffect);
            _isReloading = true;
            var particlesController = GetComponentInChildren<KrevedkoController>();
            if (particlesController != null)
            {
                particlesController.Collect(other.gameObject);
            }
        }
    }
}
