using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FontsEditor : EditorWindow
{

    static GameObject targetPrefabs;
    static Font targetFont;

    [MenuItem("Tools/UI/Change Font")]
    public static void ShowWindow()
    {
        GetWindow<FontsEditor>().Show();
    }

    public void OnGUI()
    {
        GUILayout.Space(10);
        targetPrefabs = (GameObject)EditorGUILayout.ObjectField("Target GameObject", targetPrefabs, typeof(GameObject), true);
        GUILayout.Space(10);
        targetFont = (Font)EditorGUILayout.ObjectField("Target Font", targetFont, typeof(Font), true);
        GUILayout.Space(10);
        if (GUILayout.Button("Change"))
            ModifyFont();
    }

    private void ModifyFont()
    {
        int times = 0;
        if (targetPrefabs != null && targetFont != null)
        {
            var texts = targetPrefabs.transform.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                text.font = targetFont;
                times++;
            }
        }
        Debug.Log("Totally change " + times + " places.");
    }
}
