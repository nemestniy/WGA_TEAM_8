using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausingBehaviour : StateMachineBehaviour
{

    private Animator _animator;
    private MoveController _keyManager;
    private static readonly int Unpause = Animator.StringToHash("Unpause");
    private static readonly int GoToExit = Animator.StringToHash("GoToExit");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        _keyManager = MoveController.Instance;
        PauseMenu.OnExit += Exit;
        UIManager.Instance.ShowPauseMenu();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_keyManager.GetPauseButton())
        {
            animator.SetTrigger(Unpause);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PauseMenu.OnExit -= Exit;
        UIManager.Instance.HidePauseMenu();
    }
    
    private void Exit()
    {
        _animator.SetTrigger(GoToExit);
    }
}
