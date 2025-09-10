using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets; // for ThirdPersonController
using UnityEngine.InputSystem; // new Input System

public class DialogTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    [TextArea] public string message;

    [Header("Control References")]
    public GameObject joystick;
    public GameObject jumpButton;
    public GameObject sprintButton;
    public GameObject scanButton;
    public GameObject lookButton;
    public GameObject crawlButton;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;
    private bool dialogActive = false;
    private bool typingFinished = false;

    private ThirdPersonController playerController;
    private PlayerInput playerInput;   // 👈 new

    private void Start()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<ThirdPersonController>();
        playerInput = player.GetComponent<PlayerInput>(); // 👈 get PlayerInput
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !dialogActive)
        {
            dialogActive = true;
            dialogBox.SetActive(true);

            // Hide all controls
            ToggleControls(false);

            // Disable movement by disabling PlayerInput
            if (playerInput != null) playerInput.enabled = false;

            // Start typing effect
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(message));
        }
    }

    private void Update()
    {
        if (dialogActive)
        {
            bool tapped = false;

            // For touch (mobile)
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                tapped = true;

            // For mouse (editor / PC)
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                tapped = true;

            if (tapped)
            {
                if (!typingFinished)
                {
                    // Skip typing and instantly show full text
                    if (typingCoroutine != null)
                        StopCoroutine(typingCoroutine);

                    dialogText.text = message;
                    typingFinished = true;
                }
                else
                {
                    // Close dialog
                    CloseDialog();
                }
            }
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        dialogText.text = "";
        typingFinished = false;

        foreach (char c in textToType)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingFinished = true;
    }

    private void CloseDialog()
    {
        dialogBox.SetActive(false);

        // Show controls again
        ToggleControls(true);

        // Re-enable movement
        if (playerInput != null) playerInput.enabled = true;

        dialogActive = false;
    }

    private void ToggleControls(bool state)
    {
        if (joystick != null) joystick.SetActive(state);
        if (jumpButton != null) jumpButton.SetActive(state);
        if (sprintButton != null) sprintButton.SetActive(state);
        if (scanButton != null) scanButton.SetActive(state);
        if (lookButton != null) lookButton.SetActive(state);
        if (crawlButton != null) crawlButton.SetActive(state);
    }
}
