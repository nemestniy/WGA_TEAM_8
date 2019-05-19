using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLayingBehaviour : StateMachineBehaviour
{
    private Animator _animator;
    private GameManager _gameManager;
    private static readonly int DieByEnemy = Animator.StringToHash("DieByEnemy");
    private static readonly int DieByRunOutOfEnergy = Animator.StringToHash("DieByRunOutOfEnergy");
    private static readonly int GoToExit = Animator.StringToHash("GoToExit");
    private static readonly int FindWell = Animator.StringToHash("FindWell");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        _gameManager = animator.GetComponent<GameManager>();
        Enemy.OnTrigger += OnDeathByEnemy;
        EnemyDeepWaterer.OnTrigger += OnDeathByEnemy;
        EnemyStatue.OnTrigger += OnDeathByEnemy;
        Energy.OnRanoutOfEnergy += OnDeathByRanoutOfEnergy;
        ExitTrigger.OnExit += Exit;
        Well.OnTrigger += OnFindWell;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Enemy.OnTrigger -= OnDeathByEnemy;
        EnemyDeepWaterer.OnTrigger -= OnDeathByEnemy;
        EnemyStatue.OnTrigger -= OnDeathByEnemy;
        Energy.OnRanoutOfEnergy -= OnDeathByRanoutOfEnergy;
        ExitTrigger.OnExit -= Exit;
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
