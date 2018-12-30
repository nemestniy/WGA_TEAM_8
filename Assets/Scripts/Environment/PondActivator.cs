using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PondActivator : MonoBehaviour
{
    
    [SerializeField] 
    private List<GameObject> _ponds;

    private bool isActivated = false;

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Бумс");
        if (!isActivated)
        {
            _ponds[0].SetActive(false);
            _ponds[1].SetActive(true);
        }
    }
}
