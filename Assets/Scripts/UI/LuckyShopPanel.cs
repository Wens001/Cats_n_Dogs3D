using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LuckyShopPanel : BasePanel
{
    public Button BuyItButton;
    public Button LoseItButton;


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

        LoseItButton.onClick.AddListener(() => UIPanelManager.Instance.PopPanel());
        BuyItButton.onClick.AddListener(() => OnBunleSaleButtonClick());
    }

    private void PanelConfig()
    {
        PanelInit();


    }


    private void OnBunleSaleButtonClick()
    {
        SDKManager.Instance.BuyPorduct(ShopProductNames.OneBunleSale);
    }


    #region 购买成功回调

    private void OnBuyBundleProduct()
    {
        UIPanelManager.Instance.PopPanel();
    }

    #endregion

}
