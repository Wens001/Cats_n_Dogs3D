using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RatePanel : BasePanel
{
    public RectTransform ThanksRate;
    public Button LaterButton;
    public Button RateButton;

    [Header("星级")]
    public Button OneButton;
    public Button TwoButton;
    public Button ThreeButton;
    public Button FourButton;
    public Button FiveButton;

    


    private int TheScore;

    private Transform[] stars;



    public override void OnEnter()
    {
        PanelConfig();

        gameObject.SetActive(true);
    }

    public override void OnExit()
    {

        gameObject.SetActive(false);
        GameControl.Instance.ResumeGame();

    }

    public override void OnPause()
    {
    }

    public override void OnResume()
    {
    }


    private bool haveInit = false;
    private void PanelInit()
    {
        if (haveInit)
        {
            return;
        }
        haveInit = true;

        stars = new Transform[] { OneButton.transform.GetChild(0), TwoButton.transform.GetChild(0), ThreeButton.transform.GetChild(0), FourButton.transform.GetChild(0), FiveButton.transform.GetChild(0) };

        OneButton.onClick.AddListener(() => { RateScore(1); });
        TwoButton.onClick.AddListener(() => { RateScore(2); });
        ThreeButton.onClick.AddListener(() => { RateScore(3); });
        FourButton.onClick.AddListener(() => { RateScore(4); });
        FiveButton.onClick.AddListener(() => { RateScore(5); });

        LaterButton.onClick.AddListener(()=> { UIPanelManager.Instance.PopPanel(); });
        RateButton.onClick.AddListener(() => { OnRateButtonClick(); });

    }


    private void PanelConfig()
    {
        PanelInit();

        foreach (var star in stars)
        {
            star.gameObject.SetActive(false);
        }

        RateButton.gameObject.SetActive(false);

        ThanksRate.anchoredPosition = Vector3.zero;
        ThanksRate.gameObject.SetActive(false);
    }




    private void RateScore(int score)
    {
        TheScore = score;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i < score);
        }

        RateButton.gameObject.SetActive(true);
    }



    private void OnRateButtonClick()
    {
        if (TheScore <= 3)
        {
            ThanksRate.gameObject.SetActive(true);
            LaterButton.enabled = false;
            RateButton.enabled = false;

            int num = 0;
            DOTween.To(()=> num, x => num = x, 3, .8f)
                .OnStart(()=> { ThanksRate.DOMoveY(1300, .5f); })
                .OnComplete(() => { UIPanelManager.Instance.PopPanel(); });

        }
        else
        {
            //Application.OpenURL("https://play.google.com/store/apps/details?id=com.DefaultCompany.CatNDog");


#if UNITY_EDITOR
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + "com.DefaultCompany.CatNDog");
#elif UNITY_ANDROID
            Application.OpenURL("market://details?id=" + "com.DefaultCompany.CatNDog");
#endif

            UIPanelManager.Instance.PopPanel();
        }

        Debug.Log("评分：" + TheScore);
    }


}
