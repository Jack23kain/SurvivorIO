using UnityEngine;

public class XPManager : MonoBehaviour
{
    [SerializeField] private XPBar xpBar;
    [SerializeField] private LevelUpUI levelUpUI;

    private int currentLevel = 1;
    private int currentXP = 0;

    public int CurrentLevel => currentLevel;

    private int XPRequired => 10 * currentLevel;

    private void Start()
    {
        xpBar?.SetRatio(0f);
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        xpBar?.SetRatio((float)currentXP / XPRequired);

        if (currentXP >= XPRequired)
        {
            currentXP -= XPRequired;
            currentLevel++;
            xpBar?.SetRatio(0f);
            levelUpUI?.Show();
        }
    }
}
