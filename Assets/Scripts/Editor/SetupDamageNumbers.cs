using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupDamageNumbers
{
    public static void Execute()
    {
        // ── Find a TMP font ───────────────────────────────────────────────────
        TMP_FontAsset font = null;
        foreach (var guid in AssetDatabase.FindAssets("t:TMP_FontAsset"))
        {
            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                AssetDatabase.GUIDToAssetPath(guid));
            if (font != null) break;
        }
        if (font == null) { Debug.LogError("[SurvivorIO] No TMP font found."); return; }

        // ── Create DamageNumber prefab ────────────────────────────────────────
        const string prefabPath = "Assets/Prefabs/DamageNumber.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            AssetDatabase.DeleteAsset(prefabPath);

        var go = new GameObject("DamageNumber");

        var tmp = go.AddComponent<TextMeshPro>();
        tmp.text      = "10";
        tmp.font      = font;
        tmp.fontSize  = 4f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color     = new Color(1f, 0.95f, 0.2f); // bright yellow
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 10;

        go.AddComponent<DamageNumber>();
        // Scale down so text sits nicely in the world (TMP world space units)
        go.transform.localScale = Vector3.one * 0.25f;

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);
        AssetDatabase.Refresh();
        Debug.Log("[SurvivorIO] DamageNumber prefab created.");

        // ── Wire prefab into AutoAttack on Player ─────────────────────────────
        var player = GameObject.FindWithTag("Player");
        if (player == null) { Debug.LogError("[SurvivorIO] Player not found."); return; }

        var autoAttack = player.GetComponent<AutoAttack>();
        if (autoAttack == null) { Debug.LogError("[SurvivorIO] AutoAttack not found."); return; }

        var so = new SerializedObject(autoAttack);
        so.FindProperty("damageNumberPrefab").objectReferenceValue = prefab;
        so.ApplyModifiedProperties();

        // ── Update Enemy prefab HP to 10 ──────────────────────────────────────
        const string enemyPath = "Assets/Prefabs/Enemy.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(enemyPath) != null)
        {
            using var scope = new PrefabUtility.EditPrefabContentsScope(enemyPath);
            var ec = scope.prefabContentsRoot.GetComponent<EnemyController>();
            if (ec != null)
            {
                var eso = new SerializedObject(ec);
                eso.FindProperty("maxHp").intValue = 10;
                eso.ApplyModifiedProperties();
            }
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        Debug.Log("[SurvivorIO] ✅ Damage numbers setup complete.");
    }
}
