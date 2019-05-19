using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceUI : MonoBehaviour
{
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Kill = Animator.StringToHash("Kill");

    public void ToRun()
    {
        GameManager.Instance.GetComponent<Animator>().SetTrigger(Run);
    }
    
    public void ToKill()
    {
        GameManager.Instance.GetComponent<Animator>().SetTrigger(Kill);
    }
}
