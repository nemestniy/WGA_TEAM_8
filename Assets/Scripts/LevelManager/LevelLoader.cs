using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelLoader : MonoBehaviour
{
    private AsyncOperation _asyncResult;
    private Scene _loadedScene;
    // Start is called before the first frame update
    void Start()
    {
        _loadedScene = SceneManager.GetSceneByName(LevelManager.SceneNameToLoad);
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        //SceneManager.LoadScene(LevelManager.SceneNameToLoad);
        StartCoroutine(LoadScene());

    }

    // Update is called once per frame
    void Update()
    {
        //if (_asyncResult != null && _asyncResult.isDone)
        //{
        //    Debug.Log("Loading done");
        //    if (_loadedScene == null)
        //        return;
        //    Debug.Log("Scene found");
        //    var managers = _loadedScene.GetRootGameObjects().FirstOrDefault(o => o.name == "Managers");
        //    if(managers != null)
        //    {
        //        var managerComonents = managers.GetComponentsInChildren<Manager>();
        //        if (managerComonents.Where(c => c.IsLoaded).Count() != 0)
        //            return;
        //    }
        //    SwitchScene();
        //}
    }

    IEnumerator LoadScene()
    {
        if (LevelManager.SceneNameToLoad == "")
            yield break;

        _asyncResult = SceneManager.LoadSceneAsync(LevelManager.SceneNameToLoad);
        _asyncResult.allowSceneActivation = false;

        while (!_asyncResult.isDone)
        {
            Debug.Log("Loading");
            yield return null;
        }
        Debug.Log("Loading done");
        //var managers = _loadedScene.GetRootGameObjects().FirstOrDefault(o => o.name == "Managers");
        //if (managers != null)
        //{
        //    var managerComonents = managers.GetComponentsInChildren<Manager>();
        //    if (managerComonents.Where(c => c.IsLoaded).Count() != 0)
        //        yield return null;
        //}
        //SwitchScene();
    }

    private void SwitchScene()
    {
        if (_asyncResult != null)
            _asyncResult.allowSceneActivation = true;
    }
}
