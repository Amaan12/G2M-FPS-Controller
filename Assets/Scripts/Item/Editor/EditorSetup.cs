using UnityEditor;
using UnityEngine;
using System.IO;

public class EditorSetup
{
    [MenuItem("Tools/Setup/Create Template Folders %#t")] // Ctrl/Cmd + Shift + T
    public static void CreateTemplateFolders()
    {
        string[] folders = {
            "Assets/_Project/_Scripts/Default",
            "Assets/_Project/_Scripts/Player",
            "Assets/_Project/_Scripts/AI",

            "Assets/_Project/Prefabs/Gameplay",
            "Assets/_Project/Prefabs/Environment",

            "Assets/_Project/Resources",
            
            "Assets/_Project/Art Assets/Models",
            "Assets/_Project/Art Assets/Sprites/UI",

            "Assets/_Project/Art Assets/VFX",
            "Assets/_Project/Art Assets/UI",
            "Assets/_Project/Art Assets/Audio/SFX",
            "Assets/_Project/Art Assets/Audio/Music",
            "Assets/_Project/Art Assets/Fonts",

            "Assets/_Project/Art Assets/Materials/Physics Materials",
            "Assets/_Project/Art Assets/Materials/Shader Materials",

            "Assets/Imported Assets",
        };

        foreach (string folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Debug.Log("Created folder: " + folder);
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Setup/Create Default Scene Objects")]
    public static void CreateSceneEssentials()
    {
        CreateEmptyObject("Essentials");
        CreateEmptyObject("----------------");
        CreateEmptyObject("Managers");
        CreateEmptyObject("----------------");
        CreateEmptyObject("Canvases");
        CreateEmptyObject("----------------");
        CreateEmptyObject("Environment");
        CreateEmptyObject("----------------");
        CreateEmptyObject("Gameplay");
        CreateEmptyObject("----------------");
    }

    private static void CreateEmptyObject(string name)
    {
        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        Debug.Log("Created GameObject: " + name);
    }
}
