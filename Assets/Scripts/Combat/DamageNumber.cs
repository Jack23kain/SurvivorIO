using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float lifetime   = 0.7f;

    private float timer;
    private Color startColor;

    public void Init(int amount)
    {
        label.text = amount.ToString();
        startColor = label.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        label.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
