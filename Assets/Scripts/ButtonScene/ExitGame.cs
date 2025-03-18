using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // Method to exit the game
    public void ExitApplication()
    {
        Debug.Log("Game is exiting...");

        // Exits the application
        Application.Quit();

        // If running in the Unity editor, stop the editor play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
