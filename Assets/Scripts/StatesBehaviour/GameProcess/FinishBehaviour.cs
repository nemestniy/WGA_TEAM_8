using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneManager.LoadScene(0);
    }
}
