using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] cardButtons;
    [SerializeField] private TextMeshProUGUI[] cardTitles;
    [SerializeField] private TextMeshProUGUI[] cardDescs;

    private struct Skill
    {
        public string title;
        public string desc;
        public System.Action apply;
    }

    private Skill[] skills;
    private int[] shownIndices = new int[3];

    private void Awake()
    {
        panel.SetActive(false);

        var player = GameObject.FindWithTag("Player");
        var autoAttack = player?.GetComponent<AutoAttack>();
        var playerCtrl = player?.GetComponent<PlayerController>();

        skills = new Skill[]
        {
            new Skill {
                title = "Rapid Fire",
                desc  = "Fire rate increased by 25%",
                apply = () => autoAttack?.UpgradeFireRate()
            },
            new Skill {
                title = "Power Strike",
                desc  = "Dagger damage +5",
                apply = () => autoAttack?.UpgradeDamage()
            },
            new Skill {
                title = "Swift Feet",
                desc  = "Movement speed +1.5",
                apply = () => playerCtrl?.UpgradeSpeed()
            },
            new Skill {
                title = "Wide Range",
                desc  = "Attack range +2",
                apply = () => autoAttack?.UpgradeRange()
            },
            new Skill {
                title = "Vital Boost",
                desc  = "Restore 5 HP",
                apply = () => player?.GetComponent<PlayerHealth>()?.Heal(5)
            },
        };
    }

    public void Show()
    {
        // Pick 3 unique random skills
        var indices = new System.Collections.Generic.List<int> { 0, 1, 2, 3, 4 };
        for (int i = 0; i < 3; i++)
        {
            int pick = Random.Range(i, indices.Count);
            (indices[i], indices[pick]) = (indices[pick], indices[i]);
            shownIndices[i] = indices[i];
            cardTitles[i].text = skills[indices[i]].title;
            cardDescs[i].text  = skills[indices[i]].desc;
        }

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SelectCard(int index)
    {
        skills[shownIndices[index]].apply?.Invoke();
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
