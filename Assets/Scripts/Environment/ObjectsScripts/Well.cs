using System;
using UnityEngine;

public class Well : MonoBehaviour
{
    public static event Action OnTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTrigger?.Invoke();
        }
    }
}
