using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ApplyPlayerSprite
{
    public static void Execute()
    {
        // ── 1. Load Player.png bytes → Texture2D ──────────────────────────────
        string pngPath = Application.dataPath + "/Art/Sprites/Player.png";
        if (!System.IO.File.Exists(pngPath))
        {
            Debug.LogError("[SurvivorIO] Player.png not found at: " + pngPath);
            return;
        }

        byte[] bytes = System.IO.File.ReadAllBytes(pngPath);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.Apply();

        Debug.Log($"[SurvivorIO] Player.png loaded: {tex.width}x{tex.height}");

        // ── 2. Save as .asset ─────────────────────────────────────────────────
        string assetPath = "Assets/Art/Sprites/Player.asset";
        if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) != null)
            AssetDatabase.DeleteAsset(assetPath);

        AssetDatabase.CreateAsset(tex, assetPath);
        var savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        // 100 PPU → character ~8 world units tall (796x947 → ~9.5 units, scale down in scene)
        var sprite = Sprite.Create(savedTex,
            new Rect(0, 0, savedTex.width, savedTex.height),
            new Vector2(0.5f, 0.5f), 100f);
        sprite.name = "Player";
        AssetDatabase.AddObjectToAsset(sprite, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (playerSprite == null)
        {
            Debug.LogError("[SurvivorIO] Failed to load Player sprite.");
            return;
        }

        Debug.Log($"[SurvivorIO] Player sprite ready: {playerSprite.name}");

        // ── 3. Apply to Player GameObject ─────────────────────────────────────
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var sr = player.GetComponent<SpriteRenderer>();
            sr.sprite = playerSprite;
            sr.color  = Color.white; // remove the cyan tint
            // Scale to ~1.2 world units wide (796px / 100ppu = 7.96 units, so scale to 0.15)
            player.transform.localScale = new Vector3(0.15f, 0.15f, 1f);
        }
        else
        {
            Debug.LogError("[SurvivorIO] Player GameObject not found.");
        }

        // ── 4. Save scene ─────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ Player sprite applied.");
    }
}
