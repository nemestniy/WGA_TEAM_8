using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _authorsPlane;

    [SerializeField]
    private AudioClip clickCore;

    [SerializeField]
    private AudioClip clickQuit;

    [SerializeField]
    private AudioClip clickQuitFake;

    [SerializeField]
    private GameObject _buttonsView;

    [SerializeField]
    private bool isVisibleMenu = true;

    [SerializeField]
    private bool isPlayerTryExit = false;

    public void PlayButton()
    {
        GetComponent<AudioSource>().PlayOneShot(clickCore);
        SceneManager.LoadScene("ReleaseScene");
    }
    
    public void PlayTutorButton()
    {
        GetComponent<AudioSource>().PlayOneShot(clickCore);
        SceneManager.LoadScene("TutorialTestScene");
    }
    
    public void QuitButton()
    {
        if (!isPlayerTryExit)
        {
            GetComponent<AudioSource>().PlayOneShot(clickQuitFake);
            isPlayerTryExit = true;
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(clickQuit);
            Application.Quit();
        }
    }

    public void AuthorsButton()
    {
        GetComponent<AudioSource>().PlayOneShot(clickCore);
        isVisibleMenu = false;
    }

    void HideMenu()
    {
        _buttonsView.SetActive(false);
        _authorsPlane.SetActive(true);
    }

    void ViewMenu()
    {
        _buttonsView.SetActive(true);
        _authorsPlane.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            isVisibleMenu = true;
        }

        if (isVisibleMenu) ViewMenu();
        else HideMenu();
    }
}
