using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Load a scene by its name
    public void LoadScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
    
    // Exit the game
    public void ExitGame()
    {
        Debug.Log("Exiting application");
        
#if UNITY_EDITOR
        // If we're running in the Unity editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If we're running in a built game, quit the application
            Application.Quit();
#endif
    }
}