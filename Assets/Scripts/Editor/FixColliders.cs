using UnityEngine;
using UnityEditor;

public class FixColliders
{
    public static void Execute()
    {
        // ── Enemy: BoxCollider2D to match sprite (666x900 at 100 PPU = 6.66 x 9.0 local units)
        const string enemyPath = "Assets/Prefabs/Enemy.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(enemyPath) != null)
        {
            using var scope = new PrefabUtility.EditPrefabContentsScope(enemyPath);
            var box = scope.prefabContentsRoot.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                box.size   = new Vector2(6.66f, 9.0f);
                box.offset = Vector2.zero;
            }
            Debug.Log("[SurvivorIO] Enemy BoxCollider2D set to (6.66, 9.0)");
        }

        // ── Dagger: CircleCollider2D radius to match sprite (834x211 at 100 PPU, scale 0.06)
        // Sprite is 8.34 x 2.11 local units; use half the short side as radius
        const string daggerPath = "Assets/Prefabs/Dagger.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(daggerPath) != null)
        {
            using var scope = new PrefabUtility.EditPrefabContentsScope(daggerPath);
            var circle = scope.prefabContentsRoot.GetComponent<CircleCollider2D>();
            if (circle != null)
                circle.radius = 1.5f; // ~0.09 world units — covers the blade tip
            Debug.Log("[SurvivorIO] Dagger CircleCollider2D radius set to 1.5");
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[SurvivorIO] ✅ Colliders fixed.");
    }
}
