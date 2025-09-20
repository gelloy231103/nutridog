using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text displayTextTMP;
    public string fullText = "Mechanics: In the Read me. Sort me. game, players enter a virtual room where various products are scattered around. Each product displays an ingredient label that the player must carefully read. The goal is to identify whether the product contains preservatives by looking for common ones such as sodium benzoate, potassium sorbate, or sulfites. Once the player determines if preservatives are present, they must drag and drop the product into the correct area: either the “Contains Preservatives” box or the “No Preservatives” table. Immediate feedback is given after each sorting action, correct choices earn points and interesting facts about preservatives, while mistakes prompt gentle hints encouraging the player to review the ingredients again.";
    public float typingSpeed = 0.05f;
    public float punctuationDelay = 0.2f;

    [Header("TTS Settings")]
    public bool waitForTTS = true; // Wait for TTS to finish before continuing

  
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipRequested = false;
    private TTSManager ttsManager;

    void Start()
    {

        // Get TTS manager
        ttsManager = FindFirstObjectByType<TTSManager>();

        if (ttsManager == null)
        {
            GameObject ttsObject = new GameObject("TTSManager");
            ttsManager = ttsObject.AddComponent<TTSManager>();
        }


        // Initialize UI
        if (displayTextTMP != null) displayTextTMP.text = "";

        // Start the typing effect
        if (fullText.Length > 0)
        {
            typingCoroutine = StartCoroutine(TypeText());
        }
    }

    void Update()
    {
        // Check for mouse click or touch to skip typing
        if (Mouse.current.leftButton.wasPressedThisFrame || Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
        {
            SkipTyping();
        }
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        skipRequested = false;

        // Start TTS
        if (ttsManager != null)
        {
            ttsManager.SpeakText(fullText);
        }

        for (int i = 0; i < fullText.Length; i++)
        {
            if (skipRequested) break;

            if (displayTextTMP != null) displayTextTMP.text += fullText[i];

            // Add extra delay for punctuation
            if (IsPunctuation(fullText[i]))
            {
                yield return new WaitForSeconds(punctuationDelay);
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // Ensure all text is displayed if skipped
        if (skipRequested)
        {
            if (displayTextTMP != null) displayTextTMP.text = fullText;
        }

        isTyping = false;
        
        // Wait for TTS to complete if enabled
        if (waitForTTS && ttsManager != null)
        {
            yield return new WaitForSeconds(CalculateTTSTime(fullText));
        }
    }

    private float CalculateTTSTime(string text)
    {
        // Estimate TTS time based on word count (adjust as needed)
        int wordCount = text.Split(' ').Length;
        return Mathf.Max(2f, wordCount * 0.5f); // At least 2 seconds, ~0.5 sec per word
    }

    bool IsPunctuation(char c)
    {
        return c == '.' || c == '!' || c == '?' || c == ',';
    }

    public void SkipTyping()
    {
        if (isTyping && !skipRequested)
        {
            skipRequested = true;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            
            if (displayTextTMP != null) displayTextTMP.text = fullText;
            
            isTyping = false;
            
            // Stop TTS if skipping
            if (ttsManager != null)
            {
                ttsManager.StopSpeaking();
            }
        }
    }

    public void SetNewText(string newText)
    {
        if (isTyping && typingCoroutine != null) 
            StopCoroutine(typingCoroutine);

        // Stop any current TTS
        if (ttsManager != null)
        {
            ttsManager.StopSpeaking();
        }

        fullText = newText;
        
        if (displayTextTMP != null) displayTextTMP.text = "";
        
        typingCoroutine = StartCoroutine(TypeText());
    }
}