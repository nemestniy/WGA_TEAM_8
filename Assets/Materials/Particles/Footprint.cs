﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Rock != null) Rock.GetComponent<Renderer>().sortingLayerName = "Background";
        if (Sand != null) Sand.GetComponent<Renderer>().sortingLayerName = "Background";
        if (Water != null) Water.GetComponent<Renderer>().sortingLayerName = "Background";
    }

    public ParticleSystem Sand, Water, Rock;

    // Update is called once per frame
    void Update()
    {
        var main = GetComponent<ParticleSystem>().main;
        
    }

    public void Emit()
    {
        if (BackgroundController.Instance == null)
            return;
        
        var terrain = BackgroundController.Instance.GetBiomeByPosition(transform.position);
        ParticleSystem ps;
        switch (terrain)
        {
            case BackgroundController.Biome.Rocky: ps = Rock; break;
            case BackgroundController.Biome.Sandy: ps = Sand; break;
            case BackgroundController.Biome.Water: ps = Water; break;        
            default: ps = null; break;
        }

        if (ps != null)
        {
            var main = ps.main;
            main.startRotation = -(this.transform.rotation.eulerAngles.z + Random.Range(-5, 5)) * Mathf.Deg2Rad ;            
            ps.Emit(terrain == BackgroundController.Biome.Water ? 5 : 1);
        }
    }
}
