using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class GuidePanel : MonoBehaviour
{
    public FarmMgr farmMgr;
    public TipUI tipUI;
    private Image blackImg;
    public Image handImg;
    public GameObject seedPanel;
    private Transform seed;
    public GameObject nutrientPanel;
    public Transform[] nutrientBtns;
    private bool[] isClickNutrientBtn;
    public GameObject returnBtn;
    public Text tipText;

    private void Awake()
    {
        blackImg = GetComponent<Image>();
        seed = seedPanel.transform.GetChild(0);
        nutrientBtns = new Transform[nutrientPanel.transform.childCount];
        isClickNutrientBtn = new bool[nutrientBtns.Length];
        for (int i = 0; i < nutrientBtns.Length; i++) {
            nutrientBtns[i] = nutrientPanel.transform.GetChild(i);
            isClickNutrientBtn[i] = false;
        }
        if (GameSetting.CoinCount < 200)
        {
            GameSetting.CoinCount = 200;
        }
    }

    private void OnEnable()
    {
        Messenger.AddListener<int>(ConfigFarm.ClickUnlockTip, OnClickUnlockTip);
        //Messenger.AddListener<int>(ConfigFarm.ClickHarvestTip, OnClickHarvestTip);
        Messenger.AddListener(ConfigFarm.ShowBlack, OnShowBalck);
        Messenger.AddListener<int>(ConfigFarm.ClickSeedBtn, OnClickSeedBtn);
        //Messenger.AddListener<int>(ConfigFarm.ClickNutrientBtn, OnClickNutrientBtn);
        Messenger.AddListener(ConfigFarm.CanHarvest, ShowHarvestGudie);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<int>(ConfigFarm.ClickUnlockTip, OnClickUnlockTip);
        //Messenger.RemoveListener<int>(ConfigFarm.ClickHarvestTip, OnClickHarvestTip);
        Messenger.RemoveListener(ConfigFarm.ShowBlack, OnShowBalck);
        Messenger.RemoveListener<int>(ConfigFarm.ClickSeedBtn, OnClickSeedBtn);
        //Messenger.RemoveListener<int>(ConfigFarm.ClickNutrientBtn, OnClickNutrientBtn);
        Messenger.RemoveListener(ConfigFarm.CanHarvest, ShowHarvestGudie);
    }


    // Start is called before the first frame update
    void Start()
    {
        ShowUnLockGudie();
    }


    public void SetHandPos(Vector3 _worldPos,Vector2 _offset) {
        handImg.rectTransform.anchoredPosition = UITool.WorldToScreenWithCamrea(_worldPos, farmMgr.canvas, _offset);       
    }
    public void SetHandPos(Vector3 _pos)
    {
        handImg.transform.position = _pos;
    }
    public void ShowUnLockGudie()
    {
        //farmMgr.farmlands[0].renderer.sortingOrder = 1;
        Renderer[] rends = farmMgr.farmlands[0].transform.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer child in rends) {
            child.sortingOrder = 1;
        }

        //tipUI.SetPos(0, UITool.WorldToScreenWithCamrea(farmMgr.farmlands[0].transform.position, farmMgr.canvas));
        tipUI.farmId = 0;
        tipUI.transform.position = farmMgr.ui.tips[0].transform.position;
        tipUI.ShowUnlockTip(true);
        SetHandPos(tipUI.transform.position);
        FarmMgr.CanTouch = false;
    }

    void OnClickUnlockTip(int _farmId) {
        if (GameSetting.CoinCount < ConfigFarm.UnLockFarmCost[0]) {
            GameSetting.CoinCount += ConfigFarm.UnLockFarmCost[0];
        }
        tipUI.ShowUnlockTip(false);
        ShowSeedGudie();
    }
    public void ShowSeedGudie()
    {
        SetHandPos(seed.transform.position);
        tipText.text = ConfigFarm.ChoseSeedTip;
        seedPanel.SetActive(true);
        FarmMgr.curChoseFarmId = 0;
    }
    void OnClickSeedBtn(int _seedId) {
        seedPanel.SetActive(false);
        SetAlpha(blackImg, 0);
        SetAlpha(handImg, 0);
        tipText.text ="";
        Timer.Register(5, () => { ShowNutrientGudie(); });
    }
    public void ShowNutrientGudie()
    {
        nutrientPanel.SetActive(true);
        SetFeiBtnActive(false);
        SetHandPos(nutrientBtns[0].transform.position);
        SetAlpha(handImg, 1);
        tipText.text = ConfigFarm.ChoseNutrientTip;
        if (GameSetting.CoinCount < 100) {
            GameSetting.CoinCount += 100;
        }
        FarmMgr.curChoseFarmId = 0;
    }

    private Image feiBg;
    private GameObject feiMask;
    void SetFeiBtnActive(bool _active) {
        if (feiBg == null) {
            feiBg = nutrientBtns[1].GetComponent<Image>();
            feiMask = nutrientBtns[1].Find("Image").gameObject;
        }
        feiBg.raycastTarget = _active;
        feiMask.SetActive(!_active);
    }
    
    public void OnClickNutrientBtn(int _nutrientType)
    {
        SetAlpha(blackImg, 0);
        SetAlpha(handImg, 0);
        int other = _nutrientType == 0 ? 1 : 0;
        isClickNutrientBtn[_nutrientType] = true;
        if (!isClickNutrientBtn[other])
        {
            Timer.Register(3, () => {
                SetHandPos(nutrientBtns[other].position);
                SetAlpha(handImg, 1);
                SetFeiBtnActive(true);
            });
        }
        else {
            tipText.text = "";
            nutrientPanel.SetActive(false);
        }
        nutrientBtns[_nutrientType].gameObject.SetActive(false);
        GameSetting.CoinCount -= ConfigFarm.nutrientCost;
        Messenger.Broadcast(ConfigFarm.ClickNutrientBtn, _nutrientType);
    }
    void ShowHarvestGudie() {
        tipText.text = ConfigFarm.MatureTip;
        tipUI.ShowHarvestTip(true);
        tipUI.UpdateHarvestCoin(FarmMgr.plantInfoDict[(int)PlantType.Pumpkin].sellCost);
        SetHandPos(tipUI.transform.position);
        SetAlpha(handImg, 1);
        StartCoroutine(CheckClickHarvest());
    }

    IEnumerator CheckClickHarvest() {
        yield return null;
        while (true) {
            if (farmMgr.farmlands[0].GetState() == Farmland.EState.Unlock) {
                OnClickHarvestTip();
                break;
            }
            yield return null;
        }
    }
    void OnClickHarvestTip()
    {
        tipText.text = "";
        SetHandPos(returnBtn.transform.position);
        SetAlpha(handImg, 0);
        FarmMgr.CanTouch = true;
        tipUI.ShowHarvestTip(false);
        //Timer.Register(2, () =>
        //{
        //    SetAlpha(handImg, 1);
        //    FarmMgr.isGuide = false;
        //    returnBtn.SetActive(true);
        //    OnShowBalck();
        //});
        Renderer[] rends = farmMgr.farmlands[0].transform.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer child in rends)
        {
            child.sortingOrder = 0;
        }
        SetAlpha(handImg, 1);
        FarmMgr.isGuide = false;
        gameObject.SetActive(false);
    }

    void OnShowBalck()
    {
        blackImg.color = new Color(0, 0, 0, 0.86f);
    }

    void SetAlpha(Image _img,float _a) {
        Color color = _img.color;
        color.a = _a;
        _img.color = color;
    }
}
