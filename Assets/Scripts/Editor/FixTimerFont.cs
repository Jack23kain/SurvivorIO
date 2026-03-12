using UnityEngine;
using UnityEditor;
using TMPro;

public class FixTimerFont
{
    public static void Execute()
    {
        // Find the default TMP font asset
        var guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        TMP_FontAsset font = null;
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var candidate = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (candidate != null)
            {
                font = candidate;
                Debug.Log($"[SurvivorIO] Using font: {font.name} at {path}");
                break;
            }
        }

        if (font == null)
        {
            Debug.LogError("[SurvivorIO] No TMP_FontAsset found in project.");
            return;
        }

        // Find TimerText in the scene
        var timerBar = GameObject.Find("UICanvas/TimerBar");
        if (timerBar == null) { Debug.LogError("[SurvivorIO] TimerBar not found."); return; }

        var tmp = timerBar.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp == null) { Debug.LogError("[SurvivorIO] TimerText TMP not found."); return; }

        tmp.font = font;

        EditorUtility.SetDirty(tmp);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ Timer font assigned.");
    }
}
