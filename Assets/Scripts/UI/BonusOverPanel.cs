using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BonusOverPanel : BasePanel
{
    [Header("Time Up")]
    public Transform TimeUpPanel;
    public Image NothingImage;
    public Button AddTimeButton;
    public Button TimeUpNextButton;

    [Header("Got Bonus")]
    public Transform GotBonusPanel;
    public Button GotNextButton;

    [Header("PanelMain")]
    public Transform BonusView;
    public Text CoinText;

    private BonusSlot[] bonusSlots;


    public override void OnEnter()
    {
        PanelConfig();
        gameObject.SetActive(true);

        //激励广告
        Messenger.AddListener(StringMgr.GetReward, OnGetReward);
        Messenger.AddListener(StringMgr.CloseRewardAd, OnRewardAdClose);

        Messenger.AddListener(StringMgr.FlyCoinsOver, ClosePanel);
    }

    public override void OnExit()
    {
        gameObject.SetActive(false);

        //激励广告
        Messenger.RemoveListener(StringMgr.GetReward, OnGetReward);
        Messenger.RemoveListener(StringMgr.CloseRewardAd, OnRewardAdClose);

        Messenger.RemoveListener(StringMgr.FlyCoinsOver, ClosePanel);


    }

    public override void OnPause()
    {
    }

    public override void OnResume()
    {

    }



    bool haveInit = false;
    private void PanelInit()
    {
        if (haveInit)
        {
            return;
        }
        haveInit = true;

        bonusSlots = FindObjectsOfType<BonusSlot>();

        TimeUpNextButton.onClick.AddListener(() => OnNextButtonClick());
        GotNextButton.onClick.AddListener(() => OnNextButtonClick());

        AddTimeButton.onClick.AddListener(() => OnAddTimeButtonClick());


    }

    private void PanelConfig()
    {
        PanelInit();
        BonusInit();

        CoinText.text = GameSetting.CoinCount.ToString();

        //显示 timeUp 或 GotAllBonus 界面
        TimeUpPanel.gameObject.SetActive(!GameControl.Instance.GotAllBonus);
        GotBonusPanel.gameObject.SetActive(GameControl.Instance.GotAllBonus);

    }

    //显示所获取的物品
    private void BonusInit()
    {
        for (int i = 0; i < bonusSlots.Length; i++)
        {
            if (i >= GameControl.Instance.GotBonusList.Count)
            {
                bonusSlots[i].gameObject.SetActive(false);
            }
            else
            {
                bonusSlots[i].SlotInit(GameControl.Instance.GotBonusList[i]);
                bonusSlots[i].gameObject.SetActive(true);
            }
        }

        NothingImage.gameObject.SetActive(GameControl.Instance.GotBonusList.Count == 0);
    }


    #region UI 组件

    private void OnAddTimeButtonClick()
    {
        if (SDKManager.Instance.ShowRewardedAd("BonusLevelTime"))
        {
            AddTimeListen = true;
        }
    }

    
    private void OnNextButtonClick()
    {
        bool noCoinBonus = true;
        foreach (var bonus in GameControl.Instance.GotBonusList)
        {
            if (bonus != BonusType.LuckySkin)
            {
                noCoinBonus = false;
                break;
            }
        }


        var slots = FindObjectsOfType<BonusSlot>();
        var haveFlyCoin = false;
        foreach (var slot in slots)
        {
            slot.GotTheBonus(CoinText, haveFlyCoin);
            if (!haveFlyCoin)
            {
                haveFlyCoin = true;
            }
        }

        if (noCoinBonus)
        {
            ClosePanel();
        }

    }

    //奖励关结算
    private void ClosePanel()
    {
        UIPanelManager.Instance.PopPanel();
        GameControl.Instance.LoadNextLevel();
    }


    #endregion



    #region 激励奖励

    bool AddTimeListen = false;
    private void OnGetReward()
    {
        if (AddTimeListen)
        {
            AddTimeListen = false;

            //获取奖励
            //BonusLevelMgr.Instance.timer
            Messenger.Broadcast(StringMgr.BonusLevelInit, 20);
            UIPanelManager.Instance.PopPanel();
        }
    }

    private void OnRewardAdClose()
    {
        AddTimeListen = false;
    }

    #endregion





}
