using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneBehaviour : StateMachineBehaviour
{
    [SerializeField] 
    private Cutscene _cutscene;
    
    private GameManager _gameManager;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _gameManager = animator.GetComponent<GameManager>();
        
        _gameManager.PauseManager();
        _gameManager.CallCoroutine(CutscenesManager.Instance.Show(_cutscene));
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _gameManager.ResumeManager();
    }
}
