
public static class StringMgr
{
    //tager
    public static class Tags
    {
        public static string LockObject = "LockObject";
        public static string Coin = "Coin";
        public static string EatThing_Cat = "EatThing_Cat";
        public static string EatThing_Dog = "EatThing_Dog";

    }

    //layer
    public static class Layer
    {
        public static string CatOrDog = "CatOrDog";
        public static string Ground = "Ground";
    }

    public class Event
    {
        public const string OnRestoreSuccessfully = "OnRestoreSuccessfully";

    }

    //setting
    public static string CurLevel = "levelIndex";
    public static string IsDebug = "IsDebug";

    //playerPrefs
    public static string DoubleHandLockLevel = "DoubleHandLockLevel";
    public static string HaveRate = "HaveRate";
    public static string FirstFarm = "FirstFarm";


    //EventInGame
    public static string LevelInit = "LevelInit";
    public static string CatNDogInit = "CatNDogInit";
    public static string GameStart = "GameStart";
    public static string GameWin = "GameWin";
    public static string GetWinCondition = "GetWinCondition";
    public static string LoseWinCondition = "LoseWinCondition";
    public static string BuyTheBundle = "BuyTheBundle";
    public static string FlyCoins = "FlyCoins";
    public static string FlyOtherCoins = "FlyOtherCoins";
    public static string FlyFarmCoins = "FlyFarmCoins";
    public static string FlyCoinsOver = "FlyCoinsOver";
    public static string EatSomeThing = "EatSomeThing";



    //otherEvent
    public static string ShootAgain = "ShootAgain";
    public static string OpenTheValve = "OpenTheValve";
    public static string OpenRewardBoxOver = "OpenRewardBoxOver";
    public static string BonusLevelInit = "BonusLevelInit";
    /// <summary>
    /// 要求其他对应的位置显示对应结果
    /// </summary>
    public static string ShowTheResult = "ShowTheResult";
   


    //锁定
    public static string LockHead = "LockHead";
    public static string UnlockHead = "HeadUnlock";
    public static string BouthLock = "BouthLock";
    public static string BouthDeathLock = "BouthDeathLock";
    public static string BallShotGuide = "BallShotGuide";
    
    //提示
    public static string HintBroadcast = "HintBroadcast";
    public static string HideHint = "HideHint";
    public static string otherHintBroadcast = "otherHintBroadcast";
    public static string otherHideHint = "otherHideHint";
    public static string ShowControlImage = "ShowControlImage";
    public static string ShowBitIt = "ShowBitIt";
    public static string ShowGiftImage = "ShowGiftImage";
    public static string HideGiftImage = "HideGiftImage";


    public static string GetReward = "GetReward";
    public static string CloseRewardAd = "CloseRewardAd";
    public static string DirtClean = "DirtClean";
    public static string ShipArrive = "ShipArrive";

    public static string CoinCountChange = "CoinCountChange";
    //皮肤
    public static string CurMatSkinID = "CurMatSkinID";
    public static string CurModelSkinID = "CurModelSkinID";
    public static string DefaultMatSkinID = "000";
    public static string SkinSlotRefresh = "SkinSlotRefresh";

}
