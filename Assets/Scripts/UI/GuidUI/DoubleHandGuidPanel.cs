using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DoubleHandGuidPanel : BasePanel
{
    public Image RightHandImage;
    public Image LeftHandImage;
    int num = 0;


    private Text HintText;
    private bool LockHands
    {
        get
        {
            var curLevel = GameControl.Instance.CurLevel;
            var DoubleHandLockLevel = PlayerPrefs.GetInt(StringMgr.DoubleHandLockLevel, -1);
            if (curLevel == 2 || curLevel == DoubleHandLockLevel)
            {
                return true;
            }
            else
            {
                if (DoubleHandLockLevel == -1)
                {
                    PlayerPrefs.SetInt(StringMgr.DoubleHandLockLevel, curLevel);
                    return true;
                }

                return false;
            }
        }
    }


    public override void OnEnter()
    {
        gameObject.SetActive(true);

        PanelConfig();
        RightHandImage.transform.DOMoveY(transform.position.y + 150, 1f).SetLoops(-1, LoopType.Restart);
        LeftHandImage.transform.DOMoveY(transform.position.y + 150, 1f).SetLoops(-1, LoopType.Restart);

        num = 0;
        DOTween.To(() => num, x => num = x, 1, 2f)
            .OnComplete(() => {
                UIPanelManager.Instance.PopPanel();
            });
    }

    public override void OnExit()
    {
        gameObject.SetActive(false);
        RightHandImage.transform.DOKill();
        LeftHandImage.transform.DOKill();
        DOTween.KillAll();

        GameControl.Instance.ResumeGame();
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

        HintText = transform.GetComponentInChildren<Text>();
    }
    
    private void PanelConfig()
    {
        PanelInit();

        RightHandImage.rectTransform.position = new Vector2(Screen.width * .75f, Screen.height * .25f);
        LeftHandImage.rectTransform.position = new Vector2(Screen.width * .25f, Screen.height * .25f);

        HintText.gameObject.SetActive(LockHands);
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !LockHands)
        {
            UIPanelManager.Instance.PopPanel();
        }
    }



}
