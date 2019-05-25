using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
public class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }
}

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}

#endif

public class ObjectPrefabInfo : MonoBehaviour
{    
    public int MaxCountInDungeon = 1000;    
    public int MinCountInDungeon = 0;   
    public float SpawnProbMultiplier = 1;
#if UNITY_EDITOR
    [EnumFlags] public Zone.ZoneType ZoneMask = Zone.ZoneType.All;
#else
    public Zone.ZoneType ZoneMask = Zone.ZoneType.All;
#endif

    public virtual bool Check(BackgroundController.Biome biome, int size, Hexagon owner)
    {
        var name = gameObject.name;
        return ((name.Substring(0, 5).Equals(ObjectsGenerator.biomePrefix[(int) biome]) ||
                 name.Substring(0, 5).Equals(ObjectsGenerator.anyBPrefix)) &&
                name.Substring(5, 2).Equals(ObjectsGenerator.sizePrefix[size - 1])) &&
               ObjectsGenerator.Instance.GetCountInDungeon(name) < MaxCountInDungeon &&
               (owner.GetZone().Type & ZoneMask) != 0;

    }

}
