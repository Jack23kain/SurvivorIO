using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupPlayerHealth
{
    public static void Execute()
    {
        // ── Add PlayerHealth + CircleCollider2D to Player ─────────────────────
        var player = GameObject.FindWithTag("Player");
        if (player == null) { Debug.LogError("[SurvivorIO] Player not found."); return; }

        if (player.GetComponent<PlayerHealth>() == null)
            player.AddComponent<PlayerHealth>();

        // Add a collider so enemies can physically detect contact
        if (player.GetComponent<Collider2D>() == null)
        {
            var col = player.AddComponent<CircleCollider2D>();
            // Player sprite: 796x947 at 100PPU, scale 0.075 → ~0.6 world units wide
            // Local radius: 0.3 world / 0.075 scale = 4.0 local units
            col.radius = 4f;
        }

        Debug.Log("[SurvivorIO] PlayerHealth and collider added to Player.");

        // ── Update Enemy prefab: maxHp = 9 ───────────────────────────────────
        const string enemyPath = "Assets/Prefabs/Enemy.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(enemyPath) != null)
        {
            using var scope = new PrefabUtility.EditPrefabContentsScope(enemyPath);
            var ec = scope.prefabContentsRoot.GetComponent<EnemyController>();
            if (ec != null)
            {
                var so = new SerializedObject(ec);
                so.FindProperty("maxHp").intValue = 9;
                so.ApplyModifiedProperties();
            }
        }
        Debug.Log("[SurvivorIO] Enemy prefab maxHp set to 9.");

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        Debug.Log("[SurvivorIO] ✅ Player health setup complete.");
    }
}
