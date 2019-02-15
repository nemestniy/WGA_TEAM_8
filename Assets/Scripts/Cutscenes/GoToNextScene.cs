using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextScene : MonoBehaviour
{
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("ReleaseScene");
    }
    
}
