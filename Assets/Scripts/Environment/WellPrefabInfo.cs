using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellPrefabInfo : ObjectPrefabInfo
{

    public override bool Check(BackgroundController.Biome biome, int size, Hexagon owner) 
    {
        return base.Check(biome, size, owner);
    }
}
