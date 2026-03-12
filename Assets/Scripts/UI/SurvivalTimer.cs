using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float elapsed;

    private void Update()
    {
        elapsed += Time.deltaTime;
        int minutes = (int)(elapsed / 60f);
        int seconds = (int)(elapsed % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
