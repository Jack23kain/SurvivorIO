using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FixJoystickAnchor
{
    public static void Execute()
    {
        var bgGO = GameObject.Find("UICanvas/JoystickArea/JoystickBackground");
        if (bgGO == null)
        {
            Debug.LogError("[SurvivorIO] JoystickBackground not found!");
            return;
        }

        var rt = bgGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SurvivorIO] ✅ JoystickBackground anchor fixed to center.");
    }
}
