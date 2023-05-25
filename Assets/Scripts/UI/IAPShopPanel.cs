using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPShopPanel : BasePanel
{
    public Button IAPAllButton;
    public Button IAPNoAdsButton;
    public Button CloseButton;

    public Transform NoAdsTrans;


    public override void OnEnter()
    {
        PanelConfig();

        gameObject.SetActive(true);

        Messenger.AddListener(StringMgr.BuyTheBundle, OnBuyBundleProduct);
    }

    public override void OnExit()
    {
        gameObject.SetActive(false);

        Messenger.RemoveListener(StringMgr.BuyTheBundle, OnBuyBundleProduct);
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


        CloseButton.onClick.AddListener(() => { UIPanelManager.Instance.PopPanel(); });
        IAPAllButton.onClick.AddListener(() => OnIapAllButtonClick());
        IAPNoAdsButton.onClick.AddListener(() => OnNoAdsButtonClick());

    }

    private void PanelConfig()
    {
        PanelInit();

        NoAdsTrans.gameObject.SetActive(!GameSetting.NoAds);
    }


    #region UI组件


    private void OnIapAllButtonClick()
    {
        SDKManager.Instance.BuyPorduct(ShopProductNames.AllInOneBundle);
    }

    private void OnNoAdsButtonClick()
    {
        SDKManager.Instance.BuyPorduct(ShopProductNames.NOADS);
    }



    #endregion



    private void OnBuyBundleProduct()
    {
        UIPanelManager.Instance.PopPanel();
    }


}
