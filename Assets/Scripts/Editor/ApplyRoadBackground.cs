using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ApplyRoadBackground
{
    public static void Execute()
    {
        // ── 1. Load generated PNG bytes and build a Texture2D asset ───────────
        string pngPath = Application.dataPath + "/Art/Sprites/BGRoad.png";
        if (!System.IO.File.Exists(pngPath))
        {
            Debug.LogError("[SurvivorIO] BGRoad.png not found at: " + pngPath);
            return;
        }

        byte[] bytes = System.IO.File.ReadAllBytes(pngPath);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);   // auto-resizes to actual image dimensions
        tex.wrapMode   = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Bilinear;
        tex.Apply();

        Debug.Log($"[SurvivorIO] BGRoad.png loaded: {tex.width}x{tex.height}");

        // ── 2. Save as .asset (same pattern that worked for WhiteSquare) ───────
        string assetPath = "Assets/Art/Sprites/BGRoad.asset";
        if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) != null)
            AssetDatabase.DeleteAsset(assetPath);

        AssetDatabase.CreateAsset(tex, assetPath);

        // Reload the saved texture so the sprite has a persistent reference
        var savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        // 100 PPU → each tile is ~10.24 world units (matches camera ortho size 5 / 10-unit view)
        var sprite = Sprite.Create(savedTex,
            new Rect(0, 0, savedTex.width, savedTex.height),
            new Vector2(0.5f, 0.5f), 100f);
        sprite.name = "BGRoad";
        AssetDatabase.AddObjectToAsset(sprite, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var roadSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (roadSprite == null)
        {
            Debug.LogError("[SurvivorIO] Failed to load BGRoad sprite from asset.");
            return;
        }

        Debug.Log($"[SurvivorIO] BGRoad sprite ready: {roadSprite.name}");

        // ── 3. Rebuild Background GameObject ─────────────────────────────────
        var oldBg = GameObject.Find("Background");
        if (oldBg != null) Object.DestroyImmediate(oldBg);

        var bgGO = new GameObject("Background");
        bgGO.transform.position = new Vector3(0f, 0f, 5f);

        var sr = bgGO.AddComponent<SpriteRenderer>();
        sr.sprite       = roadSprite;
        sr.drawMode     = SpriteDrawMode.Tiled;
        sr.size         = new Vector2(200f, 200f);   // large enough to never see edges
        sr.sortingOrder = -100;

        bgGO.AddComponent<BackgroundFollow>();

        // ── 4. Save scene ─────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ Road background applied.");
    }
}
