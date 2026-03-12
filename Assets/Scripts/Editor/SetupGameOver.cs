using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SetupGameOver
{
    public static void Execute()
    {
        // ── Find font ─────────────────────────────────────────────────────────
        TMP_FontAsset font = null;
        foreach (var guid in AssetDatabase.FindAssets("t:TMP_FontAsset"))
        {
            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guid));
            if (font != null) break;
        }

        var canvasGO = GameObject.Find("UICanvas");
        if (canvasGO == null) { Debug.LogError("[SurvivorIO] UICanvas not found."); return; }

        // Remove old if re-running
        var old = canvasGO.transform.Find("GameOverPanel");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // ── Full-screen overlay ───────────────────────────────────────────────
        var panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvasGO.transform, false);
        var panelRT = panel.AddComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero; panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero; panelRT.offsetMax = Vector2.zero;
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.85f);

        // ── "GAME OVER" text ──────────────────────────────────────────────────
        var titleGO = new GameObject("GameOverTitle");
        titleGO.transform.SetParent(panel.transform, false);
        var titleRT = titleGO.AddComponent<RectTransform>();
        titleRT.anchorMin        = new Vector2(0f, 0.55f);
        titleRT.anchorMax        = new Vector2(1f, 0.85f);
        titleRT.offsetMin        = Vector2.zero;
        titleRT.offsetMax        = Vector2.zero;
        var titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
        titleTMP.text      = "GAME OVER";
        titleTMP.font      = font;
        titleTMP.fontSize  = 90;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.color     = new Color(1f, 0.25f, 0.25f);
        titleTMP.alignment = TextAlignmentOptions.Center;

        // ── Retry button ──────────────────────────────────────────────────────
        var btnGO = new GameObject("RetryButton");
        btnGO.transform.SetParent(panel.transform, false);
        var btnRT = btnGO.AddComponent<RectTransform>();
        btnRT.anchorMin        = new Vector2(0.5f, 0.5f);
        btnRT.anchorMax        = new Vector2(0.5f, 0.5f);
        btnRT.pivot            = new Vector2(0.5f, 0.5f);
        btnRT.anchoredPosition = new Vector2(0f, -80f);
        btnRT.sizeDelta        = new Vector2(320f, 100f);

        var btnImg = btnGO.AddComponent<Image>();
        btnImg.color = new Color(0.15f, 0.7f, 0.3f);

        var btn = btnGO.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.2f, 0.9f, 0.4f);
        colors.pressedColor     = new Color(0.1f, 0.5f, 0.2f);
        btn.colors = colors;

        var btnLabelGO = new GameObject("Label");
        btnLabelGO.transform.SetParent(btnGO.transform, false);
        var btnLabelRT = btnLabelGO.AddComponent<RectTransform>();
        btnLabelRT.anchorMin = Vector2.zero; btnLabelRT.anchorMax = Vector2.one;
        btnLabelRT.offsetMin = Vector2.zero; btnLabelRT.offsetMax = Vector2.zero;
        var btnTMP = btnLabelGO.AddComponent<TextMeshProUGUI>();
        btnTMP.text      = "Retry";
        btnTMP.font      = font;
        btnTMP.fontSize  = 48;
        btnTMP.fontStyle = FontStyles.Bold;
        btnTMP.color     = Color.white;
        btnTMP.alignment = TextAlignmentOptions.Center;

        // ── GameOverUI component ──────────────────────────────────────────────
        var goUI = canvasGO.AddComponent<GameOverUI>();
        var goUISO = new SerializedObject(goUI);
        goUISO.FindProperty("panel").objectReferenceValue = panel;
        goUISO.ApplyModifiedProperties();

        // Wire Retry button at runtime via a helper (AddListener doesn't persist)
        // GameOverUI.Retry() is called via a persistent listener
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            btn.onClick,
            goUI.Retry);

        // ── Wire GameOverUI into PlayerHealth ─────────────────────────────────
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                var healthSO = new SerializedObject(health);
                healthSO.FindProperty("gameOverUI").objectReferenceValue = goUI;
                healthSO.ApplyModifiedProperties();
            }
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        Debug.Log("[SurvivorIO] ✅ Game Over screen setup complete.");
    }
}
