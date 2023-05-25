using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmland : MonoBehaviour
{
    public enum EState { 
        Lock,//未开锁
        Unlock,//解锁了
        Sow,//播种
        Bud,//幼芽状态，需要养分
        Grow,//生长状态
        Mature//成熟状态
    }
    [HideInInspector]
    public FarmMgr farmMgr;
    private EState state = EState.Unlock;
    [HideInInspector]
    public int id;
    public PlantInfo plantInfo;

    private bool needWater = true;//缺水
    private bool needFei = true;//缺肥
    private DateTime harvestTime;//预期收获时间
    private bool isBIgTree = false;//苹果树成熟过了吗

    //组件
    [HideInInspector]
    public new Renderer renderer;
    public Transform soil;
    public Transform seed;
    private Transform gain;
    private Transform apple_fruit;
    Transform AppleFruit
    {
        get {
            if (apple_fruit == null)
            {
                if (gain != null)
                {
                    apple_fruit = gain.Find("obj_98");
                }
            }
            return apple_fruit;
        }       
    }

    // Start is called before the first frame update
    void Start()
    {
        InitFarm();
    }

    string GetParm(string _parm) {
        return _parm + id;
    } 
    /// <summary>
    /// 初始化
    /// </summary>
    void InitFarm() {
        TryGetComponent(out renderer);
        //没解锁
        if (GameSetting.UnLockFarmId < id)
        {
            //参数赋值
            needWater = true;
            needFei = true;
            //harvestTime = 0;
            SetState(EState.Lock);
        }
        else {
            //参数赋值
            needWater = GameSetting.GetNeedWater(id) == 1 ? true : false;
            needFei = GameSetting.GetNeedFei(id) == 1 ? true : false;
            harvestTime = TimeTool.StringToDateTime(GameSetting.GetHarvestTime(id));

            int plantId = GameSetting.GetPlantId(id);
            if (plantId > 0)
            {
                plantInfo = FarmMgr.plantInfoDict[plantId];
                ShowSoidPlant(true);
                //是否缺养分
                if (NeedNutrient())
                {
                    SetState(EState.Bud);
                }
                else if (!IsMaturity())
                {
                    //还在生长
                    SetState(EState.Grow);
                }
                else {
                    //成熟了
                    SetState(EState.Mature);
                }
            }
            else {
                //还没种下植物
                SetState(EState.Unlock);
            }           
        }      
    }    

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            GameSetting.CoinCount += 1000;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetMaturityNow();
        }
#endif     
    }

    private void OnMouseDown()
    {
        FarmMgr.isTouchFarm = true;
        if (!FarmMgr.CanTouch)
            return;
        if (FarmMgr.isGuide && id > 0) {
            return;
        }
        FarmMgr.curChoseFarmId = id;
        farmMgr.ShowChoseTipCav(id, true);
        switch (state)
        {
            case EState.Lock:
                ClickUnlockBtn();
                break;
            case EState.Unlock:               
                farmMgr.ui.ShowSeedPanel(true);
                break;
            case EState.Bud:
                farmMgr.ui.UpdateNutrientPanel(0,needWater);
                farmMgr.ui.UpdateNutrientPanel(1, needFei);
                if(NeedNutrient())
                    farmMgr.ui.ShowNutrientPanel(true);
                break;
            case EState.Grow:
                Messenger.Broadcast(ConfigFarm.ClickNothing);
                farmMgr.ShowChoseTipCav(id,true);
                break;
            case EState.Mature:
                ClickHarvestTip();
                break;
        }
        Messenger.Broadcast(ConfigFarm.ShowInterstitial);
        //ToHarvest();
    }

    public EState GetState()
    {
        return state;
    }
    //设置状态
    void SetState(EState _state)
    {
        switch (_state)
        {
            case EState.Lock:
                ToLock();
                break;
            case EState.Unlock:
                ToUnlock();
                break;
            case EState.Sow:
                ToSow();
                break;
            case EState.Bud:
                ToBud();
                break;
            case EState.Grow:
                ToGrow();
                break;
            case EState.Mature:
                ToMaturity();
                break;
        }
        state = _state;
        Debug.Log("set to "+_state);
    }
    //显示耕地
    public void ShowFarm()
    {
        gameObject.SetActive(true);
    }
    //锁住耕地
    void ToLock() {
        renderer.material.SetColor("Color",new Color(1,1,1,0.5f));
        //UI显示
        farmMgr.ui.UnlockTip(id,false);
    }

    public void ClickUnlockBtn() {       
        //超过耕地解锁id，用最后一个。例如耕地4以后都是1000，那么耕地5的花费id用耕地4的id
        int unLockCost = farmMgr.GetUnlockCost(id);
        if (state == EState.Lock && GameSetting.CoinCount >= unLockCost)
        {
            SetState(EState.Unlock);
            GameSetting.UnLockFarmId = id;
            GameSetting.CoinCount -= unLockCost;
            //farmMgr.ui.UpdateCoin();
            farmMgr.ShowFarm(id + 1);
            Debug.Log(string.Format("解锁耕地{0},花费{1}", id, unLockCost));
        }
        else {
            Messenger.Broadcast(ConfigFarm.NoEnoughMoney);
        }
        Messenger.Broadcast(ConfigFarm.ShowInterstitial);
    }
    
    //解锁耕地
    void ToUnlock() {
        renderer.material.SetColor("Color", new Color(1, 1, 1, 1));
        farmMgr.ui.UnlockTip(id,true);
    }
    public void OnClickSeedBtn(int _seedId) {
        plantInfo = FarmMgr.plantInfoDict[_seedId];
        SetState(EState.Sow);

    }
    /// <summary>
    /// 播种
    /// </summary>
    public void ToSow() {
        SetPlantId(plantInfo.id);        
    }
    //播种动画
    public void ShowSowAnim() {
        soil.gameObject.SetActive(true);
        seed.localScale = Vector3.zero;
        seed.gameObject.SetActive(true);
        seed.DOScale(ConfigFarm.budSize, 1.5f).OnComplete(()=> {
            SetState(EState.Bud);
            farmMgr.ui.UpdateNutrientPanel(0, needWater);
            farmMgr.ui.UpdateNutrientPanel(1, needFei);
            if (NeedNutrient())
                farmMgr.ui.ShowNutrientPanel(true);
        });
    }

    void ShowSoidPlant(bool _isShow) {
        if (soil.gameObject.activeSelf!=_isShow)
        {
            soil.gameObject.SetActive(_isShow);
            seed.localScale = ConfigFarm.budSize;
            seed.gameObject.SetActive(_isShow);
            if (plantInfo!=null && plantInfo.id == (int)PlantType.Apple && GameSetting.GetAppleHarvestCount(id)>0) {
                if (gain == null) {
                    gain = Instantiate(Resources.Load<GameObject>("Prefab/" + plantInfo.name)).transform;
                    gain.position = transform.position;
                    ShowAppleFruie(false);
                    //gain.Find("obj_98").gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 幼芽缺养分状态
    /// </summary>
    void ToBud() {       
        farmMgr.ui.ShowNutrientTip(id, needWater, needFei);
    }
    /// <summary>
    /// 是否需要养分（水和土）
    /// </summary>
    /// <returns></returns>
    public bool NeedNutrient() {
        if (needWater || needFei)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 给养分，0：浇水，1：施肥
    /// </summary>
    /// <param name="_nutrientType"></param>
    public void GiveNutrient(int _nutrientType) {
        if (_nutrientType == 0) {
            SetNeedWater(false);
        }
        else {
            SetNeedFei(false);
        }
        farmMgr.ui.ShowNutrientTip(id, needWater, needFei);
        if (!NeedNutrient()) {
            //新手引导第一次3秒
            if(FarmMgr.isGuide)
                SetHarvestTime(4);
            else
                SetHarvestTime(plantInfo.growTime);
            SetState(EState.Grow);                     
        }
    }
    /// <summary>
    /// 如果不缺水和不缺肥就倒计时生长
    /// </summary>
    void ToGrow() {
        FarmTimeMgr.Instance.UpdateHarvestTimes(id, harvestTime,true);
        GameSetting.SetIsGrow(id, true);
        StartCoroutine(IEGrowTime());
        farmMgr.ui.ShowGrowTimeTip(id,true);
    }
    WaitForSeconds halfSecond = new WaitForSeconds(0.2f);
    //生长倒计时
    IEnumerator IEGrowTime() {
        TimeSpan timespan ;
        while (true) {
            timespan = harvestTime - DateTime.Now;
            farmMgr.ui.UpdateGrowTimeTip(id,timespan);
            if (timespan.TotalSeconds > 0)
                yield return halfSecond;
            else {
                if (gain == null)
                {
                    gain = Instantiate(Resources.Load<GameObject>("Prefab/" + plantInfo.name)).transform;
                    gain.position = transform.position;
                    gain.localScale = Vector3.zero;
                    gain.DOScale(Vector3.one, 1).OnComplete(() =>
                    {
                        SetState(EState.Mature);
                    });
                }
                else
                {
                    AppleFruit.DOScale(ConfigFarm.appleFruitSize, 1).onComplete=()=> SetState(EState.Mature);
                }
                break;
            }
        }        
    }

    /// <summary>
    /// 成熟了
    /// </summary>
    void ToMaturity() {
        if (gain == null)
        {
            gain = Instantiate(Resources.Load<GameObject>("Prefab/" + plantInfo.name)).transform;
            gain.position = transform.position;
        }
        else {
            //苹果树的
            if (plantInfo.id == (int)PlantType.Apple)
                ShowAppleFruie(true);
                //gain.Find("obj_98").gameObject.SetActive(true);
        }
        farmMgr.ui.ShowHarvestTip(id,true,plantInfo.sellCost);       
        //Messenger.Broadcast(ConfigFarm.CanHarvest);
    }
    /// <summary>
    /// 收获
    /// </summary>
    void ToHarvest() {

    }
    //成熟了吗
    bool IsMaturity() {
        return (harvestTime - DateTime.Now).TotalSeconds > 0 ? false : true;
    }

    public void ClickHarvestTip() {       
        GameSetting.SetIsGrow(id, false);
        //GameSetting.CoinCount += plantInfo.sellCost;
        farmMgr.ui.GotTheBonus(id,id==0?true:false, plantInfo.sellCost);
        SetNeedWater(true);
        SetNeedFei(true);
        GameObject ps = Instantiate(Resources.Load<GameObject>("Prefab/HarvestEffect"));
        ps.transform.position = transform.position;
        if (plantInfo.id < (int)PlantType.Apple)
        {
            Destroy(gain.gameObject);
            gain = null;
            plantInfo = null;
            ShowSoidPlant(false);
            SetPlantId(0);
            SetState(EState.Unlock);
        }
        else
        {
            //gain.Find("obj_98").gameObject.SetActive(false);
            ShowAppleFruie(false);
            GameSetting.SetAppleHarvestCount(id, GameSetting.GetAppleHarvestCount(id) + 1);
            SetState(EState.Bud);
        }
        farmMgr.ui.ShowHarvestTip(id, false);
        FarmTimeMgr.Instance.ToHarvest(id);
        Messenger.Broadcast(ConfigFarm.ClickNothing);
        Messenger.Broadcast(ConfigFarm.ShowInterstitial);
    }

    /// <summary>
    /// 立马成熟
    /// </summary>
    public void SetMaturityNow() {
        SetHarvestTime(0);
        FarmTimeMgr.Instance.UpdateHarvestTimes(id, harvestTime, !needWater && !needFei);
    }

    void ShowAppleFruie(bool _isShow) {
        if (_isShow)
            AppleFruit.localScale = ConfigFarm.appleFruitSize;
        else
            AppleFruit.localScale = Vector3.zero;
    }
    

    #region 设置参数并保存

    void SetNeedWater(bool _isNeed) {
        needWater = _isNeed;
        GameSetting.SetNeedWater(id, _isNeed);
    }

    void SetNeedFei(bool _isNeed) {
        needFei = _isNeed;
        GameSetting.SetNeedFei(id, _isNeed);
    }
    void SetHarvestTime(int _growTime) {
        harvestTime = DateTime.Now.AddSeconds(_growTime);
        Debug.Log("预计成熟时间: "+harvestTime.ToString());
        GameSetting.SetHarvestTime(id, harvestTime.ToString());
    }
    void SetPlantId(int _plantId) {
        plantInfo = FarmMgr.plantInfoDict[_plantId];
        GameSetting.SetPlantId(id, _plantId);
    }
    #endregion
}
