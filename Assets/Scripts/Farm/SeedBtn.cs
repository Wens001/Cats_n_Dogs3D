using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedBtn : MonoBehaviour
{
    //获取方式
    public enum EBuyType { 
        Money,
        LookAd
    }
    public PlantType type;
    public EBuyType buyType;
    //右上角图标
    public GameObject[] rightUpIcons;
    //当前右上角图标
    private GameObject curShowIcon;
    public Text seedNumText;
    public Text seedPriceText;
    private int hasSeed;

    private void Awake()
    {
        hasSeed = GameSetting.GetSeedNum((int)type);
        curShowIcon = rightUpIcons[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        UpdateRightUpIcon(hasSeed);
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


    /// <summary>
    /// 点击有库存
    /// </summary>
    public void OnSeedBtnClick() {
        if (type > 0)
        {
            if (hasSeed > 0)
            {
                Messenger.Broadcast(ConfigFarm.ClickSeedBtn, (int)type);
                GameSetting.SetSeedNum((int)type, --hasSeed);
            }
            else
            {
                if (buyType == EBuyType.Money)
                {
                    if (GameSetting.CoinCount >= FarmMgr.plantInfoDict[(int)type].buyCost)
                    {
                        GameSetting.CoinCount -= FarmMgr.plantInfoDict[(int)type].buyCost;
                        Messenger.Broadcast(ConfigFarm.ClickSeedBtn, (int)type);
                    }
                    else {
                        Messenger.Broadcast(ConfigFarm.NoEnoughMoney);
                    }
                }
                else
                {
                    if (SDKManager.Instance.ShowRewardedAd("GetAppleSeed"))
                    {
                        isLookingAd = true;
                    }
                }
            }
            UpdateRightUpIcon(hasSeed);
        }
        else {
            Debug.LogWarning("没设定种子类型");
        }
    }

    void UpdateRightUpIcon(int _hasSeed)
    {
        GameObject newShowIcon;
        if (_hasSeed > 0)
        {
            seedNumText.text = _hasSeed.ToString();
            newShowIcon = rightUpIcons[0];
        }
        else {
            if (buyType == EBuyType.Money)
            {
                newShowIcon = rightUpIcons[1];
                seedPriceText.text = FarmMgr.plantInfoDict[(int)type].buyCost.ToString();
            }
            else {
                newShowIcon = rightUpIcons[2];
            }
        }
        //如果将要显示的右上角图标是亮的，说明不用改变
        if (!newShowIcon.activeSelf)
        {
            newShowIcon.SetActive(true);
            curShowIcon.SetActive(false);
            curShowIcon = newShowIcon;
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
            GameSetting.SetSeedNum((int)type, ++hasSeed);
            UpdateRightUpIcon(hasSeed);
            Messenger.Broadcast(ConfigFarm.ResetInterStitial);
        }
    }
    private void OnRewardAdClose()
    {
        isLookingAd = false;
    }
    #endregion
}
