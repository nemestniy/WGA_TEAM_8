using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterDamageBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private float _timeOfDarknes = 1;
    
    private float _currentTime;

    private static readonly int AfterDamageEnded = Animator.StringToHash("AfterDamageEnded");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _currentTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_currentTime < _timeOfDarknes)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            animator.SetTrigger(AfterDamageEnded);
        }
    }
}
