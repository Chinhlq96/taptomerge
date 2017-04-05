using Google.JarResolver;
using UnityEditor;
using UnityEngine;

/// AdMob dependencies file.
[InitializeOnLoad]
public static class AdMobDependencies
{
    /// The name of the plugin.
    private static readonly string PluginName = "AdMobUnity";

    /// Initializes static members of the class.
    static AdMobDependencies()
    {
        PlayServicesSupport svcSupport =
            PlayServicesSupport.CreateInstance(PluginName, EditorPrefs.GetString("AndroidSdkRoot"),
                    "ProjectSettings");

        svcSupport.DependOn("com.google.android.gms", "play-services-ads", "LATEST");

        // Marshmallow permissions requires app-compat.
        try {
            svcSupport.DependOn("com.android.support", "appcompat-v7", "23.1.0+");
        } catch (System.Exception e) {
            Debug.LogWarning(e.Message);
        }
    }
}
