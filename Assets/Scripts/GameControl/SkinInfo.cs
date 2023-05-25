using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skin", menuName = "Creat Skin")]
[SerializeField]
public class SkinInfo : ScriptableObject
{
    public string SkinName;
    public string ID;
    public SkinType skinType;

    //模型
    private readonly string catModelName = "CatPrefab";
    private readonly string dogModelName = "DogPrefab";

    //材质
    private readonly string catMatName = "CatMat";
    private readonly string dogMatName = "DogMat";
    private readonly string ropeMatName = "RopeMat";

    [Header("获取方式")]
    public GetWay getWay;
    public int CoinCost;


    public Sprite Icon { get => Resources.Load<Sprite>("Icon/" + ID); }
    public Material CatMat {
        get
        {
            var mat = Resources.Load<Material>("Skins/" + ID + "/" + catMatName);
            return mat == null ? Resources.Load<Material>("Skins/000/CatMat") : mat;
        }
    }
    public Material DogMat {
        get
        {
            var mat = Resources.Load<Material>("Skins/" + ID + "/" + dogMatName);
            return mat == null ? Resources.Load<Material>("Skins/000/DogMat") : mat;
        }
    }
    public Material RopeMat {
        get
        {
            if (GameControl.Instance.levelSetting && GameControl.Instance.levelSetting.EatLevel)
            {
                var mat = Resources.Load<Material>("TempMat/" + ID + " " + ropeMatName);

                return mat == null ? Resources.Load<Material>("TempMat/000 RopeMat") : mat;
            }
            else
            {
                var mat = Resources.Load<Material>("Skins/" + ID + "/" + ropeMatName);
                return mat == null ? Resources.Load<Material>("Skins/000/RopeMat") : mat;
            }
        }
    }

    public GameObject CatSkinModel { get => Resources.Load<GameObject>("Skins/" + ID + "/" + catModelName); }
    public GameObject DogSkinModel { get => Resources.Load<GameObject>("Skins/" + ID + "/" + dogModelName); }

}


public enum SkinType
{
    /// <summary>
    /// 材质皮肤
    /// </summary>
    SkinMaterial,
    /// <summary>
    /// 外设皮肤
    /// </summary>
    SkinModel,
}

public enum GetWay
{
    WatchAd,
    CoinBuy
}

