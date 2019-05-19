using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTransitionStateBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private int _botModeNum;
    [SerializeField]
    private int _topModeNum;

    [SerializeField]
    private int _mouseButtonState;
    
    [SerializeField]
    private float _timeToChange = 1;
    
    private float _currentTime;
    private static readonly int HasChanged = Animator.StringToHash("HasChanged");
    private static readonly int ButtonState = Animator.StringToHash("ButtonState");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetInteger(ButtonState) == 0)
        {
            _currentTime = _timeToChange;
        }
        else
        {
            _currentTime = 0;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (animator.GetInteger(ButtonState) == _mouseButtonState)
        {
            if (_currentTime < _timeToChange)
            {
                _currentTime += Time.deltaTime;
            }
            else
            {
                animator.SetTrigger(HasChanged);
            }
            Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(_topModeNum, _botModeNum,_currentTime / _timeToChange);
        }
        else
        {
            if (_currentTime >= 0)
            {
                _currentTime -= Time.deltaTime;
            }
            else
            {
                animator.SetTrigger(HasChanged);
            }
            Player.Instance.transform.GetChild(0).GetComponent<Lamp>().SetLightMode(_topModeNum, _botModeNum,_currentTime / _timeToChange);
        }
    }
}
