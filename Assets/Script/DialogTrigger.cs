using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;
using UnityEngine.InputSystem;

public class DialogTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    [TextArea] public string message;

    [Header("UI to Disable")]
    public GameObject uiGameObjectToDisable;

    [Header("Control References")]
    public GameObject joystick;
    public GameObject jumpButton;
    public GameObject sprintButton;
    public GameObject scanButton;
    public GameObject lookButton;
    public GameObject crawlButton;

    [Header("Player Reference")]
    public GameObject player; // Assign the player in inspector

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    [Header("Trigger Settings")]
    public bool disableTriggerAfterUse = true;
    public bool disableUIGameObject = true;
    public bool useTextToSpeech = true;

    private Coroutine typingCoroutine;
    private bool dialogActive = false;
    private bool typingFinished = false;
    private bool hasCollided = false;

    private ThirdPersonController playerController;
    private StarterAssetsInputs playerInputs;
    private PlayerInput playerInput;
    private TTSManager ttsManager;
    private BoxCollider boxCollider;
    private KingNPCRandomWalk npcMovement;

    private void Start()
    {
        // Find the player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        playerController = player.GetComponent<ThirdPersonController>();
        playerInput = player.GetComponent<PlayerInput>();
        playerInputs = player.GetComponent<StarterAssetsInputs>();
        
        // Get NPC movement
        npcMovement = GetComponent<KingNPCRandomWalk>();
        
        // Get TTS manager
        ttsManager = FindFirstObjectByType<TTSManager>();
        if (ttsManager == null)
        {
            GameObject ttsObject = new GameObject("TTSManager");
            ttsManager = ttsObject.AddComponent<TTSManager>();
        }
        
        // Get box collider
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !dialogActive && !hasCollided)
        {
            hasCollided = true;
            TriggerDialog();
        }
    }

    public void TriggerDialog()
    {
        dialogActive = true;
        dialogBox.SetActive(true);

        // Disable UI GameObject if specified
        if (disableUIGameObject && uiGameObjectToDisable != null)
        {
            uiGameObjectToDisable.SetActive(false);
        }

        // Hide all controls
        ToggleControls(false);

        // Disable player movement
        DisablePlayerMovement();

        // Disable NPC movement
        DisableNPCMovement();

        // Disable box collider
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // Start TTS if enabled
        if (useTextToSpeech && ttsManager != null)
        {
            ttsManager.SpeakText(message);
        }

        // Start typing effect
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private void DisablePlayerMovement()
    {
        // Method 1: Disable PlayerInput (new Input System)
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        // Method 2: Reset input values to stop movement
        if (playerInputs != null)
        {
            playerInputs.MoveInput(Vector2.zero);
            playerInputs.LookInput(Vector2.zero);
            playerInputs.JumpInput(false);
            playerInputs.SprintInput(false);
        }

        // Method 3: Disable controller directly
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    private void EnablePlayerMovement()
    {
        // Method 1: Enable PlayerInput
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        // Method 2: Re-enable controller
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private void DisableNPCMovement()
    {
        if (npcMovement != null)
        {
            npcMovement.SetMovement(false);
            npcMovement.enabled = false; // Completely disable the script
        }
    }

    private void EnableNPCMovement()
    {
        if (npcMovement != null)
        {
            npcMovement.enabled = true;
            npcMovement.SetMovement(true);
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
                    
                    // Stop TTS if skipping
                    if (useTextToSpeech && ttsManager != null)
                    {
                        ttsManager.StopSpeaking();
                    }
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

        // Re-enable UI GameObject if it was disabled
        if (disableUIGameObject && uiGameObjectToDisable != null)
        {
            uiGameObjectToDisable.SetActive(true);
        }

        // Show controls again
        ToggleControls(true);

        // Re-enable player movement
        EnablePlayerMovement();

        // Re-enable NPC movement
        EnableNPCMovement();

        // Stop TTS
        if (useTextToSpeech && ttsManager != null)
        {
            ttsManager.StopSpeaking();
        }

        // Disable this GameObject after dialog is closed if enabled
        if (disableTriggerAfterUse)
        {
            gameObject.SetActive(false);
        }

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

    // Public method to allow NPC click to trigger dialog again
    public void OnNPCInteract()
    {
        if (!dialogActive)
        {
            TriggerDialog();
        }
    }
}