using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController controller;
    private static IExtensionProvider extensions;

    public static string removeAds = "remove_ads"; // ID produk sama dengan di Play Console

    void Start()
    {
        if (controller == null)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(removeAds, ProductType.NonConsumable);
            UnityPurchasing.Initialize(this, builder);
        }
    }

    public void BuyRemoveAds()
    {
        controller.InitiatePurchase(removeAds);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (e.purchasedProduct.definition.id == removeAds)
        {
            Debug.Log("Remove Ads Purchased!");
            PlayerPrefs.SetInt("RemoveAds", 1);
            PlayerPrefs.Save();

            // ✅ Matikan iklan lewat AdsController
            if (AdsController.Instance != null)
                AdsController.Instance.DisableAds();
        }
        return PurchaseProcessingResult.Complete;
    }

    // ✅ Dipanggil saat inisialisasi berhasil
    public void OnInitialized(IStoreController c, IExtensionProvider e)
    {
        controller = c;
        extensions = e;
    }

    // ✅ Versi lama (masih wajib ada untuk kompatibilitas)
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Init Failed: " + error);
    }

    // ✅ Versi baru (harus ditambahkan supaya error hilang)
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("IAP Init Failed: " + error + " - " + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError("Purchase failed: " + product.definition.id + " - " + reason);
    }
}
