using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private bool isTutor;
    public static event Action OnExit;
    
    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }
    
    public void SkipLvlButton()
    {
        if (isTutor)
        {
            SceneManager.LoadScene("ReleaseScene");
        }
        else
        {
            OnExit?.Invoke();
        }
        
    }
    
    public void RestartLvlButton()
    {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
