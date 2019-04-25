﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectiveBehaviour : LampStateBahaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LampOnStateEnter(animator, stateInfo, layerIndex);
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetDetectiveMaterial();
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        LampOnStateUpdate(animator, animatorStateInfo, layerIndex);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LampOnStateExit(animator, stateInfo, layerIndex);
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetNormalMaterial();
    }
}
