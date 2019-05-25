﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{

    private Energy _playerEnergy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _playerEnergy = PlayerManager.Instance?.Player?.GetComponent<Energy>();
        if ((Player.Instance.GetCurrentZone().Type & Zone.ZoneType.Madness) != 0)
        {
            GetComponent<Renderer>().material
                .SetFloat("_CreepyMult", Mathf.Pow(1.5f - _playerEnergy?.CurrentEnergyLvl ?? 1, 1));
        }
        else
        {
            GetComponent<Renderer>().material
                .SetFloat("_CreepyMult", Mathf.Pow(1 - _playerEnergy?.CurrentEnergyLvl ?? 1, 1));
        }
    }
}
