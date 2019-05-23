using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private GameObject _pauseMenu;
    
    #region Singletone
    public static UIManager Instance { get; private set; }
    public UIManager() : base()
    {
        Instance = this;
    }
    #endregion
    
    private void Awake()
    {
        _pauseMenu = transform.GetChild(0).gameObject;
    }

    public void ShowPauseMenu()
    {
        if (SceneManager.GetActiveScene().name == "TutorialTestScene")//TODO:кастыыыыыль
        {
            GameManagerTrial.Instance.PauseManager();
        }
        else
        {
            GameManager.Instance.PauseManager();
        }
        
        _pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (SceneManager.GetActiveScene().name == "TutorialTestScene")//TODO:кастыыыыыль
        {
            GameManagerTrial.Instance.ResumeManager();
        }
        else
        {
            GameManager.Instance.ResumeManager();
        }
        _pauseMenu.SetActive(false);
    }
}
