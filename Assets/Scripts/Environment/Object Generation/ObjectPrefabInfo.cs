using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPrefabInfo : MonoBehaviour
{    
    public int MaxCountInDungeon = 1000;    
    public int MinCountInDungeon = 0;   
    public float SpawnProbMultiplier = 1;
    public byte ZoneMask = 255;

    public virtual bool Check(BackgroundController.Biome biome, int size, Hexagon owner)
    {
        var name = gameObject.name;                
        return ((name.Substring(0, 5).Equals(ObjectsGenerator.biomePrefix[(int) biome]) || name.Substring(0, 5).Equals(ObjectsGenerator.anyBPrefix)) &&
                name.Substring(5, 2).Equals(ObjectsGenerator.sizePrefix[size - 1])) && ObjectsGenerator.Instance.GetCountInDungeon(name) < MaxCountInDungeon;
    }

}
