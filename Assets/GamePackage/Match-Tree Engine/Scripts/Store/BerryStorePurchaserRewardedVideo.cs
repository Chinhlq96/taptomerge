using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BerryStorePurchaserRewardedVideo : MonoBehaviour {

    public Button purchaseButton;
    public string goodId;
    public int goodCount;

    void Start() {
        if (purchaseButton) purchaseButton.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        AdAssistant.Instance.ShowAds(GetReward);
    }

    public void GetReward() {
        BerryStoreAssistant.Instance.Purchase(0, goodId, goodCount);
    }
}
