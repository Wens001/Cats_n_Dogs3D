using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 礼物获取界面
/// </summary>
public class GetGiftPanel : BasePanel
{
    public Text coinText;
    public Transform RewardCoinTrans;
    public Transform IconTrans;

    public Button KeepItButton;
    public Button LoseItButton;


    private bool IsCoinGift;
    private bool firstGot;

   


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

        LoseItButton.onClick.AddListener(() => OnCloseButtonClick());
        KeepItButton.onClick.AddListener(() => OnKeepItButtonClick());

    }

    private void PanelConfig()
    {
        PanelInit();

        firstGot = GameControl.Instance.FirstGotGift;
        IsCoinGift = GameControl.Instance.IsCoinGift;
        LoseItButton.gameObject.SetActive(!firstGot);
        KeepItButton.transform.GetChild(0).gameObject.SetActive(!firstGot);
        GameControl.Instance.FirstGotGift = false;

        coinText.text = GameSetting.CoinCount.ToString();
        ShowBonus(IsCoinGift);
        DelayShowNextButton();
    }



    private void ShowBonus(bool isCoinBonus)
    {
        RewardCoinTrans.gameObject.SetActive(isCoinBonus);
        
        IconTrans.gameObject.SetActive(!isCoinBonus);
        if (!isCoinBonus)
        {
            var luckySkin = GameControl.Instance.BonusSkin;
            if (luckySkin)
            {
                IconTrans.GetComponent<Image>().sprite = luckySkin.Icon;
            }
            else
            {
                IsCoinGift = true;
                RewardCoinTrans.gameObject.SetActive(true);
            }
        }
    }


    private void OnCloseButtonClick()
    {
        ClosePanel();
    }


    private void OnKeepItButtonClick()
    {
        if (firstGot)       //首次无需激励
        {
            //获取礼物
            if (IsCoinGift)
            {
                //GameSetting.CoinCount += 300;
                Messenger.Broadcast(StringMgr.FlyCoins, RewardCoinTrans.position, coinText, 300);
            }
            else
            {
                SkinManager.Instance.GetNewSkin(GameControl.Instance.BonusSkin);
                ClosePanel();
            }

            return;
        }


        var requireStr = "GotGift_" + (IsCoinGift ? "Coin" : "Skin");
        if (SDKManager.Instance.ShowRewardedAd(requireStr))
        {
            GetGiftListen = true;
        }

    }


    private void ClosePanel()
    {
        UIPanelManager.Instance.PopPanel();

        if (GameControl.Instance.BonusLevel)
        {
            GameControl.Instance.LoadNextLevel();
        }
    }




    #region 激励奖励

    bool GetGiftListen = false;
    private void OnGetReward()
    {
        if (GetGiftListen)
        {
            GetGiftListen = false;

            //获取礼物
            if (IsCoinGift)
            {
                //GameSetting.CoinCount += 300;
                Messenger.Broadcast(StringMgr.FlyCoins, RewardCoinTrans.position, coinText, 300);
            }
            else
            {
                SkinManager.Instance.GetNewSkin(GameControl.Instance.BonusSkin);
                ClosePanel();
            }
        }
    }

    private void OnRewardAdClose()
    {
        GetGiftListen = false;
    }




    #endregion


    private void DelayShowNextButton()
    {
        LoseItButton.gameObject.SetActive(false);
        if (firstGot)
        {
            return;
        }

        int num = 0;
        DOTween.To(() => num, x => num = x, 1, 3.5f)
            .OnComplete(() =>
            {
                LoseItButton.gameObject.SetActive(true);
            });
    }





}
