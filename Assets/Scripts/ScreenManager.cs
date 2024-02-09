using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public void FistBoss()
    {
        // Load the main game scene
        SceneManager.LoadScene("FistBoss");
    }

    public void BackToLobby()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void SkillTree()
    {
        SceneManager.LoadScene("SkillTree");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }

    // Add more methods for other buttons like Options, etc.
}