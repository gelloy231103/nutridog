using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // New Input System namespace

public class Splash : MonoBehaviour
{
    void Update()
    {
        // Detect mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LoadNextScene();
        }

        // Detect screen touch
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (PlayerPrefs.HasKey("Username") && !string.IsNullOrEmpty(PlayerPrefs.GetString("Username")))
        {
            SceneManager.LoadScene("WelcomeScene"); // Username exists
        }
        else
        {
            SceneManager.LoadScene("UsernameInput"); // Ask for username
        }
    }
}
