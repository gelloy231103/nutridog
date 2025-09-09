using UnityEngine;
using TMPro; // Required for TMP
using UnityEngine.SceneManagement;

public class UsernameInput : MonoBehaviour
{
    public TMP_InputField usernameField; // TMP input field

    public void ConfirmUsername()
    {
        string username = usernameField.text.Trim();

        if (string.IsNullOrEmpty(username))
            return;

        // Save username
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.Save();

        // Go to WelcomeScene
        SceneManager.LoadScene("WelcomeScene");
    }
}
