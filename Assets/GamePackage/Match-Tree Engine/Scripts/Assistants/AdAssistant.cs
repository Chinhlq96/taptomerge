using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ChartboostSDK;
using GoogleMobileAds.Api;
using Berry.Utils;
#if !UNITY_WEBGL
using UnityEngine.Advertisements;
#endif

public class AdAssistant : SingletonMonoBehaviour<AdAssistant> {
    
    float lastShow = 0;
    public int ad_show_min_delay = 3;
    // Settings
    // AdColony
    public string AdColony_AppID_Android = "";
    public string AdColony_ZoneID_Android = "";
    public string AdColony_AppID_iOS = "";
    public string AdColony_ZoneID_iOS = "";
    // Chartboost
    public string Chartboost_AppID_Android = "";
    public string Chartboost_Signature_Android = "";
    public string Chartboost_AppID_iOS = "";
    public string Chartboost_Signature_iOS = "";
    // AdMob
    public string AdMob_Interstitial_Android = "";
    public string AdMob_Interstitial_iOS = "";
    public bool AdMob_loading = false;
    InterstitialAd AdMob_Interstitial;




    public enum AdNetwork {
        UnityAds,
        Chartboost,
        AdColony,
        AdMob
    }
    public enum AdType {
        Interstitial,
        Rewarded,
        Banner
    }
    public List<AdNetwork> allNetworks = new List<AdNetwork>() { AdNetwork.UnityAds, AdNetwork.Chartboost, AdNetwork.AdColony, AdNetwork.AdMob };
    
    bool CBdownloading = false;
    
    public System.Action reward = null;
    public static System.Action onAdClose = delegate {};
    Dictionary<string, bool> adColonyVideoAvailable = new Dictionary<string, bool>();

    void Awake() {
        onAdClose += OnAdClose;
        DebugPanel.AddDelegate("Show Video Ads", () => {
            ShowAds();
        });
    }

    void Start() {
        #if !UNITY_WEBGL
        // Initialize
        AdColony.Configure(Application.version.ToString(), GetAdColonyIDs(), new string[] { GetZoneId(AdNetwork.AdColony) });
        Chartboost.setAutoCacheAds(true);

        // AdColony
        AdColony.OnVideoFinished += AdColonyReward;
        AdColony.OnVideoFinished += (bool b) => {onAdClose.Invoke();};
        AdColony.OnAdAvailabilityChange += OnAdAvailabilityChange;

        // Chartboost
        Chartboost.didCloseInterstitial += ChartboostReward;
        Chartboost.didCloseInterstitial += (CBLocation loc) => {onAdClose.Invoke();};
        Chartboost.didCacheInterstitial += CBDownloadComplete;

        // AdMob
        AdMob_Interstitial = new InterstitialAd(GetAdMobIDs());
        #endif
    }

    void OnAdAvailabilityChange(bool available, string zone_id) {
        if (!adColonyVideoAvailable.ContainsKey(zone_id))
            adColonyVideoAvailable.Add(zone_id, false);
        adColonyVideoAvailable[zone_id] = available;
    }

    void OnAdOpen() {
        //AudioAssistant.main.BeQuieter(true);
    }

    void OnAdClose() {
        //AudioAssistant.main.BeQuieter(false);
    }

    string GetAdColonyIDs() {
        switch (Application.platform) {
            case RuntimePlatform.Android:
                return AdColony_AppID_Android;
            case RuntimePlatform.IPhonePlayer:
                return AdColony_AppID_iOS;
        }
        return "";
    }

    string GetAdMobIDs() {
        switch (Application.platform) {
            case RuntimePlatform.Android: return AdMob_Interstitial_Android;
            case RuntimePlatform.IPhonePlayer: return AdMob_Interstitial_iOS;
        }
        return "";
    }

    void CBDownloadComplete(CBLocation loc) {
        CBdownloading = true;
    }


    void Update() {
        foreach (AdNetwork network in Enum.GetValues(typeof (AdNetwork)))
            if (allNetworks.Contains(network))
                DebugPanel.Log(network.ToString(), "Ads", IsReady(network));

        if (!Chartboost.hasInterstitial(CBLocation.Default) && !CBdownloading) {
            CBdownloading = true;
            Chartboost.cacheInterstitial(CBLocation.Default);
        }

        if (!AdMob_Interstitial.IsLoaded() && !AdMob_loading) {
            AdMob_loading = true;
            AdMob_Interstitial = new InterstitialAd(GetAdMobIDs());
            AdMob_Interstitial.LoadAd(new AdRequest.Builder().Build());
        } else if (AdMob_Interstitial.IsLoaded())
            AdMob_loading = false;

    }

    bool IsReady(AdNetwork network) {
        #if !UNITY_WEBGL
        if (network == AdNetwork.UnityAds) return Advertisement.IsReady(GetZoneId(network));
        if (network == AdNetwork.Chartboost) return Chartboost.hasInterstitial(CBLocation.Default);
        if (network == AdNetwork.AdColony) return adColonyVideoAvailable.ContainsKey(GetZoneId(network)) ? adColonyVideoAvailable[GetZoneId(network)] : false;
        if (network == AdNetwork.AdMob) return AdMob_Interstitial.IsLoaded();
        #endif
        return false;
    }

    Action chartboostRewardAction;
    void ChartboostReward(CBLocation location) {
        if (reward != null) {
            reward.Invoke();
            reward = null;
        }
    }
    void AdColonyReward(bool ad_shown) {
        if (ad_shown && reward != null) {
            reward.Invoke();
            reward = null;
        }
    }

    public void ShowAds(Action reward = null) {
        this.reward = reward;

        if (Application.isEditor) {
            if (reward != null) {
                reward.Invoke();
                reward = null;
            }
            return;
        }

        List<AdNetwork> targetNetworks = new List<AdNetwork>();
        foreach (AdNetwork network in allNetworks)
            if (!targetNetworks.Contains(network) && network != AdNetwork.AdMob)
                targetNetworks.Add(network);

        if (targetNetworks.Count == 0)
            return;

        AdNetwork target = targetNetworks.GetRandom();

        ShowAds(target);
    }

    void ShowAds(AdNetwork network) {
        StartCoroutine(ShowingAds(network));
    }

    IEnumerator ShowingAds(AdNetwork network) {
        if (UIPanel.uiAnimation > 0)
            yield return 0;

        if (!IsReady(network) || lastShow + AdAssistant.Instance.ad_show_min_delay * 60 > Time.unscaledTime)
            yield break;

        lastShow = Time.unscaledTime;

        #if !UNITY_WEBGL
        switch (network) {
            case AdNetwork.UnityAds:
                Advertisement.Show(GetZoneId(network), new ShowOptions {
                    resultCallback = result => {
                        if (result == ShowResult.Finished) {
                            if (reward != null) {
                                reward.Invoke();
                                reward = null;
                            }
                        }
                    }
                });
                yield break;
            case AdNetwork.Chartboost:
                OnAdOpen();
                Chartboost.showInterstitial(CBLocation.Default);
                yield break;
            case AdNetwork.AdColony:
                AdColony.ShowVideoAd(GetZoneId(network));
                yield break;
            case AdNetwork.AdMob:
                AdMob_Interstitial.Show();
                yield break;
        }
        #endif
    }

    string GetZoneId(AdNetwork network) {
        switch (network) {
            case AdNetwork.Chartboost:
                return UIManager.Instance.GetCurrentPage();
            case AdNetwork.AdColony: 
                if (Application.platform == RuntimePlatform.Android) return AdColony_ZoneID_Android;
                if (Application.platform == RuntimePlatform.IPhonePlayer) return AdColony_ZoneID_iOS;
                return "";
            case AdNetwork.UnityAds:
                return "defaultZone";
        }
        return "";
    }


    public bool HasAnyVideo() {
        foreach (AdNetwork network in allNetworks)
            if (IsReady(network))
                return true;
        return false;
    }
}