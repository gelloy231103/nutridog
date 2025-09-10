using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ClaimButton : MonoBehaviour
{
    [Header("UI")]
    public Button claimButton;           
    public TextMeshProUGUI coinsText;    
    public TextMeshProUGUI rewardText;   

    [Header("Cooldown Overlay")]
    public Image darkOverlay;            // new
    public TextMeshProUGUI countdownText; // new

    public float cooldownTime = 10f;     
    public float animationDuration = 4f; 

    private int totalCoins;
    private Coroutine cooldownRoutine;

    private void Start()
    {
        // Load saved coins
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        UpdateCoinsUI(totalCoins);

        claimButton.onClick.AddListener(OnClaim);

        // Load cooldown
        long savedCooldownEnd = long.Parse(PlayerPrefs.GetString("CooldownEnd", "0"));
        long currentTime = GetUnixTime();

        if (currentTime < savedCooldownEnd)
        {
            float remaining = savedCooldownEnd - currentTime;
            StartCooldown(remaining);
        }
        else
        {
            ReadyState();
        }
    }

    private void OnClaim()
    {
        // Clean reward text (remove commas)
        string cleanText = rewardText.text.Replace(",", "");
        if (!int.TryParse(cleanText, out int rewardValue))
        {
            Debug.LogWarning("Reward text is not a valid number: " + rewardText.text);
            return;
        }

        int oldCoins = totalCoins;
        totalCoins += rewardValue;

        // Save coins
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();

        // Animate coin increase
        StopAllCoroutines();
        StartCoroutine(AnimateCoins(oldCoins, totalCoins));

        // Start cooldown
        long cooldownEnd = GetUnixTime() + (long)cooldownTime;
        PlayerPrefs.SetString("CooldownEnd", cooldownEnd.ToString());
        PlayerPrefs.Save();

        StartCooldown(cooldownTime);
    }

    private void StartCooldown(float time)
    {
        claimButton.interactable = false;
        darkOverlay.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);

        if (cooldownRoutine != null) StopCoroutine(cooldownRoutine);
        cooldownRoutine = StartCoroutine(CooldownCountdown(time));
    }

    private IEnumerator CooldownCountdown(float time)
    {
        float remaining = time;

        while (remaining > 0f)
        {
            countdownText.text = Mathf.CeilToInt(remaining).ToString();
            remaining -= Time.deltaTime;
            yield return null;
        }

        ReadyState();
    }

    private void ReadyState()
    {
        claimButton.interactable = true;
        darkOverlay.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
    }

    private IEnumerator AnimateCoins(int startValue, int endValue)
    {
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            int current = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));
            UpdateCoinsUI(current);

            yield return null;
        }

        UpdateCoinsUI(endValue);
    }

    private void UpdateCoinsUI(int value)
    {
        coinsText.text = value.ToString("N0");
    }

    private long GetUnixTime()
    {
        return System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
