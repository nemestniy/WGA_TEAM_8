using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatToBaseBehaviour : StateMachineBehaviour
{
    private const int PREV_MODE_NUM = 1;
    private const int NEXT_MODE_NUM = 0;
    [SerializeField] private float _timeToChange = 1;
    private float _currentTime;
    private static readonly int HasChanged = Animator.StringToHash("HasChanged");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _currentTime = 0;
    }

    private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (_currentTime < _timeToChange)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            animator.SetTrigger(HasChanged);
        }
        Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(NEXT_MODE_NUM, PREV_MODE_NUM,_currentTime); 
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
