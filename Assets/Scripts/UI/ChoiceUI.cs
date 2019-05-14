using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceUI : MonoBehaviour
{
    public void ToRun()
    {
        GameManager.Instance.Run();
    }
    
    public void ToKill()
    {
        GameManager.Instance.Kill();
    }
}
