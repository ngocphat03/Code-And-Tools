using UnityEngine;
using UnityEngine.Events;

using GoogleMobileAds.Api;

public class GoogleAdsMobController : MonoBehaviour
{

    private InterstitialAd _interstitial_Ad;
    private RewardedAd _rewardedAd;
    private BannerView _bannerView;
    
    public UnityEvent onAdLoadedEvent;
    public UnityEvent onAdFailedToLoadEvent;
    public UnityEvent onAdOpeningEvent;
    public UnityEvent onAdFailedToShowEvent;
    public UnityEvent onUserEarnedRewardEvent;
    public UnityEvent onAdClosedEvent;

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adID = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private string _adID = "ca-app-pub-3940256099942544/1712485313";
#else
    private string _adID = "unused";
#endif

    private void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

        // Request Ads
        RequestAndLoadRewardedAd();
        RequestAndLoadInterstitialAd();
        RequestBanner();


        DontDestroyOnLoad(this.gameObject);
    }



    public void RequestAndLoadRewardedAd()
    {
        // Create new rewarded ad instance
        RewardedAd.Load(_adID, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.LogWarning("Rewarded ad failed to load with error: " + loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.LogWarning("Rewarded ad failed to load.");
                    return;
                }

                Debug.LogWarning("Rewarded ad loaded.");
                _rewardedAd = ad;

        // Add event rewarded ads

                // Open reward ads
                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.LogWarning("Rewarded ad opening.");
                    onAdOpeningEvent.Invoke();
                };

                // Close reward ads
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.LogWarning("Rewarded ad closed.");
                    RequestAndLoadInterstitialAd();
                    onAdClosedEvent.Invoke();
                };

                // Load reward ads failed
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.LogWarning("Rewarded ad failed to show with error: " +
                               error.GetMessage());
                };

                // Run when reward ads success (1)
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    Debug.LogError("Reward ads success");
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Rewarded ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    Debug.LogWarning("msg");
                };
                // Run when reward ads success (2)
                // ad.OnUserEarnedReward += HandleUserEarnedReward;
            });
    
    }
    public void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("Rewarded ad granted a reward: " + reward.Amount);
            });
        }
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("ABCDEFG");
    }


    private void RequestBanner()
    {
        // Clear up banner before reusing
        if(_bannerView != null)
        {
            _bannerView.Destroy();
        }
        // Create a 320x50 banner at the top of the screen.
        _bannerView = new BannerView(_adID, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        _bannerView.LoadAd(request);
    }
    public void ShowBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Show();
        }
    }
    public void HideBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Hide();
        }
    }

    public void ShowInterstitialAd()
    {
        if(_interstitial_Ad != null && _interstitial_Ad.CanShowAd())
        {
            _interstitial_Ad.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet");
        }
    }
    public void RequestAndLoadInterstitialAd()
    {
        // Clean up interstitial before using it
        if(_interstitial_Ad != null)
        {
            _interstitial_Ad.Destroy();
        }

        // Load interstitial ads
        InterstitialAd.Load(_adID, CreateAdRequest(),
            (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.LogWarning("Interstitial ad failed to load with error: " +
                        loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.LogWarning("Interstitial ad failed to load.");
                    return;
                }

                Debug.LogWarning("Interstitial ad loaded.");
                _interstitial_Ad = ad;

        // Add event interstitial ads

                // Open interstitial ads
                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.LogWarning("Interstitial ad opening.");
                    onAdOpeningEvent.Invoke();
                };

                // Close interstitial ads
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.LogWarning("Interstitial ad closed.");
                    onAdClosedEvent.Invoke();
                };

                // Load interstitial ads failed
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.LogWarning("Interstitial ad failed to show with error: " +
                                error.GetMessage());
                };

                // Run when interstitial ads success
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    Debug.LogWarning("Get a gift");
                };
            });
    }
    
    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
    }
}