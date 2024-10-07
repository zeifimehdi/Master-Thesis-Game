using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public string sceneToLoad; // Name of the scene to load when the play button is clicked

    public void OnPlayButtonClicked(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // Load the specified scene
        ClearAllPlayerPrefs();
    }

    public void ClearAllPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.DeleteKey("CurrentLevel");
            Debug.Log("Cleared preference with key: ") ;
        }
    }
}
