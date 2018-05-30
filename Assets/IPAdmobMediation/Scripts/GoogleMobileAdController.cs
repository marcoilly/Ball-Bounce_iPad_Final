#define AdmobM

using System;
using UnityEngine;
#if AdmobM
using GoogleMobileAds;
using GoogleMobileAds.Api;
#endif
public class GoogleMobileAdController : MonoBehaviour
{
	public delegate void GoogleMobileAdControllerDelegate(bool isVideoComplete);

	public static event GoogleMobileAdControllerDelegate OnDidCloseRewardVideoSuccess;
	private static GoogleMobileAdController _instance;
	#if AdmobM
	private BannerView bannerView;
	private InterstitialAd interstitial;
	private NativeExpressAdView nativeExpressAdView;
	private RewardBasedVideoAd rewardBasedVideo;
	#endif

	private static string outputMessage = string.Empty;
	private bool isBannerLoaded = false;

	private bool isRewardVideoPlaying = false;
	private bool isRewardVideoComplete = false;
	public static string OutputMessage
	{
		set { outputMessage = value; }
	}

	public static GoogleMobileAdController Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject go = new GameObject("GoogleMobileAdController");
				go.AddComponent<GoogleMobileAdController>();
			}
			return _instance;
		}
	}

	void Awake()
	{
		if (_instance == null)
		{
			DontDestroyOnLoad(gameObject);
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Start()
	{
		#if AdmobM

		// Get singleton reward based video ad reference.
		this.rewardBasedVideo = RewardBasedVideoAd.Instance;

		// RewardBasedVideoAd is a singleton, so handlers should only be registered once.
		this.rewardBasedVideo.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
		this.rewardBasedVideo.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
		this.rewardBasedVideo.OnAdOpening += this.HandleRewardBasedVideoOpened;
		this.rewardBasedVideo.OnAdStarted += this.HandleRewardBasedVideoStarted;
		this.rewardBasedVideo.OnAdRewarded += this.HandleRewardBasedVideoRewarded;
		this.rewardBasedVideo.OnAdClosed += this.HandleRewardBasedVideoClosed;
		this.rewardBasedVideo.OnAdLeavingApplication += this.HandleRewardBasedVideoLeftApplication;

		#endif
	}

	float timer = 0;

	bool callbackRewarVideoComplete = false;
	public void Update()
	{
		timer += Time.deltaTime;
		if (timer >= 4)
		{
			timer = 0;
			if (!isBannerLoaded)
			{
				//                Debug.LogError("You forget to add IAP conditon for banner");   /// HHH
				// if(IAP conditon)   
				if (!PlayerPrefsX.GetBool (GameKeys.kprefIsAdRemoved)) {
					RequestBanner ();
				}

			}
			if (!HasRewardVideoAD() && !isRewardVideoPlaying)
			{
				RequestRewardBasedVideo();
			}
			if (!HasInterstitialAD())
			{
				RequestInterstitial();
			}
		}

		if (callbackRewarVideoComplete)
		{
			callbackRewarVideoComplete = false;
			if (OnDidCloseRewardVideoSuccess != null)
			{
				OnDidCloseRewardVideoSuccess(isRewardVideoComplete);
			}
		}
	}

	/* 
    public void OnGUI()
    {
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        style.alignment = TextAnchor.LowerRight;
        style.fontSize = (int)(Screen.height * 0.06);
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

        // Puts some basic buttons onto the screen.
        GUI.skin.button.fontSize = (int)(0.035f * Screen.width);
        float buttonWidth = 0.35f * Screen.width;
        float buttonHeight = 0.15f * Screen.height;
        float columnOnePosition = 0.1f * Screen.width;
        float columnTwoPosition = 0.55f * Screen.width;

        Rect requestBannerRect = new Rect(
                                     columnOnePosition,
                                     0.05f * Screen.height,
                                     buttonWidth,
                                     buttonHeight);
        if (GUI.Button(requestBannerRect, "Request\nBanner"))
        {
            this.RequestBanner();
        }

        Rect destroyBannerRect = new Rect(
                                     columnOnePosition,
                                     0.225f * Screen.height,
                                     buttonWidth,
                                     buttonHeight);
        if (GUI.Button(destroyBannerRect, "Destroy\nBanner"))
        {
            this.bannerView.Destroy();
        }

        Rect requestInterstitialRect = new Rect(
                                           columnOnePosition,
                                           0.4f * Screen.height,
                                           buttonWidth,
                                           buttonHeight);
        if (GUI.Button(requestInterstitialRect, "Request\nInterstitial"))
        {
            this.RequestInterstitial();
        }

        Rect showInterstitialRect = new Rect(
                                        columnOnePosition,
                                        0.575f * Screen.height,
                                        buttonWidth,
                                        buttonHeight);
        if (GUI.Button(showInterstitialRect, "Show\nInterstitial"))
        {
            this.ShowInterstitial();
        }

        Rect destroyInterstitialRect = new Rect(
                                           columnOnePosition,
                                           0.75f * Screen.height,
                                           buttonWidth,
                                           buttonHeight);
        if (GUI.Button(destroyInterstitialRect, "Destroy\nInterstitial"))
        {
            this.interstitial.Destroy();
        }

        Rect requestNativeExpressAdRect = new Rect(
                                              columnTwoPosition,
                                              0.05f * Screen.height,
                                              buttonWidth,
                                              buttonHeight);
        if (GUI.Button(requestNativeExpressAdRect, "Request Native\nExpress Ad"))
        {
            this.RequestNativeExpressAdView();
        }

        Rect destroyNativeExpressAdRect = new Rect(
                                              columnTwoPosition,
                                              0.225f * Screen.height,
                                              buttonWidth,
                                              buttonHeight);
        if (GUI.Button(destroyNativeExpressAdRect, "Destory Native\nExpress Ad"))
        {
            this.nativeExpressAdView.Destroy();
        }

        Rect requestRewardedRect = new Rect(
                                       columnTwoPosition,
                                       0.4f * Screen.height,
                                       buttonWidth,
                                       buttonHeight);
        if (GUI.Button(requestRewardedRect, "Request\nRewarded Video"))
        {
            this.RequestRewardBasedVideo();
        }

        Rect showRewardedRect = new Rect(
                                    columnTwoPosition,
                                    0.575f * Screen.height,
                                    buttonWidth,
                                    buttonHeight);
        if (GUI.Button(showRewardedRect, "Show\nRewarded Video"))
        {
            this.ShowRewardBasedVideo();
        }

        Rect textOutputRect = new Rect(
                                  columnTwoPosition,
                                  0.925f * Screen.height,
                                  buttonWidth,
                                  0.05f * Screen.height);
        GUI.Label(textOutputRect, outputMessage);
    }

    */

	#if AdmobM


	// Returns an ad request with custom ad targeting.
	private AdRequest CreateAdRequest()
	{
	return new AdRequest.Builder()
	// .AddTestDevice(AdRequest.TestDeviceSimulator)
	// .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
	.AddKeyword("game")
	.SetGender(Gender.Male)
	.SetBirthday(new DateTime(1985, 1, 1))
	.TagForChildDirectedTreatment(false)
	.AddExtra("color_bg", "9B30FF")
	.Build();
	}

	#endif

	private void RequestBanner()
	{
		// These ad units are configured to always serve test ads.
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-9622394062773093/7343099404";
		#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-9622394062773093/7187186739";  /// Original
		#else
		string adUnitId = "unexpected_platform";
		#endif

		#if AdmobM

		if (this.bannerView != null)
		DestoryBanner();
		// Create a 320x50 banner at the top of the screen.
		this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

		// Register for ad events.
		this.bannerView.OnAdLoaded += this.HandleAdLoaded;
		this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
		this.bannerView.OnAdOpening += this.HandleAdOpened;
		this.bannerView.OnAdClosed += this.HandleAdClosed;
		this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

		// Load a banner ad.
		this.bannerView.LoadAd(this.CreateAdRequest());

		#endif
		}

		private void RequestInterstitial()
		{
		// These ad units are configured to always serve test ads.
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-9622394062773093/4262189230";
		#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-9622394062773093/4701172935";  /// Original
		#else
		string adUnitId = "unexpected_platform";
		#endif

		#if AdmobM

		// Create an interstitial.
		this.interstitial = new InterstitialAd(adUnitId);

		// Register for ad events.
		this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
		this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
		this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
		this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
		this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

		// Load an interstitial ad.
		this.interstitial.LoadAd(this.CreateAdRequest());

		#endif
		}

		private void RequestNativeExpressAdView()
		{
		// These ad units are configured to always serve test ads.
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-3940256099942544/1072772517";
		#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-3940256099942544/2562852117";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		#if AdmobM

		// Create a 320x150 native express ad at the top of the screen.
		this.nativeExpressAdView = new NativeExpressAdView(
		adUnitId,
		new AdSize(320, 150),
		AdPosition.Top);

		// Register for ad events.
		this.nativeExpressAdView.OnAdLoaded += this.HandleNativeExpressAdLoaded;
		this.nativeExpressAdView.OnAdFailedToLoad += this.HandleNativeExpresseAdFailedToLoad;
		this.nativeExpressAdView.OnAdOpening += this.HandleNativeExpressAdOpened;
		this.nativeExpressAdView.OnAdClosed += this.HandleNativeExpressAdClosed;
		this.nativeExpressAdView.OnAdLeavingApplication += this.HandleNativeExpressAdLeftApplication;

		// Load a native express ad.
		this.nativeExpressAdView.LoadAd(this.CreateAdRequest());

		#endif
		}

		private void RequestRewardBasedVideo()
		{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-9622394062773093/5850479364";
		#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-9622394062773093/1256751948";  /// Original
		#else
		string adUnitId = "unexpected_platform";
		#endif

		#if AdmobM

		this.rewardBasedVideo.LoadAd(this.CreateAdRequest(), adUnitId);

		#endif
		}


		public bool HasInterstitialAD()
		{
		#if AdmobM

		if (this.interstitial == null)
		{
		return false;
		}
		else
		{
		return this.interstitial.IsLoaded();
		}

		#else

		return true;

		#endif
		}

		public bool HasRewardVideoAD()
		{
		#if AdmobM

		if (this.rewardBasedVideo == null)
		{
		return false;
		}
		else
		{
		return this.rewardBasedVideo.IsLoaded();
		}
		#else

		return true;

		#endif
		}

		public void ShowInterstitial()
		{
		#if AdmobM

		if (this.interstitial.IsLoaded())
		{
		this.interstitial.Show();
		}
		else
		{
		MonoBehaviour.print("Interstitial is not ready yet");
		}

		#endif
		}

		public void ShowRewardBasedVideo(GoogleMobileAdControllerDelegate callBack)
		{
		#if AdmobM


		if (this.rewardBasedVideo.IsLoaded())
		{
		isRewardVideoComplete = false;
		callbackRewarVideoComplete = false;
		OnDidCloseRewardVideoSuccess = callBack;
		this.rewardBasedVideo.Show();
		}
		else
		{
		MonoBehaviour.print("Reward based video ad is not ready yet");
		}

		#endif
		}

		public void DestoryBanner()
		{
		#if AdmobM

		this.bannerView.Destroy();

		#endif
		}

		#if AdmobM


		#region Banner callback handlers

		public void HandleAdLoaded(object sender, EventArgs args)
		{
		isBannerLoaded = true;
		MonoBehaviour.print("HandleAdLoaded event received");
		}

		public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
		isBannerLoaded = false;
		MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
		}

		public void HandleAdOpened(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleAdOpened event received");
		}

		public void HandleAdClosed(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleAdClosed event received");
		}

		public void HandleAdLeftApplication(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleAdLeftApplication event received");
		}

		#endregion

		#region Interstitial callback handlers

		public void HandleInterstitialLoaded(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleInterstitialLoaded event received");
		}

		public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
		MonoBehaviour.print(
		"HandleInterstitialFailedToLoad event received with message: " + args.Message);
		}

		public void HandleInterstitialOpened(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleInterstitialOpened event received");
		}

		public void HandleInterstitialClosed(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleInterstitialClosed event received");
		}

		public void HandleInterstitialLeftApplication(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleInterstitialLeftApplication event received");
		}

		#endregion

		#region Native express ad callback handlers

		public void HandleNativeExpressAdLoaded(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleNativeExpressAdAdLoaded event received");
		}

		public void HandleNativeExpresseAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
		MonoBehaviour.print(
		"HandleNativeExpressAdFailedToReceiveAd event received with message: " + args.Message);
		}

		public void HandleNativeExpressAdOpened(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleNativeExpressAdAdOpened event received");
		}

		public void HandleNativeExpressAdClosed(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleNativeExpressAdAdClosed event received");
		}

		public void HandleNativeExpressAdLeftApplication(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleNativeExpressAdAdLeftApplication event received");
		}

		#endregion

		#region RewardBasedVideo callback handlers

		public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
		}

		public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
		MonoBehaviour.print(
		"HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
		}

		public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
		{
		isRewardVideoPlaying = true;
		MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
		}

		public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
		}

		public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
		{
		Debug.Log("HandleRewardBasedVideoClosed event received");
		isRewardVideoPlaying = false;
		callbackRewarVideoComplete = true;
		}

		public void HandleRewardBasedVideoRewarded(object sender, Reward args)
		{
		isRewardVideoComplete = true;
		string type = args.Type;
		double amount = args.Amount;
		Debug.Log("HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
		}

		public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
		{
		MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
		}

		#endregion

		#endif
		}
