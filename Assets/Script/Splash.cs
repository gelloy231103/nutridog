using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Splash : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LoadNextScene();
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (PlayerPrefs.HasKey("Username") && !string.IsNullOrEmpty(PlayerPrefs.GetString("Username")))
        {
            SceneManager.LoadScene("Menu"); 
        }
        else
        {
            SceneManager.LoadScene("UsernameInput"); 
        }
    }
}
