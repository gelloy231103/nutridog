using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TTSManager : MonoBehaviour
{
    [Header("TTS Settings")]
    public bool enableTTS = true;
    
    private AudioSource currentAudioSource;
    
    public void SpeakText(string text)
    {
        if (!enableTTS || string.IsNullOrEmpty(text)) return;
        
        StartCoroutine(DownloadTTSAudio(text));
    }
    
    IEnumerator DownloadTTSAudio(string text)
    {
        // Use StreamElements TTS API (HTTPS, no API key needed)
        string url = "https://api.streamelements.com/kappa/v2/speech?voice=Brian&text=" + UnityWebRequest.EscapeURL(text);
        
        using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            // Add headers to avoid blocking
            webRequest.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(webRequest);
                PlayAudioClip(clip);
                Debug.Log("TTS playing: " + text);
            }
            else
            {
                Debug.LogError("TTS Request failed: " + webRequest.error);
                Debug.Log("TTS would say: " + text);
                
                // Fallback to system TTS if available
                TrySystemTTS(text);
            }
        }
    }
    
    void TrySystemTTS(string text)
    {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        SpeakWindows(text);
        #elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        SpeakMac(text);
        #endif
    }
    
    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    void SpeakWindows(string text)
    {
        try
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "powershell";
            process.StartInfo.Arguments = $"-Command \"Add-Type -AssemblyName System.Speech; $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; $speak.Speak('{text.Replace("'", "''")}')\"";
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Windows TTS failed: " + e.Message);
        }
    }
    #endif
    
    #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    void SpeakMac(string text)
    {
        try
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "say";
            process.StartInfo.Arguments = $"\"{text}\"";
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Mac TTS failed: " + e.Message);
        }
    }
    #endif
    
    void PlayAudioClip(AudioClip clip)
    {
        // Clean up previous audio source
        if (currentAudioSource != null && currentAudioSource.gameObject != null)
        {
            Destroy(currentAudioSource.gameObject);
        }
        
        // Create new audio source
        GameObject tempObject = new GameObject("TempAudio");
        currentAudioSource = tempObject.AddComponent<AudioSource>();
        currentAudioSource.clip = clip;
        currentAudioSource.Play();
        
        // Destroy when finished
        Destroy(tempObject, clip.length);
    }
    
    public void StopSpeaking()
    {
        if (currentAudioSource != null)
        {
            currentAudioSource.Stop();
            if (currentAudioSource.gameObject != null)
            {
                Destroy(currentAudioSource.gameObject);
            }
        }
    }
}