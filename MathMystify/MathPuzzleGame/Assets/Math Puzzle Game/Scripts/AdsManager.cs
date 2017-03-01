using UnityEngine;
using System.Collections;
using System;
using Ververg;

#if GOOGLE_MOBILE_ADS
using GoogleMobileAds;
using GoogleMobileAds.Api;
#endif

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace Ververg
{
    public class AdsManager : Singleton<AdsManager>
    {
#if GOOGLE_MOBILE_ADS
        BannerView bannerView;
        InterstitialAd interstitial;
        AdRequest requestBanner;
        AdRequest requestInterstitial;
#endif

#if GOOGLE_MOBILE_ADS
        public string AdmobBannerIdANDROID = "ca-app-pub-4501064062171971/8189358440";
        public string AdmobInterstitialIdANDROID = "ca-app-pub-4501064062171971/9666091642";
#endif

        public bool basedTimeInterstitialAtGameOver = false;
        public int numberOfPlayToShowInterstitial = 5;
        public float numberOfMinutesToShowAnInterstitialAtGameOver = 2;
        public bool ShowIntertitialAtStart = false;
        // Interstitials will use Unity Ads Video if its ready instead Admob
        public bool UseUnityAdsVideoInterstitial = false;
        public bool NO_ADS = false;

        float realTimeSinceStartup;

        void Awake()
        {
            instance = this;
            Set();
        }

        void Start()
        {
            if (ShowIntertitialAtStart)
            {
                _ShowInterstitial();
            }
        }

        void Update()
        {

        }

        public void Set()
        {
            realTimeSinceStartup = Time.realtimeSinceStartup;

#if GOOGLE_MOBILE_ADS
            bannerView = new BannerView(AdmobBannerIdANDROID, AdSize.SmartBanner, AdPosition.Bottom);
            requestBanner = new AdRequest.Builder().Build();
            interstitial = new InterstitialAd(AdmobInterstitialIdANDROID);
#endif
            RequestInterstitial();
        }

        void RequestInterstitial()
        {
#if GOOGLE_MOBILE_ADS
            requestInterstitial = new AdRequest.Builder().Build();
            interstitial.LoadAd(requestInterstitial);
#endif
        }

#if GOOGLE_MOBILE_ADS
        // Called when an ad request has successfully loaded.
        void HandleAdLoaded(object sender, EventArgs e)
        {

        }
        // Called when an ad request failed to load.
        void HandleAdFailedToLoad(object sender, EventArgs e)
        {
            Invoke("ShowBanner", 10);
        }
        // Called when an ad is clicked.
        void HandleAdOpened(object sender, EventArgs e)
        {

        }
        // Called when the user is about to return to the app after an ad click.
        void HandleAdClosing(object sender, EventArgs e)
        {

        }
        // Called when the user returned from the app after an ad click.
        void HandleAdClosed(object sender, EventArgs e)
        {

        }
        // Called when the ad click caused the user to leave the application.
        void HandleAdLeftApplication(object sender, EventArgs e)
        {

        }
#endif
        public void ShowBanner()
        {
            if (NO_ADS)
                return;
#if GOOGLE_MOBILE_ADS
            bannerView.LoadAd(requestBanner);
            bannerView.Show();
            bannerView.AdLoaded -= HandleAdLoaded;
            bannerView.AdFailedToLoad -= HandleAdFailedToLoad;
            bannerView.AdOpened -= HandleAdOpened;
            bannerView.AdClosed -= HandleAdClosed;

            // Called when an ad request has successfully loaded.
            bannerView.AdLoaded += HandleAdLoaded;
            // Called when an ad request failed to load.
            bannerView.AdFailedToLoad += HandleAdFailedToLoad;
            // Called when an ad is clicked.
            bannerView.AdOpened += HandleAdOpened;
            // Called when the user returned from the app after an ad click.
            bannerView.AdClosed += HandleAdClosed;
#endif
        }

        public void ShowAdsGameOver()
        {
            if (NO_ADS)
                return;
            bool showAds = false;

            if (basedTimeInterstitialAtGameOver)
            {
                float t = Time.realtimeSinceStartup;

                float ourTIme = numberOfMinutesToShowAnInterstitialAtGameOver * 60;
                if ((t - realTimeSinceStartup) > ourTIme)
                {
                    _ShowInterstitial();
                    realTimeSinceStartup = t;
                }
            }
            else
            {
                int count = PlayerPrefs.GetInt("numberOfPlayToShowInterstitial", 0);

                showAds = count >= numberOfPlayToShowInterstitial;

                if (showAds)
                {
                    PlayerPrefs.SetInt("numberOfPlayToShowInterstitial", 0);
                    PlayerPrefs.Save();
                    _ShowInterstitial();
                }
                else
                {
                    PlayerPrefs.SetInt("numberOfPlayToShowInterstitial", count);
                    PlayerPrefs.Save();
                }

            }
        }

        private void _ShowInterstitial()
        {
            if (NO_ADS)
                return;
            if(UseUnityAdsVideoInterstitial)
            {
#if UNITY_ADS
                if(Advertisement.IsReady())
                {
                    Advertisement.Show();
                }
                else
                {
#if GOOGLE_MOBILE_ADS
                    if (interstitial.IsLoaded())
                    {
                        interstitial.Show();
                    }
                    else
                    {
                        RequestInterstitial();
                    }
#endif
                }
#endif
            }
            else
            {
#if GOOGLE_MOBILE_ADS
                if (interstitial.IsLoaded())
                {
                    interstitial.Show();
                }
                else
                {
                    RequestInterstitial();
                }
#endif
            }
        }

        public void Show_Banner()
        {
#if GOOGLE_MOBILE_ADS
            if (bannerView != null)
            {
                Debug.Log("Show current banner");
                bannerView.Show();
            }
#endif
        }

        public void Hide_Banner()
        {
#if GOOGLE_MOBILE_ADS
            if (bannerView != null)
            {
                Debug.Log("Hide current banner");
                bannerView.Hide();
            }
#endif
        }
    }
}