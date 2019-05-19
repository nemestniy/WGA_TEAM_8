using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunOutOfEnergyBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private float _deathDelay;
    
    private Animator _gameManagerAnimator;
    private static readonly int Next = Animator.StringToHash("Next");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _gameManagerAnimator = animator;
        animator.GetComponent<GameManager>().CallCoroutine(DieWithDelay(_deathDelay));
    }
    
    private IEnumerator DieWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _gameManagerAnimator.SetTrigger(Next);
        yield return null;
    }
}
