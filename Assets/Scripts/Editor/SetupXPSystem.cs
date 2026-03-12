using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SetupXPSystem
{
    public static void Execute()
    {
        // ── Find TMP font ─────────────────────────────────────────────────────
        TMP_FontAsset font = null;
        foreach (var guid in AssetDatabase.FindAssets("t:TMP_FontAsset"))
        {
            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guid));
            if (font != null) break;
        }
        if (font == null) { Debug.LogError("[SurvivorIO] No TMP font found."); return; }

        var whiteSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/WhiteSquare.asset");

        // ══ 1. XP GEM PREFAB ═════════════════════════════════════════════════
        var gemSprite = BuildSpriteAsset("Assets/Art/Sprites/XPGem.png",
            "Assets/Art/Sprites/XPGem.asset", 100f);

        const string gemPrefabPath = "Assets/Prefabs/XPGem.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(gemPrefabPath) != null)
            AssetDatabase.DeleteAsset(gemPrefabPath);

        var gemGO = new GameObject("XPGem");
        var gemSR = gemGO.AddComponent<SpriteRenderer>();
        gemSR.sprite       = gemSprite;
        gemSR.sortingOrder = 2;
        gemGO.transform.localScale = Vector3.one * 0.05f;
        gemGO.AddComponent<XPGem>();

        var gemPrefab = PrefabUtility.SaveAsPrefabAsset(gemGO, gemPrefabPath);
        Object.DestroyImmediate(gemGO);
        Debug.Log("[SurvivorIO] XPGem prefab created.");

        // ── Wire gem prefab into Enemy prefab ─────────────────────────────────
        const string enemyPath = "Assets/Prefabs/Enemy.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(enemyPath) != null)
        {
            using var scope = new PrefabUtility.EditPrefabContentsScope(enemyPath);
            var ec = scope.prefabContentsRoot.GetComponent<EnemyController>();
            if (ec != null)
            {
                var so = new SerializedObject(ec);
                so.FindProperty("xpGemPrefab").objectReferenceValue = gemPrefab;
                so.ApplyModifiedProperties();
            }
        }
        Debug.Log("[SurvivorIO] XPGem wired to Enemy prefab.");

        // ══ 2. XP BAR UI ══════════════════════════════════════════════════════
        var canvasGO = GameObject.Find("UICanvas");
        if (canvasGO == null) { Debug.LogError("[SurvivorIO] UICanvas not found."); return; }

        // Remove old XP bar
        var oldXP = canvasGO.transform.Find("XPBar");
        if (oldXP != null) Object.DestroyImmediate(oldXP.gameObject);

        // XP bar: thin strip at the very top of screen
        var xpBarRoot = new GameObject("XPBar");
        xpBarRoot.transform.SetParent(canvasGO.transform, false);

        var xpBarRT = xpBarRoot.AddComponent<RectTransform>();
        xpBarRT.anchorMin        = new Vector2(0f, 1f);
        xpBarRT.anchorMax        = new Vector2(1f, 1f);
        xpBarRT.pivot            = new Vector2(0.5f, 1f);
        xpBarRT.anchoredPosition = Vector2.zero;
        xpBarRT.sizeDelta        = new Vector2(0f, 16f);

        // Background
        var xpBG = new GameObject("Background");
        xpBG.transform.SetParent(xpBarRoot.transform, false);
        var xpBGRT = xpBG.AddComponent<RectTransform>();
        xpBGRT.anchorMin = Vector2.zero; xpBGRT.anchorMax = Vector2.one;
        xpBGRT.offsetMin = Vector2.zero; xpBGRT.offsetMax = Vector2.zero;
        var xpBGImg = xpBG.AddComponent<Image>();
        xpBGImg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // Fill (uses fillAmount)
        var xpFill = new GameObject("Fill");
        xpFill.transform.SetParent(xpBarRoot.transform, false);
        var xpFillRT = xpFill.AddComponent<RectTransform>();
        xpFillRT.anchorMin = Vector2.zero; xpFillRT.anchorMax = Vector2.one;
        xpFillRT.offsetMin = Vector2.zero; xpFillRT.offsetMax = Vector2.zero;
        var xpFillImg = xpFill.AddComponent<Image>();
        xpFillImg.color      = new Color(0.15f, 0.6f, 1f); // blue-cyan
        xpFillImg.type       = Image.Type.Filled;
        xpFillImg.fillMethod = Image.FillMethod.Horizontal;
        xpFillImg.fillAmount = 0f;

        var xpBarComp = xpBarRoot.AddComponent<XPBar>();
        var xpBarSO = new SerializedObject(xpBarComp);
        xpBarSO.FindProperty("fill").objectReferenceValue = xpFillImg;
        xpBarSO.ApplyModifiedProperties();

        // ══ 3. LEVEL-UP UI ════════════════════════════════════════════════════
        var oldLU = canvasGO.transform.Find("LevelUpPanel");
        if (oldLU != null) Object.DestroyImmediate(oldLU.gameObject);

        // Full-screen overlay panel
        var luPanel = new GameObject("LevelUpPanel");
        luPanel.transform.SetParent(canvasGO.transform, false);
        var luRT = luPanel.AddComponent<RectTransform>();
        luRT.anchorMin = Vector2.zero; luRT.anchorMax = Vector2.one;
        luRT.offsetMin = Vector2.zero; luRT.offsetMax = Vector2.zero;
        var luBG = luPanel.AddComponent<Image>();
        luBG.color = new Color(0f, 0f, 0f, 0.7f);

        // "Level Up!" title
        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(luPanel.transform, false);
        var titleRT = titleGO.AddComponent<RectTransform>();
        titleRT.anchorMin        = new Vector2(0f, 0.75f);
        titleRT.anchorMax        = new Vector2(1f, 1f);
        titleRT.offsetMin        = Vector2.zero;
        titleRT.offsetMax        = Vector2.zero;
        var titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
        titleTMP.text      = "LEVEL UP!";
        titleTMP.font      = font;
        titleTMP.fontSize  = 72;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.color     = new Color(1f, 0.9f, 0.1f);
        titleTMP.alignment = TextAlignmentOptions.Center;

        // 3 skill cards
        var cardButtons  = new Button[3];
        var cardTitles   = new TextMeshProUGUI[3];
        var cardDescs    = new TextMeshProUGUI[3];

        float[] xPositions = { -380f, 0f, 380f };
        for (int i = 0; i < 3; i++)
        {
            var card = new GameObject($"Card{i}");
            card.transform.SetParent(luPanel.transform, false);
            var cardRT = card.AddComponent<RectTransform>();
            cardRT.anchorMin        = new Vector2(0.5f, 0.5f);
            cardRT.anchorMax        = new Vector2(0.5f, 0.5f);
            cardRT.pivot            = new Vector2(0.5f, 0.5f);
            cardRT.anchoredPosition = new Vector2(xPositions[i], -40f);
            cardRT.sizeDelta        = new Vector2(320f, 380f);

            var cardImg = card.AddComponent<Image>();
            cardImg.color = new Color(0.15f, 0.18f, 0.25f, 1f);

            var btn = card.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.25f, 0.3f, 0.45f);
            colors.pressedColor     = new Color(0.1f, 0.12f, 0.18f);
            btn.colors = colors;
            cardButtons[i] = btn;

            // Title
            var ctGO = new GameObject("Title");
            ctGO.transform.SetParent(card.transform, false);
            var ctRT = ctGO.AddComponent<RectTransform>();
            ctRT.anchorMin        = new Vector2(0f, 0.6f);
            ctRT.anchorMax        = new Vector2(1f, 1f);
            ctRT.offsetMin        = new Vector2(10f, 0f);
            ctRT.offsetMax        = new Vector2(-10f, 0f);
            var ctTMP = ctGO.AddComponent<TextMeshProUGUI>();
            ctTMP.font      = font;
            ctTMP.fontSize  = 32;
            ctTMP.fontStyle = FontStyles.Bold;
            ctTMP.color     = Color.white;
            ctTMP.alignment = TextAlignmentOptions.Center;
            cardTitles[i]   = ctTMP;

            // Description
            var cdGO = new GameObject("Desc");
            cdGO.transform.SetParent(card.transform, false);
            var cdRT = cdGO.AddComponent<RectTransform>();
            cdRT.anchorMin        = new Vector2(0f, 0.1f);
            cdRT.anchorMax        = new Vector2(1f, 0.6f);
            cdRT.offsetMin        = new Vector2(10f, 0f);
            cdRT.offsetMax        = new Vector2(-10f, 0f);
            var cdTMP = cdGO.AddComponent<TextMeshProUGUI>();
            cdTMP.font      = font;
            cdTMP.fontSize  = 22;
            cdTMP.color     = new Color(0.8f, 0.8f, 0.8f);
            cdTMP.alignment = TextAlignmentOptions.Center;
            cdTMP.textWrappingMode = TextWrappingModes.Normal;
            cardDescs[i]    = cdTMP;
        }

        // LevelUpUI component on the panel
        var luComp = luPanel.AddComponent<LevelUpUI>();
        var luSO = new SerializedObject(luComp);
        luSO.FindProperty("panel").objectReferenceValue = luPanel;
        luSO.FindProperty("cardButtons").arraySize  = 3;
        luSO.FindProperty("cardTitles").arraySize   = 3;
        luSO.FindProperty("cardDescs").arraySize    = 3;
        for (int i = 0; i < 3; i++)
        {
            luSO.FindProperty("cardButtons").GetArrayElementAtIndex(i).objectReferenceValue = cardButtons[i];
            luSO.FindProperty("cardTitles").GetArrayElementAtIndex(i).objectReferenceValue  = cardTitles[i];
            luSO.FindProperty("cardDescs").GetArrayElementAtIndex(i).objectReferenceValue   = cardDescs[i];
        }
        luSO.ApplyModifiedProperties();

        // Wire button clicks — index 0/1/2
        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            cardButtons[idx].onClick.AddListener(() => luComp.SelectCard(idx));
        }

        // ══ 4. XPManager on Player ════════════════════════════════════════════
        var player = GameObject.FindWithTag("Player");
        if (player == null) { Debug.LogError("[SurvivorIO] Player not found."); return; }

        var xpMgr = player.GetComponent<XPManager>();
        if (xpMgr == null) xpMgr = player.AddComponent<XPManager>();
        var xpMgrSO = new SerializedObject(xpMgr);
        xpMgrSO.FindProperty("xpBar").objectReferenceValue     = xpBarComp;
        xpMgrSO.FindProperty("levelUpUI").objectReferenceValue = luComp;
        xpMgrSO.ApplyModifiedProperties();

        // ── Save ──────────────────────────────────────────────────────────────
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        Debug.Log("[SurvivorIO] ✅ XP system setup complete.");
    }

    private static Sprite BuildSpriteAsset(string pngPath, string assetPath, float ppu)
    {
        string fullPath = Application.dataPath + pngPath.Replace("Assets", "");
        if (!System.IO.File.Exists(fullPath))
        {
            Debug.LogError("[SurvivorIO] PNG not found: " + fullPath);
            return null;
        }
        byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.Apply();

        if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) != null)
            AssetDatabase.DeleteAsset(assetPath);

        AssetDatabase.CreateAsset(tex, assetPath);
        var savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        var sprite = Sprite.Create(savedTex, new Rect(0, 0, savedTex.width, savedTex.height),
            new Vector2(0.5f, 0.5f), ppu);
        sprite.name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        AssetDatabase.AddObjectToAsset(sprite, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }
}
