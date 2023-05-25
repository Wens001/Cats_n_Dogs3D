using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingPanel : BasePanel
{
    [Header("GameSetting")]
    public Button CloseButton;
    public Toggle volumeToggle;
    public Toggle shakeToggle;

    [Header("GamePrivacy")]
    public Button PrivacyButton;
    public Button RestoreButton;
    public Text ConnectUsText;
    public Image RestoreSuccess;

    [Header("ConnectWay")]
    public Button FbButton;
    public Button InstagramButton;
    public Button LionButton;
    public Button TwitterButton;
    public Button YoutubeButton;


    public override void OnEnter()
    {
        PanelInit();
        PanelConfig();
        gameObject.SetActive(true);

        Messenger.AddListener(StringMgr.Event.OnRestoreSuccessfully, OnRestoreSuccessfully);
    }

    public override void OnExit()
    {
        Messenger.RemoveListener(StringMgr.Event.OnRestoreSuccessfully, OnRestoreSuccessfully);


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

        CloseButton.onClick.AddListener(() => { UIPanelManager.Instance.PopPanel(); });
        
        PrivacyButton.onClick.AddListener(() => {
            LionStudios.GDPR.LionGDPR.Show();
        });
        //隐私按钮显示
        PrivacyButton.gameObject.SetActive(!GameSetting.IOS_HighVersion && LionStudios.GDPR.LionGDPR.Status == LionStudios.GDPR.LionGDPR.UserStatus.Applies);

        volumeToggle.onValueChanged.AddListener(OnVolumeToggleClick);
        shakeToggle.onValueChanged.AddListener(OnShakeToggleClick);

#if UNITY_IOS || UNITY_EDITOR
        RestoreButton.gameObject.SetActive(true);
#elif UNITY_ANDROID
        RestoreButton.gameObject.SetActive(false);
#endif
        RestoreButton.onClick.AddListener(() => {
            SDKManager.Instance.ProductRestore();
        });

        //联系链接
        YoutubeButton.onClick.AddListener(()=> { Application.OpenURL("https://www.youtube.com/lionstudioscc"); });
        FbButton.onClick.AddListener(()=> { Application.OpenURL("https://www.facebook.com/lionstudios.cc/"); });
        InstagramButton.onClick.AddListener(()=> { Application.OpenURL("https://www.instagram.com/lionstudioscc/"); });
        TwitterButton.onClick.AddListener(()=> { Application.OpenURL("https://twitter.com/LionStudiosCC"); });
        LionButton.onClick.AddListener(()=> { Application.OpenURL("https://lionstudios.cc"); });

    }

    private void PanelConfig()
    {
        volumeToggle.isOn = GameSetting.AudioSwitch;
        OnVolumeToggleClick(GameSetting.AudioSwitch);


        shakeToggle.isOn = GameSetting.ShakeSwitch;
        OnShakeToggleClick(GameSetting.ShakeSwitch);

        ConnectUsInit();


    }



    private void OnVolumeToggleClick(bool isOn)
    {
        volumeToggle.targetGraphic.gameObject.SetActive(!isOn);

        GameSetting.AudioSwitch = isOn;

        GameControl.Instance.VolumeConfig();
    }

    private void OnShakeToggleClick(bool isOn)
    {
        shakeToggle.targetGraphic.gameObject.SetActive(!isOn);

        GameSetting.ShakeSwitch = isOn;
    }

    private void ConnectUsInit()
    {
        var systemLangue = Application.systemLanguage;

        switch (systemLangue)
        {
            case SystemLanguage.Korean:
                ConnectUsText.text = "우리와 연결하세요!";

                break;
            case SystemLanguage.Japanese:
                ConnectUsText.text = "私達と繋がりましょう！";

                break;
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                ConnectUsText.text = "请联系我们！";

                break;
            case SystemLanguage.ChineseTraditional:
                ConnectUsText.text = "與我們連結！";

                break;
            case SystemLanguage.Estonian:
                ConnectUsText.text = "¡Síguenos en las redes!";

                break;
            case SystemLanguage.German:
                ConnectUsText.text = "Setze dich mit uns in Verbindung!";
                break;
            case SystemLanguage.Russian:
                ConnectUsText.text = "Свяжитесь с нами!";

                break;

            default:
                ConnectUsText.text = "Connect with us!";

                break;
        }

    }


    private void OnRestoreSuccessfully()
    {
        RestoreSuccess.transform.DOLocalMoveY(0, .5f)
            .OnComplete(() => {
                RestoreSuccess.DOFade(0, .3f).SetDelay(.8f)
                .OnComplete(() => {
                    RestoreSuccess.transform.localPosition = Vector3.up * -1300;
                    RestoreSuccess.DOFade(1, .1f);
                });
            });

    }


}
