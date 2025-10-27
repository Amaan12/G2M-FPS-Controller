using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FastEnterPlayModeEnabler
{
    private const string EditorPrefKey = "FastEnterPlayModeConfigured";

    static FastEnterPlayModeEnabler()
    {
        // Only apply once
        if (!EditorPrefs.GetBool(EditorPrefKey, false))
        {
            ApplyFastEnterPlayMode();
            EditorPrefs.SetBool(EditorPrefKey, true);
            Debug.Log("✅ Fast Enter Play Mode applied automatically (no reload domain or scene).");
        }
    }

    [MenuItem("Tools/No Reload Domain or Scene/Apply Fast Enter Play Mode")]
    public static void ApplyFastEnterPlayMode()
    {
        EditorSettings.enterPlayModeOptionsEnabled = true;
        EditorSettings.enterPlayModeOptions =
            EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;

        Debug.Log("⚡ Fast Enter Play Mode enabled via menu (domain + scene reload disabled).");
    }

    [MenuItem("Tools/No Reload Domain or Scene/Reset 'Applied' Flag")]
    public static void ResetApplyFlag()
    {
        EditorPrefs.DeleteKey(EditorPrefKey);
        Debug.Log("🔁 FastEnterPlayModeConfigured flag reset. Will reapply automatically on next reload.");
    }
}
