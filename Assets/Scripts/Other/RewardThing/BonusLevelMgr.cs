using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusLevelMgr : Singleton<BonusLevelMgr>
{
    private Transform bonusTrans;
    private string InitSkinID = "011";

    /// <summary>
    /// 前两样为老鼠和礼盒
    /// </summary>
    private int BonusThingIndex
    {
        get
        {
            var index = GameControl.Instance.BonusIndex;
            if (index >= transform.childCount)
            {
                index = (index - 2) % transform.childCount + 2;
            }
            return index;
        }
    }

    private int RewardBoxNum;


    private void Awake()
    {
        Messenger.AddListener(StringMgr.OpenRewardBoxOver, OnRewardBoxOpen);
    }

    private void Start()
    {
        BonusThingsInit();

        SDKManager.Instance.OnBonusLeveStart(BonusThingIndex + 1);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.OpenRewardBoxOver, OnRewardBoxOpen);


        SDKManager.Instance.OnBonusLevelComplete(BonusThingIndex + 1, GameControl.Instance.GotBonusList.Count);
    }


    private void BonusThingsInit()
    {
        if (BonusThingIndex != -1)
        {
            bonusTrans = transform.GetChild(BonusThingIndex);
            for (int i = 0; i < transform.childCount; i++)
            {
                var trans = transform.GetChild(i);
                trans.gameObject.SetActive(trans == bonusTrans);
            }
        }
        else
        {
            bonusTrans = GetComponentInChildren<BonusProperty>().transform;
        }

        RewardBoxNum = FindObjectsOfType<RewardBoxAction>().Length;

        var bonusProperty = bonusTrans.GetComponent<BonusProperty>();

        if (bonusProperty && bonusProperty.LevelTimer > 0)
        {
            Messenger.Broadcast(StringMgr.BonusLevelInit, bonusProperty.LevelTimer);
        }
    }





    private void OnRewardBoxOpen()
    {
        if (RewardBoxNum == 0)
        {
            //达成所有宝箱
            //弹出结算界面（临时弹出）
            GameControl.Instance.GotAllBonus = true;
            UIPanelManager.Instance.PushPanel(UIPanelType.BonusOverPanel);

            Messenger.Broadcast(StringMgr.HideGiftImage);

        }
    }


    /// <summary>
    /// 打开箱子时立刻确定奖励
    /// </summary>
    /// <returns></returns>
    public BonusType RandomBonus()
    {
        BonusType bonusType = BonusType.Coin_50;
        //两个宝箱的奖励关的，第一个宝箱固定给个皮肤
        if (RewardBoxNum == 2 && RewardBoxNum == bonusTrans.childCount)
        {
            if (SkinManager.Instance.CheckSkinGot(InitSkinID))
            {
                bonusType = GameControl.Instance.RandomBonusCoin();
            }
            else
            {
                GameControl.Instance.BonusSkin = SkinManager.Instance.FindSkin(InitSkinID);
                return BonusType.LuckySkin;
            }
        }
        else
        {
            if (RewardBoxNum > 1)
            {
                bonusType = GameControl.Instance.RandomBonusCoin();
            }
            else
            {
                bonusType = GameControl.Instance.RandomAllBonus();
            }
        }


        RewardBoxNum -= 1;
        return bonusType;
    }





}
