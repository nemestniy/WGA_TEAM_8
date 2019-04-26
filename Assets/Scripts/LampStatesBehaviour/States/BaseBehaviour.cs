using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBehaviour : StateMachineBehaviour
{
    private const int CURRENT_MODE_NUM = 0;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //1 means mode is not changing
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(CURRENT_MODE_NUM,CURRENT_MODE_NUM,1); 
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
