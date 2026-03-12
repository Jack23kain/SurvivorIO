using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupGameScene
{
    public static void Execute()
    {
        // ── 1. Camera ────────────────────────────────────────────────────────
        var camGO = GameObject.FindWithTag("MainCamera");
        var cam = camGO.GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.12f, 1f);
        cam.transform.position = new Vector3(0f, 0f, -10f);

        var cameraFollow = camGO.GetComponent<CameraFollow>() ?? camGO.AddComponent<CameraFollow>();

        // ── 2. Player ────────────────────────────────────────────────────────
        // Remove any existing Player
        var existing = GameObject.Find("Player");
        if (existing != null) Object.DestroyImmediate(existing);

        var player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        player.tag = "Player";

        // Square sprite placeholder
        var sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        sr.color = new Color(0.2f, 0.8f, 1f, 1f); // cyan-ish
        player.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        // Rigidbody2D – no gravity, no rotation
        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // PlayerController (joystick wired after canvas is created)
        player.AddComponent<PlayerController>();

        // Wire CameraFollow → Player
        var cfSO = new SerializedObject(cameraFollow);
        cfSO.FindProperty("target").objectReferenceValue = player.transform;
        cfSO.ApplyModifiedProperties();

        // ── 3. Event System ──────────────────────────────────────────────────
        var existingES = Object.FindFirstObjectByType<EventSystem>();
        if (existingES != null) Object.DestroyImmediate(existingES.gameObject);

        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<InputSystemUIInputModule>(); // New Input System compatible

        // ── 4. Canvas ────────────────────────────────────────────────────────
        var existingCanvas = GameObject.Find("UICanvas");
        if (existingCanvas != null) Object.DestroyImmediate(existingCanvas);

        var canvasGO = new GameObject("UICanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── 5. Joystick Area (full-screen transparent hit zone) ───────────────
        var joystickAreaGO = new GameObject("JoystickArea");
        joystickAreaGO.transform.SetParent(canvasGO.transform, false);

        var areaRT = joystickAreaGO.AddComponent<RectTransform>();
        areaRT.anchorMin = Vector2.zero;
        areaRT.anchorMax = Vector2.one;
        areaRT.offsetMin = Vector2.zero;
        areaRT.offsetMax = Vector2.zero;

        // Invisible image needed for raycast hits
        var areaImg = joystickAreaGO.AddComponent<Image>();
        areaImg.color = new Color(0f, 0f, 0f, 0f);

        var fj = joystickAreaGO.AddComponent<FloatingJoystick>();

        // ── 6. Joystick Background ────────────────────────────────────────────
        var bgGO = new GameObject("JoystickBackground");
        bgGO.transform.SetParent(joystickAreaGO.transform, false);

        var bgRT = bgGO.AddComponent<RectTransform>();
        bgRT.sizeDelta = new Vector2(200f, 200f);
        bgRT.anchorMin = bgRT.anchorMax = new Vector2(0f, 0f);
        bgRT.pivot = new Vector2(0.5f, 0.5f);
        bgRT.anchoredPosition = Vector2.zero;

        var bgImg = bgGO.AddComponent<Image>();
        bgImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        bgImg.color = new Color(1f, 1f, 1f, 0.25f);
        bgImg.type = Image.Type.Simple;

        // ── 7. Joystick Handle ────────────────────────────────────────────────
        var handleGO = new GameObject("JoystickHandle");
        handleGO.transform.SetParent(bgGO.transform, false);

        var handleRT = handleGO.AddComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(90f, 90f);
        handleRT.anchorMin = handleRT.anchorMax = new Vector2(0.5f, 0.5f);
        handleRT.pivot = new Vector2(0.5f, 0.5f);
        handleRT.anchoredPosition = Vector2.zero;

        var handleImg = handleGO.AddComponent<Image>();
        handleImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        handleImg.color = new Color(1f, 1f, 1f, 0.75f);

        // ── 8. Wire FloatingJoystick references ───────────────────────────────
        var fjSO = new SerializedObject(fj);
        fjSO.FindProperty("background").objectReferenceValue = bgRT;
        fjSO.FindProperty("handle").objectReferenceValue = handleRT;
        fjSO.ApplyModifiedProperties();

        // ── 9. Wire PlayerController → Joystick + InputActionAsset ───────────
        var pc = player.GetComponent<PlayerController>();
        var inputAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(
            "Assets/InputSystem_Actions.inputactions");
        var pcSO = new SerializedObject(pc);
        pcSO.FindProperty("joystick").objectReferenceValue = fj;
        pcSO.FindProperty("inputActions").objectReferenceValue = inputAsset;
        pcSO.ApplyModifiedProperties();

        // ── 10. Save scene ────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ Game scene setup complete — Player + Floating Joystick ready.");
    }
}
