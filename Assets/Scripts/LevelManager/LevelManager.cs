using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager
{
    public static readonly string LevelManagerSceneName = "LevelManager";
    public static string SceneNameToLoad;

    public static void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(LevelManagerSceneName, LoadSceneMode.Single);
        var levelManagerScene = SceneManager.GetSceneByName(LevelManagerSceneName);
        SceneNameToLoad = levelName;
    }
}

