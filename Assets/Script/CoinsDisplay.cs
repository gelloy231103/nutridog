using UnityEngine;
using TMPro;

public class CoinsDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    private void Start()
    {
        int savedCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        coinsText.text = savedCoins.ToString("N0");
    }
}
