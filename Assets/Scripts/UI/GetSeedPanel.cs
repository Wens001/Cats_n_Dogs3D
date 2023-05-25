using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GetSeedPanel : BasePanel
{
    [Header("UI组件")]
    public Button FarmButton;
    public Button NextTimeButton;
    public Button KeepItButton;
    public Text SeedNumText;

    private Transform FirstTrans;
    private Transform UsualTrans;



    private bool FirstFarm
    {
        get => PlayerPrefs.GetInt(StringMgr.FirstFarm, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(StringMgr.FirstFarm, value ? 1 : 0);
        }
    }


    #region 监听


    private void OnEnable()
    {
        //激励广告
        Messenger.AddListener(StringMgr.GetReward, OnGetReward);
        Messenger.AddListener(StringMgr.CloseRewardAd, OnRewardAdClose);
    }


    private void OnDisable()
    {
        //激励广告
        Messenger.RemoveListener(StringMgr.GetReward, OnGetReward);
        Messenger.RemoveListener(StringMgr.CloseRewardAd, OnRewardAdClose);
    }

    #endregion


    public override void OnEnter()
    {
        PanelConfig();

        gameObject.SetActive(true);

        SeedConfig();
    }

    public override void OnExit()
    {
        GameControl.Instance.BackMainCanvasSetting();
        //GameControl.Instance.LoadLevel(GameControl.Instance.CurLevel);
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

        FirstTrans = FarmButton.transform.parent;
        UsualTrans = KeepItButton.transform.parent;


        FarmButton.onClick.AddListener(() => {
            GameControl.Instance.CurLevel += 1;
            UIPanelManager.Instance.PopPanel();
            //转去农场场景
            GameControl.Instance.LoadFarmScene();
            GetSeed();
        });

        KeepItButton.onClick.AddListener(() => {
            if (SDKManager.Instance.ShowRewardedAd("GetSeed"))
            {
                GetSeedListen = true;
            }
        });


        NextTimeButton.onClick.AddListener(() => {
            UIPanelManager.Instance.PopPanel();
            GameControl.Instance.LoadNextLevel();
        });

    }

    private void PanelConfig()
    {
        PanelInit();

        FirstTrans.gameObject.SetActive(FirstFarm);
        UsualTrans.gameObject.SetActive(!FirstFarm);
        if (FirstFarm)
        {
            FirstFarm = false;
        }


    }



    #region 激励奖励

    bool GetSeedListen = false;
    private void OnGetReward()
    {
        if (GetSeedListen)
        {
            GetSeedListen = false;

            GetSeed();
            //UIPanelManager.Instance.PopPanel();
            GameControl.Instance.LoadNextLevel();

        }
    }

    private void OnRewardAdClose()
    {
        GetSeedListen = false;
    }




    #endregion


    #region SeedForGet

    
    
    PlantType PlantSeed
    {
        get 
        {
            return (PlantType)PlayerPrefs.GetInt("SeedIndex", (int)PlantType.Pumpkin);
        }

        set
        {
            PlayerPrefs.SetInt("SeedIndex", (int)value);
        }
    }

    int plantID = 1;
    int AddNum = 1;


    private void SeedConfig()
    {
        plantID = (int)PlantSeed;
        AddNum = PlantSeed == PlantType.Pumpkin ? 2 : 1;

        SeedNumText.text = "x " + AddNum;
        SeedNumText.gameObject.SetActive(AddNum != 1);

        LookAtTheSeed(PlantSeed);
    }



    private void GetSeed()
    {
        var num = GameSetting.GetSeedNum(plantID) + AddNum;

        GameSetting.SetSeedNum(plantID, num);
    }






    #endregion






    //指向种子
    private void LookAtTheSeed(PlantType _plantSeed)
    {
        var seedPrefab = Resources.Load<GameObject>("PlantSeed/" + _plantSeed);
        var seed = Instantiate(seedPrefab, Vector3.one * -50, Quaternion.identity);

        //下次切换
        switch (_plantSeed)
        {
            case PlantType.Pumpkin:
                PlantSeed = PlantType.Corn;
                break;
            case PlantType.Corn:
                PlantSeed = PlantType.Pumpkin;
                break;
            default:
                break;
        }
        

        //镜头切换
        FindObjectOfType<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        CameraControl.Instance.ChangeCamera(CameraType.CM_ShopView);
        CameraControl.Instance.LookAtSomething(seed.transform);
        GameControl.Instance.ChangeCullingToCatNDog();
        StartCoroutine(DelayLookAt(seed.transform));
    }

    IEnumerator DelayLookAt(Transform seed)
    {
        yield return new WaitForEndOfFrame();
        seed.LookAt(Camera.main.transform);

        var angle = 30;
        seed.DOLocalRotate(Vector3.up * angle, .3f)
            .OnComplete(()=> {
                seed.DOLocalRotate(Vector3.up * -angle, .6f).SetLoops(-1, LoopType.Yoyo);
            });
        //seed.DOLocalRotate(,)

    }






}
