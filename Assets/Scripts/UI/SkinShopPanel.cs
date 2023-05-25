using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkinShopPanel : BasePanel
{
    public Button CloseButton;
    public Button CleanModelButton;
    public RectTransform content;
    public Image CoinNotEnoughImage;


    private SkinShopSlot[] skinSlots;
    private List<SkinInfo> skinsInfo;
    private CameraType lastCamera;

    public override void OnEnter()
    {
        PanelConfig();
        lastCamera = CameraControl.Instance.GetCurCamera();
        //CameraControl.Instance.ChangeCamera(CameraType.CM_ShopView);

        gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        CameraControl.Instance.ChangeCamera(lastCamera);
        gameObject.SetActive(false);
    }

    public override void OnPause()
    {
    }

    public override void OnResume()
    {
    }

    private void PanelConfig()
    {
        PanelInit();

        content.anchoredPosition = Vector3.zero;
        CoinNotEnoughImage.gameObject.SetActive(false);

        Messenger.Broadcast(StringMgr.SkinSlotRefresh);
    }


    bool haveInit = false;
    private void PanelInit()
    {
        if (haveInit)
        {
            return;
        }
        haveInit = true;

        skinSlots = transform.GetComponentsInChildren<SkinShopSlot>();
        skinsInfo = SkinManager.Instance.skinsInfo;

        CloseButton.onClick.AddListener(() =>
        {
            GameControl.Instance.UnloadSpecialScene();
            UIPanelManager.Instance.PopPanel();
        });

        CleanModelButton.onClick.AddListener(() => { SkinManager.Instance.PutOffModelSkin(); });

        SkinSlotsInit();
    }


    private void SkinSlotsInit()
    {
        for (int i = 0; i < skinSlots.Length; i++)
        {
            skinSlots[i].SlotInit();
        }
    }


    //金币不足弹窗
    private void Update()
    {
        NotEnoughShowTimer.OnUpdate(Time.deltaTime);
        if (NotEnoughShowTimer.IsFinish)
        {
            CoinNotEnoughImage.gameObject.SetActive(false);
        }

    }


    MyTimer NotEnoughShowTimer = new MyTimer(1);
    public void OnCoinNotEnough()
    {
        CoinNotEnoughImage.transform.DOKill();

        CoinNotEnoughImage.gameObject.SetActive(true);
        NotEnoughShowTimer.ReStart();

        CoinNotEnoughImage.transform.localPosition = Vector3.down * 800;
        CoinNotEnoughImage.transform.DOLocalMoveY(0, .3f);
    }




}
