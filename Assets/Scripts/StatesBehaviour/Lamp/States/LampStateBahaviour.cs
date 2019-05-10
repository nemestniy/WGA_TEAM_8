using System.Collections.Generic;
using UnityEngine;

public class LampStateBahaviour : StateMachineBehaviour
{
    [SerializeField]
    private int _currentModeNum;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //1 means mode is not changing
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(_currentModeNum,_currentModeNum,1);
    }
}
