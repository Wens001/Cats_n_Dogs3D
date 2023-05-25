using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载在gameControl物体上
/// </summary>
public class SkinManager : Singleton<SkinManager>
{
    public List<SkinInfo> skinsInfo = new List<SkinInfo>();
    public List<SkinInfo> LuckySkins = new List<SkinInfo>();

    [Header("内购")]
    public List<SkinInfo> IapSkins;
    //装饰品
    private List<SkinInfo> modelSkinsInfoList = new List<SkinInfo>();

    [HideInInspector] public string CurMatSkinID { get => PlayerPrefs.GetString(StringMgr.CurMatSkinID, StringMgr.DefaultMatSkinID); }
    [HideInInspector] public string CurModelSkinID { get => PlayerPrefs.GetString(StringMgr.CurModelSkinID, ""); }

    private SkinInfo luckySkin;

    private void Awake()
    {
        //皮肤初始化（必）
        if (PlayerPrefs.GetInt("_Skin_Init_", 0) == 0)
        {
            PlayerPrefs.SetInt("_Skin_Init_", 1);

            PlayerPrefs.SetString(StringMgr.CurMatSkinID, StringMgr.DefaultMatSkinID);
            PlayerPrefs.SetString(StringMgr.CurModelSkinID, "");
            PlayerPrefs.SetInt(StringMgr.DefaultMatSkinID, 1);
        }

        foreach (var skin in skinsInfo)
        {
            if (skin.skinType == SkinType.SkinModel)
            {
                modelSkinsInfoList.Add(skin);
            }
        }
    }

    private void OnEnable()
    {
        Messenger.AddListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, OnCatNDogInit);
    }



    private void OnDisable()
    {
        Messenger.RemoveListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, OnCatNDogInit);
    }



    public void GetNewSkin(SkinInfo skin, bool putOn = true)
    {
        PlayerPrefs.SetInt(skin.ID, 1);
        SDKManager.Instance.OnSkinUnlock(skin.SkinName);

        //装备皮肤
        if (putOn)
        {
            PutOnSkin(skin);
        }
    }

    /// <summary>
    /// 装备皮肤
    /// </summary>
    /// <param name="skin"></param>
    public void PutOnSkin(SkinInfo skin, bool putOnSkin = true)
    {
        //换肤逻辑
        /***********************/
        switch (skin.skinType)
        {
            //换皮
            case SkinType.SkinMaterial:
                foreach (var renderer in CatRenderers)
                {
                    renderer.material = skin.CatMat;
                }
                foreach (var renderer in DogRenderers)
                {
                    renderer.material = skin.DogMat;
                }
                ropeRenerer.material = skin.RopeMat;

                //有外设的全身皮肤也穿上外设
                foreach (var model in WholeSkinModes)
                {
                    model.gameObject.SetActive(false);
                }
                WholeSkinModes.Clear();

                if (skin.CatSkinModel != null)
                {
                    WholeSkinModes.Add(Instantiate(skin.CatSkinModel, CatTrans));
                    WholeSkinModes.Add(Instantiate(skin.DogSkinModel, DogTrans));

                    //有装饰品时，隐藏全身皮肤的装饰
                    if (SkinModels.Count > 0)
                    {
                        foreach (var model in WholeSkinModes)
                        {
                            model.gameObject.SetActive(false);
                        }
                    }
                }

                break;
            //穿模型
            case SkinType.SkinModel:
                PutOffModelSkin();

                SkinModels.Add(Instantiate(skin.CatSkinModel, CatTrans));
                SkinModels.Add(Instantiate(skin.DogSkinModel, DogTrans));

                //隐藏全身皮肤的装饰
                foreach (var model in WholeSkinModes)
                {
                    model.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }


        if (putOnSkin)
        {
            //记录穿戴的皮肤
            switch (skin.skinType)
            {
                case SkinType.SkinMaterial:
                    PlayerPrefs.SetString(StringMgr.CurMatSkinID, skin.ID);
                    break;
                case SkinType.SkinModel:
                    PlayerPrefs.SetString(StringMgr.CurModelSkinID, skin.ID);
                    break;
                default:
                    break;
            }

            Messenger.Broadcast(StringMgr.SkinSlotRefresh);
        }
    }

    public void PutOffModelSkin()
    {
        //脱下装饰品时，显示全身皮肤的装饰
        foreach (var model in WholeSkinModes)
        {
            model.SetActive(true);
        }

        foreach (var obj in SkinModels)
        {
            obj.SetActive(false);
        }
        SkinModels.Clear();

        PlayerPrefs.SetString(StringMgr.CurModelSkinID, "");
        Messenger.Broadcast(StringMgr.SkinSlotRefresh);
    }

    #region 幸运皮肤

    int lastLuckySkinIndex = 0;
    public SkinInfo RandomLuckySkin()
    {
        if (luckySkin == null || CheckSkinGot(luckySkin))
        {
            luckySkin = null;

            //幸运皮肤
            lastLuckySkinIndex = PlayerPrefs.GetInt("lastLuckySkinIndex", 0);
            for (int i = lastLuckySkinIndex; i < LuckySkins.Count; i++)
            {
                if (!CheckSkinGot(LuckySkins[i]))
                {
                    luckySkin = LuckySkins[i];
                    lastLuckySkinIndex = i;
                    return luckySkin;
                }
            }

            //随机皮肤
            List<SkinInfo> DontGetSkins = new List<SkinInfo>();
            foreach (var skin in skinsInfo)
            {
                if (CheckSkinGot(skin) == false)
                {
                    DontGetSkins.Add(skin);
                }
            }

            if (DontGetSkins.Count > 0)
            {
                luckySkin = DontGetSkins[Random.Range(0, DontGetSkins.Count)];
            }
            else
            {
                luckySkin = null;
            }
        }
        return luckySkin;
    }

    //lose幸运皮肤
    public  bool loseIt = false;
    public void LoseTheLuckySkin()
    {
        loseIt = true;
    }


    public void OnNextLevelButtonClick()
    {
        if (loseIt)
        {
            loseIt = false;
            luckySkin = null;

            if (lastLuckySkinIndex < LuckySkins.Count)
            {
                PlayerPrefs.SetInt("lastLuckySkinIndex", lastLuckySkinIndex + 1);
            }
        }
    }


    #endregion

    #region 皮肤检测

    public bool CheckSkinGot(SkinInfo skin)
    {
        return PlayerPrefs.GetInt(skin.ID, 0) == 1;
    }

    public bool CheckSkinGot(string SkinID)
    {
        return PlayerPrefs.GetInt(SkinID, 0) == 1;
    }


    public bool CheckSkinPutOn(SkinInfo skin)
    {
        if (CurModelSkinID == skin.ID || CurMatSkinID == skin.ID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    /// <summary>
    /// 根据皮肤ID获取皮肤信息
    /// </summary>
    /// <param name="stingID"></param>
    /// <returns></returns>
    public SkinInfo FindSkin(string skinID)
    {
        foreach (var skin in skinsInfo)
        {
            if (skin.ID.Equals(skinID))
            {
                return skin;
            }
        }
        return null;
    }


    Renderer[] CatRenderers;
    Renderer[] DogRenderers;
    Renderer ropeRenerer;
    Transform CatTrans;
    Transform DogTrans;

    /// <summary>
    /// 当前装备的装饰品
    /// </summary>
    private static List<GameObject> SkinModels = new List<GameObject>();
    /// <summary>
    /// 当前装备的全身皮肤的装饰
    /// </summary>
    private static List<GameObject> WholeSkinModes = new List<GameObject>();
    private void OnCatNDogInit(TouchMove cat, TouchMove dog)
    {
        CatRenderers = cat.transform.GetChild(0).GetComponentsInChildren<Renderer>();
        DogRenderers = dog.transform.GetChild(0).GetComponentsInChildren<Renderer>();
        ropeRenerer = cat.transform.parent.GetComponentInChildren<Obi.ObiRope>().GetComponent<Renderer>();

        CatTrans = cat.transform.GetChild(0).GetChild(0).GetChild(0);
        DogTrans = dog.transform.GetChild(0).GetChild(0).GetChild(0);
        SkinModels.Clear();
        WholeSkinModes.Clear();

        //泳池关卡，固定初始化皮肤
        var boy = FindObjectOfType<BoyAction>();
        if (boy && boy.CurState == BoyState.WaitForHelp)
        {
            return;
        }


        foreach (var skin in skinsInfo)
        {
            if (CheckSkinPutOn(skin))
            {
                PutOnSkin(skin);
            }
        }

        //幸运皮肤展示界面
        if (LuckyInit)
        {
            LuckyInit = false;

            foreach (var obj in SkinModels)
            {
                obj.gameObject.SetActive(false);
            }
            SkinModels.Clear();

            foreach (var skin in skinsInfo)
            {
                if (skin.ID == StringMgr.DefaultMatSkinID)
                {
                    PutOnSkin(skin, false);
                    break;
                }
            }

            PutOnSkin(luckySkin, false);
        }
    }

    bool LuckyInit = false;
    //提示当前为幸运皮肤展示界面
    public void LuckyTryPutOn()
    {
        LuckyInit = true;

    }


    #region 内购所获皮肤

    public void IAPforSkins()
    {
        foreach (var skin in IapSkins)
        {
            GetNewSkin(skin, false);
        }
    }


    #endregion

}
