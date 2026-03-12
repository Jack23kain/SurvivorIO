using UnityEngine;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    [SerializeField] private Image fill;

    public void SetRatio(float ratio)
    {
        fill.fillAmount = Mathf.Clamp01(ratio);
    }
}
