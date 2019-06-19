using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Update()
    {
        Debug.Log("EventSystem.current.firstSelectedGameObject: " + EventSystem.current.firstSelectedGameObject);
    }
}
