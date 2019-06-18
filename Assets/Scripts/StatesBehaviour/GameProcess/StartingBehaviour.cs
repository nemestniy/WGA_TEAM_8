using UnityEngine;

public class StartingBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private Cutscene _cutscene;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MapManager.Instance.StartManager(); //instantiate map
        if (_cutscene.isVideo)
        {
            CutscenesManager.Instance.ShowVideo(_cutscene.video);
        }
        else
        {
            animator.GetComponent<GameManager>().CallCoroutine(CutscenesManager.Instance.ShowFrames(_cutscene)); //show start cutscene
        }
        
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<GameManager>().StartManager(); //start the game
    }
}
