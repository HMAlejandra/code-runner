using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class SetupLevel
{
    static SetupLevel()
    {
        EditorApplication.delayCall += () =>
        {
            AddTag("Goal");
            AddTag("Void");
            // AddTag("Player"); // Comentado para evitar registro duplicado
            AddTag("BarreraA");
            AddTag("BarreraB");
            AssignTagsInScene();
        };
    }

    static void AssignTagsInScene()
    {
        var goal = GameObject.Find("Goal");
        if (goal != null && goal.tag != "Goal")
        {
            goal.tag = "Goal";
            EditorUtility.SetDirty(goal);
        }

        var voidZone = GameObject.Find("VoidZone");
        if (voidZone != null && voidZone.tag != "Void")
        {
            voidZone.tag = "Void";
            EditorUtility.SetDirty(voidZone);
        }

        var robot = GameObject.Find("3D Gum Bot");
        if (robot != null && robot.tag != "Player")
        {
            robot.tag = "Player";
            EditorUtility.SetDirty(robot);
        }

        Debug.Log("[CodeRunner] Tags auto-assigned!");
    }

    static void AddTag(string tag)
    {
        var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (asset == null) return;
        var so = new SerializedObject(asset);
        var tags = so.FindProperty("tags");

        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        so.ApplyModifiedProperties();
    }
}
