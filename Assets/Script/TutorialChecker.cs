using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialChecker : MonoBehaviour
{
    void Start()
    {
        // Check if tutorial was already completed
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 1)
        {
            // Skip to Menu
            SceneManager.LoadScene("Menu");
        }
    }

    // Call this when player finishes the tutorial
    public void CompleteTutorial()
    {
        PlayerPrefs.SetInt("TutorialDone", 1); // Save as done
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
}
