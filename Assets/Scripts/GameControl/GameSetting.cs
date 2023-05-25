using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting
{
    /********** In Game ************/
    public static float MaxSpd = 6;
    public static float _force = 45;
    public static int levelCount = 81;

    public static int CoinCount {
        get => PlayerPrefs.GetInt("CoinCount", 0);
        set {
            PlayerPrefs.SetInt("CoinCount", value);
            Messenger.Broadcast<int>(StringMgr.CoinCountChange, value);
        }
    }

    public static bool AudioSwitch
    {
        get
        {
            return PlayerPrefs.GetInt("NoAudio", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("NoAudio", value == true ? 1 : 0);
        }
    }
    public static bool ShakeSwitch
    {
        get
        {
            return PlayerPrefs.GetInt("ShakeSwitch", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("ShakeSwitch", value == true ? 1 : 0);
        }
    }

    public static bool NoAds
    {
        get
        {
            return PlayerPrefs.GetInt("NoAds", 0) == 1;
        }

        set
        {
            PlayerPrefs.SetInt("NoAds", value == true ? 1 : 0);
        }
    }

    public static bool GotTheBundle
    {
        get => PlayerPrefs.GetInt("GotTheBundle", 0) == 1;
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt("GotTheBundle", 1);
                NoAds = true;
            }
        }
    }

    /// <summary>
    /// 在游戏开始时，判定为true；
    /// 在进入下一关时，判定为false；
    /// </summary>
    public static bool IsReplay
    {
#if UNITY_EDITOR
        get; set;
#else
        get => PlayerPrefs.GetInt("IsReplay", 0) == 1;
        set => PlayerPrefs.SetInt("IsReplay", value == true ? 1 : 0);
#endif
    }

    //IOS  14.5版本
    public static bool IOS_HighVersion = false;


    //adjust沙盒模式开关
    public readonly static bool SandboxMode = false;


    /******** Debug *********/
    public static bool IsDebug;
    public static bool GreenGround;
    public static bool GreenBG;


    #region 农场参数
    public static int HasFarmGuide
    {
        get => PlayerPrefs.GetInt("HasFarmGuide", 1);
        set => PlayerPrefs.SetInt("HasFarmGuide", value);
    }
    //解锁到哪个农田
    public static int UnLockFarmId
    {
        get => PlayerPrefs.GetInt(ConfigFarm.UnLockFarmId, -1);
        set
        {
            PlayerPrefs.SetInt(ConfigFarm.UnLockFarmId, value);
        }
    }
    public static int FarmLv
    {
        get => PlayerPrefs.GetInt(ConfigFarm.FarmLv, 0);
        set
        {
            PlayerPrefs.SetInt(ConfigFarm.FarmLv, value);
        }
    }
    //农田种了什么植物
    public static int GetPlantId(int _farmId)
    {
        return PlayerPrefs.GetInt(ConfigFarm.PlantId + _farmId, 0);
    }
    public static void SetPlantId(int _farmId, int _plantId)
    {
        PlayerPrefs.SetInt(ConfigFarm.PlantId + _farmId, _plantId);
    }
    //是否缺水
    public static int GetNeedWater(int _farmId)
    {
        return PlayerPrefs.GetInt(ConfigFarm.NeedWater + _farmId, 1);
    }
    public static void SetNeedWater(int _farmId, bool _isNeed)
    {
        PlayerPrefs.SetInt(ConfigFarm.NeedWater + _farmId, _isNeed ? 1 : 0);
    }
    //是否缺肥
    public static int GetNeedFei(int _farmId)
    {
        return PlayerPrefs.GetInt(ConfigFarm.NeedFei + _farmId, 1);
    }
    public static void SetNeedFei(int _farmId, bool _isNeed)
    {
        PlayerPrefs.SetInt(ConfigFarm.NeedFei + _farmId, _isNeed ? 1 : 0);
    }
    //是否在成长（主要用于全局时间判断是否成熟条件之一，否则要用缺水缺肥2个变量过于繁琐）
    public static bool GetIsGrow(int _farmId)
    {
        return PlayerPrefs.GetInt(ConfigFarm.IsGrow + _farmId, 0) == 1 ? true : false;
    }
    public static void SetIsGrow(int _farmId, bool _isGrow)
    {
        PlayerPrefs.SetInt(ConfigFarm.IsGrow + _farmId, _isGrow ? 1 : 0);
    }
    ////收获时间
    //public static float GetHarvestTime(int _farmId)
    //{
    //    return PlayerPrefs.GetFloat(ConfigFarm.HarvestTime + _farmId, 0);
    //}
    //收获时间
    public static string GetHarvestTime(int _farmId)
    {
        return PlayerPrefs.GetString(ConfigFarm.HarvestTime + _farmId, DateTime.Now.ToString());
    }
    public static void SetHarvestTime(int _farmId, string _timeStamp)
    {
        PlayerPrefs.SetString(ConfigFarm.HarvestTime + _farmId, _timeStamp);
    }
    //种子数
    public static int GetSeedNum(int _plantId)
    {
        return PlayerPrefs.GetInt("seed" + _plantId, 0);
    }

    public static void SetSeedNum(int _plantId, int _num)
    {
        PlayerPrefs.SetInt("seed" + _plantId, _num);
    }
    //苹果树收获的次数
    public static int GetAppleHarvestCount(int _farmId)
    {
        return PlayerPrefs.GetInt(ConfigFarm.AppleHarvestCount + _farmId, 0);
    }

    public static void SetAppleHarvestCount(int _farmId, int _num)
    {
        PlayerPrefs.SetInt(ConfigFarm.AppleHarvestCount + _farmId, _num);
    }
    #endregion



}





public enum CatOrDog
{
    Default,
    Cat,
    Dog,
}

public enum AnimalType
{
    Duck = 0,
    Chick
}

public enum GameStatus
{
    GameWin,
    GameFail,
    GameFailReplay,
    InGameReplay,
    OutFarm,
    InFarm
}

public enum GameProcess
{
    OutGame,
    InGame,
    PauseGame,
}

public enum CameraType
{
    None,
    CM_LookForward,
    CM_LookLeft,
    /// <summary>
    /// 第一关聚焦
    /// </summary>
    CM_LookFocus,
    CM_Bowling,
    CM_BalloonView,
    CM_StropView,
    CM_TopView,
    CM_ShopView,
}


public enum TriggerResult
{
    TriggerWin,
    DelayWin,
    TriggerCandyWin,
    TriggerBarrowWin,
    TriggerShipWin,
    TriggerFootballWin,
    TriggerBoyWin,
    TriggerWinThingWin,

    TriggerFail = 100,
    HumanFood,

}

public enum SimapleMoveType
{
    None,
    MoveX,
    MoveZ,
}
