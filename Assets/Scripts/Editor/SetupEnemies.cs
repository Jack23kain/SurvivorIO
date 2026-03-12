using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupEnemies
{
    public static void Execute()
    {
        // ── 1. Create Enemy Prefab ────────────────────────────────────────────
        const string prefabDir = "Assets/Prefabs";
        const string prefabPath = "Assets/Prefabs/Enemy.prefab";

        if (!AssetDatabase.IsValidFolder(prefabDir))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        // Build the enemy GameObject
        var enemyGO = new GameObject("Enemy");

        var sr = enemyGO.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        sr.color = new Color(1f, 0.25f, 0.25f, 1f); // red
        enemyGO.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

        var rb = enemyGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        var col = enemyGO.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;

        enemyGO.AddComponent<EnemyController>();
        enemyGO.tag = "Enemy";

        // Save as prefab
        var prefab = PrefabUtility.SaveAsPrefabAsset(enemyGO, prefabPath);
        Object.DestroyImmediate(enemyGO);

        Debug.Log($"[SurvivorIO] Enemy prefab saved to {prefabPath}");

        // ── 2. Add EnemySpawner to scene ──────────────────────────────────────
        var existing = GameObject.Find("EnemySpawner");
        if (existing != null) Object.DestroyImmediate(existing);

        var spawnerGO = new GameObject("EnemySpawner");
        var spawner = spawnerGO.AddComponent<EnemySpawner>();

        var spawnerSO = new SerializedObject(spawner);
        spawnerSO.FindProperty("enemyPrefab").objectReferenceValue = prefab;
        spawnerSO.ApplyModifiedProperties();

        // ── 3. Tag setup — ensure "Enemy" tag exists ──────────────────────────
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        bool hasEnemyTag = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == "Enemy")
            {
                hasEnemyTag = true;
                break;
            }
        }
        if (!hasEnemyTag)
        {
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = "Enemy";
            tagManager.ApplyModifiedProperties();
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ M2 setup complete — EnemySpawner added to scene.");
    }
}
