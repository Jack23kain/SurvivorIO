using UnityEngine;
using UnityEditor;

public class ApplyEnemySprite
{
    public static void Execute()
    {
        // ── 1. Load Enemy.png bytes → Texture2D ──────────────────────────────
        string pngPath = Application.dataPath + "/Art/Sprites/Enemy.png";
        if (!System.IO.File.Exists(pngPath))
        {
            Debug.LogError("[SurvivorIO] Enemy.png not found at: " + pngPath);
            return;
        }

        byte[] bytes = System.IO.File.ReadAllBytes(pngPath);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.Apply();

        Debug.Log($"[SurvivorIO] Enemy.png loaded: {tex.width}x{tex.height}");

        // ── 2. Save as .asset ─────────────────────────────────────────────────
        string assetPath = "Assets/Art/Sprites/Enemy.asset";
        if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) != null)
            AssetDatabase.DeleteAsset(assetPath);

        AssetDatabase.CreateAsset(tex, assetPath);
        var savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        var sprite = Sprite.Create(savedTex,
            new Rect(0, 0, savedTex.width, savedTex.height),
            new Vector2(0.5f, 0.5f), 100f);
        sprite.name = "Enemy";
        AssetDatabase.AddObjectToAsset(sprite, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (enemySprite == null)
        {
            Debug.LogError("[SurvivorIO] Failed to load Enemy sprite.");
            return;
        }

        Debug.Log($"[SurvivorIO] Enemy sprite ready: {enemySprite.name}");

        // ── 3. Apply to Enemy prefab ──────────────────────────────────────────
        const string prefabPath = "Assets/Prefabs/Enemy.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
        {
            Debug.LogError("[SurvivorIO] Enemy.prefab not found.");
            return;
        }

        using var scope = new PrefabUtility.EditPrefabContentsScope(prefabPath);
        var sr = scope.prefabContentsRoot.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = enemySprite;
            sr.color  = Color.white;
        }
        // 666x900 at 100 PPU = ~6.66 x 9 world units — scale to ~0.7 world units wide
        scope.prefabContentsRoot.transform.localScale = new Vector3(0.1f, 0.1f, 1f);

        AssetDatabase.SaveAssets();

        Debug.Log("[SurvivorIO] ✅ Enemy sprite applied to prefab.");
    }
}
