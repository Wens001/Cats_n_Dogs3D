using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HarvestHintPanel : BasePanel
{
    public Button GotoFarmButton;
    public Button LaterButton;


    #region CallBack

    private void OnEnable()
    {
        Messenger.AddListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, ChangeCullingToCatNDog);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, ChangeCullingToCatNDog);

    }


    #endregion


    public override void OnEnter()
    {
        PanelConfig();
        gameObject.SetActive(true);

        LoadSpecialScene();

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



    bool haveInit = false;
    private void PanelInit()
    {
        if (haveInit)
        {
            return;
        }
        haveInit = true;


        GotoFarmButton.onClick.AddListener(() => OnGotoFarmButtonClick());
        LaterButton.onClick.AddListener(() => GameControl.Instance.LoadNextLevel());


    }

    private void PanelConfig()
    {
        PanelInit();


    }




    private void OnGotoFarmButtonClick()
    {
        GameControl.Instance.CurLevel += 1;
        UIPanelManager.Instance.PopPanel();
        //转去农场场景
        GameControl.Instance.LoadFarmScene();
    }





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
