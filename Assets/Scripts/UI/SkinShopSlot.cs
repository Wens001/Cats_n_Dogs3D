using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinShopSlot : MonoBehaviour
{
    public SkinInfo curSkin;

    [Header("商位栏配置")]
    public Image IconImage;
    public Transform WatchGet;
    public Transform BuyGet;
    public Button SlotButton;
    public Image SelectedImage;

    private SkinShopPanel skinShopPanel;

    private void OnEnable()
    {
        Messenger.AddListener(StringMgr.SkinSlotRefresh, SlotRefresh);
        //激励广告
        Messenger.AddListener(StringMgr.GetReward, OnGetReward);
        Messenger.AddListener(StringMgr.CloseRewardAd, OnRewardAdClose);
    }


    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.SkinSlotRefresh, SlotRefresh);

        //激励广告
        Messenger.RemoveListener(StringMgr.GetReward, OnGetReward);
        Messenger.RemoveListener(StringMgr.CloseRewardAd, OnRewardAdClose);
    }




    bool haveInit = false;
    public void SlotInit()
    {
        if (curSkin == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (haveInit)
        {
            return;
        }
        haveInit = true;

        skinShopPanel = transform.GetComponentInParent<SkinShopPanel>();

        SelectedImage.gameObject.SetActive(false);
        BuyGet.gameObject.SetActive(false);
        WatchGet.gameObject.SetActive(false);
        IconImage.sprite = curSkin.Icon;
        IconImage.SetNativeSize();
        switch (curSkin.getWay)
        {
            case GetWay.WatchAd:
                WatchGet.gameObject.SetActive(true);
                break;
            case GetWay.CoinBuy:
                BuyGet.GetComponentInChildren<Text>().text = curSkin.CoinCost.ToString();
                BuyGet.gameObject.SetActive(true);
                break;
            default:
                break;
        }

        SlotButton.onClick.AddListener(OnSlotButtonClick);
    }

    /// <summary>
    /// 刷新状态
    /// </summary>
    public void SlotRefresh()
    {
        if (curSkin == null)
        {
            gameObject.SetActive(false);
        }

        //已获得
        if (SkinManager.Instance.CheckSkinGot(curSkin))
        {
            BuyGet.gameObject.SetActive(false);
            WatchGet.gameObject.SetActive(false);
        }

        //已装备
        SelectedImage.gameObject.SetActive(SkinManager.Instance.CheckSkinPutOn(curSkin));
        //if (SkinManager.Instance.CurMatSkinID == curSkin.ID || SkinManager.Instance.CurModelSkinID == curSkin.ID)
        //{
        //    SelectedImage.gameObject.SetActive(true);
        //}
        //else
        //{
        //    SelectedImage.gameObject.SetActive(false);
        //}
    }


    private void OnSlotButtonClick()
    {
        if (SelectedImage.isActiveAndEnabled)
        {
            return;
        }

        //已获得
        if (SkinManager.Instance.CheckSkinGot(curSkin))
        {
            //换皮肤
            SkinManager.Instance.PutOnSkin(curSkin);
        }
        //未获得
        else
        {
            switch (curSkin.getWay)
            {
                //看广告
                case GetWay.WatchAd:
                    if (SDKManager.Instance.ShowRewardedAd("SkinShop_Skin" + curSkin.SkinName))
                    {
                        RewardListen = true;
                    }
                    break;
                //金币购买
                case GetWay.CoinBuy:

                    if (GameSetting.CoinCount >= curSkin.CoinCost)
                    {
                        //获取皮肤
                        SkinManager.Instance.GetNewSkin(curSkin);

                        GameSetting.CoinCount -= curSkin.CoinCost;
                    }
                    else
                    {
                        //金币不足
                        skinShopPanel.OnCoinNotEnough();

                    }

                    break;
                default:
                    break;
            }
        }
    }



    #region 激励奖励回调

    bool RewardListen;
    private void OnGetReward()
    {
        if (RewardListen)
        {
            RewardListen = false;

            //获取皮肤
            SkinManager.Instance.GetNewSkin(curSkin);

        }
    }

    private void OnRewardAdClose()
    {
        RewardListen = false;
    }


    #endregion




}
