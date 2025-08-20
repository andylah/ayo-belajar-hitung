using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsController : MonoBehaviour
{
    public static AdsController Instance;

#if UNITY_EDITOR
    string bannerAdUnitId = "unused";
    string InterstitialAdUnitId = "unused";
#elif UNITY_ANDROID
    string bannerAdUnitId = "ca-app-pub-3619184054099014/1748124773";
    string InterstitialAdUnitId = "ca-app-pub-3619184054099014/2899879752";
#else
    string bannerAdUnitId = "unexpected_platform";
    string InterstitialAdUnitId = "unexpected_platform";
#endif

    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Start()
    {
        // ✅ Jangan load ads kalau sudah beli Remove Ads
        if (PlayerPrefs.GetInt("RemoveAds", 0) == 1)
        {
            Debug.Log("Ads disabled (Remove Ads purchased).");
            return;
        }

        MobileAds.Initialize((InitializationStatus status) =>
        {
            RequestAds();
            LoadInterstitialAd();
        });
    }

    private void RequestAds()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(bannerAdUnitId, adaptiveSize, AdPosition.Bottom);

        _bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        _bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;

        AdRequest adRequest = new AdRequest();
        _bannerView.LoadAd(adRequest);
    }

    #region Banner callback
    private void OnBannerAdLoaded()
    {
        Debug.Log("Banner view loaded an ad.");
    }

    private void OnBannerAdLoadFailed(LoadAdError error)
    {
        Debug.LogError("Banner view failed: " + error);
    }
    #endregion

    #region Interstitial
    private void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(InterstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.Log("Interstitial failed: " + error);
                return;
            }
            Debug.Log("Interstitial loaded.");
            _interstitialAd = ad;
            InterstitialEvent(ad);
        });
    }

    public void ShowInterstitialAd()
    {
        if (PlayerPrefs.GetInt("RemoveAds", 0) == 1)
        {
            Debug.Log("Remove Ads active → no interstitial.");
            return;
        }

        if (_interstitialAd != null)
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial not ready, reloading...");
            LoadInterstitialAd();
        }
    }

    private void InterstitialEvent(InterstitialAd ad)
    {
        _interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial closed. Reloading...");
            LoadInterstitialAd();
        };
    }
    #endregion

    // ✅ Dipanggil oleh IAPManager setelah pembelian berhasil
    public void DisableAds()
    {
        Debug.Log("DisableAds() called → turning off ads.");
        PlayerPrefs.SetInt("RemoveAds", 1);
        PlayerPrefs.Save();

        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }
    }
}
