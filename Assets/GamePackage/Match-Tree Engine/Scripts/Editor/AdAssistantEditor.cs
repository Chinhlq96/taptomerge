using UnityEngine;
using UnityEditor;
using ChartboostSDK;
using EditorUtils;

[CustomEditor(typeof(AdAssistant))]
public class AdAssistantEditor : MetaEditor {

    AdAssistant main;

    public override Object FindTarget() {
        return AdAssistant.Instance;
    }

    public override void OnInspectorGUI() {
        if (!metaTarget) {
            EditorGUILayout.HelpBox("AdAssistant is missing", MessageType.Error);
            return;
        }
        main = (AdAssistant) metaTarget;
        Undo.RecordObject(main, "Ad settings changed");
        EditorGUILayout.Space();
        main.ad_show_min_delay = Mathf.RoundToInt(EditorGUILayout.Slider("Ad Minimal Delay (" + main.ad_show_min_delay.ToString() + " min.)", main.ad_show_min_delay, 0, 59));

        if (main.allNetworks.Contains(AdAssistant.AdNetwork.UnityAds)) {
            GUILayout.Label("Unity Ads", EditorStyles.boldLabel);
            if (GUILayout.Button("Turn off", GUILayout.Width(60)))
                main.allNetworks.Remove(AdAssistant.AdNetwork.UnityAds);
            EditorGUILayout.HelpBox("AppIDs for Unity Ads you can edit in the Services view (Ctrl+0). You need to choose 'Ads' service and in the 'Advanced' area you will find AppIDs fields.", MessageType.Info);
        } else {
            if (GUILayout.Button("Turn on Unity Ads", GUILayout.Width(150)))
                main.allNetworks.Add(AdAssistant.AdNetwork.UnityAds);
        }

        GUILayout.Space(20);

        if (main.allNetworks.Contains(AdAssistant.AdNetwork.AdMob)) {
            GUILayout.Label("AdMob", EditorStyles.boldLabel);
            if (GUILayout.Button("Turn off", GUILayout.Width(60)))
                main.allNetworks.Remove(AdAssistant.AdNetwork.AdMob);
            main.AdMob_Interstitial_Android = EditorGUILayout.TextField("Android Interstitial ID", main.AdMob_Interstitial_Android);
            main.AdMob_Interstitial_iOS = EditorGUILayout.TextField("iOS Interstitial ID", main.AdMob_Interstitial_iOS);
        } else {
            if (GUILayout.Button("Turn on AdMob", GUILayout.Width(150)))
                main.allNetworks.Add(AdAssistant.AdNetwork.AdMob);
        }

        GUILayout.Space(20);

        if (main.allNetworks.Contains(AdAssistant.AdNetwork.Chartboost)) {
            GUILayout.Label("Chartboost", EditorStyles.boldLabel);
            if (GUILayout.Button("Turn off", GUILayout.Width(60)))
                main.allNetworks.Remove(AdAssistant.AdNetwork.Chartboost);
            if (GUILayout.Button("Edit", GUILayout.Width(60)))
                CBSettings.Edit();
        } else {
            if (GUILayout.Button("Turn on Chartboost", GUILayout.Width(150)))
                main.allNetworks.Add(AdAssistant.AdNetwork.Chartboost);
        }

        GUILayout.Space(20);

        if (main.allNetworks.Contains(AdAssistant.AdNetwork.AdColony)) {
            GUILayout.Label("AdColony", EditorStyles.boldLabel);
            if (GUILayout.Button("Turn off", GUILayout.Width(60)))
                main.allNetworks.Remove(AdAssistant.AdNetwork.AdColony);
            main.AdColony_AppID_Android = EditorGUILayout.TextField("Android AppID", main.AdColony_AppID_Android);
            main.AdColony_ZoneID_Android = EditorGUILayout.TextField("Android ZoneID", main.AdColony_ZoneID_Android);
            main.AdColony_AppID_iOS = EditorGUILayout.TextField("iOS AppID", main.AdColony_AppID_iOS);
            main.AdColony_ZoneID_iOS = EditorGUILayout.TextField("iOS ZoneID", main.AdColony_ZoneID_iOS);
        } else {
            if (GUILayout.Button("Turn on AdColony", GUILayout.Width(150)))
                main.allNetworks.Add(AdAssistant.AdNetwork.AdColony);
        }
    }
}
