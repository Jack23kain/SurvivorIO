using UnityEngine;

public class PlayerHPBar : MonoBehaviour
{
    [SerializeField] private Transform fill;

    // Bar width in player local units (player scale 0.075 → 8 local = ~0.6 world units)
    private const float BarLocalWidth = 8f;

    public void SetRatio(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        fill.localScale    = new Vector3(BarLocalWidth * ratio, fill.localScale.y, 1f);
        fill.localPosition = new Vector3(BarLocalWidth * (ratio - 1f) * 0.5f,
                                         fill.localPosition.y,
                                         fill.localPosition.z);
    }
}
