using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingBehaviour : StateMachineBehaviour
{
    private Animator _animator;
    private GameManager _gameManager;
    private MoveController _keyManager;
    private static readonly int DieByEnemy = Animator.StringToHash("DieByEnemy");
    private static readonly int DieByRunOutOfEnergy = Animator.StringToHash("DieByRunOutOfEnergy");
    private static readonly int GoToExit = Animator.StringToHash("GoToExit");
    private static readonly int FindWell = Animator.StringToHash("FindWell");
    private static readonly int Pause = Animator.StringToHash("Pause");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        _gameManager = animator.GetComponent<GameManager>();
        _keyManager = MoveController.Instance;
        Enemy.OnTrigger += OnDeathByEnemy;
        EnemyDeepWaterer.OnTrigger += OnDeathByEnemy;
        EnemyStatue.OnTrigger += OnDeathByEnemy;
        Energy.OnRanoutOfEnergy += OnDeathByRanoutOfEnergy;
        ExitTrigger.OnExit += Exit;
        Well.OnTrigger += OnFindWell;
        
        UIManager.Instance.ShowPauseMenu();//TODO:КАСТЫЫЫЫЛЬ
        UIManager.Instance.HidePauseMenu();
    }

    private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (_keyManager.GetPauseButton())
        {
            animator.SetTrigger(Pause);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Enemy.OnTrigger -= OnDeathByEnemy;
        EnemyDeepWaterer.OnTrigger -= OnDeathByEnemy;
        EnemyStatue.OnTrigger -= OnDeathByEnemy;
        Energy.OnRanoutOfEnergy -= OnDeathByRanoutOfEnergy;
        ExitTrigger.OnExit -= Exit;
        Well.OnTrigger -= OnFindWell;
    }

    private void OnFindWell()
    {
        _gameManager.GetMapManager().MakeMapPass();
        _animator.SetTrigger(FindWell);
    }

    private void OnDeathByEnemy()
    {
        _animator.SetTrigger(DieByEnemy);
    }

    private void OnDeathByRanoutOfEnergy()
    {
        _animator.SetTrigger(DieByRunOutOfEnergy);
    }

    private void Exit()
    {
        _animator.SetTrigger(GoToExit);
    }
}
