using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPrefabInfo : ObjectPrefabInfo
{
    public override bool Check(BackgroundController.Biome biome, int size, Hexagon owner)
    {
        return base.Check(biome, size, owner) && owner.ChildObjects.All(a => a.GetComponent<CharacterPrefabInfo>() == null);
    }
}
