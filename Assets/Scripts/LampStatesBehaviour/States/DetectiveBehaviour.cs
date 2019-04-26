using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectiveBehaviour : StateMachineBehaviour
{
    private const int CURRENT_MODE_NUM = 2;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetDetectiveMaterial();
        //1 means mode is not changing
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(CURRENT_MODE_NUM,CURRENT_MODE_NUM,1); 
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetNormalMaterial();
    }
}
