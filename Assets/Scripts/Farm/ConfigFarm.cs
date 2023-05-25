using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigFarm {
    public const int MaxFarmNum = 6;
    //当前最新解锁的耕地id
    public const string UnLockFarmId = "UnLockFarmId";
    public const string NeedWater = "NeedWater";
    public const string NeedFei = "NeedFertilizer";
    public const string IsGrow = "IsGrow";
    public const string HarvestTime = "HarvestTime";
    public const string AppleHarvestCount = "AppleHarvestCount";
    public const string FarmLv = "FarmLv";
    public const string PlantId = "PlantId";

    //猫狗起始点
    public static Vector3 catdogStartPos= new Vector3(0,0,-7.2f);
    public static int[] UnLockFarmCost = { 50, 200, 500, 1000 };
    public static int[] UpgradeCost = { 500, 1000, 2000};
    public static int nutrientCost = 50;
    //幼芽起始大小
    public static Vector3 budSize = new Vector3(3, 3, 3);
    public static Vector3 appleFruitSize = new Vector3(2.9f, 2.9f, 2.9f);
    //引导界面
    public const string GotoFarm = "You've got some seeds,\nLet's go to the farm!";
    public const string ChoseSeedTip = "Choose the seeds you wanna plant";
    public const string ChoseNutrientTip = "Crops need water and fertilizer to grow";
    public const string GrowTip = "Crops need time to grow";
    public const string MatureTip = "The crop is mature,\nclick to harvest and sell your crop";

    //回调事件
    public const string Sow = "Sow";
    public const string ShowUpgradePanel = "UpdradeFarm";
    public const string ClickUnlockTip = "ClickUnlockTip";
    public const string ClickSeedBtn = "ClickSeedBtn";
    public const string ClickGrowAdBtn = "ClickGrowAdBtn";
    public const string ClickNutrientBtn = "ClickNutrientBtn";
    public const string CanHarvest = "CanHarvest";
    public const string ClickHarvestTip = "ClickHarvestTip";
    public const string ClickNothing = "ClickNothing";
    public const string ShowBlack = "ShowBlack";
    public const string ShowInterstitial = "ShowInterstitial";
    public const string ResetInterStitial = "ResetInterStitial";
    public const string NoEnoughMoney = "NoEnoughMoney";
}
