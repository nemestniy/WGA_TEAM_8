using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatToBaseBehaviour : TransitionStateBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TransitionOnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TransitionOnStateUpdate(animator, stateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TransitionOnStateExit(animator, stateInfo, layerIndex);
    }
}
