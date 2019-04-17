using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyAffecter : MonoBehaviour
{
    [SerializeField]
    private float _changeValue = 50;

    [SerializeField] 
    private float _blockEnergyChanges = 1;
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<Energy>().ChangeEnergyLvl(_changeValue,_blockEnergyChanges);
        }
    }
}
