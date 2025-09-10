using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Call this from the Start Game button
    public void LoadGameScene()
    {
        SceneManager.LoadScene("CityScene"); // Replace with your scene name if different
    }

    // Call this from the Quit Game button
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // Useful to confirm it works in the editor
        Application.Quit();     // This will quit the built application
    }
}
