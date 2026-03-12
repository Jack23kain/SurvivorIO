using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupAutoAttack
{
    public static void Execute()
    {
        // ── 1. Build Dagger sprite asset ──────────────────────────────────────
        string pngPath = Application.dataPath + "/Art/Sprites/Dagger.png";
        if (!System.IO.File.Exists(pngPath))
        {
            Debug.LogError("[SurvivorIO] Dagger.png not found at: " + pngPath);
            return;
        }

        byte[] bytes = System.IO.File.ReadAllBytes(pngPath);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.Apply();

        string assetPath = "Assets/Art/Sprites/Dagger.asset";
        if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) != null)
            AssetDatabase.DeleteAsset(assetPath);

        AssetDatabase.CreateAsset(tex, assetPath);
        var savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        var sprite = Sprite.Create(savedTex,
            new Rect(0, 0, savedTex.width, savedTex.height),
            new Vector2(0.5f, 0.5f), 100f);
        sprite.name = "Dagger";
        AssetDatabase.AddObjectToAsset(sprite, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var daggerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (daggerSprite == null)
        {
            Debug.LogError("[SurvivorIO] Failed to load Dagger sprite.");
            return;
        }
        Debug.Log($"[SurvivorIO] Dagger sprite ready: {daggerSprite.name} ({tex.width}x{tex.height})");

        // ── 2. Create Dagger prefab ───────────────────────────────────────────
        const string prefabPath = "Assets/Prefabs/Dagger.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            AssetDatabase.DeleteAsset(prefabPath);

        var daggerGO = new GameObject("Dagger");

        var sr = daggerGO.AddComponent<SpriteRenderer>();
        sr.sprite       = daggerSprite;
        sr.sortingOrder = 1;
        // 834x211 at 100 PPU = 8.34 x 2.11 world units — scale to ~0.5 units wide
        daggerGO.transform.localScale = new Vector3(0.06f, 0.06f, 1f);

        var rb = daggerGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var col = daggerGO.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        daggerGO.AddComponent<Dagger>();

        PrefabUtility.SaveAsPrefabAsset(daggerGO, prefabPath);
        Object.DestroyImmediate(daggerGO);
        AssetDatabase.Refresh();

        var daggerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Debug.Log("[SurvivorIO] Dagger prefab created.");

        // ── 3. Add AutoAttack to Player, assign prefab ────────────────────────
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[SurvivorIO] Player not found in scene.");
            return;
        }

        var autoAttack = player.GetComponent<AutoAttack>();
        if (autoAttack == null)
            autoAttack = player.AddComponent<AutoAttack>();

        var so = new SerializedObject(autoAttack);
        so.FindProperty("daggerPrefab").objectReferenceValue = daggerPrefab;
        so.ApplyModifiedProperties();

        // ── 4. Make sure Enemy collider is NOT a trigger (so dagger can hit it)
        const string enemyPrefabPath = "Assets/Prefabs/Enemy.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(enemyPrefabPath) != null)
        {
            using var scope = new PrefabUtility.EditPrefabContentsScope(enemyPrefabPath);
            var box = scope.prefabContentsRoot.GetComponent<BoxCollider2D>();
            if (box != null) box.isTrigger = false;
        }

        // ── 5. Save scene ─────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ Auto-attack setup complete.");
    }
}
