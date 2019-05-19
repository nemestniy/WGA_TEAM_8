using UnityEngine;

public class StartingBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private Cutscene _cutscene;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MapManager.Instance.StartManager(); //instantiate map
        animator.GetComponent<GameManager>().CallCoroutine(CutscenesManager.Instance.Show(_cutscene)); //show start cutscene
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<GameManager>().StartManager(); //start the game
    }
}
