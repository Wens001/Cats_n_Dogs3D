using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipUI : MonoBehaviour
{
    public int farmId;
    [Header("解锁提示")]
    public GameObject unLockTip;
    public Text unlockCoinText;
    [Header("养分提示")]
    public GameObject nutrientTip;
    public GameObject waterTip;
    public GameObject feiTip;
    [Header("生长提示")]
    public GameObject growTip;
    public Text growTimeText;
    public Button adBtn;
    [Header("收获提示")]
    public GameObject harvestTip;   
    public Text harvestCoinText;
    // Start is called before the first frame update

    private void Awake()
    {
        adBtn.onClick.AddListener(OnClickAdBtn);
        unLockTip.GetComponentInChildren<Button>().onClick.AddListener(()=> {
            Messenger.Broadcast(ConfigFarm.ClickUnlockTip,farmId);
        });
        harvestTip.GetComponentInChildren<Button>().onClick.AddListener(() => {
            Messenger.Broadcast(ConfigFarm.ClickHarvestTip, farmId);
        });
    }

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

    public void SetPos(int _id,Vector2 _pos) {
        farmId = _id;
        GetComponent<RectTransform>().anchoredPosition = _pos;
    }

    public void ShowUnlockTip(bool _isShow) {
        unLockTip.SetActive(_isShow);
    }
    public void ShowNutrientTip(bool _isShow)
    {
        if (nutrientTip.activeSelf != _isShow)
            nutrientTip.SetActive(_isShow);
    }
    public void ShowGrowTip(bool _isShow)
    {
        if (FarmMgr.isGuide)
        {
            adBtn.gameObject.SetActive(false);
        }
        else {
            adBtn.gameObject.SetActive(true);
        }
        growTip.SetActive(_isShow);
    }
    public void ShowHarvestTip(bool _isShow)
    {
        harvestTip.SetActive(_isShow);
    }

    public void UpdateUnlockCost(int _coin) {
        unlockCoinText.text = _coin.ToString();
    }

    public void UpdateNutrient(bool _needWater,bool _needFei) {
        if (waterTip.activeSelf != _needWater) {
            waterTip.SetActive(_needWater);
        }
        if (feiTip.activeSelf != _needFei)
        {
            feiTip.SetActive(_needFei);
        }
    }

    public void UpdateGrowTime(TimeSpan _span) {
        if (_span.Hours > 0) 
            growTimeText.text = string.Format("{0}:{1:D2}:{2:D2}", _span.Hours, _span.Minutes, _span.Seconds);
        else
            growTimeText.text = string.Format("{0:D2}:{1:D2}", _span.Minutes, _span.Seconds);
    }

    public void UpdateHarvestCoin(int _coin) {
        harvestCoinText.text = _coin.ToString();
    }
    public void OnClickAdBtn() {
        if (SDKManager.Instance.ShowRewardedAd("growTime"))
        {
            isLookingAd = true;
            //Messenger.Broadcast(ConfigFarm.ClickGrowAdBtn, farmId);
        }
    }
    #region 激励奖励回调

    bool isLookingAd = false;
    /// <summary>
    /// 获取激励奖励
    /// </summary>
    private void OnGetReward()
    {
        //激励
        if (isLookingAd)
        {
            isLookingAd = false;
            Messenger.Broadcast(ConfigFarm.ClickGrowAdBtn, farmId);
        }
    }
    private void OnRewardAdClose()
    {
        isLookingAd = false;

    }

    #endregion
}
