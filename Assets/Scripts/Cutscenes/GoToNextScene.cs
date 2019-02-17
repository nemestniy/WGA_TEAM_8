﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextScene : MonoBehaviour
{
    
    private void Update()
    {
        if (UnityEngine.Input.GetKey(KeyCode.Mouse0))
        {
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("ReleaseScene");
    }
    
}
