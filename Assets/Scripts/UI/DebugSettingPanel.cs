using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugSettingPanel : BasePanel
{
    public Text curLevelText;
    public Toggle LogSwitchToggle;
    public Toggle NoAdsToggle;
    public Toggle GreenGroundToggle;
    public Toggle GreenBGToggle;
    public Toggle PerformanceToggle;
    public Button addLevelButton;
    public Button minusLevelButton;
    public Button UnlockSkinsButton;
    public Button AddMoneyButton;
    public Button CloseButton;
    public Button CleanButton;
    public Button ABtestButton;
    public Button AdsSceneButton;
    public Button LionDebuggerButton;
    public Text ABTestShowText;

    public InputField inputField;

    public override void OnEnter()
    {
        PanelConfig();


        SDKManager.Instance.HideBanner();

        gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        if (haveChangedLevel)
        {
            GameControl.Instance.LoadLevel(GameControl.Instance.CurLevel);
            haveChangedLevel = false;
        }


        gameObject.SetActive(false);
    }

    public override void OnPause()
    {
    }

    public override void OnResume()
    {
    }


    bool haveChangedLevel;
    bool haveInit = false;
    private void PanelInit()
    {
        if (haveInit)
        {
            return;
        }
        haveInit = true;

        addLevelButton.onClick.AddListener(() => {
            haveChangedLevel = true;
            curLevelText.text = (GameControl.Instance.CurLevel += 1).ToString();
        });

        minusLevelButton.onClick.AddListener(() => {
            haveChangedLevel = true;
            GameControl.Instance.CurLevel -= 1;
            curLevelText.text = GameControl.Instance.CurLevel.ToString();
        });

        //关卡跳转
        inputField.onValueChanged.AddListener((string level) => {
            
            if (int.TryParse(level, out int levelNum))
            {
                GameControl.Instance.CurLevel = levelNum;
                curLevelText.text = levelNum.ToString();
                haveChangedLevel = true;
            }
        });


        LogSwitchToggle.onValueChanged.AddListener((bool ison) =>
        {
            if (GameControl.Instance.Log)
            {
                GameControl.Instance.Log.gameObject.SetActive(ison);
            }
        });

        //去广告
        NoAdsToggle.onValueChanged.AddListener((bool ison) =>
        {
            GameSetting.NoAds = ison;
            SDKManager.Instance.HideBanner();
        });

        //全皮肤
        UnlockSkinsButton.onClick.AddListener(()=> {
            var skins = SkinManager.Instance.skinsInfo;
            foreach (var skin in skins)
            {
                PlayerPrefs.SetInt(skin.ID, 1);
            }
        });

        AddMoneyButton.onClick.AddListener(() => {
            GameSetting.CoinCount = 9999;
        });

        CleanButton.onClick.AddListener(() =>
        {
            GameControl.Instance.CleanData();
        });

        //ab分组
        ABtestButton.onClick.AddListener(() =>{
            SDKManager.Instance.ChangeABGroup();
            ABTestShowText.text = "ABTest: \n" + SDKManager.Instance.abGroup;
        });


        //绿幕
        GreenGroundToggle.onValueChanged.AddListener( (isOn) =>
        {
            GameSetting.GreenGround = isOn;
            GameControl.Instance.ChangeGreenGround();
        });

        GreenBGToggle.onValueChanged.AddListener( (isOn) =>
        {
            GameSetting.GreenBG = isOn;
            GameControl.Instance.ChangeGreenBG();
        });

        //lion分析工具
        PerformanceToggle.isOn = true;
        PerformanceToggle.onValueChanged.AddListener((isOn) =>
        {
            GameControl.Instance.ActivePerformanceTool(isOn);
        });



        //广告关卡——西瓜关卡
        AdsSceneButton.onClick.AddListener(() => { GameControl.Instance.LoadOtherLevel("Watermelon_AdsScene"); });


        CloseButton.onClick.AddListener(() => { UIPanelManager.Instance.PopPanel(); });

        //lion debugger
        LionDebuggerButton.onClick.AddListener(() => {
            UIPanelManager.Instance.PopPanel();
            LionStudios.Debugging.LionDebugger.Show(true);
        });

    }


    private void PanelConfig()
    {
        PanelInit();

        ABTestShowText.text = "ABTest: \n" + SDKManager.Instance.abGroup;
        curLevelText.text = GameControl.Instance.CurLevel.ToString();
        NoAdsToggle.isOn = GameSetting.NoAds;
        GreenGroundToggle.isOn = GameSetting.GreenGround;
        GreenBGToggle.isOn = GameSetting.GreenBG;


        inputField.textComponent.text = GameControl.Instance.CurLevel.ToString();

    }


}
