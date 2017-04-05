using UnityEngine;
using System.Collections;
using EditorUtils;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class NativeShareEditor : MetaEditor {
    public override Object FindTarget() {
        return NativeShareMgr.Instance;
    }

    public override void OnInspectorGUI() {
        if (!metaTarget) {
            EditorGUILayout.HelpBox("SessionAssistant is missing", MessageType.Error);
            return;
        }

        NativeShareMgr main = (NativeShareMgr) metaTarget;
        Undo.RecordObject(main, "");



        EditorGUILayout.Space();
        main.subject = EditorGUILayout.TextField("Subject", main.subject);

        EditorGUILayout.Space();
        main.urlAndroid = EditorGUILayout.TextField("URL Android", main.urlAndroid);

        EditorGUILayout.Space();
        main.urlIOS = EditorGUILayout.TextField("URL IOS", main.urlIOS);
        EditorGUILayout.Space();
        GUILayout.Label("Text Share", GUILayout.Width(300));
        main.textShare = EditorGUILayout.TextArea(main.textShare, GUI.skin.textArea, GUILayout.ExpandWidth(true), GUILayout.Height(300));

     
        EditorGUILayout.Space();
    }
}

public class ProjectParametersEditor : MetaEditor {
    public override Object FindTarget() {
        return ProjectParameters.Instance;
    }

    public override void OnInspectorGUI() {
        if (!metaTarget) {
            EditorGUILayout.HelpBox("SessionAssistant is missing", MessageType.Error);
            return;
        }

        ProjectParameters main = (ProjectParameters) metaTarget;
        Undo.RecordObject(main, "");

       

        EditorGUILayout.Space();
        main.ios_AppID = EditorGUILayout.TextField("iOS AppID", main.ios_AppID);

      
        EditorGUILayout.Space();
    }
}

public class ServiceMgrEditor : MetaEditor {
    public override Object FindTarget() {
        return ServiceAssistant.Instance;
    }

    public override void OnInspectorGUI() {
        if (!metaTarget) {
            EditorGUILayout.HelpBox("ServiceAssistant is missing", MessageType.Error);
            return;
        }

        ServiceAssistant main = (ServiceAssistant) metaTarget;

        Undo.RecordObject(main, "");

        main.lifes_refilled_notification = EditorGUILayout.BeginToggleGroup("Lifes Refilling Notification", main.lifes_refilled_notification);
        if (GUILayout.Button("Edit Text", GUILayout.Width(100)))
            AllInOnePanel.CreateBerryPanel().EditLocalization("notification_lifesrefilled");
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space();

        main.daily_reward_notification = EditorGUILayout.BeginToggleGroup("Daily Reward Notification", main.daily_reward_notification);
        main.daily_reward_hour = Mathf.RoundToInt(EditorGUILayout.Slider("Notification Time: " + main.daily_reward_hour.ToString("D2") + ":00", main.daily_reward_hour, 0, 23));
        if (GUILayout.Button("Edit Text", GUILayout.Width(100)))
            AllInOnePanel.CreateBerryPanel().EditLocalization("notification_dailyreward");
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space();

        main.good_morning_notification = EditorGUILayout.BeginToggleGroup("Good Morning Notification", main.good_morning_notification);
        main.good_morning_hour = Mathf.RoundToInt(EditorGUILayout.Slider("Notification Time: " + main.good_morning_hour.ToString("D2") + ":00", main.good_morning_hour, 0, 23));
        main.good_morning_days_delay = Mathf.RoundToInt(EditorGUILayout.Slider("Days Delay: " + main.good_morning_days_delay.ToString() + " day(s)", main.good_morning_days_delay, 1, 14));
        if (GUILayout.Button("Edit Text", GUILayout.Width(100)))
            AllInOnePanel.CreateBerryPanel().EditLocalization("notification_goodmorning");
        EditorGUILayout.EndToggleGroup();
    }
}
