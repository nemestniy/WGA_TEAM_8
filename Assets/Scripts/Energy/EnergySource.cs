
using UnityEngine;

public class EnergySource : MonoBehaviour
{
    
    [SerializeField]
    private Sprite _spriteOff;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject.GetComponent<Energy>().CurrentEnergyLvl < 1)
        {
            GetComponent<SpriteRenderer>().sprite = _spriteOff;
            transform.GetChild(0).gameObject.SetActive(false);
            
            AudioManager.Instance.TriggerSoundEvent("Energy consumed");
        }
    }
    
}
