using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var main = GetComponent<ParticleSystem>().main;
        main.startRotation = -this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }  
}
