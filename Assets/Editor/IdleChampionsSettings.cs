#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class IdleChampionsSettings {
    static IdleChampionsSettings() {
        SetProjectSettings();
    }
    
    static void SetProjectSettings() {
        // Set package name (Android/iOS)
        if (PlayerSettings.applicationIdentifier != "com.yourcompany.idlechampionsrp") {
            PlayerSettings.applicationIdentifier = "com.yourcompany.idlechampionsrp";
            Debug.Log("Set package name: com.yourcompany.idlechampionsrp");
        }
        
        // Set product name
        if (PlayerSettings.productName != "Idle Champions: Realms of Power") {
            PlayerSettings.productName = "Idle Champions: Realms of Power";
            Debug.Log("Set product name: Idle Champions: Realms of Power");
        }
        
        // Set company name (optional)
        if (PlayerSettings.companyName != "YourCompany") {
            PlayerSettings.companyName = "YourCompany";
        }
        
        // Android specific settings
        #if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22; // Android 5.1
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        
        // Orientation
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        
        // Scripting backend IL2CPP for performance
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        
        Debug.Log("Android settings configured for Idle Champions: Realms of Power");
        #endif
        
        // iOS specific settings (if needed later)
        #if UNITY_IOS
        PlayerSettings.iOS.buildNumber = "1";
        #endif
        
        AssetDatabase.SaveAssets();
        Debug.Log("✅ Idle Champions: Realms of Power — Project settings applied!");
    }
    
    [MenuItem("IdleChampions/Realms of Power/Apply Project Settings")]
    static void ManualApply() {
        SetProjectSettings();
        EditorUtility.DisplayDialog("Settings Applied", 
            "Project settings updated for Idle Champions: Realms of Power\n\n" +
            "Package: com.yourcompany.idlechampionsrp\n" +
            "Product: Idle Champions: Realms of Power\n" +
            "Android: Portrait, IL2CPP, ARMv7+ARM64", "OK");
    }
}
#endif
