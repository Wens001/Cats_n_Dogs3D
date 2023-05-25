using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LionStudios;
using LionStudios.Debugging;
using LionStudios.Runtime.Sdks;
using LionStudios.Ads;
using System;
using com.adjust.sdk;
using com.adjust.sdk.purchase;
using UnityEngine.Purchasing;
using Facebook.Unity;
using System.Runtime.InteropServices;
using Firebase.Analytics;
using System.IO;

public class SDKManager : Singleton<SDKManager>/*, IStoreListener*/
{
    private int StartLevel { get { return PlayerPrefs.GetInt("StartLevel", 0); } set { PlayerPrefs.SetInt("StartLevel", value); } }
    private int CompleteLevel { get { return PlayerPrefs.GetInt("CompleteLevel", 0); } set { PlayerPrefs.SetInt("CompleteLevel", value); } }

    private PlayerConfig playerConfig = new PlayerConfig();

    private MyTimer interstitialTimer;
    private MyTimer InterstitialStartTimer;
    private MyTimer RVInterstitialTimer;


    public void BeforeSceneLoad()
    {
        MaxSDKinit();
        LionKit.OnInitialized += () =>
        {
            AdjustInit();
            FBInit();
            FirebaseInit();
            ShowLionDebugger();

            StartCoroutine(OpenAdTrack());
        };
         
    }



    public void SDKinit()
    {
        AdsCallBackAddition();
        IAPInit();
        ABTestInit();

        Analytics.OnLogEvent += (LionGameEvent gameEvent) =>
        {
            if (FB.IsInitialized)
            {
                FB.LogAppEvent(gameEvent.eventName, parameters: gameEvent.eventParams);
                Debug.Log("触发FB_log —— " + gameEvent.eventName);
            }
            else
            {
                Debug.LogError("触发FB_log失败 —— 未初始化； 信息：" + gameEvent.eventName);
            }
        };

    }




    /// <summary>
    /// 广告初始化函数
    /// </summary>
    private void AdsCallBackAddition()
    {
        InitializeInterstitialAds();
        InitializeRewardedAds();
        BannaerAdsCallBack();
        CrossPromoCallBack();
    }

    private void Update()
    {
        interstitialTimer.OnUpdate(Time.deltaTime);
        InterstitialStartTimer.OnUpdate(Time.deltaTime);
        RVInterstitialTimer.OnUpdate(Time.deltaTime);
    }


    #region 横屏广告

    public void BannaerAdsCallBack()
    {
        // Attach callback
        //MaxSdkCallbacks.OnBannerAdLoadedEvent += OnInterstitialLoadedEvent;
        //MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
        //MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
        //MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailed;

        // Load the first interstitial
        MaxSdk.CreateBanner(LionSettings.AppLovin.BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
    }

    public void ShowBanner()
    {
        if (GameSetting.NoAds)
        {
            return;
        }
        MaxSdk.ShowBanner(LionSettings.AppLovin.BannerAdUnitId);
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(LionSettings.AppLovin.BannerAdUnitId);
    }

    private void OnBannerAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorCode)
    {
        Debug.LogError("Bannner 加载失败，错误Code：" + errorCode);
    }


    #endregion

    #region 插屏广告

    GameStatus interstitialMsg;

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayEvent;

        //applovin 的初始化中自带广告load所以不用再次load
        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(LionSettings.AppLovin.InterstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(adUnitId) will now return 'true'

        // Reset retry attempt
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorCode)
    {
        // Interstitial ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

        //retryAttempt++;
        //double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        double retryDelay = 2;

        Invoke("LoadInterstitial", (float)retryDelay);

        //Debug.LogError("插屏加载失败");
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorCode, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        LoadInterstitial();

        //打点参数
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["placement"] = interstitialMsg.ToString();
        eventParams["level"] = GameControl.Instance.CurLevel;
        eventParams["ABtest"] = ABGroupName;
        //插屏显示失败打点
        Analytics.Events.InterstitialFailedToDisplay(eventParams);
    }

    private void OnInterstitialDisplayEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

        Debug.Log("。。。插屏显示回调。。。");
    }


    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        LoadInterstitial();

        //打点参数
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["placement"] = interstitialMsg.ToString();
        eventParams["level"] = GameControl.Instance.CurLevel;
        eventParams["ABtest"] = ABGroupName;

        //关闭插屏打点
        Analytics.Events.InterstitialEnd(eventParams);

        //Debug.Log("关闭插屏");
        OnInterstitialHidden();
        interstitialTimer.ReStart();

    }


    /// <summary>
    /// 展示插屏广告
    /// </summary>
    public bool ShowInterstitial(GameStatus gameStatus = default)
    {
        interstitialMsg = gameStatus;

        //打点参数
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["placement"] = gameStatus.ToString();
        eventParams["level"] = GameControl.Instance.CurLevel;
        eventParams["ABtest"] = ABGroupName;

        //请求插屏打点
        Analytics.Events.InterstitialShowRequest(eventParams);

        Debug.Log("插屏广告加载。。。" + MaxSdk.IsInterstitialReady(LionSettings.AppLovin.InterstitialAdUnitId));

        //插屏显示机制
        if (!interstitialTimer.IsFinish || !InterstitialStartTimer.IsFinish || GameControl.Instance.CurLevel < playerConfig.InterstitialStartLevel)
        {
            Debug.Log("未达到插屏显示条件");
            return false;
        }

        //debug 模式下坚持播放，以便查看log信息；
        if ((MaxSdk.IsInterstitialReady(LionSettings.AppLovin.InterstitialAdUnitId) || GameSetting.IsDebug) && !GameSetting.NoAds)
        {
            interstitialTimer.ReStart();
            MaxSdk.ShowInterstitial(LionSettings.AppLovin.InterstitialAdUnitId);

            //插屏打点
            //Analytics.Events.InterstitialStart(eventParams);

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 关闭插屏时的操作
    /// </summary>
    private void OnInterstitialHidden()
    {
        switch (interstitialMsg)
        {
            case GameStatus.GameWin:
                UIPanelManager.Instance.PopPanel();
                UIPanelManager.Instance.PushPanel(UIPanelType.GameWinPanel);
                break;
            case GameStatus.GameFail:
                break;
            case GameStatus.GameFailReplay:
                GameControl.Instance.GameReplay();
                break;
            case GameStatus.InGameReplay:
                GameControl.Instance.GameReplay();
                break;
            default:
                break;
        }
    }

    #endregion

    #region 激励广告

    private void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        //applovin 的初始化中自带广告load所以不用再次load
        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(LionSettings.AppLovin.RewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(adUnitId) will now return 'true'

        // Reset retry attempt
    }
    //加载失败
    private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
    {
        // Rewarded ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

        //retryAttempt++;
        //double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        //Invoke("LoadRewardedAd", (float)retryDelay);
        Invoke("LoadRewardedAd", 4);

        //Debug.LogError("激励加载失败");
    }


    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        LoadRewardedAd();

        //打点参数
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["placement"] = interstitialMsg.ToString();
        eventParams["level"] = GameControl.Instance.CurLevel;
        eventParams["ABtest"] = ABGroupName;

        //激励显示失败打点
        Analytics.Events.RewardVideoFailedToDisplay(eventParams);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId) { }

    private void OnRewardedAdClickedEvent(string adUnitId) { }

    private void OnRewardedAdDismissedEvent(string adUnitId)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();

        //打点参数
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["placement"] = interstitialMsg.ToString();
        eventParams["level"] = GameControl.Instance.CurLevel;
        eventParams["ABtest"] = ABGroupName;


        //关闭激励打点
        Analytics.Events.RewardVideoEnd(eventParams);

        //Debug.Log("关闭激励");
        Messenger.Broadcast(StringMgr.CloseRewardAd);
    }

    /// <summary>
    /// 获得激励奖励
    /// </summary>
    /// <param name="adUnitId"></param>
    /// <param name="reward"></param>
    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
    {
        // Rewarded ad was displayed and user should receive the reward
        Messenger.Broadcast(StringMgr.GetReward);
    }


    /// <summary>
    /// 显示激励
    /// </summary>
    /// <param name="require">激励显示位置</param>
    /// <returns></returns>
    public bool ShowRewardedAd(string require)
    {
        if (!RVInterstitialTimer.IsFinish)
        {
            Debug.Log("未到激励加载间隔");
            return false;
        }

        //Debug模式获取奖励
        if (GameSetting.IsDebug && GameSetting.NoAds)
        {
            StartCoroutine(DelayBroastReward());
            return true;
        }

        //打点参数
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["placement"] = require;
        eventParams["level"] = GameControl.Instance.CurLevel;
        eventParams["ABtest"] = ABGroupName;


        //请求激励打点
        Analytics.Events.RewardVideoShowRequest(eventParams);

        Debug.Log("激励广告加载。。。" + MaxSdk.IsRewardedAdReady(LionSettings.AppLovin.RewardedAdUnitId));


        //debug 模式下坚持播放，以便查看log信息；
        if (MaxSdk.IsRewardedAdReady(LionSettings.AppLovin.RewardedAdUnitId) || GameSetting.IsDebug)
        {
            MaxSdk.ShowRewardedAd(LionSettings.AppLovin.RewardedAdUnitId);

            //激励播放打点
            //Analytics.Events.RewardVideoStart(eventParams);

            return true;
        }
        else
        {
            return false;
        }

    }
    private IEnumerator DelayBroastReward()
    {
        yield return new WaitForEndOfFrame();
        Messenger.Broadcast(StringMgr.GetReward);
    }


    #endregion

    #region 交叉广告

    private void CrossPromoCallBack()
    {
        //MaxSdkCallbacks.OnCrossPromoAdLoadFailedEvent += OnCrossPromoLoadFail;
        //MaxSdkCallbacks.OnCrossPromoAdCollapsedEvent += OnCrossPromoAdCollapsedEvent;
        //MaxSdkCallbacks.OnCrossPromoAdExpandedEvent += OnCrossPromoAdExpandedEvent;
    }

    private void OnCrossPromoLoadFail(string adUnitId, int errorCode)
    {
        Debug.LogError("交叉广告回调 加载失败，错误Code：" + errorCode);
    }

    private void OnCrossPromoAdCollapsedEvent(string adUnitId)
    {
        Debug.LogError("交叉广告回调 崩溃？ Collapsed");
    }

    private void OnCrossPromoAdExpandedEvent(string adUnitId)
    {
        Debug.LogError("交叉广告回调 展开？ Expanded");
    }



    public void ShowCrossPromo()
    {
        //Debug.Log("显示交叉广告");
        //CrossPromo.Show();
    }

    public void HideCrossPromo()
    {
        //Debug.Log("隐藏交叉广告");
        //CrossPromo.Hide();
    }


    #endregion

    #region 内购

    private void IAPInit()
    {
        //LionStudios.lioninapp
        IAPManager.Instance.InitializeIAPManager(
            (IAPOperationStatus status, string message, List<StoreProduct> shopProducts) =>
            {
                if (status == IAPOperationStatus.Fail)
                {
                    Debug.LogError("内购初始化失败： " + message);

                }
                else
                {
                    Debug.LogError("内购初始化成功： " + message);
                    CheckIAP();
                }
            });

    }

    public void BuyPorduct(ShopProductNames ProductName)
    {
        IAPManager.Instance.BuyProduct(ProductName,
            (IAPOperationStatus status, string message, StoreProduct product) =>
            {
                if (status == IAPOperationStatus.Success)
                {
                    OnGotProduct(ProductName);
                }
                else
                {
                    Debug.LogError("购买失败。。。" + message);
                    CheckIAP();
                }
            });
    }



    //回购
    public void ProductRestore()
    {
        IAPManager.Instance.RestorePurchases(
            (IAPOperationStatus status, string message, StoreProduct product) =>
            {
                if (status == IAPOperationStatus.Success)
                {
                    var ProductName = (ShopProductNames)Enum.Parse(typeof(ShopProductNames), product.productName);

                    switch (ProductName)
                    {
                        case ShopProductNames.NOADS:
                            OnGotProduct(ProductName);
                            break;
                        case ShopProductNames.AllInOneBundle:
                        case ShopProductNames.OneBunleSale:
                            if (!GameSetting.GotTheBundle)
                            {
                                OnGotProduct(ProductName);
                            }
                            break;
                        default:
                            break;
                    }

                    Messenger.Broadcast(StringMgr.Event.OnRestoreSuccessfully);
                }
                else
                {
                    Debug.LogError("回购失败。。。");
                }
            });


    }


    public void CheckIAP()
    {
        if (!IAPManager.Instance.IsInitialized())
        {
            Debug.LogError("检测IAP，但IAP未初始化");
            return;
        }

        if (!GameSetting.NoAds)
        {
            GameSetting.NoAds = IAPManager.Instance.IsActive(ShopProductNames.NOADS);
        }

        if (!GameSetting.GotTheBundle)
        {
            var haveGot = IAPManager.Instance.IsActive(ShopProductNames.AllInOneBundle)
                            || IAPManager.Instance.IsActive(ShopProductNames.OneBunleSale);

            if (haveGot)
            {
                GameSetting.GotTheBundle = true;
                GameSetting.CoinCount += 10000;
                SkinManager.Instance.IAPforSkins();
                Messenger.Broadcast(StringMgr.BuyTheBundle);
            }
        }
    }


    private void OnGotProduct(ShopProductNames ProductName)
    {
        switch (ProductName)
        {
            case ShopProductNames.NOADS:
                GameSetting.NoAds = true;
                break;
            case ShopProductNames.AllInOneBundle:
            case ShopProductNames.OneBunleSale:
                GameSetting.GotTheBundle = true;
                GameSetting.CoinCount += 10000;
                SkinManager.Instance.IAPforSkins();
                Messenger.Broadcast(StringMgr.BuyTheBundle);
                break;
            default:
                break;
        }
    }



    /// <summary>
    /// 购买完成回调
    /// </summary>
    /// <param name="purchaseEventArgs"></param>
    public void OnPurchaseComplete(PurchaseEventArgs purchaseEventArgs)
    {
        Debug.LogError("购买结束, 执行unity purchase回调");

        ValidateAndTrackPurchase(purchaseEventArgs, (validationState) =>
        {
            if (validationState == ADJPVerificationState.ADJPVerificationStatePassed)
            {
                //Debug.Log("Purchase is valid, do smth");
                Debug.LogError("购买结束, adjust回调时间执行中。。。");
            }
        });
    }


    #endregion


    #region Adjust about

#if UNITY_IOS
    private string Iap_purchase = "ejupli";
    private string purchase_failed = "e7jrv0";
    private string purchase_unknown = "mcy30o";
    private string purchase_notverified = "492ko8";
    private string AdjustToken = "pa0rqehgyg3k";


#else
    private string Iap_purchase = "7a47hq";
    private string purchase_failed = "mk1jar";
    private string purchase_unknown = "xuhbda";
    private string purchase_notverified = "bcvki1";
    private string AdjustToken = "xdoc3100b8xs";
#endif


    private void AdjustInit()
    {
        var adjustEnv = GameSetting.SandboxMode ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production;
        var adjustPEnv = GameSetting.SandboxMode ? ADJPEnvironment.Sandbox : ADJPEnvironment.Production;

        //adjust初始化
        var adjustConfig = new AdjustConfig(AdjustToken, adjustEnv, true);
        adjustConfig.setLogLevel(AdjustLogLevel.Info);
        adjustConfig.setSendInBackground(true);
        new GameObject("Adjust").AddComponent<com.adjust.sdk.Adjust>();
        Adjust.start(adjustConfig);


        //初始化Adjust内购 并验证
        var adjustPVConfig = new ADJPConfig(AdjustToken, adjustPEnv);
        adjustPVConfig.SetLogLevel(ADJPLogLevel.Info);
        new GameObject("AdjustPurchase").AddComponent<AdjustPurchase>();
        AdjustPurchase.Init(adjustPVConfig);

    }

    /// <summary>
    /// Adjust验证并追踪购买（打点？）
    /// </summary>
    /// <param name="purchaseEventArgs"></param>
    /// <param name="resultCallback"></param>
    public void ValidateAndTrackPurchase(PurchaseEventArgs purchaseEventArgs, Action<ADJPVerificationState> resultCallback)
    {
        var IPrice = purchaseEventArgs.purchasedProduct.metadata.localizedPrice;
        var price = decimal.ToDouble(IPrice);
        var currencyCode = purchaseEventArgs.purchasedProduct.metadata.isoCurrencyCode;
        var transactionID = purchaseEventArgs.purchasedProduct.transactionID;
        var productID = purchaseEventArgs.purchasedProduct.definition.id;
        var receiptDict = (Dictionary<string, object>)MiniJson.JsonDecode(purchaseEventArgs.purchasedProduct.receipt);
        var payload = (receiptDict != null && receiptDict.ContainsKey("Payload")) ? (string)receiptDict["Payload"] : "";


        Action<ADJPVerificationInfo> verificationCb = (verificationInfo) =>
        {
            AdjustEvent adjustEvent = null;
            switch (verificationInfo.VerificationState)
            {
                case ADJPVerificationState.ADJPVerificationStatePassed:
                    adjustEvent = new AdjustEvent(Iap_purchase);
                    adjustEvent.setRevenue(price, currencyCode);
                    adjustEvent.setTransactionId(transactionID); // in-SDK deduplication
                    adjustEvent.addCallbackParameter("productID", productID);
                    adjustEvent.addCallbackParameter("transactionID", transactionID);
                    com.adjust.sdk.Adjust.trackEvent(adjustEvent);
                    break;
                case ADJPVerificationState.ADJPVerificationStateFailed:
                    adjustEvent = new AdjustEvent(purchase_failed);
                    adjustEvent.addCallbackParameter("productID", productID);
                    adjustEvent.addCallbackParameter("transactionID", transactionID);
                    com.adjust.sdk.Adjust.trackEvent(adjustEvent);
                    break;
                case ADJPVerificationState.ADJPVerificationStateUnknown:
                    adjustEvent = new AdjustEvent(purchase_unknown);
                    adjustEvent.addCallbackParameter("productID", productID);
                    adjustEvent.addCallbackParameter("transactionID", transactionID);
                    com.adjust.sdk.Adjust.trackEvent(adjustEvent);
                    break;
                default:
                    adjustEvent = new AdjustEvent(purchase_notverified);
                    adjustEvent.addCallbackParameter("productID", productID);
                    adjustEvent.addCallbackParameter("transactionID", transactionID);
                    com.adjust.sdk.Adjust.trackEvent(adjustEvent);
                    break;
            }

            if (resultCallback != null)
            {
                resultCallback(verificationInfo.VerificationState.Value);
            }

        };


#if UNITY_IOS
        AdjustPurchase.VerifyPurchaseiOS(payload, transactionID, productID, verificationCb);
#elif UNITY_ANDROID

        var jsonDetailsDict = (!string.IsNullOrEmpty(payload)) ? (Dictionary<string, object>)MiniJson.JsonDecode(payload) : null;
        var json = (jsonDetailsDict != null && jsonDetailsDict.ContainsKey("json")) ? (string)jsonDetailsDict["json"] : "";
        var gpDetailsDict = (!string.IsNullOrEmpty(json)) ? (Dictionary<string, object>)MiniJson.JsonDecode(json) : null;
        var purchaseToken = (gpDetailsDict != null && gpDetailsDict.ContainsKey("purchaseToken")) ? (string)gpDetailsDict["purchaseToken"] : "";

        AdjustPurchase.VerifyPurchaseAndroid(productID, purchaseToken, "", verificationCb);
#endif
    }

    public string TryGetAdjustIdfa()
    {
        return Adjust.getIdfa();
    }


    #endregion

    #region FB About


    private void FBInit()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            FB.Init(() =>
            {
                FB.ActivateApp();
            }
            );
        }
    }




    #endregion

    #region Firebase

    private void FirebaseInit()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // subscribe to firebase events
                // subscribe here so avoid error if dependency check fails
                Debug.Log("firebase 依赖添加 : " + FirebaseVersion);
                //LionStudios.Analytics.OnLogEvent += LogFirebaseEvent;
                //LionStudios.Analytics.OnLogEventUA += LogUAFirebaseEvent;
            }
            else
            {
                Debug.LogError($"Firebase: Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });


    }



    Dictionary<string, object> firebasePkgJson;
    public string FirebaseVersion
    {
        get
        {
            const string default_ver = "7.2.0";

            // get firebase version
            if (firebasePkgJson == null)
            {
                string jsonPath = Path.GetFullPath(
                    Path.Combine("Packages", "com.google.firebase.app", "package.json"));

                if (File.Exists(jsonPath))
                {
                    firebasePkgJson =
                        MiniJson.JsonDecode(File.ReadAllText(jsonPath)) as Dictionary<string, object>;

                }
                else
                {
                    string path = Path.Combine(Application.dataPath, "Firebase", "Plugins");

                    if (Directory.Exists(path))
                    {
                        return default_ver;
                    }
                }
            }

            if (firebasePkgJson != null)
            {
                return firebasePkgJson.ContainsKey("version")
                    ? firebasePkgJson["version"] as string
                    : default_ver;
            }
            else
            {
                return "7.1.0";
            }

            return "- - -";
        }
    }



    #endregion


    #region 关卡打点

    public void OnLevelStart(int level)
    {
        if (StartLevel < level)
        {
            Dictionary<string, object> eventParams = new Dictionary<string, object>();
            eventParams["level"] = level;
            eventParams["ABtest"] = ABGroupName;

            Analytics.Events.LevelStarted(eventParams);
            StartLevel = level;
            Debug.Log("游戏开始打点：" + level);
        }
    }

    public void OnLevelComplete(int level)
    {
        if (CompleteLevel < level)
        {
            Dictionary<string, object> eventParams = new Dictionary<string, object>();
            eventParams["level"] = level;
            eventParams["ABtest"] = ABGroupName;


            Analytics.Events.LevelComplete(eventParams);
            CompleteLevel = level;
            Debug.Log("游戏完成打点：" + level);

        }
    }

    public void OnLevelFailed(int level)
    {
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["level"] = level;
        eventParams["ABtest"] = ABGroupName;

        Analytics.Events.LevelFailed(eventParams);
        Debug.Log("游戏失败打点：" + level);
    }

    public void OnlevelRestart(int level, string gameState)
    {
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["level"] = level;
        eventParams["gameState"] = gameState;
        eventParams["ABtest"] = ABGroupName;

        Analytics.Events.LevelRestart(eventParams);

        Debug.Log("游戏重开打点：" + level);
    }

    public void OnlevelSkip(int level, string gameState)
    {
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["level"] = level;
        eventParams["gameState"] = gameState;
        eventParams["ABtest"] = ABGroupName;

        Analytics.Events.LevelSkipped(eventParams);

        Debug.Log("游戏跳关打点：" + level);
    }

    /***********  奖励关打点  **************/
    public void OnBonusLeveStart(int level)
    {
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["level"] = level;
        eventParams["ABtest"] = ABGroupName;

        Analytics.LogEvent("Bonus level_start", eventParams);

        Debug.Log("奖励关开始打点——关卡：" + level);
    }

    public void OnBonusLevelComplete(int level, int BoxNum)
    {
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["level"] = level;
        eventParams["ABtest"] = ABGroupName;
        eventParams["number"] = BoxNum;

        Analytics.LogEvent("Bonus level_complete", eventParams);

        Debug.Log("奖励关完成打点——关卡：" + level + "报箱数：" + BoxNum);
    }

    #endregion


    #region AB Test

    //打点用
    public string ABGroupName
    {
        get => PlayerPrefs.GetString("ab_experiment_group", playerConfig.ab_experiment_group);
    }

    //游戏内逻辑用
    public bool OldLevel { get => false; /* PlayerPrefs.GetInt("level_optimized", playerConfig.level_optimized) == 0;*/ }

    public bool AdsFofBonus { get => PlayerPrefs.GetInt("bonuslevel_optimized", playerConfig.bonuslevel_optimized) == 1; }

    public bool HaveBonus { get =>/* true*/PlayerPrefs.GetInt("bonuslevel", playerConfig.bonuslevel) == 1; }
    /// <summary>
    /// 幸运皮肤显示间隔
    /// </summary>
    public int SkinRv_freq { get => PlayerPrefs.GetInt("skinRV_freq", playerConfig.skinRV_freq); }

    public bool HaveFarmSystem { get => PlayerPrefs.GetInt("upgrade_meta", playerConfig.upgrade_meta) == 1; }


    //ab分组显示用
    public ABGroup abGroup = ABGroup.a;




    private void ABTestInit()
    {
        //初始赋值
        interstitialTimer = new MyTimer(playerConfig.interstitialTimer);
        InterstitialStartTimer = new MyTimer(playerConfig.InterstitialStartTimer);
        RVInterstitialTimer = new MyTimer(playerConfig.RVInterstitialTimer);

        ABTestConfig();     //加载默认ab分组
        AppLovin.WhenInitialized(() =>
        {
            AppLovin.LoadRemoteData(playerConfig);
            Debug.LogError("AppLovin初始化回调 获取广告机制数据" +
                "\n ABTest分组名为： " + playerConfig.ab_experiment_group +
                "\n 庄园参数为： " + playerConfig.upgrade_meta
                 );
            GetABTestGroup(playerConfig);

            interstitialTimer.ResetTimer(playerConfig.interstitialTimer);
            InterstitialStartTimer.ResetTimer(playerConfig.InterstitialStartTimer);
            RVInterstitialTimer.ResetTimer(playerConfig.RVInterstitialTimer);
        });

    }

    private void ABTestConfig()
    {
        abGroup = (ABGroup)Enum.Parse(typeof(ABGroup), playerConfig.ab_experiment_group.Split('_')[0]);
    }

    private void GetABTestGroup(PlayerConfig playerConfig)
    {
        PlayerPrefs.SetString("ab_experiment_group", playerConfig.ab_experiment_group);
        PlayerPrefs.SetInt("level_optimized", playerConfig.level_optimized);
        PlayerPrefs.SetInt("bonuslevel", playerConfig.bonuslevel);
        PlayerPrefs.SetInt("skinRV_freq", playerConfig.skinRV_freq);
        PlayerPrefs.SetInt("upgrade_meta", playerConfig.upgrade_meta);

        ABTestConfig();
    }

    //debug模式-手动切换分组
    public void ChangeABGroup()
    {
        switch (abGroup)
        {
            //切换b组
            case ABGroup.a:
#if UNITY_ANDROID
                playerConfig.level_optimized = 1;
                playerConfig.ab_experiment_group = "b_0427";
                playerConfig.upgrade_meta = 1;
#elif UNITY_IOS
                playerConfig.ab_experiment_group = "b_0415";
                playerConfig.bonuslevel = 1;
                playerConfig.skinRV_freq = 3;
#endif
                break;

            //ios——切换c组；android——切换a组
            case ABGroup.b:
#if UNITY_ANDROID
                playerConfig.level_optimized = 1;
                playerConfig.ab_experiment_group = "a_0427";
                playerConfig.upgrade_meta = 0;
#elif UNITY_IOS
                playerConfig.ab_experiment_group = "c_0415";
                playerConfig.bonuslevel = 1;
                playerConfig.skinRV_freq = 4;
#endif
                break;

            //iOS——切换A组
            case ABGroup.c:
#if UNITY_IOS
                playerConfig.ab_experiment_group = "a_0415";
                playerConfig.bonuslevel = 0;
                playerConfig.skinRV_freq = 3;
#endif
                break;

            default:
                break;
        }


        GetABTestGroup(playerConfig);
    }

    #endregion


    #region LionDebugger

    private void ShowLionDebugger()
    {
#if !UNITY_EDITOR

        if (GameSetting.IsDebug && !LionDebugger.IsShowing())
        {
            LionDebugger.Show(true);
        }
#endif
    }

    #endregion



    public void OnSkinUnlock(string SkinName)
    {
        Dictionary<string, object> EventParams = new Dictionary<string, object>();
        EventParams["SkinName"] = SkinName;
        EventParams["ABtest"] = ABGroupName;

        Analytics.LogEvent("Skin_unlocked", EventParams);
    }






    static MaxSdkBase.SdkConfiguration maxSDKConfig;
    /// <summary>
    /// maxSDK初始化回调方法。
    /// 为了通过maxSDK获取设备信息打开广告追踪，需要确保maxSDK初始化完成
    /// 对于lionSDK初始化，需在打开广告追踪前完成。
    /// </summary>
    void MaxSDKinit()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR

            //判定iOS版本
            var result = MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5");
            if (result != MaxSdkUtils.VersionComparisonResult.Lesser)
            {
                // Note that App transparency tracking authorization can be checked via `sdkConfiguration.AppTrackingStatus` for Unity Editor and iOS targets
                // 1. Set Facebook ATE flag here, THEN
                //FB.Mobile.SetAdvertiserTrackingEnabled(sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized);             
                //this.AttachTimer(1, () =>
                // {
                //     AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized);
                //     Debug.Log("打开数据追踪");
                // });
                /** 使用新的API代替以上方法 **/

                GameSetting.IOS_HighVersion = true;

                Debug.Log("Adjust IDFA: " + SDKManager.Instance.TryGetAdjustIdfa());
            }
#endif
        };
    }


    /// <summary>
    /// 打开广告追踪
    /// <para>需在lionkit初始化后执行</para>
    /// <para>需在maxSDK完成后以便获取设备信息</para>
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenAdTrack()
    {
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR
        if (GameSetting.IOS_HighVersion)
        {
            while (!MaxSdkUnityEditor.IsInitialized())
            {
                yield return new WaitForEndOfFrame();
            }

            var sdkConfiguration = MaxSdkUnityEditor.GetSdkConfiguration();
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized);
            Debug.Log("打开数据追踪");
        }
#endif
    }


}



public class PlayerConfig
{
    public float interstitialTimer = 15f;
    public float RVInterstitialTimer = 0;
    public float InterstitialStartTimer = 30;
    public float InterstitialStartLevel = 2;

#if UNITY_ANDROID
    public string ab_experiment_group = "a_0427";
#elif UNITY_IOS
    public string ab_experiment_group = "a_0514";
#endif

    public int level_optimized = 1;     //旧关卡优化AB组
    public int bonuslevel = 1;
    public int skinRV_freq = 3;
    public int upgrade_meta = 0;

    public int bonuslevel_optimized = 0;    //奖励关ab组
}

public enum ABGroup
{
    a,
    b,
    c
}

namespace AudienceNetwork
{
    public static class AdSettings
    {
        [DllImport("__Internal")]
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
        }
    }

}