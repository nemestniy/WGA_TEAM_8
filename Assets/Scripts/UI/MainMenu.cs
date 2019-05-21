using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _authorsPlane;
    
    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }
    
    public void PlayTutorButton()
    {
//        SceneManager.LoadScene(1);
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            _authorsPlane.SetActive(false);
        }
    }
}
