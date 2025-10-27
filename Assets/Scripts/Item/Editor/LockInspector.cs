using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LockInspector
{
    
    [MenuItem("Edit/Lock Inspector %l")]
    public static void Lock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;

        foreach (var activeEditor in ActiveEditorTracker.sharedTracker.activeEditors)
        {
            if (activeEditor.target is not Transform) continue;
            var transform = (Transform)activeEditor.target;
            var propInfo = transform.GetType().GetProperty("constrainProportionsScale", BindingFlags.NonPublic | BindingFlags.Instance);
            if (propInfo == null) continue;
            var value = (bool)propInfo.GetValue(transform, null);
            propInfo.SetValue(transform, !value, null);
        }

        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }

    [MenuItem("Edit/Lock Inspector %l", true)]
    public static bool Validate()
    {
        return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
    }
}
