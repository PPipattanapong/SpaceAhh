using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneByName2 : MonoBehaviour
{
    // Public field to set the scene name in the Inspector
    public string sceneName;

    // Method to load the scene based on the name entered in the Inspector
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not specified or is empty.");
        }
    }
}
