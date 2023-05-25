using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LuckySkinPanel : BasePanel
{
    public Button KeepItButton;
    public Button LoseItButton;
    public Image LuckySkinImageShow;


    private SkinInfo luckySkin;


    private void OnEnable()
    {
        //Debug.Log("幸运界面 OnEnable");
        Messenger.AddListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, ChangeCullingToCatNDog);
        //激励广告
        Messenger.AddListener(StringMgr.GetReward, OnGetReward);
        Messenger.AddListener(StringMgr.CloseRewardAd, OnRewardAdClose);
    }

    private void OnDisable()
    {
        //Debug.Log("幸运界面 OnDisable");
        Messenger.RemoveListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, ChangeCullingToCatNDog);
        //激励广告
        Messenger.RemoveListener(StringMgr.GetReward, OnGetReward);
        Messenger.RemoveListener(StringMgr.CloseRewardAd, OnRewardAdClose);
    }


    public override void OnEnter()
    {
        PanelConfig();

        if (hideNow)
        {
            UIPanelManager.Instance.PopPanel();
            return;
        }

        LoadSpecialScene();
        SkinManager.Instance.LuckyTryPutOn();

        gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        UnloadSpecialScene();

        gameObject.SetActive(false);
    }

    public override void OnPause()
    {
    }

    public override void OnResume()
    {
    }

    bool haveInit;
    private void PanelInit()
    {
        if (haveInit)
        {
            return;
        }
        haveInit = true;

        LoseItButton.onClick.AddListener(() => {
            SkinManager.Instance.LoseTheLuckySkin();
            UIPanelManager.Instance.PopPanel();
        });

        KeepItButton.onClick.AddListener(OnKeepItButtonClick);

        //摇晃
        KeepItButton.transform.DOLocalRotate(new Vector3(0, 0, 5), .2f).SetLoops(-1, LoopType.Yoyo);

    }

    bool hideNow = false;
    private void PanelConfig()
    {
        PanelInit();
        DelayShowLoseItButton();

        luckySkin = SkinManager.Instance.RandomLuckySkin();

        if (luckySkin)
        {
            LuckySkinImageShow.sprite = luckySkin.Icon;
        }
        else
        {
            hideNow = true;
        }


    }

    private void DelayShowLoseItButton()
    {
        LoseItButton.gameObject.SetActive(false);

        int num = 0;
        DOTween.To(() => num, x => num = x, 1, 3.5f)
           .OnComplete(() => {
               LoseItButton.gameObject.SetActive(true);
           });
    }


    #region UI组件

    private void OnKeepItButtonClick()
    {
        if (SDKManager.Instance.ShowRewardedAd("LuckySkin_Get"))
        {
            newSkinRewardListen = true;
        }
    }



    #endregion


    #region 激励奖励

    bool newSkinRewardListen = false;
    private void OnGetReward()
    {
        if (newSkinRewardListen)
        {
            newSkinRewardListen = false;

            //获取皮肤
            SkinManager.Instance.GetNewSkin(luckySkin);
            SkinManager.Instance.loseIt = false;

            UIPanelManager.Instance.PopPanel();
        }
    }

    private void OnRewardAdClose()
    {
        newSkinRewardListen = false;
    }




    #endregion


    #region 切场景显示

    private void LoadSpecialScene()
    {
        GameControl.Instance.LoadModelShowScene();
    }

    private void ChangeCullingToCatNDog(TouchMove cat, TouchMove dog)
    {
        GameControl.Instance.ChangeCullingToCatNDog();
        CameraControl.Instance.ChangeCamera(CameraType.CM_LookLeft);
    }

    private void UnloadSpecialScene()
    {
        GameControl.Instance.BackMainCanvasSetting();
        GameControl.Instance.UnloadSpecialScene(false);
    }


    #endregion



}
