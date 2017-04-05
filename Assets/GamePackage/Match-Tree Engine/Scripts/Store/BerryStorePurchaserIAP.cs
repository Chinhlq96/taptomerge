using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BerryStorePurchaserIAP : MonoBehaviour {

    public Text price;
    public Button purchaseButton;
    public string sku;
    public string goodId;
    public int goodCount;

	void Awake () {
        purchaseButton.onClick.AddListener(Purchase);
	}

    void OnEnable() {
        Refresh();
    }

	void Purchase () {
        BerryStoreAssistant.Instance.PurchaseIAP(sku, goodId, goodCount);
	}

    public void Refresh() {
        price.text = "N/A";
        if (BerryStoreAssistant.Instance.marketItemPrices.ContainsKey(sku))
            price.text = BerryStoreAssistant.Instance.marketItemPrices[sku];
       // purchaseButton.gameObject.SetActive(SignMode.GetMode() != SignMode.SignModeType.Processing);
    }
}
