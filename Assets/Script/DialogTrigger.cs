using UnityEngine;
using TMPro;
using System.Collections;

public class DialogTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogBox;              // Panel to show
    public TextMeshProUGUI dialogText;        // Text component inside panel
    [TextArea] public string message;         // Message to display

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;         // Delay between letters

    private Coroutine typingCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogBox.SetActive(true);

            // Reset text and start typing
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(message));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop typing if player leaves
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogText.text = "";
            dialogBox.SetActive(false);
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        dialogText.text = "";

        foreach (char c in textToType)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
