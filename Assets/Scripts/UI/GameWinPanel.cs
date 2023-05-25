using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameWinPanel : BasePanel
{
    public Button ClaimButton;
    public Button nextButton;
    public Button replayButton;
    public Text MoneyText;
    public Text RewardMoneyText;
    public Text MoreRewardMoneyText;
    public Image RewardAdImage;
    public Image SkinIconShade;
    public Image SkinIcon;
    public Text shadeText;
    public GameObject Pointer;

    private Image[] coins;
    private Material SkinShadeMat;


    [Header("结算特效")]
    private ParticleSystem vfx;
    private Canvas canvas;

    //首次多倍奖励
    private bool FirstCoinReward
    {
        get
        {
            return PlayerPrefs.GetInt("FirstCoinReward", 1) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt("FirstCoinReward", value ? 1 : 0);
        }

    }

    //结算
    private int rewardCount;
    private int moreRewardCount;

    public override void OnEnter()
    {
        PanelConfig();

        gameObject.SetActive(true);

        //胜利打点
        //Debug.Log("胜利界面弹出");
        SDKManager.Instance.OnLevelComplete(GameControl.Instance.CurLevel);


        //激励广告
        Messenger.AddListener(StringMgr.GetReward, OnGetReward);
        Messenger.AddListener(StringMgr.CloseRewardAd, OnRewardAdClose);

        //显示交叉广告
        SDKManager.Instance.ShowCrossPromo();

        Messenger.AddListener(StringMgr.FlyCoinsOver, OnCoinFlyOver);

    }

    public override void OnExit()
    {
        StopAllCoroutines();
        ClaimButton.transform.DOKill();
        gameObject.SetActive(false);

        //激励广告
        Messenger.RemoveListener(StringMgr.GetReward, OnGetReward);
        Messenger.RemoveListener(StringMgr.CloseRewardAd, OnRewardAdClose);

        //隐藏交叉广告
        SDKManager.Instance.HideCrossPromo();

        Messenger.RemoveListener(StringMgr.FlyCoinsOver, OnCoinFlyOver);
    }

    public override void OnPause()
    {
    }

    public override void OnResume()
    {
        if (SkinManager.Instance.loseIt)
        {
            //切换皮肤
            SkinIconShade.gameObject.SetActive(true);
            shadeText.gameObject.SetActive(true);
            SkinIcon.gameObject.SetActive(false);
        }
    }


    bool haveInit = false;
    private void PanelInit()
    {
        if (haveInit)
        {
            return;
        }
        haveInit = true;

        SkinShadeMat = SkinIconShade.material;
        canvas = transform.GetComponentInParent<Canvas>();

        nextButton.onClick.AddListener(OnNextButtonClick);
        replayButton.onClick.AddListener(OnReplayButtonClick);
        ClaimButton.onClick.AddListener(OnClaimButtonClick);

    }

    private void PanelConfig()
    {
        PanelInit();
        ButtonEnable();

        DelayShowNextButton();

        //金币数
        rewardCount = 50 + GameControl.Instance.GotCoinCount;
        MoneyText.text = GameSetting.CoinCount.ToString();
        RewardMoneyText.text = rewardCount.ToString();
        //moreRewardCount = rewardCount * 4;
        //MoreRewardMoneyText.text = moreRewardCount.ToString();

        //指针转动
        rollBool = true;
        RewardAdImage.gameObject.SetActive(!FirstCoinReward);

        //按钮缩放
        ClaimButton.transform.localScale = Vector3.one * .8f;
        ClaimButton.transform.DOScale(Vector3.one, .8f).SetLoops(-1, LoopType.Yoyo);

        //canvas更改为相机前（看到烟花）
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 2;
        vfx = Camera.main.GetComponentInChildren<ParticleSystem>(true);
        vfx.transform.localPosition = new Vector3(0, -.5f, 1.3f);
        vfx.transform.localScale = Vector3.one * 1.2f;
        vfx.Play();

        //canvas改回屏幕前
        //int num = 0;
        //DOTween.To(() => num, x => num = x, 3, 2f)
        //    .OnComplete(() =>
        //    {
        //    });

        LuckySkinShow();
    }

    //奖励转盘
    bool rollBool = true;
    bool changeBool = true;
    private int PointerRotationDir = 1;
    private float speed = 90;
    private void Update()
    {
        //指针转动
        if (rollBool)
        {
            Pointer.transform.Rotate(Vector3.forward * PointerRotationDir * speed * Time.deltaTime, Space.Self);

            var angle = 180 - Mathf.Abs(Pointer.transform.localRotation.eulerAngles.z - 180);
            if ((angle >= 85) && changeBool)
            {
                PointerRotationDir *= -1;
                changeBool = false;

                float num = 0;
                DOTween.To(() => num, x => num = x, 1, .4f)
                    .OnComplete(() =>
                    {
                        changeBool = true;
                    });
            }

            if (angle <= 20)
            {
                moreRewardCount = rewardCount * 5;
            }
            else if (angle <= 40)
            {
                moreRewardCount = rewardCount * 4;
            }
            else if (angle <= 63)
            {
                moreRewardCount = rewardCount * 3;
            }
            else
            {
                moreRewardCount = rewardCount * 2;
            }

            MoreRewardMoneyText.text = moreRewardCount.ToString();
        }
    }

    #region Other

    private void LuckySkinShow()
    {
        if (SkinManager.Instance.RandomLuckySkin() == null)
        {
            SkinIconShade.gameObject.SetActive(false);
            shadeText.gameObject.SetActive(false);
            SkinIcon.gameObject.SetActive(false);
            return;
        }

        var skinLevel = SDKManager.Instance.SkinRv_freq;
        //显示皮肤进度
        SkinIconShade.gameObject.SetActive(true);
        shadeText.gameObject.SetActive(true);
        SkinIcon.gameObject.SetActive(false);

        float extentCount = GameControl.Instance.CurLevel % skinLevel;
        bool luckyShow = extentCount == 0;

        ClaimButton.interactable = !luckyShow;

        int number = luckyShow ? 100 : (int)(((float)extentCount / skinLevel) * 100);
        shadeText.text = "NEW SKIN:" + number + "%";

        int num = 0;
        DOTween.To(() => num, x => num = x, number, 1.2f)
            .OnUpdate(() =>
            {
                SkinShadeMat.SetFloat("_Curve", num * .01f);
            })
            .OnComplete(() =>
            {
                if (luckyShow)
                {
                    SkinIcon.sprite = SkinManager.Instance.RandomLuckySkin().Icon;
                    UIPanelManager.Instance.PushPanel(UIPanelType.LuckySkinPanel);


                    ClaimButton.interactable = true;

                    //切换皮肤
                    SkinIconShade.gameObject.SetActive(false);
                    shadeText.gameObject.SetActive(false);
                    SkinIcon.gameObject.SetActive(true);
                }
            });

    }

    //延迟显示下一关按钮
    private void DelayShowNextButton()
    {
        nextButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        if (FirstCoinReward)
        {
            return;
        }

        int num = 0;
        DOTween.To(() => num, x => num = x, 1, 2.5f)
            .OnComplete(() =>
            {
                nextButton.gameObject.SetActive(true);
                replayButton.gameObject.SetActive(true);
            });
    }

    #endregion

    #region UI组件
    private void OnNextButtonClick()
    {
        SkinManager.Instance.OnNextLevelButtonClick();
        //GameControl.Instance.LoadNextLevel();
        OnCloseWinPanel(nextButton.transform, rewardCount);
    }

    private void OnReplayButtonClick()
    {
        GameControl.Instance.GameReplay();

        SDKManager.Instance.OnlevelRestart(GameControl.Instance.CurLevel, "Game_Win");
    }

    private void OnClaimButtonClick()
    {
        if (FirstCoinReward)
        {
            FirstCoinReward = false;
            rollBool = false;

            OnCloseWinPanel(ClaimButton.transform, moreRewardCount);
            return;
        }


        if (SDKManager.Instance.ShowRewardedAd("4_coinReward"))
        {
            MoreRewardListen = true;
            rollBool = false;
        }
    }

    #endregion


    #region 激励奖励回调

    bool MoreRewardListen = false;
    /// <summary>
    /// 获取激励奖励
    /// </summary>
    private void OnGetReward()
    {
        //激励
        if (MoreRewardListen)
        {
            MoreRewardListen = false;

            OnCloseWinPanel(ClaimButton.transform, moreRewardCount);
        }
    }
    private void OnRewardAdClose()
    {
        MoreRewardListen = false;

    }

    #endregion

    #region 金币 \ 农场界面

    private void OnCloseWinPanel(Transform parent, int CoinNum)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        ClaimButton.enabled = false;
        replayButton.enabled = false;
        nextButton.enabled = false;

        Messenger.Broadcast(StringMgr.FlyCoins, parent.position, MoneyText, CoinNum);
    }

    private void ButtonEnable()
    {
        ClaimButton.enabled = true;
        replayButton.enabled = true;
        nextButton.enabled = true;
    }


    bool haveAppeared = false;
    private void OnCoinFlyOver()
    {
        if (gameObject.activeSelf)
        {
            UIPanelManager.Instance.PopPanel();

            if (SDKManager.Instance.HaveFarmSystem)
            {
                bool getSeed = GameControl.Instance.CurLevel == 4 || (GameControl.Instance.CurLevel - 4) % 5 == 0;
                if (getSeed)
                {
                    //获得种子界面
                    UIPanelManager.Instance.PushPanel(UIPanelType.GetSeedPanel);
                }
                else if (!haveAppeared && FarmTimeMgr.HasHarvest())
                {
                    haveAppeared = true;

                    //作物成熟界面
                    UIPanelManager.Instance.PushPanel(UIPanelType.HarvestHintPanel);
                }
                else
                {
                    GameControl.Instance.LoadNextLevel();
                }
            }
            else
            {
                GameControl.Instance.LoadNextLevel();
            }
        }
    }

    #endregion

}
