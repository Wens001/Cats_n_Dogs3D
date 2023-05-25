using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FarmMgr : MonoBehaviour
{
    public Farmland[] farmlands;
    public FarmUI ui;
    [HideInInspector]
    public Canvas canvas;
    public static Dictionary<int,PlantInfo> plantInfoDict;
    public int farmLv = 0;
    public static int curChoseFarmId = -1;
    public GameObject curHouse;
    public CatNDogFarmCtrl catDogCtrl;
    public static bool CanTouch = false;
    public static bool isTouchFarm = false;
    public static bool isGuide;
    public Transform UpgradeBan;
    public ParticleSystem upgradeEffect;
    [Header("种子袋")] 
    public Transform seedBag;
    private Renderer seedRender;
    [Header("水壶")]
    public Transform kettle;
    [Header("推车")]
    public Transform cart;
    public HingeJoint[] carthand;
    public GameObject feiBags;
    [Header("农夫")]
    public Transform farmer;
    [Header("围栏")]
    public GameObject fences;
    [HideInInspector]
    public bool canPopChaping;
    [HideInInspector]
    public float startPlaneDistance;
    [Header("选中框")]
    public Transform choseTipCav;
    private MyTimer InterstitialTimer;

    private void Awake()
    {
        StartCoroutine(Init());
    }

    private void Start()
    {
        CanTouch = true;
        InterstitialTimer = new MyTimer(15);
        //StartCoroutine(AddAdEvent());
    }

    private void Update()
    {
        if (!InterstitialTimer.IsFinish && !isGuide) {
            InterstitialTimer.OnUpdate(Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            GameSetting.CoinCount -= 100;
        }
    }
    IEnumerator AddAdEvent() {
        canPopChaping = false;
        yield return new WaitForSeconds(15);
        canPopChaping = true;
    }

    IEnumerator Init() {
        //UIPanelManager.Instance.PopPanel();
        UIPanelManager.Instance.PushPanel(UIPanelType.FarmPanel);
        yield return null;

        //初始化UI
        canvas = GameControl.Instance.GetComponentInChildren<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        startPlaneDistance = canvas.planeDistance;
        canvas.planeDistance = 2;
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0;
        farmLv = GameSetting.FarmLv;   
        ui = GameObject.FindObjectOfType<FarmUI>();
        ui.farmMgr = this;
        ui.canvas = canvas;
        ui.InitUI();
        //初始化场景物体
        for (int i = 0; i < farmlands.Length; i++)
        {
            farmlands[i].farmMgr = this;
            farmlands[i].id = i;
        }
        int showFarmCount = PlayerPrefs.GetInt(ConfigFarm.UnLockFarmId, -1) + 1;
        showFarmCount = showFarmCount < farmlands.Length ? showFarmCount : farmlands.Length - 1;
        for (int i = 0; i <= showFarmCount; i++)
        {
            farmlands[i].ShowFarm();
        }
        catDogCtrl.DogHeadMove(catDogCtrl.dogHead.transform.position + new Vector3(2.5f, 0, 0), () =>
        {
            catDogCtrl.dogHead.transform.localEulerAngles = new Vector3(0, 90, 0);
        });
        ShowHouse();
        ui.UpdateUpgradeTipPos();
        isGuide = GameSetting.HasFarmGuide == 1 ? true : false;
        if (isGuide)
        {
            GameSetting.HasFarmGuide = 0;
            ui.guidePanel.gameObject.SetActive(true);
        }
        seedRender = seedBag.GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        Messenger.AddListener(ConfigFarm.ShowUpgradePanel, ShowUpgradePanel);
        Messenger.AddListener<int>(ConfigFarm.ClickSeedBtn, OnClickSeedBtn);
        Messenger.AddListener<int>(ConfigFarm.ClickNutrientBtn, OnClickNutrientBtn);
        Messenger.AddListener(ConfigFarm.ClickNothing, OnClickNothing);
        Messenger.AddListener<int>(ConfigFarm.ClickUnlockTip, OnClickUnlockTip);
        Messenger.AddListener<int>(ConfigFarm.ClickHarvestTip, OnClickHarvestTip);
        Messenger.AddListener<int>(ConfigFarm.ClickGrowAdBtn, OnClickGrowAdBtn);
        Messenger.AddListener(ConfigFarm.ShowInterstitial, ShowInterstitial);
        Messenger.AddListener(ConfigFarm.ResetInterStitial,ResetInterstitialTime);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(ConfigFarm.ShowUpgradePanel, ShowUpgradePanel);
        Messenger.RemoveListener<int>(ConfigFarm.ClickSeedBtn, OnClickSeedBtn);
        Messenger.RemoveListener<int>(ConfigFarm.ClickNutrientBtn, OnClickNutrientBtn);
        Messenger.RemoveListener(ConfigFarm.ClickNothing, OnClickNothing);
        Messenger.RemoveListener<int>(ConfigFarm.ClickUnlockTip, OnClickUnlockTip);
        Messenger.RemoveListener<int>(ConfigFarm.ClickHarvestTip, OnClickHarvestTip);
        Messenger.RemoveListener<int>(ConfigFarm.ClickGrowAdBtn, OnClickGrowAdBtn);
        Messenger.RemoveListener(ConfigFarm.ShowInterstitial, ShowInterstitial);
        Messenger.RemoveListener(ConfigFarm.ResetInterStitial, ResetInterstitialTime);
    }


    /// <summary>
    /// 获取对应耕地的解锁费用
    /// </summary>
    /// <param name="_farmId"></param>
    /// <returns></returns>
    public int GetUnlockCost(int _farmId) {
        int unLockCostId = _farmId < ConfigFarm.UnLockFarmCost.Length ? _farmId : ConfigFarm.UnLockFarmCost.Length - 1;
        return ConfigFarm.UnLockFarmCost[unLockCostId];
    }

    void ShowHouse() {
        if (farmLv < ConfigFarm.UpgradeCost.Length)
        {
            ui.UpdateUpgradeCost(ConfigFarm.UpgradeCost[farmLv]);
            ui.ShowUpgradeTip(true);
        }
        else
        {
            StartCoroutine(FarmerHangOut());           
            ui.ShowUpgradeTip(false);
            UpgradeBan.gameObject.SetActive(false);
            fences.SetActive(true);
        }
        if (farmLv <= 0)
            return;
        curHouse.SetActive(false);
        GameObject newHouse = Instantiate(Resources.Load<GameObject>("Prefab/farm0" + farmLv), curHouse.transform.parent);
        newHouse.transform.position = curHouse.transform.position;
        newHouse.transform.rotation = curHouse.transform.rotation;
        newHouse.transform.localScale = curHouse.transform.localScale;
        curHouse = newHouse;
    }

    //可以升级不
    public bool canUpgrade() {
        if (farmLv < ConfigFarm.UpgradeCost.Length) {
            if (GameSetting.CoinCount >= ConfigFarm.UpgradeCost[farmLv]) {
                return true;
            }
        }
        return false;
    }

    void ShowUpgradePanel() {
        Messenger.Broadcast(ConfigFarm.ShowInterstitial);
        if (canUpgrade())
        {
            //CanTouch = false;
            UpgradeFarm();
        }
        else {
            Messenger.Broadcast(ConfigFarm.NoEnoughMoney);
        }
    }

    //升级农场
    public void UpgradeFarm()
    {
        if (canUpgrade()) {
            upgradeEffect.gameObject.SetActive(true);
            upgradeEffect.Play();
            GameSetting.CoinCount -= ConfigFarm.UpgradeCost[farmLv];
            farmLv++;
            GameSetting.FarmLv = farmLv;
            ShowHouse();
            //ui.ShowUpgradePanel(false);            
            CanTouch = true;
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.UpgradeFarmClip);
        }
    }

    //显示耕地,用在解锁新耕地时，把下一个耕地显示
    public void ShowFarm(int _farmId) {
        if (_farmId < farmlands.Length) {
            farmlands[_farmId].ShowFarm();
        }
    }

    /// <summary>
    /// 点击种子事件回调
    /// </summary>
    /// <param name="_seedId"></param>
    public void OnClickSeedBtn(int _seedId)
    {
        if (curChoseFarmId >= 0) {           
            CanTouch = false;
            farmlands[curChoseFarmId].OnClickSeedBtn(_seedId);
            catDogCtrl.DoKillAnim();
            //farmlands[curChoseFarmId].OnClickSeedBtn(_seedId);
            //第一种是放在猫狗中间，猫狗包围推上去，有几率没包住或者推飞了
            //seedBag.position = ConfigFarm.catdogStartPos + new Vector3(0, 0.3f, 2);
            //seedBag.localEulerAngles = new Vector3(-90, 0, 0);
            //第二种是放在狗嘴边，固定跟随狗头(需要取消种子袋的刚体和碰撞器)
            seedRender.material.mainTexture = Resources.Load<Texture>("UI/" + (PlantType)_seedId);
            seedBag.SetParent(catDogCtrl.dogHead.transform);
            seedBag.localPosition = new Vector3(-0.37f, 0, 0.26f);
            seedBag.gameObject.SetActive(true);
            catDogCtrl.BothHeadMove(farmlands[curChoseFarmId].transform.position, () =>
            {
                seedBag.gameObject.SetActive(false);
                ToSow();
                //curChoseFarmId = -1;               
                //Debug.Log(string.Format("第{0}块农田播种了{1}",curChoseFarmId+1, plantInfoDict[_seedId].name));              
                catDogCtrl.BothHeadMove(ConfigFarm.catdogStartPos,()=> {
                    CanTouch = true;
                    Messenger.Broadcast(ConfigFarm.ShowBlack);
                } );
            });
            Messenger.Broadcast(ConfigFarm.ShowInterstitial);
        }
        ui.ShowSeedPanel(false);
    }
    /// <summary>
    /// 点击养分按钮事件回调
    /// </summary>
    /// <param name="_nutrientType">0：浇水，1：施肥</param>
    public void OnClickNutrientBtn(int _nutrientType)
    {
        if (curChoseFarmId >= 0)
        {
            Messenger.Broadcast(ConfigFarm.ShowInterstitial);
            CanTouch = false;
            catDogCtrl.DoKillAnim();
            if (_nutrientType == 0)
            {
                kettle.SetParent(catDogCtrl.dogHead.transform);
                kettle.localPosition = new Vector3(0.014f, -0.24f, 0.42f);
                kettle.localEulerAngles = new Vector3(0, -90, -17.5f);
                kettle.gameObject.SetActive(true);
                catDogCtrl.GoToWatering(farmlands[curChoseFarmId].transform.position, () =>
                {
                    farmlands[curChoseFarmId].GiveNutrient(_nutrientType);
                    kettle.gameObject.SetActive(false);
                    catDogCtrl.BothHeadMove(ConfigFarm.catdogStartPos, () =>
                    {
                        CanTouch = true;
                        Messenger.Broadcast(ConfigFarm.ShowBlack);
                    });
                });
            }
            else {
                //catDogCtrl.BothHeadMove(farmlands[curChoseFarmId].transform.position, () =>
                //{
                //    farmlands[curChoseFarmId].GiveNutrient(_nutrientType);
                //    kettle.gameObject.SetActive(false);
                //    catDogCtrl.BothHeadMove(ConfigFarm.catdogStartPos, () =>
                //    {
                //        CanTouch = true;
                //        Messenger.Broadcast(ConfigFarm.ShowBlack);
                //    });
                //});
                StartCoroutine(GotoFei(farmlands[curChoseFarmId].transform.position));
            }
            ui.UpdateNutrientPanel(_nutrientType, false);          
        }
    }

    IEnumerator GotoFei(Vector3 _pos)
    {
        cart.position = new Vector3(0, 0, -5.6f);
        cart.gameObject.SetActive(true);
        feiBags.SetActive(true);
        catDogCtrl.SetBothLock(true);
        catDogCtrl.dogHead.rigi.constraints = RigidbodyConstraints.None;
        catDogCtrl.catHead.rigi.constraints = RigidbodyConstraints.None;
        carthand[0].connectedBody = catDogCtrl.catHead.rigi;
        carthand[1].connectedBody = catDogCtrl.dogHead.rigi;

        _pos -= new Vector3(0, 0, 3);
        Vector3 dir = _pos - catDogCtrl.transform.position;
        dir.y = 0;
        float moveTime = dir.magnitude / (GameSetting.MaxSpd + 2);
        bool isReach = false;
        cart.DOMove(_pos, moveTime).SetUpdate(UpdateType.Fixed).onComplete = () =>
        {
            isReach = true;
        };
        while (!isReach) {
            yield return null;
        }
        cart.DORotate(new Vector3(0, 90, 60), 0.5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1);
        feiBags.SetActive(false);
        farmlands[curChoseFarmId].GiveNutrient(1);
        cart.DORotate(new Vector3(0, 90, 0), 0.5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1);        
        cart.DOMove(ConfigFarm.catdogStartPos, moveTime).SetUpdate(UpdateType.Fixed).onComplete = () =>
        {
            cart.gameObject.SetActive(false);
            catDogCtrl.SetBothLock(false);
            carthand[0].connectedBody = null;
            carthand[1].connectedBody = null;
            catDogCtrl.dogHead.rigi.constraints = RigidbodyConstraints.FreezeRotationX| RigidbodyConstraints.FreezeRotationZ;
            catDogCtrl.catHead.rigi.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        };
        CanTouch = true;
        //Messenger.Broadcast(ConfigFarm.ShowBlack);
    }

    void ToSow() {
        farmlands[curChoseFarmId].ShowSowAnim();
    }

    void OnClickNothing() {
        ui.ShowSeedPanel(false);
        ui.ShowNutrientPanel(false);
        ShowChoseTipCav(curChoseFarmId, false);
    }

    void OnClickUnlockTip(int _farmId) {
        if (_farmId >= 0) {
            farmlands[_farmId].ClickUnlockBtn();
        }
    }

    void OnClickHarvestTip(int _farmId) {
        if (_farmId >= 0)
        {
            farmlands[_farmId].ClickHarvestTip();
        }
    }

    void OnClickGrowAdBtn(int _farmId) {
        if (_farmId >= 0)
        {
            farmlands[_farmId].SetMaturityNow();
            ResetInterstitialTime();
        }
    }

    Vector3 leftPos = new Vector3(-8, 0, 14.7f);
    Vector3 rightPos = new Vector3(8, 0, 14.7f);
    //农夫闲逛
    IEnumerator FarmerHangOut() {
        farmer.gameObject.SetActive(true);
        yield return null;
        Animator farmreAnim = farmer.GetComponentInChildren<Animator>();
        WaitForSeconds wait3Second = new WaitForSeconds(5);
        while (true) {
            farmreAnim.SetBool("walk", true);
            farmer.DORotate(new Vector3(0, 90, 0), 0.5f).SetEase(Ease.Unset);
            farmer.DOMove(rightPos, 5).SetEase(Ease.Linear);
            yield return wait3Second;
            farmreAnim.SetBool("walk", false);
            farmer.DORotate(new Vector3(0, Random.Range(180, 210), 0),1).SetEase(Ease.Unset);
            yield return new WaitForSeconds(Random.Range(5, 15)) ;
            farmer.DORotate(new Vector3(0, 270, 0), 0.5f).SetEase(Ease.Unset);
            farmer.DOMove(leftPos, 5).SetEase(Ease.Linear);
            farmreAnim.SetBool("walk", true);
            yield return wait3Second;
            farmreAnim.SetBool("walk", false);
            farmer.DORotate(new Vector3(0, Random.Range(150, 180), 0), 1).SetEase(Ease.Unset);
            yield return new WaitForSeconds(Random.Range(5, 15));
        }
    }

    public void ShowChoseTipCav(int _farmId,bool _isShow) {
        if (_isShow) {
            choseTipCav.position = farmlands[_farmId].transform.position;
        }
        if(_isShow != choseTipCav.gameObject.activeSelf)
            choseTipCav.gameObject.SetActive(_isShow);
    }
    //显示插屏
    void ShowInterstitial() {
        if (InterstitialTimer.IsFinish && !isGuide) {
            Debug.Log("农场插屏广告");
            InterstitialTimer.ReStart();
            SDKManager.Instance.ShowInterstitial(GameStatus.InFarm);            
        }
    }

    void ResetInterstitialTime() {
        InterstitialTimer.ReStart();
        Debug.Log("重置农场插屏广告");
    }
}
