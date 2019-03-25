using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyAffecter : MonoBehaviour
{
    [SerializeField]
    private float _changeVlue = 50;

    [SerializeField] 
    private float _blockEnergyChsnges = 1;
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<Energy>().ChangeEnergyLvl(_changeVlue,_blockEnergyChsnges);
        }
    }
}
