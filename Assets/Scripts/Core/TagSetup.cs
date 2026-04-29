using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class TagSetup
{
    static TagSetup()
    {
        AddTag("Goal");
        AddTag("Void");
        AddTag("BarreraA");
        AddTag("BarreraB");
    }

    static void AddTag(string tag)
    {
        var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (asset == null) return;
        var so = new SerializedObject(asset);
        var tags = so.FindProperty("tags");

        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        so.ApplyModifiedProperties();
        so.Update();
    }
}
#endif
