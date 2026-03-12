using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FixVisuals
{
    public static void Execute()
    {
        // ── Folder setup ──────────────────────────────────────────────────────
        if (!AssetDatabase.IsValidFolder("Assets/Art"))
            AssetDatabase.CreateFolder("Assets", "Art");
        if (!AssetDatabase.IsValidFolder("Assets/Art/Sprites"))
            AssetDatabase.CreateFolder("Assets/Art", "Sprites");

        // ── 1. Create sprites via .asset (no importer needed) ─────────────────
        var whiteSquare = CreateSpriteAsset("Assets/Art/Sprites/WhiteSquare.asset", 128, 128, 100,
            (x, y) => Color.white);

        var bgSprite = CreateSpriteAsset("Assets/Art/Sprites/BGTile.asset", 64, 64, 64,
            (x, y) => ((x / 16 + y / 16) % 2 == 0)
                ? new Color(0.14f, 0.18f, 0.14f)
                : new Color(0.11f, 0.14f, 0.11f),
            wrapRepeat: true);

        if (whiteSquare == null) { Debug.LogError("[SurvivorIO] WhiteSquare sprite is null!"); return; }
        if (bgSprite    == null) { Debug.LogError("[SurvivorIO] BGTile sprite is null!"); return; }

        Debug.Log($"[SurvivorIO] Sprites ready — WhiteSquare: {whiteSquare.name}, BGTile: {bgSprite.name}");

        // ── 2. Fix Player ─────────────────────────────────────────────────────
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var sr = player.GetComponent<SpriteRenderer>();
            sr.sprite = whiteSquare;
            sr.color = new Color(0.2f, 0.8f, 1f);
            player.transform.localScale = Vector3.one;
        }

        // ── 3. Fix Enemy Prefab ───────────────────────────────────────────────
        const string enemyPrefabPath = "Assets/Prefabs/Enemy.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(enemyPrefabPath) != null)
        {
            using var scope = new PrefabUtility.EditPrefabContentsScope(enemyPrefabPath);
            var sr = scope.prefabContentsRoot.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = whiteSquare;
                sr.color = new Color(1f, 0.25f, 0.25f);
            }
            scope.prefabContentsRoot.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        }

        // ── 4. Background ─────────────────────────────────────────────────────
        var oldBg = GameObject.Find("Background");
        if (oldBg != null) Object.DestroyImmediate(oldBg);

        var bgGO = new GameObject("Background");
        bgGO.transform.position = new Vector3(0f, 0f, 5f);

        var bgSR = bgGO.AddComponent<SpriteRenderer>();
        bgSR.sprite = bgSprite;
        bgSR.drawMode = SpriteDrawMode.Tiled;
        bgSR.size = new Vector2(120f, 120f);
        bgSR.sortingOrder = -100;

        bgGO.AddComponent<BackgroundFollow>();

        // ── Save ──────────────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ Visuals fixed.");
    }

    private static Sprite CreateSpriteAsset(string assetPath, int w, int h, int ppu,
        System.Func<int, int, Color> colorFn, bool wrapRepeat = false)
    {
        // Delete stale asset so we always get a fresh one
        if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) != null)
            AssetDatabase.DeleteAsset(assetPath);

        // Build texture in memory
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode   = wrapRepeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, colorFn(x, y));
        tex.Apply();

        // Persist texture as a Unity asset
        AssetDatabase.CreateAsset(tex, assetPath);

        // Load it back so the sprite gets a proper persistent reference
        var savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        // Create sprite and store as sub-asset on the same file
        var sprite = Sprite.Create(savedTex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), ppu);
        sprite.name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        AssetDatabase.AddObjectToAsset(sprite, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }
}
