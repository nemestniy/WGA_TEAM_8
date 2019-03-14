using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLoadLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelManager.LoadLevel("ReleaseScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
