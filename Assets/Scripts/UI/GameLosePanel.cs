using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLosePanel : BasePanel
{
    public Button ReplayButton;
    public Button SkipButton;
    public Image Title;
    public Image DogLoseTitle;




    public override void OnEnter()
    {
        LosePanelInit();

        Title.gameObject.SetActive(!GameControl.Instance.FailByDog);
        DogLoseTitle.gameObject.SetActive(GameControl.Instance.FailByDog);
        GameControl.Instance.FailByDog = false;


        gameObject.SetActive(true);

        //打点
        SDKManager.Instance.OnLevelFailed(GameControl.Instance.CurLevel);

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
    private void LosePanelInit()
    {
        if (haveInit)
        {
            return;
        }

        haveInit = true;
        ReplayButton.onClick.AddListener(OnReplayButtonClick);
        SkipButton.onClick.AddListener(OnSkipButtonClick);
    }
  

    private void OnReplayButtonClick()
    {
        //重玩打点
        SDKManager.Instance.OnlevelRestart(GameControl.Instance.CurLevel, "Game_Fail");

        if (SDKManager.Instance.ShowInterstitial(GameStatus.GameFailReplay) == false)
        {
            GameControl.Instance.GameReplay();
        }
    }


    private void OnSkipButtonClick()
    {
        if(SDKManager.Instance.ShowRewardedAd("GameFail_Skip"))
        {
            SkipRewardListen = true;
        }

    }

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
            SDKManager.Instance.OnlevelSkip(GameControl.Instance.CurLevel, "Game_Fail");
        }
    }

    public void OnRewardAdClose()
    {
        SkipRewardListen = false;
    }

    #endregion


}
