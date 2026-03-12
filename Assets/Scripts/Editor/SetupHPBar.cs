using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupHPBar
{
    public static void Execute()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) { Debug.LogError("[SurvivorIO] Player not found."); return; }

        // Load the white square sprite we already have
        var whiteSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/WhiteSquare.asset");
        if (whiteSprite == null) { Debug.LogError("[SurvivorIO] WhiteSquare.asset not found."); return; }

        // ── Remove old HP bar if re-running ──────────────────────────────────
        var oldBar = player.transform.Find("HPBar");
        if (oldBar != null) Object.DestroyImmediate(oldBar.gameObject);

        // ── HPBar root (holds the component, positioned below player) ─────────
        // Player sprite: 947px tall / 100PPU = 9.47 local units → bottom at -4.74
        // Place bar slightly below feet
        var barRoot = new GameObject("HPBar");
        barRoot.transform.SetParent(player.transform, false);
        barRoot.transform.localPosition = new Vector3(0f, -5.8f, -0.1f);

        // ── Background (dark) ─────────────────────────────────────────────────
        var bg = new GameObject("Background");
        bg.transform.SetParent(barRoot.transform, false);
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localScale    = new Vector3(8f, 0.85f, 1f);

        var bgSR = bg.AddComponent<SpriteRenderer>();
        bgSR.sprite       = whiteSprite;
        bgSR.color        = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        bgSR.sortingOrder = 2;

        // ── Fill (green) ──────────────────────────────────────────────────────
        var fill = new GameObject("Fill");
        fill.transform.SetParent(barRoot.transform, false);
        fill.transform.localPosition = Vector3.zero;
        fill.transform.localScale    = new Vector3(8f, 0.85f, 1f);

        var fillSR = fill.AddComponent<SpriteRenderer>();
        fillSR.sprite       = whiteSprite;
        fillSR.color        = new Color(0.2f, 0.85f, 0.25f); // green
        fillSR.sortingOrder = 3;

        // ── Wire PlayerHPBar component ────────────────────────────────────────
        var hpBarComp = barRoot.AddComponent<PlayerHPBar>();
        var soBar = new SerializedObject(hpBarComp);
        soBar.FindProperty("fill").objectReferenceValue = fill.transform;
        soBar.ApplyModifiedProperties();

        // ── Wire PlayerHealth → HPBar ─────────────────────────────────────────
        var health = player.GetComponent<PlayerHealth>();
        if (health == null) health = player.AddComponent<PlayerHealth>();

        var soHealth = new SerializedObject(health);
        soHealth.FindProperty("hpBar").objectReferenceValue = hpBarComp;
        soHealth.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        Debug.Log("[SurvivorIO] ✅ HP bar setup complete.");
    }
}
