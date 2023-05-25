using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FarmUI : BasePanel
{
    public FarmMgr farmMgr;
    public Canvas canvas;
    public Text coinText;
    public Button[] seedBtns;
    [Header("升级面板")]
    public GameObject upgradePanel;
    public RectTransform upgradeTip;
    public Text upgradeCostText;
    [Header("种子面板")]
    public GameObject seedPanel;
    [Header("养分面板")]
    public GameObject nutrientPanel;
    public GameObject waterBtn;
    public GameObject feiBtn;
    [Header("引导面板")]
    public GuidePanel guidePanel;
    [Header("农田提示")]
    public TipUI[] tips;
    [Header("黑屏切场景")]
    public Image blackImg;
    [Header("没钱提示")]
    public Transform noEnoughMoney;

    private GameObject curDownPanelChose;

    public override void OnEnter()
    {
        Messenger.AddListener<int>(StringMgr.CoinCountChange, UpdateCoin);
        Messenger.AddListener(ConfigFarm.NoEnoughMoney, ShowNoEnoughMoney);
    }

    public override void OnPause(){}
    public override void OnResume(){}

    public override void OnExit()
    {
        Messenger.RemoveListener<int>(StringMgr.CoinCountChange, UpdateCoin);
        Messenger.RemoveListener(ConfigFarm.NoEnoughMoney, ShowNoEnoughMoney);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {       
        UpdateCoin(GameSetting.CoinCount);
    }

    public void InitUI() {
        for (int i = 0; i < tips.Length; i++) {
            tips[i].SetPos(i,UITool.WorldToScreenWithCamrea(farmMgr.farmlands[i].transform.position, canvas));
        }
        guidePanel.farmMgr = farmMgr;
        upgradeTip.GetComponent<Button>().onClick.AddListener(UpgradeFarmTip);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0)) {
            TouchNothing(Input.mousePosition);
        }
    }
    /// <summary>
    /// 耕地解锁提示
    /// </summary>
    /// <param name="_unlock">是否解锁</param>
    public void UnlockTip(int _id, bool _unlock) {
        if (!_unlock)
        {            
            tips[_id].UpdateUnlockCost(farmMgr.GetUnlockCost(_id));
        }
        //解锁了隐藏ui
        tips[_id].ShowUnlockTip(!_unlock);
    }
    /// <summary>
    /// 显示选种子面板
    /// </summary>
    /// <param name="_isShow"></param>
    public void ShowSeedPanel(bool _isShow)
    {
        if (seedPanel.activeSelf != _isShow)
            seedPanel.SetActive(_isShow);
        if (_isShow) {
            if(curDownPanelChose != null && curDownPanelChose != seedPanel)
                curDownPanelChose.SetActive(false);
            curDownPanelChose = seedPanel;
        }          
        else
            curDownPanelChose = null;
    }

    public void UpdateCoin(int _count) {
        coinText.text = _count.ToString();
    }

    /// <summary>
    /// 养分提示
    /// </summary>
    /// <param name="_needWater">缺水</param>
    /// <param name="_needFei">缺肥</param>
    public void ShowNutrientTip(int _farmId, bool _needWater,bool _needFei) {       
        if (_needWater || _needFei)
        {
            tips[_farmId].UpdateNutrient(_needWater, _needFei);
            tips[_farmId].ShowNutrientTip(true);
        }
        else {
            tips[_farmId].ShowNutrientTip(false);
        }
    }
    //更新养分面板
    public void UpdateNutrientPanel(int _nutrientType, bool _isShow) {
        if (_nutrientType == 0)
        {
            if (waterBtn.activeSelf != _isShow)
            {
                waterBtn.SetActive(_isShow);
            }
        }
        else {
            if (feiBtn.activeSelf != _isShow)
            {
                feiBtn.SetActive(_isShow);
            }
        }
        if (!waterBtn.activeSelf && !feiBtn.activeSelf) {
            ShowNutrientPanel(false);
        }
    }

    /// <summary>
    /// 显示选养分面板
    /// </summary>
    /// <param name="_isShow"></param>
    public void ShowNutrientPanel(bool _isShow) {
        if (nutrientPanel.activeSelf != _isShow)
            nutrientPanel.SetActive(_isShow);
        if (_isShow)
        {
            if (curDownPanelChose!=null && curDownPanelChose != nutrientPanel)
                curDownPanelChose.SetActive(false);
            curDownPanelChose = nutrientPanel;
        }
        else
            curDownPanelChose = null;
    }

    /// <summary>
    /// 养分按钮事件
    /// </summary>
    /// <param name="_nutrientType">0:缺水，1:缺肥</param>
    public void OnNutrientBtnClick(int _nutrientType) {
        if (!FarmMgr.CanTouch) {
            return;
        }
        if (GameSetting.CoinCount >= ConfigFarm.nutrientCost)
        {
            GameSetting.CoinCount -= ConfigFarm.nutrientCost;
            Messenger.Broadcast(ConfigFarm.ClickNutrientBtn, _nutrientType);
        }
        else {
            Messenger.Broadcast(ConfigFarm.NoEnoughMoney);
        }
    }

    public void ShowGrowTimeTip(int _farmId,bool _isShow) {       
        tips[_farmId].ShowGrowTip(_isShow);
    }

    /// <summary>
    /// 成长提示
    /// </summary>
    /// <param name="_farmId">哪块土地</param>
    /// <param name="_span">还需多少秒</param>
    public void UpdateGrowTimeTip(int _farmId, TimeSpan _span) {
        if (_span.TotalSeconds > 0)
        {
            tips[_farmId].UpdateGrowTime(_span);
        }
        else {
            tips[_farmId].ShowGrowTip(false);
        }
    }
    /// <summary>
    /// 收获提示
    /// </summary>
    /// <param name="_farmId">哪块土地</param>
    /// <param name="_isShow">是否显示</param>
    /// <param name="_money">卖出的价钱</param>
    public void ShowHarvestTip(int _farmId,bool _isShow, int _money=0) {
        if (_isShow) {
            tips[_farmId].UpdateHarvestCoin(_money);
        }
        tips[_farmId].ShowHarvestTip(_isShow);
    }
    //调整升级提示位置
    public void UpdateUpgradeTipPos() {
        upgradeTip.anchoredPosition = UITool.WorldToScreenWithCamrea(farmMgr.UpgradeBan.position, canvas,new Vector2(0,-385));
    }
    public void UpdateUpgradeCost(int _cost) {
        upgradeCostText.text = _cost.ToString();
    }
    public void ShowUpgradeTip(bool _isShow) {
        if (upgradeTip.gameObject.activeSelf != _isShow) {
            upgradeTip.gameObject.SetActive(_isShow);
        }
    }
    /// <summary>
    /// 升级面板
    /// </summary>
    /// <param name="_isShow">是否显示</param>
    public void ShowUpgradePanel(bool _isShow) {
        upgradePanel.SetActive(_isShow);
    }

    public void OnClickUpgradeBtn() {
        farmMgr.UpgradeFarm();
    }
    public void OnClickNoUpgradeBtn()
    {
        FarmMgr.CanTouch = true;
        //ShowUpgradePanel(false);
    }

    public bool TouchNothing(Vector2 position)
    {
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = position;
        //射线检测ui
        List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, uiRaycastResultCache);
        if (uiRaycastResultCache.Count>0 &&uiRaycastResultCache[0].gameObject==gameObject &&!FarmMgr.isTouchFarm) {
            FarmMgr.isTouchFarm = false;
            Messenger.Broadcast(ConfigFarm.ClickNothing);
            return true;
        }
        FarmMgr.isTouchFarm = false;
        return false;
    }

    //返回按钮事件
    public void GotoMainGame() {        
        if (guidePanel.gameObject.activeSelf) {
            guidePanel.gameObject.SetActive(false);
        }
        Messenger.Broadcast(ConfigFarm.ShowInterstitial);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.planeDistance = farmMgr.startPlaneDistance;
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        GameControl.Instance.LoadLevel(GameControl.Instance.CurLevel);
        //if (farmMgr.canPopChaping)
        //{          
        //    blackImg.DOColor(new Color(0, 0, 0, 1), 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        //    {
        //        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        //        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        //        GameControl.Instance.LoadLevel(GameControl.Instance.CurLevel);
        //    });
        //}
        //else {
        //    blackImg.DOColor(new Color(0, 0, 0, 1), 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        //    {
        //        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //        canvas.planeDistance = farmMgr.startPlaneDistance;
        //        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        //        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        //        GameControl.Instance.LoadLevel(GameControl.Instance.CurLevel);
        //    });
        //}
    }

    public void GotTheBonus(int _farmId, bool haveFlyCoin,int coinNum)
    {
        //coinFlyPanel.gameObject.SetActive(true);
        Vector3 _pos = Camera.main.WorldToScreenPoint(farmMgr.farmlands[_farmId].transform.position);
        Messenger.Broadcast(StringMgr.FlyFarmCoins, _pos, coinText, coinNum);
        return;
    }

    IEnumerator noEnoughMoneyIE =null;
    WaitForSeconds oneSecond = new WaitForSeconds(1);
    IEnumerator IeNoEnoughMoney() {
        noEnoughMoney.localPosition = new Vector3(0, -1080, 0);
        noEnoughMoney.gameObject.SetActive(true);
        noEnoughMoney.DOKill();
        noEnoughMoney.DOLocalMoveY(0, 0.5f).SetEase(Ease.Linear);
        yield return oneSecond;
        noEnoughMoney.gameObject.SetActive(false);
    }

    void ShowNoEnoughMoney() {
        if (noEnoughMoneyIE != null) {
            StopCoroutine(noEnoughMoneyIE);
            noEnoughMoneyIE = null;
        }
        noEnoughMoneyIE = IeNoEnoughMoney();
        StartCoroutine(noEnoughMoneyIE);
    }

    void UpgradeFarmTip() {
        if (FarmMgr.CanTouch && !FarmMgr.isGuide)
            Messenger.Broadcast(ConfigFarm.ShowUpgradePanel);
    }
}
