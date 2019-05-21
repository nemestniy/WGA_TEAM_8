using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlaceholder : MonoBehaviour
{
    public GameObject Compile()
    {

        if (!IsInit)
        {
            Object = ObjectsGenerator.Instance.SpawnObject(this);
            Object.transform.SetParent(this.transform);            
        }        
        IsInit = true;
        return Object;            
    }

    public bool IsInit = false;
    public int Size;
    public Hexagon Owner;
    private GameObject Object;
}
