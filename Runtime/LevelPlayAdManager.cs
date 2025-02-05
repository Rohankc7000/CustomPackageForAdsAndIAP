using com.unity3d.mediation;
using System;
using Unity.VisualScripting;
using UnityEngine;


public class LevelPlayAdManager : MonoBehaviour
{
    public string AppKey;
    public string BannerAdUnitId;
    public string RewardedAdUnitId;
    public string InterstitialAdUnitId;

    // Instance for Ads
    public LevelPlayBannerAd BannerAd;
    public LevelPlayInterstitialAd InterstitialAd;

    //my cutsom actions
    public static Action OnRewardedSuccess;
    public static Action OnRewaredAdWatchCompleteFailed;
    public static Action OnRewaredAdNotAvailable;

    public bool IsRewardedAdsCompletelyPlayed;

    public static LevelPlayAdManager Instance;

    private void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Instance = this;
            IsRewardedAdsCompletelyPlayed = false;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //int playerAge = PlayerPrefs.GetInt(Constants.PlayerAge, 13);
        //IronSource.Agent.validateIntegration();
        LevelPlayAdFormat[] legacyAdFormats = new[] { LevelPlayAdFormat.REWARDED, LevelPlayAdFormat.BANNER, LevelPlayAdFormat.INTERSTITIAL };
        LevelPlay.Init(AppKey, adFormats: legacyAdFormats);
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
    }

    private void OnEnable()
    {
        // For the Banner Ads
        BannerAd = new LevelPlayBannerAd(BannerAdUnitId, LevelPlayAdSize.BANNER);
        BannerAdsEventRegister();

        //For the Interstitial Ads
        InterstitialAd = new LevelPlayInterstitialAd(InterstitialAdUnitId);
        InterstitialAdsEventRegister();

        //For the Rewarded Ads
        RewardedAdsEventRegister();
    }

    private void BannerAdsEventRegister()
    {
        BannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        BannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        BannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        BannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        BannerAd.OnAdClicked += BannerOnAdClickedEvent;
        BannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        BannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        BannerAd.OnAdExpanded += BannerOnAdExpandedEvent;
    }
    private void InterstitialAdsEventRegister()
    {
        InterstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        InterstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        InterstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        InterstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        InterstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        InterstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        InterstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
    }
    private void RewardedAdsEventRegister()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    #region -------------------- Public Methods ---------------------

    public void LoadBannerAds()
    {
        if (BannerAdUnitId != null)
        {
            BannerAd.LoadAd();
        }
    }
    public void ShowBannerAds()
    {
        BannerAd.ShowAd();
    }
    public void HideBannerAds()
    {
        BannerAd.HideAd();
    }
    public void LoadIntersitialAds()
    {
        if (InterstitialAd != null)
        {
            InterstitialAd.LoadAd();
        }
    }
    public void ShowIntersitialAds()
    {
        if (InterstitialAd.IsAdReady())
        {
            //EasyGameUI.instance.IsAdsPlaying = true;
            InterstitialAd.ShowAd();
        }
        else
        {
            Debug.Log("Intestitial Ads not ready....");
        }
    }

    public void LoadRewardedAds()
    {
        if (RewardedAdUnitId != null && IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.loadRewardedVideo();
        }
    }

    public void ShowRewardedAds()
    {
        //EasyGameUI.instance.IsAdsPlaying = true;
        IronSource.Agent.showRewardedVideo();
    }


    #endregion

    #region ----------- Level Play Callbacks ------------------
    private void SdkInitializationCompletedEvent(LevelPlayConfiguration configuration)
    {
        Debug.Log(configuration.ToString());
        LoadIntersitialAds();
        LoadRewardedAds();
        Debug.Log("Successfully initialzed LevelPlay SDK");
    }
    private void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log("Failed initialzed LevelPlay SDK");
    }



    #endregion ------------------------------------------------

    #region -------------------- Banner Ads Callbacks ---------------------

    void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log(adInfo.ToString());
    }
    void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError) { }
    void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError) { }
    void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo) { }

    #endregion

    #region -------------------- Intersitial Ads Callbacks ---------------------

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log(adInfo.ToString());
    }
    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error) { }
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError) { }
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { }

    #endregion

    #region -------------------- Rewarded Ads Callbacks ---------------------

    /************* RewardedVideo AdInfo Delegates *************/
    // Indicates that there’s an available ad.
    // The adInfo object includes information about the ad that was loaded successfully
    // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {

    }
    // Indicates that no ads are available to be displayed
    // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    void RewardedVideoOnAdUnavailable()
    {

    }
    // The Rewarded Video ad view has opened. Your activity will loose focus.
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {

    }

    // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        if (!IsRewardedAdsCompletelyPlayed)
        {
            OnRewaredAdWatchCompleteFailed?.Invoke();
        }
        IsRewardedAdsCompletelyPlayed = false;
        //EasyGameUI.instance.IsAdsPlaying = false;
    }
    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        OnRewardedSuccess?.Invoke();
        IsRewardedAdsCompletelyPlayed = true;
        //EasyGameUI.instance.IsAdsPlaying = false;
    }

    // The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        OnRewaredAdNotAvailable?.Invoke();
        //EasyGameUI.instance.IsAdsPlaying = false;
    }
    // Invoked when the video ad was clicked.
    // This callback is not supported by all networks, and we recommend using it only if
    // it’s supported by all networks you included in your build.
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {

    }
    #endregion
}