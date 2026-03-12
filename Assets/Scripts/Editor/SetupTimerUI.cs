using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SetupTimerUI
{
    public static void Execute()
    {
        // ── Find existing UICanvas ────────────────────────────────────────────
        var canvasGO = GameObject.Find("UICanvas");
        if (canvasGO == null)
        {
            Debug.LogError("[SurvivorIO] UICanvas not found.");
            return;
        }

        // ── Remove old timer if it exists ─────────────────────────────────────
        var old = canvasGO.transform.Find("TimerBar");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // ── Timer bar background ──────────────────────────────────────────────
        var barGO = new GameObject("TimerBar");
        barGO.transform.SetParent(canvasGO.transform, false);

        var barRT = barGO.AddComponent<RectTransform>();
        // Anchor top-center, stretch horizontally
        barRT.anchorMin        = new Vector2(0f, 1f);
        barRT.anchorMax        = new Vector2(1f, 1f);
        barRT.pivot            = new Vector2(0.5f, 1f);
        barRT.anchoredPosition = Vector2.zero;
        barRT.sizeDelta        = new Vector2(0f, 80f); // height 80px

        var barImg = barGO.AddComponent<Image>();
        barImg.color = new Color(0f, 0f, 0f, 0.55f);

        // ── Timer text ────────────────────────────────────────────────────────
        var textGO = new GameObject("TimerText");
        textGO.transform.SetParent(barGO.transform, false);

        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin        = Vector2.zero;
        textRT.anchorMax        = Vector2.one;
        textRT.offsetMin        = Vector2.zero;
        textRT.offsetMax        = Vector2.zero;

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text                = "00:00";
        tmp.fontSize            = 52;
        tmp.fontStyle           = FontStyles.Bold;
        tmp.color               = Color.white;
        tmp.alignment           = TextAlignmentOptions.Center;
        tmp.enableAutoSizing    = false;

        // ── SurvivalTimer component ───────────────────────────────────────────
        var timer = barGO.AddComponent<SurvivalTimer>();
        var so = new SerializedObject(timer);
        so.FindProperty("timerText").objectReferenceValue = tmp;
        so.ApplyModifiedProperties();

        // ── Save scene ────────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ Timer UI setup complete.");
    }
}
