using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static event Action OnExit;
    
    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }
    
    public void SkipLvlButton()
    {
        OnExit?.Invoke();
    }
    
    public void RestartLvlButton()
    {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
