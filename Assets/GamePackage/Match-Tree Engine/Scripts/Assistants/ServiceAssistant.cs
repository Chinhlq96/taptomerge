using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Berry.Utils;
#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using LocalNotification = UnityEngine.iOS.LocalNotification;
#endif

public class ServiceAssistant : SingletonMonoBehaviour<ServiceAssistant> {

    public bool lifes_refilled_notification = true;
    public bool daily_reward_notification = true;
    public bool good_morning_notification = true;

    bool rate_it_showed = false;
    bool daily_reward_showed = false;

    public int daily_reward_hour = 10;
    public int good_morning_hour = 10;
    public int good_morning_days_delay = 2;

    void Awake() {
        if (Application.isEditor)
            Application.runInBackground = true;
       this.enabled = false;
        UIManager.onShowPage += LevelMapPopup;
        rate_it_showed = PlayerPrefs.GetInt("Rated") == 1;

        #if UNITY_IOS
        NotificationServices.RegisterForNotifications(
            UnityEngine.iOS.NotificationType.Alert |
            UnityEngine.iOS.NotificationType.Badge |
            UnityEngine.iOS.NotificationType.Sound);
        #endif

        RemoveAllNotifications();
        UpdateNotifications();
    }


    void LevelMapPopup(string page) {
        if (!Instance.enabled) return;
        StartCoroutine(LevelMapPopupRoutine(page));
    }

    IEnumerator LevelMapPopupRoutine(string page) {
        UpdateNotifications();

        if (page != "LevelList")
            yield break;

        yield return 0;

        while (UIPanel.uiAnimation > 0)
            yield return 0;
        if (UIManager.Instance.GetCurrentPage() != page)
            yield break;

        yield return 0;


        // Rate It
        if (!rate_it_showed) {
         //   if (ProfileAssistant.main.local_profile.current_level < 10)
          //      yield break;
            if (UnityEngine.Random.value > 0.3f)
                yield break;
            UIManager.Instance.ShowPage("RateIt");
            yield break;
        }
    }

    public void RateIt() {
        string link = GetAppLink(Application.platform);
        if (link != "")
            Application.OpenURL(link);
        rate_it_showed = true;
        PlayerPrefs.SetInt("Rated", 1);
        UIManager.Instance.ShowPage("LevelList");
    }

    public void DownloadUpdate() {
        string link = GetAppLink();
        if (link != "")
            Application.OpenURL(link);
        UIManager.Instance.SetPanelVisible("NewVersion", false);
    }

    public void Later() {
        rate_it_showed = true;
        UIManager.Instance.ShowPage("LevelList");
    }

    public void NoThanks() {
        rate_it_showed = true;
        PlayerPrefs.SetInt("Rated", 1);
        UIManager.Instance.ShowPage("LevelList");
    }

    public static string GetAppLink(bool native = true) {
        return GetAppLink(Application.platform, native);
    }
    public static string GetAppLink(RuntimePlatform platform, bool native = true) {
        if (native)
            switch (platform) {
                case RuntimePlatform.Android:
                    return "market://details?id=" + Application.identifier;
                case RuntimePlatform.IPhonePlayer:
                return "itms-apps://itunes.apple.com/app/id" + ProjectParameters.Instance.ios_AppID;
            }

        switch (platform) {
            case RuntimePlatform.Android:
                return "https://play.google.com/store/apps/details?id=" + Application.identifier;
            case RuntimePlatform.IPhonePlayer:
                return "https://itunes.apple.com/es/app/candy-crush-jelly-saga/id" + ProjectParameters.Instance.ios_AppID;
            default:
                return "http://u3d.as/c3R";
        }
    }

    public void Quit() {
        Application.Quit();
    }

    void OnApplicationQuit() {
        UpdateNotifications();
    }

    Dictionary<string, int> notification_ids = new Dictionary<string, int>();
    void RemoveAllNotifications() {
#region Android
#if UNITY_ANDROID
        string raw = PlayerPrefs.GetString("NotificationIDs");
        if (!string.IsNullOrEmpty(raw))
            notification_ids = raw.Split(',').Select(x => x.Split(':')).ToDictionary(x => x[0], x => int.Parse(x[1]));
        else
            return;
        foreach (var key in notification_ids.Values)
            AndroidLocalNotification.CancelNotification(key);
        notification_ids.Clear();
        PlayerPrefs.DeleteKey("NotificationIDs");
#endif
#endregion
      
#region iOS
#if UNITY_IOS
        NotificationServices.CancelAllLocalNotifications();
#endif
#endregion
    }

    public void UpdateNotifications() {
        if (LocalizationAssistant.Instance == null)
            return;

#region Android
#if UNITY_ANDROID
        string raw = PlayerPrefs.GetString("NotificationIDs");
        if (!string.IsNullOrEmpty(raw)) {
            notification_ids = raw.Split(',').Select(x => x.Split(':')).ToDictionary(x => x[0], x => int.Parse(x[1]));
        } else
            notification_ids = new Dictionary<string, int>();


    
        PlayerPrefs.SetString("NotificationIDs", string.Join(",", notification_ids.Select(x => string.Format("{0}:{1}", x.Key, x.Value)).ToArray()));
        PlayerPrefs.Save();
#endif
#endregion

#region iOS
#if UNITY_IOS
        RemoveAllNotifications();
        LocalNotification notification;

        // Daily reward
        if (ProfileAssistant.main != null && ProfileAssistant.main.local_profile != null) {
            notification = new LocalNotification();
            notification.fireDate = DateTime.Now + Utils.GetDelay(daily_reward_hour, 0, 0);
            notification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
            notification.alertBody = LocalizationAssistant.main["notification_dailyreward_text"];
            notification.soundName = LocalNotification.defaultSoundName;    
            NotificationServices.ScheduleLocalNotification(notification);
        }

        // Lifes
        if (ProfileAssistant.main != null && ProfileAssistant.main.local_profile != null) {
            if ((ProfileAssistant.main.local_profile.next_life_time - DateTime.Now).TotalSeconds > 30) {
                notification = new LocalNotification();
                notification.fireDate = ProfileAssistant.main.local_profile.next_life_time;
                notification.alertBody = LocalizationAssistant.main["notification_lifesrefilled_text"];
                notification.soundName = LocalNotification.defaultSoundName;
                NotificationServices.ScheduleLocalNotification(notification);
            }
        }

        // Good morning
        if (ProfileAssistant.main != null && ProfileAssistant.main.local_profile != null) {
            notification = new LocalNotification();
            notification.fireDate = DateTime.Now + Utils.GetDelay(9, 0, 0);
            notification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
            notification.alertBody = LocalizationAssistant.main["notification_goodmorning_text"];
            notification.soundName = LocalNotification.defaultSoundName;
            NotificationServices.ScheduleLocalNotification(notification);
        }

        // Test
        notification = new LocalNotification();
        notification.fireDate = DateTime.Now + new TimeSpan(0, 1, 0);
        notification.alertBody = "Test \ue012";
        NotificationServices.ScheduleLocalNotification(notification);
#endif
#endregion
    }

//    public List<string> RequriedLocalizationKeys() {
//        return new List<string>() {
//            "notification_dailyreward_title",
//            "notification_dailyreward_text",
//            "notification_lifesrefilled_title",
//            "notification_lifesrefilled_text",
//            "notification_goodmorning_title",
//            "notification_goodmorning_text"
//        };
//    }
}
