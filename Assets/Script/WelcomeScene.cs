using UnityEngine;
using TMPro; // for TextMeshPro

public class WelcomeScene : MonoBehaviour
{
    public TMP_Text profileText; // assign in Inspector

    void Start()
    {
        // Load saved username
        string username = PlayerPrefs.GetString("Username", "Guest");

        // Reflect username in profile text
        profileText.text = username;
    }
}
