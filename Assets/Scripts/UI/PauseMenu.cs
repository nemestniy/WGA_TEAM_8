using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static event Action OnExit;
    public AudioClip click;


    public void GoToMainMenuButton()
    {
        GetComponent<AudioSource>().PlayOneShot(click);
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitButton()
    {
        GetComponent<AudioSource>().PlayOneShot(click);
        Application.Quit();
    }
    
    public void SkipLvlButton()
    {
        GetComponent<AudioSource>().PlayOneShot(click);
        if (SceneManager.GetActiveScene().name == "TutorialTestScene")
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
        GetComponent<AudioSource>().PlayOneShot(click);
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
