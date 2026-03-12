using UnityEngine;
using UnityEditor;

public class FixEnemyCollider
{
    public static void Execute()
    {
        const string enemyPath = "Assets/Prefabs/Enemy.prefab";
        using var scope = new PrefabUtility.EditPrefabContentsScope(enemyPath);
        var box = scope.prefabContentsRoot.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.isTrigger = true;
            Debug.Log("[SurvivorIO] Enemy BoxCollider2D set to trigger.");
        }
        AssetDatabase.SaveAssets();
        Debug.Log("[SurvivorIO] ✅ Enemy collider fixed.");
    }
}
