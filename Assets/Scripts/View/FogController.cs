using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{

    private Energy _playerEnergy;

    // Update is called once per frame
    void Update()
    {
        _playerEnergy = PlayerManager.Instance?.player?.GetComponent<Energy>();
        GetComponent<Renderer>().material
            .SetFloat("_CreepyMult", Mathf.Pow(1 - _playerEnergy?.CurrentEnergyLvl ?? 1, 1));
        //if ((player.Instance.GetCurrentZone().Type & Zone.ZoneType.Madness) != 0)
        //{
        //    GetComponent<Renderer>().material
        //        .SetFloat("_CreepyMult", Mathf.Pow(1.5f - _playerEnergy?.CurrentEnergyLvl ?? 1, 1));
        //}
        //else
        //{
        //    GetComponent<Renderer>().material
        //        .SetFloat("_CreepyMult", Mathf.Pow(1 - _playerEnergy?.CurrentEnergyLvl ?? 1, 1));
        //}
    }
}
