using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class TentacleTrap : MonoBehaviour
{
    private static readonly int attackHash = Animator.StringToHash("TentacleAttack");
    private Collider2D _collider;
    private Animator _animator;
    private bool isAlive = true;
    [SerializeField] private SpriteRenderer _mandala;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<UnityEngine.Animator>();
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private static readonly int GotDamage = Animator.StringToHash("GotDamage");

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAlive && other.gameObject.layer == LayerMask.NameToLayer("Player") )
        {
            _animator.SetBool(attackHash, true);
            isAlive = false;
            other.transform.GetChild(0).GetComponent<Animator>().SetTrigger(GotDamage);
            _mandala.color = new Color(0, 0, 0, 0);
            AudioManager.Instance.TriggerSoundEvent("Tentacle attack");
        }
    }
}
