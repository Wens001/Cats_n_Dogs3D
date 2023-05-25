using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterBonusLvPanel : BasePanel
{
    public Button AcceptedButton;
    public Button NextTimeButton;


    public override void OnEnter()
    {
        PanelConfig();

        gameObject.SetActive(true);
        //激励广告
        Messenger.AddListener(StringMgr.GetReward, OnGetReward);
        Messenger.AddListener(StringMgr.CloseRewardAd, OnRewardAdClose);
    }

    public override void OnExit()
    {
        gameObject.SetActive(false);
        //激励广告
        Messenger.RemoveListener(StringMgr.GetReward, OnGetReward);
        Messenger.RemoveListener(StringMgr.CloseRewardAd, OnRewardAdClose);
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


        AcceptedButton.onClick.AddListener(OnAcceptedButtonClick);
        NextTimeButton.onClick.AddListener(OnNextTimeButtonClick);
    }

    private void PanelConfig()
    {
        PanelInit();
       
    }



    #region UI组件

    private void OnAcceptedButtonClick()
    {
        if (SDKManager.Instance.ShowRewardedAd("EnterBonusLevel"))
        {
            AcceptedListen = true;
        }
    }

    private void OnNextTimeButtonClick()
    {
        UIPanelManager.Instance.PopPanel();
        GameControl.Instance.LoadNextLevel();
    }



    #endregion


    #region 激励奖励

    bool AcceptedListen = false;
    private void OnGetReward()
    {
        if (AcceptedListen)
        {
            AcceptedListen = false;

            //获取奖励
            UIPanelManager.Instance.PopPanel();
        }
    }

    private void OnRewardAdClose()
    {
        AcceptedListen = false;
    }

    #endregion




}
