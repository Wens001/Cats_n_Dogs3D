using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class GamePlayPanel : BasePanel
{
    public Text curLevelText;
    public Image BonusTitleImage;
    public Text CoinText;

    [Header("准备界面UI组件")]
    public Transform OutGameTrans;
    public Button SettingButton;
    public Button ShopButton;
    public Button FarmButton;
    public Button NoAdsButton;
    public Button IAPShopButton;
    public Image DragAndPlay;
    public Image HoldAndDrag;


    [Header("游戏内UI组件")]
    public Transform InGameTrans;
    public Button ReplayButton;
    public Button SkipButton;

    public Image CatControlImage;
    public Image DogControlImage;
    public Image GuideHandImage;

    //计时器
    public Transform levelTimerTrans;
    public Image LevelTimerFillImage;
    public Text LevelTimerText;

    Image dogImage;
    Image catImage;

    [Header("提示气泡")]
    public RectTransform CatWant;
    public RectTransform DogWant;
    public RectTransform GiftShow;
    public Sprite BitItSprite;
    Image CatNeedImage;
    Image DogNeedImage;


    public List<RectTransform> OtherHints = new List<RectTransform>();
    private Dictionary<GameObject, RectTransform> HintsShowDic = new Dictionary<GameObject, RectTransform>();

    [Header("debug按钮")]
    public Button debugButton;

    bool dragPlayListen = false;


    public override void OnEnter()
    {
        PanelConfig();
        gameObject.SetActive(true);
        Messenger.AddListener(StringMgr.BouthDeathLock, OnHeadsDeathLock);
        Messenger.AddListener<SlingShotBall>(StringMgr.BallShotGuide, BallShotGuide);
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.ShowBitIt, ShowBitItSprite);

        //提示
        Messenger.AddListener<CatOrDog, Sprite>(StringMgr.HintBroadcast, ShowCatOrDoglWant);
        Messenger.AddListener<CatOrDog>(StringMgr.HideHint, HideCatOrDogWant);
        Messenger.AddListener<GameObject, Sprite>(StringMgr.otherHintBroadcast, OnShowOtherHint);
        Messenger.AddListener<GameObject>(StringMgr.otherHideHint, OnHideOtherHint);
        Messenger.AddListener<Transform, Sprite>(StringMgr.ShowGiftImage, OnShowGift);
        Messenger.AddListener(StringMgr.HideGiftImage, HideGift);

        Messenger.AddListener<CatOrDog>(StringMgr.ShowControlImage, ShowControlImage);
        Messenger.AddListener<int>(StringMgr.CoinCountChange, OnCoinChange);

        //激励广告
        Messenger.AddListener(StringMgr.GetReward, OnGetReward);
        Messenger.AddListener(StringMgr.CloseRewardAd, OnRewardAdClose);

        //奖励关初始化
        Messenger.AddListener<int>(StringMgr.BonusLevelInit, BonusLevelInit);



        SDKManager.Instance.ShowBanner();
    }

    public override void OnExit()
    {
        SDKManager.Instance.HideBanner();
        levelTimer = null;
        levelTimerTrans.gameObject.SetActive(false);
        gameObject.SetActive(false);


        Messenger.RemoveListener(StringMgr.BouthDeathLock, OnHeadsDeathLock);
        Messenger.RemoveListener<SlingShotBall>(StringMgr.BallShotGuide, BallShotGuide);

        //提示
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.ShowBitIt, ShowBitItSprite);
        Messenger.RemoveListener<CatOrDog, Sprite>(StringMgr.HintBroadcast, ShowCatOrDoglWant);
        Messenger.RemoveListener<CatOrDog>(StringMgr.HideHint, HideCatOrDogWant);
        Messenger.RemoveListener<GameObject, Sprite>(StringMgr.otherHintBroadcast, OnShowOtherHint);
        Messenger.RemoveListener<GameObject>(StringMgr.otherHideHint, OnHideOtherHint);
        Messenger.RemoveListener<Transform, Sprite>(StringMgr.ShowGiftImage, OnShowGift);
        Messenger.RemoveListener(StringMgr.HideGiftImage, HideGift);

        Messenger.RemoveListener<CatOrDog>(StringMgr.ShowControlImage, ShowControlImage);
        Messenger.RemoveListener<int>(StringMgr.CoinCountChange, OnCoinChange);


        //激励广告
        Messenger.RemoveListener(StringMgr.GetReward, OnGetReward);
        Messenger.RemoveListener(StringMgr.CloseRewardAd, OnRewardAdClose);

        //奖励关初始化
        Messenger.RemoveListener<int>(StringMgr.BonusLevelInit, BonusLevelInit);

        //
        HintsShowDic.Clear();
    }

    #region 特殊委托增删
    private void Awake()
    {
        //猫狗
        Messenger.AddListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, OnCatNDogInit);
    }


    private void OnDestroy()
    {
        //猫狗
        Messenger.RemoveListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, OnCatNDogInit);
    }
    #endregion



    public override void OnPause()
    {
        GameControl.Instance.PauseGame();
        //HintsShowDic.Clear();

        //主要用于皮肤商店界面隐藏大礼包按钮
        OutGameTrans.gameObject.SetActive(false);


        SDKManager.Instance.HideBanner();
    }

    public override void OnResume()
    {
        GameControl.Instance.ResumeGame();
        ShowSkinHint();
        IAPShopButton.gameObject.SetActive(!GameSetting.GotTheBundle);
        NoAdsButton.gameObject.SetActive(!GameSetting.NoAds);
        
        //主要用于皮肤商店界面隐藏大礼包按钮
        OutGameTrans.gameObject.SetActive(GameControl.Instance.GameProcess != GameProcess.InGame);


        SDKManager.Instance.ShowBanner();
    }


    private void Update()
    {
        if (GameControl.Instance.GameProcess == GameProcess.PauseGame)
        {
            return;
        }

        /*********  点击开始  ****************/
        if (dragPlayListen && Input.GetMouseButtonDown(0) && !EventSystem.current.currentSelectedGameObject)
        {
            StartGame();
        }

        /************  计时器计时  ********************/
        LeveTiemrUpdate();

        /*** 隐藏手指引导 ***/
        handHideTimer.OnUpdate(Time.deltaTime);
        if (handHideListen && handHideTimer.IsFinish /*Input.GetMouseButtonDown(0)*/)
        {
            handHideListen = false;
            GuideHandImage.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            OtherFollow();
            GiftBGShow();
        }

    }


    private void StartGame()
    {
        DragPlaySet(false);
        GameControl.Instance.StartGame();

        SDKManager.Instance.ShowBanner();
        SDKManager.Instance.OnLevelStart(GameControl.Instance.CurLevel);

        //再次确认摇杆位置
        CatControlImage.rectTransform.position = catImagePosition;
        DogControlImage.rectTransform.position = dogImagePosition;

        //第一关开始时切换正常视角
        if (GameControl.Instance.CurLevelIndex == 1)
        {
            CameraControl.Instance.ChangeCamera(GameControl.Instance.levelSetting.cameraType);
        }

        //切换双指引导   （旧关卡只有第五关-足球,有双指引导）
        if (GameControl.Instance.levelSetting.guidType == GuidType.DoubleHandGuid)
        {
            UIPanelManager.Instance.PushPanel(UIPanelType.DoubleHandGuidPanel);

            //if ((SDKManager.Instance.OldLevel && GameControl.Instance.CurLevel == 5) || !SDKManager.Instance.OldLevel)
            //{
            //    UIPanelManager.Instance.PushPanel(UIPanelType.DoubleHandGuidPanel);
            //}
        }
        //重玩判定
        if (!GameControl.Instance.BonusLevel)
        {
            GameSetting.IsReplay = true;
        }
        //游戏开始时更改视角
        if (GameControl.Instance.levelSetting.SecondType != CameraType.None)
        {
            CameraControl.Instance.ChangeCamera(GameControl.Instance.levelSetting.SecondType);
        }

        Messenger.Broadcast(StringMgr.GameStart);
    }


    bool haveInit = false;
    Vector2 catImagePosition;
    Vector2 dogImagePosition;
    private void PanelInit()
    {
        if (haveInit)
        {
            return;
        }
        haveInit = true;

        //catImagePosition = CatControlImage.rectTransform.position;
        //dogImagePosition = DogControlImage.rectTransform.position;
        catImagePosition = new Vector2(Screen.width * .75f, Screen.height * .25f);
        dogImagePosition = new Vector2(Screen.width * .25f, Screen.height * .25f);


        dogImage = DogControlImage.transform.GetChild(0).GetComponent<Image>();
        catImage = CatControlImage.transform.GetChild(0).GetComponent<Image>();

        CatNeedImage = CatWant.GetChild(1).GetComponent<Image>();
        DogNeedImage = DogWant.GetChild(1).GetComponent<Image>();

        TouchesManager touchesManager = GameControl.Instance.gameObject.GetComponent<TouchesManager>();
        if (touchesManager)
        {
            touchesManager.GamePlayPanelM = this;
        }

        //游戏内重玩
        ReplayButton.onClick.AddListener(InGameReplay);

        //游戏内跳关
        SkipButton.onClick.AddListener(InGameSkip);

        //设置按钮
        SettingButton.onClick.AddListener(() => { UIPanelManager.Instance.PushPanel(UIPanelType.SettingPanel); });

        //商店按钮
        ShopButton.onClick.AddListener(() =>
        {
            GameControl.Instance.LoadModelShowScene();
            UIPanelManager.Instance.PushPanel(UIPanelType.SkinShopPanel);
        });

        //农场按钮
        FarmButton.onClick.AddListener(() => {
            UIPanelManager.Instance.PopPanel();
            GameControl.Instance.LoadFarmScene();
        });

        //大礼包按钮
        IAPShopButton.onClick.AddListener(() => UIPanelManager.Instance.PushPanel(UIPanelType.IAPShopPanel));

        //去广告按钮
        NoAdsButton.onClick.AddListener(() =>{
            OnNoAdsButtonClick();
        });


        //debug按钮操作
        debugButton.onClick.AddListener(() => { UIPanelManager.Instance.PushPanel(UIPanelType.DebugSettingPanel); });
        debugButton.enabled = GameSetting.IsDebug;


    }

    private void PanelConfig()
    {
        PanelInit();
        DragPlaySet(true);
        SDKManager.Instance.CheckIAP();

        CoinText.text = GameSetting.CoinCount.ToString();

        GuideHandImage.gameObject.SetActive(false);

        //关卡显示配置
        curLevelText.text = "LEVEL " + GameControl.Instance.CurLevel.ToString();
        //摇杆配置
        CatControlImage.rectTransform.position = catImagePosition;
        DogControlImage.rectTransform.position = dogImagePosition;
        catImage.transform.localPosition = Vector3.zero;
        dogImage.transform.localPosition = Vector3.zero;
        CatControlImage.gameObject.SetActive(true);
        DogControlImage.gameObject.SetActive(true);
        //隐藏对话气泡
        CatWant.gameObject.SetActive(false);
        DogWant.gameObject.SetActive(false);
        foreach (var hint in OtherHints)
        {
            hint.gameObject.SetActive(false);
        }
        GiftShow.gameObject.SetActive(false);

        //商店按钮提示
        ShowSkinHint();

        haveDeathLock = false;
        //开关debug按钮
        debugButton.enabled = GameSetting.IsDebug;

        //去广告按钮
        NoAdsButton.gameObject.SetActive(!GameSetting.NoAds && GameControl.Instance.CurLevel > 1);
        IAPShopButton.gameObject.SetActive(!GameSetting.GotTheBundle && GameControl.Instance.CurLevel > 1);


        BonusLevelUIConfig();
    }


    private void DragPlaySet(bool dragActive)
    {

        OutGameTrans.gameObject.SetActive(dragActive);
        InGameTrans.gameObject.SetActive(!dragActive);
        dragPlayListen = dragActive;

        CatControlImage.gameObject.SetActive(!dragActive & !haveDeathLock);
        DogControlImage.gameObject.SetActive(!dragActive & !haveDeathLock);
    }


    #region 皮肤/农场提示

    /// <summary>
    /// 显示是否够金币买皮肤  & 是否有作物可收获
    /// </summary>
    private void ShowSkinHint()
    {
        Transform hint = ShopButton.transform.GetChild(0);
        hint.gameObject.SetActive(false);
        foreach (var skin in SkinManager.Instance.skinsInfo)
        {
            if (skin.getWay == GetWay.CoinBuy && GameSetting.CoinCount >= skin.CoinCost && !SkinManager.Instance.CheckSkinGot(skin))
            {
                hint.gameObject.SetActive(true);
                break;
            }
        }


        //农场收获提示

        FarmButton.gameObject.SetActive(SDKManager.Instance.HaveFarmSystem && GameControl.Instance.CurLevel > 4);
        Transform farmHint = FarmButton.transform.GetChild(0);
        farmHint.gameObject.SetActive(FarmTimeMgr.HasHarvest());
    }

    #endregion


    #region CallBack


    bool SkipRewardListen = false;
    /// <summary>
    /// 获取激励奖励
    /// </summary>
    public void OnGetReward()
    {
        //激励
        if (SkipRewardListen)
        {
            SkipRewardListen = false;

            GameControl.Instance.LoadNextLevel();

            //跳关打点
            SDKManager.Instance.OnlevelSkip(GameControl.Instance.CurLevel, "InGame");
        }
    }
    public void OnRewardAdClose()
    {
        SkipRewardListen = false;

    }



    MyTimer handHideTimer = new MyTimer(2f);
    bool handHideListen = false;
    /// <summary>
    /// 保龄球引导
    /// </summary>
    /// <param name="ball"></param>
    private void BallShotGuide(SlingShotBall ball)
    {
        GuideHandImage.transform.position = Camera.main.WorldToScreenPoint(ball.transform.position);
        GuideHandImage.gameObject.SetActive(true);
        float pointY = GuideHandImage.transform.position.y;

        GuideHandImage.transform.DOMoveY(pointY - 200f, .5f).SetLoops(-1, LoopType.Yoyo);
        handHideListen = true;

        handHideTimer.ReStart();
    }


    bool haveDeathLock = false;
    /// <summary>
    /// 锁死时，隐藏摇杆
    /// </summary>
    private void OnHeadsDeathLock()
    {
        haveDeathLock = true;
        CatControlImage.gameObject.SetActive(false);
        DogControlImage.gameObject.SetActive(false);
    }


    private void OnCoinChange(int count)
    {
        CoinText.text = count.ToString();
    }

    //dragPlay 和 HoldDrag 的显示判断
    private void OnCatNDogInit(TouchMove cat, TouchMove dog)
    {
        //优化，当前关卡为保龄球关卡时，显示 holdDrag
        bool holdDragShow = FindObjectOfType<SlingShot>();
        HoldAndDrag.gameObject.SetActive(holdDragShow);
        DragAndPlay.gameObject.SetActive(!holdDragShow);

    }


    #endregion


    #region UI组件事件

    private void OnNoAdsButtonClick()
    {
        SDKManager.Instance.BuyPorduct(ShopProductNames.NOADS);
    }


    private void InGameReplay()
    {
        //重玩打点
        SDKManager.Instance.OnlevelRestart(GameControl.Instance.CurLevel, "Game_Replay");

        if (SDKManager.Instance.ShowInterstitial(GameStatus.InGameReplay) == false)
        {
            GameControl.Instance.GameReplay();
        }
    }


    private void InGameSkip()
    {
        if (SDKManager.Instance.ShowRewardedAd("InGame_Skip"))
        {
            SkipRewardListen = true;
        }
    }

    #endregion


    #region 提示气泡

    private void ShowCatOrDoglWant(CatOrDog animalType, Sprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        DOTween.Kill(num);
        switch (animalType)
        {
            case CatOrDog.Default:
                CatNeedImage.sprite = sprite;
                DogNeedImage.sprite = sprite;
                CatWant.gameObject.SetActive(true);
                DogWant.gameObject.SetActive(true);
                break;
            case CatOrDog.Cat:
                CatNeedImage.sprite = sprite;
                CatWant.gameObject.SetActive(true);
                break;
            case CatOrDog.Dog:
                DogNeedImage.sprite = sprite;
                DogWant.gameObject.SetActive(true);
                break;
            default:
                break;
        }

        CatNeedImage.SetNativeSize();
        DogNeedImage.SetNativeSize();
    }

    private void HideCatOrDogWant(CatOrDog animalType)
    {
        switch (animalType)
        {
            case CatOrDog.Default:
                CatWant.gameObject.SetActive(false);
                DogWant.gameObject.SetActive(false);
                break;
            case CatOrDog.Cat:
                CatWant.gameObject.SetActive(false);
                break;
            case CatOrDog.Dog:
                DogWant.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    int num;
    /// <summary>
    /// 监听，当咬住时显示“BitIt”
    /// </summary>
    /// <param name="lockObject"></param>
    /// <param name="head"></param>
    private void ShowBitItSprite(LockObjectBase lockObject, TouchMove head)
    {
        if (lockObject is WinCondition || GameControl.Instance.GameProcess == GameProcess.OutGame)
        {
            return;
        }
        ShowCatOrDoglWant(head.selfType, BitItSprite);

        num = 0;
        DOTween.To(() => num, x => num = x, 100, 1f).OnComplete(() =>
        {
            HideCatOrDogWant(head.selfType);
        });
    }


    /// <summary>
    /// 猫狗气泡跟随
    /// </summary>
    /// <param name="cat"></param>
    /// <param name="dog"></param>
    public void CatOrDogWantFollow(TouchMove cat, TouchMove dog)
    {
        CatWant.transform.position = Camera.main.WorldToScreenPoint(cat.transform.position);
        DogWant.transform.position = Camera.main.WorldToScreenPoint(dog.transform.position);
    }

    /******其他提示*********/

    private void OnShowOtherHint(GameObject FollowObj, Sprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        if (HintsShowDic.ContainsKey(FollowObj))
        {
            var image = HintsShowDic[FollowObj].GetChild(1).GetComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize();
            return;
        }

        foreach (var otherHint in OtherHints)
        {
            if (!otherHint.gameObject.activeSelf)
            {
                var image = otherHint.GetChild(1).GetComponent<Image>();
                image.sprite = sprite;
                image.SetNativeSize();
                otherHint.gameObject.SetActive(true);

                HintsShowDic.Add(FollowObj, otherHint);

                break;
            }
        }
    }

    private void OnHideOtherHint(GameObject FollowObj)
    {
        if (HintsShowDic.ContainsKey(FollowObj))
        {
            HintsShowDic[FollowObj].gameObject.SetActive(false);
            HintsShowDic.Remove(FollowObj);
        }
    }

    private void OtherFollow()
    {
        foreach (var item in HintsShowDic)
        {
            //ChickWant.transform.position = Camera.main.WorldToScreenPoint(Chick.transform.position);
            item.Value.transform.position = Camera.main.WorldToScreenPoint(item.Key.transform.position);

        }
    }


    /******* 礼物展示 **********/

    private Transform giftTrans;
    private MyTimer GiftShowTimer = new MyTimer(3);


    private void OnShowGift(Transform theGift, Sprite sprite)
    {
        if (theGift == null || sprite == null)
        {
            return;
        }

        giftTrans = theGift;
        GiftShow.transform.position = Camera.main.WorldToScreenPoint(giftTrans.position);
        GiftShow.GetChild(1).GetComponent<Image>().sprite = sprite;

        GiftShow.gameObject.SetActive(true);
        GiftShowTimer.ReStart();
    }

    private void HideGift()
    {
        giftTrans = null;
        GiftShow.gameObject.SetActive(false);
    }


    private Transform GiftBG;
    private float spd = 30;
    private void  GiftBGShow()
    {
        if (GiftBG == null)
        {
            GiftBG = GiftShow.GetChild(0);
            return;
        }

        if (giftTrans != null)
        {
            GiftBG.Rotate(Vector3.forward, spd * Time.deltaTime, Space.World);
            GiftShow.position = Camera.main.WorldToScreenPoint(giftTrans.transform.position);
            GiftShowTimer.OnUpdate(Time.deltaTime);
            if (GiftShowTimer.IsFinish)
            {
                HideGift();
            }
        }

    }

    //private void  Gif

    #endregion


    #region 摇杆

    /******* Touch ***********/
    public void OnTouchEnter(CatOrDog animalType, Touch touch)
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        if (obj)
        {
            //Debug.Log(obj.name);
            return;
        }

        switch (animalType)
        {
            case CatOrDog.Cat:
                CatControlImage.rectTransform.position = touch.position;
                break;
            case CatOrDog.Dog:
                DogControlImage.rectTransform.position = touch.position;
                break;
            default:
                break;
        }
    }


    public void OnTouchHold(CatOrDog animalType, Touch touch)
    {
        switch (animalType)
        {
            case CatOrDog.Cat:
                catImage.transform.position = touch.position;
                catImage.transform.localPosition = Vector3.ClampMagnitude(catImage.transform.localPosition, 90f);
                break;
            case CatOrDog.Dog:
                dogImage.transform.position = touch.position;
                dogImage.transform.localPosition = Vector3.ClampMagnitude(dogImage.transform.localPosition, 90f);
                break;
            default:
                break;
        }
    }


    public void OnTouchEnd(CatOrDog animalType)
    {
        switch (animalType)
        {
            case CatOrDog.Cat:
                catImage.transform.localPosition = Vector3.zero;
                CatControlImage.rectTransform.position = catImagePosition;
                break;
            case CatOrDog.Dog:
                dogImage.transform.localPosition = Vector3.zero;
                DogControlImage.rectTransform.position = dogImagePosition;
                break;
            default:
                break;
        }
    }


    /***** Mouse ********/
    public void OnMouseClickDown(CatOrDog animalType)
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        if (obj)
        {
            return;
        }


        switch (animalType)
        {
            case CatOrDog.Cat:
                CatControlImage.rectTransform.position = Input.mousePosition;
                CatControlImage.gameObject.SetActive(true);
                //CatControlImage.rectTransform.position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                break;
            case CatOrDog.Dog:
                DogControlImage.rectTransform.position = Input.mousePosition;
                DogControlImage.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void OnMouseHold(CatOrDog animalType)
    {
        switch (animalType)
        {
            case CatOrDog.Cat:
                catImage.transform.position = Input.mousePosition;
                catImage.transform.localPosition = Vector3.ClampMagnitude(catImage.transform.localPosition, 90f);
                break;
            case CatOrDog.Dog:
                dogImage.transform.position = Input.mousePosition;
                dogImage.transform.localPosition = Vector3.ClampMagnitude(dogImage.transform.localPosition, 90f);
                break;
            default:
                break;
        }
    }

    public void OnMouseClickUp(CatOrDog animalType)
    {
        switch (animalType)
        {
            case CatOrDog.Cat:
                catImage.transform.localPosition = Vector3.zero;
                CatControlImage.rectTransform.position = catImagePosition;
                break;
            case CatOrDog.Dog:
                dogImage.transform.localPosition = Vector3.zero;
                DogControlImage.rectTransform.position = dogImagePosition;
                break;
            default:
                break;
        }
    }

    //显示摇杆
    public void ShowControlImage(CatOrDog catOrDog)
    {

        switch (catOrDog)
        {
            case CatOrDog.Cat:
                CatControlImage.gameObject.SetActive(true);
                break;
            case CatOrDog.Dog:
                DogControlImage.gameObject.SetActive(true);
                break;
            default:
                CatControlImage.gameObject.SetActive(true);
                DogControlImage.gameObject.SetActive(true);
                break;
        }

    }


    #endregion


    #region 奖励关卡计时 显示
    private MyTimer levelTimer;
    private MyTimer SecondTimer = new MyTimer(1);

    /// <summary>
    /// 奖励关，在bonusMgr的start周期和加时间时播放信息(显示计时）
    /// </summary>
    /// <param name="LevelTime"></param>
    private void BonusLevelInit(int  LevelTime)
    {
        //levelTimerTrans.gameObject.SetActive(bonusProperty != null);

        if (LevelTime  > 0)
        {
            levelTimer = new MyTimer(LevelTime);
            levelTimer.ReStart();

            LevelTimerFillImage.fillAmount = 1;
            LevelTimerText.text = LevelTime.ToString();
            levelTimerTrans.gameObject.SetActive(true);
        }
    }


    private void LeveTiemrUpdate()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame && levelTimer != null && !levelTimer.IsFinish)
        {
            levelTimer.OnUpdate(Time.deltaTime);
            SecondTimer.OnUpdate(Time.deltaTime);

            LevelTimerFillImage.fillAmount = levelTimer.GetRatioRemaining;
            if (SecondTimer.IsFinish)
            {
                LevelTimerText.text = "00:" + (levelTimer.DurationTime - (int)levelTimer.timer).ToString("00");
            }

            if (levelTimer.IsFinish)
            {
                //GameControl.Instance.GameFail();
                GameControl.Instance.GotAllBonus = false;
                UIPanelManager.Instance.PushPanel(UIPanelType.BonusOverPanel);

                //
                AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.TimeUpClip);
            }
        }

    }

    /// <summary>
    /// 奖励关主界面配置
    /// </summary>
    private void BonusLevelUIConfig()
    {
        var bonusLevel = GameControl.Instance.BonusLevel;

        ReplayButton.gameObject.SetActive(!bonusLevel);
        curLevelText.gameObject.SetActive(!bonusLevel);
        BonusTitleImage.gameObject.SetActive(bonusLevel);

        if (bonusLevel && SDKManager.Instance.AdsFofBonus)
        {
            UIPanelManager.Instance.PushPanel(UIPanelType.EnterBonusLvPanel);
        }
    }

    #endregion




}
