using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionStateBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private int _prevModeNum;
    [SerializeField]
    private int _nextModeNum;
    
    [SerializeField]
    private float _timeToChange = 1;
    
    private float _currentTime;
    private static readonly int HasChanged = Animator.StringToHash("HasChanged");
    private Lamp _playersLamp;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playersLamp = PlayerManager.Instance.player.transform.GetChild(0).GetComponent<Lamp>();
        _currentTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (_currentTime < _timeToChange)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            animator.SetTrigger(HasChanged);
        }
        _playersLamp.SetLightMode(_nextModeNum, _prevModeNum,_currentTime / _timeToChange);
    }
}
