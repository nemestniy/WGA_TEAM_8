﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectiveBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetDetectiveMaterial();
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetNormalMaterial();
    }
}
