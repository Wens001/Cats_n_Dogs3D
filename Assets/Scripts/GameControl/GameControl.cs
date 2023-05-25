using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
using System;
using LionStudios;
using LionStudios.GDPR;
using Obi;
using DG.Tweening;
using UnityEngine.Networking;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class GameControl : MonoBehaviour
{
    public static GameControl Instance;


    [HideInInspector] public GameProcess GameProcess { get; private set; }

    /// <summary>
    /// 需在猫狗前查询到
    /// </summary>
    [HideInInspector] public LevelSetting levelSetting;
    [HideInInspector] public int GotCoinCount;
    [HideInInspector] public bool FailByDog = false;


    public int CurLevel
    {
        get
        {
            return PlayerPrefs.GetInt(StringMgr.CurLevel, 1);
        }
        set
        {
            var num = Mathf.Clamp(value, 1, value);
            PlayerPrefs.SetInt(StringMgr.CurLevel, num);
        }
    }
    /// <summary>
    /// 关卡下标（由1开始）
    /// </summary>
    public int CurLevelIndex
    {
        get
        {
            var index = CurLevel % GameSetting.levelCount;
            if (index == 0)
            {
                index = GameSetting.levelCount;
            }
            return index;
        }
    }

    /// <summary>
    /// 手机log日志
    /// </summary>
    [HideInInspector] public Transform Log;



    private ObiFixedUpdater catNdog;
    private CinemachineTargetGroup cinemachineTargetGroup;
    private int WinConditionNum = 0;
    /// <summary>
    /// 背景图
    /// </summary>
    private Canvas otherCanvas;

    /// <summary>
    /// lion 分析工具
    /// </summary>
    private PerfToolGenerator Performance;




    //旧关卡下标
    private readonly List<int> OldScenesIndexList = new List<int>() { 3, 4, 8, 22, 33, 36 };
    //奖励关卡进入下标（结束后）
    private readonly List<int> BonusLevelIndexList = new List<int>() { 5, 9, 13, 19, 24, 30, 35, 41, 48, 55, 62 };


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        /*****************  在此切换场景  *****************************/
        otherCanvas = transform.Find("OtherCanvas").GetComponent<Canvas>();
        if (otherCanvas)
        {
            BGImage = otherCanvas.GetComponentInChildren<Image>();
        }


        Messenger.AddListener(StringMgr.GetWinCondition, OnGetWinCondition);
        Messenger.AddListener(StringMgr.LoseWinCondition, OnLoseWinCondition);
        SceneManager.sceneLoaded += OnSceneLoad;

        //锁帧
        Application.targetFrameRate = 60;

        //主画布
        MainCanvas = transform.Find("Canvas").GetComponent<Canvas>();

        SDKManager.Instance.SDKinit();
        GDPRInit();
        DebugModeConfig();

        FarmTimeMgr.Instance.Init();
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnGameStart()
    {
        Debug.Log("游戏加载前 调用");
        SDKManager.Instance.BeforeSceneLoad();
    }



    private void Start()
    {
        //由login界面进入
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LoadLevel(CurLevel);
            Performance = FindObjectOfType<PerfToolGenerator>();
        }
        else
        {
            string[] strs = SceneManager.GetActiveScene().name.Split(' ');
            foreach (var str in strs)
            {
                if (int.TryParse(str, out int levelIndex))
                {
                    CurLevel = levelIndex;
                    break;
                }
            }
        }


        //显示广告元窗口
        if (GameSetting.IsDebug)
        {
            MaxSdk.ShowMediationDebugger();
        }
        //隐藏lion分析窗口
        else
        {
            ActivePerformanceTool(false);
        }


        //隐藏log面板
        Log = transform.Find("IngameDebugConsole");
        if (!GameSetting.IsDebug && Log)
        {
            Log.gameObject.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        Messenger.RemoveListener(StringMgr.GetWinCondition, OnGetWinCondition);
        Messenger.RemoveListener(StringMgr.LoseWinCondition, OnLoseWinCondition);
        SceneManager.sceneLoaded -= OnSceneLoad;

    }


    private void Update()
    {
        /*****Test******/
        if (Input.GetKeyDown(KeyCode.C))
        {
            CleanData();
        }


        /**************/
        if (Input.GetKeyDown(KeyCode.F))
        {
            LionGDPR.Show();
            //FindObjectOfType<ObiRope>().ApplyTearingRightNow();
            //UIPanelManager.Instance.PushPanel(UIPanelType.EnterBonusLvPanel);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            LionGDPR.HideBanner();
            //UIPanelManager.Instance.PopPanel();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (LionStudios.Debugging.LionDebugger.IsShowing())
            {
                LionStudios.Debugging.LionDebugger.Hide();
            }
            else
            {
                LionStudios.Debugging.LionDebugger.Show();
                //LionStudios.Debugging.LionDebugger.AddInfo("Firebase", SDKManager.Instance.FirebaseVersion);
                
            }
        }

    }


    #region 吞食

    private Material RopeMat;
    private float FitSize = .5f;


    private void GetChangeIndex()
    {
        RopeMat = catNdog.GetComponentInChildren<ObiRope>().GetComponent<Renderer>().material;

        //RopeMat.shader.
    }


    public void EatThenOut(CatOrDog catOrDog = CatOrDog.Cat, float size = .5f)
    {
        RopeMat.SetFloat("_Size", size);
        float StartCurve = catOrDog == CatOrDog.Cat ? 0 : 1;
        float EndCurve = catOrDog == CatOrDog.Cat ? 1 : 0;

        DOTween.To(() => StartCurve, x => StartCurve = x, EndCurve, 1f)
            .OnUpdate(() =>
            {
                RopeMat.SetFloat("_Curve", StartCurve);
            });
    }

    float size = 0;
    public void EatAndStore(float AddSize = .1f)
    {
        //RopeMat.SetFloat("_Size", .5f);
        RopeMat.SetFloat("_Range", .4f);
        RopeMat.SetFloat("_Curve", .1f);

        RopeMat.SetFloat("_Size", size = Mathf.Min(size + AddSize, 2));
    }

    #endregion


    #region CallBack

    private void OnSceneLoad(Scene scene, LoadSceneMode loadMode)
    {
        //非login场景加载
        if (scene.buildIndex != 0)
        {
            //调控声音
            VolumeConfig();

            //调整绿幕
            if (GameSetting.IsDebug && GameSetting.GreenGround)
            {
                ChangeGreenGround();
            }

            //查找猫狗，关卡设置
            levelSetting = FindObjectOfType<LevelSetting>();
            FindCatNDog();


            otherCanvas.worldCamera = Camera.main;

            //从幸运皮肤界面回来
            if (!restart)
            {
                restart = true;
                return;
            }

            //正式关卡（ab方式加载场景，不可通过build下标判断）
            if (!scene.name.Equals("ShopScene"))
            {
                LoadLevelSetting();
                Messenger.Broadcast(StringMgr.LevelInit, CurLevelIndex);
                LoadLuckyShopPanel();
            }
        }
    }


    private void OnGetWinCondition()
    {
        WinConditionNum++;
        Debug.Log("当前达成：" + WinConditionNum + "目标需达成：" + levelSetting.WinConditionsNum);

        if (WinConditionNum == levelSetting.WinConditionsNum)
        {
            GameWin();
        }
    }

    private void OnLoseWinCondition()
    {
        WinConditionNum--;
    }

    public void VolumeConfig()
    {
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (var audio in audios)
        {
            audio.volume = GameSetting.AudioSwitch ? 1 : 0;
        }
    }

    #endregion


    #region GDPR

    private Button[] buttons;
    private void GDPRInit()
    {
        LionGDPR.OnOpen += () =>
        {
            //Debug.Log("GDPR 当前状态：" + LionGDPR.Status);
            //Debug.Log("GDPR Completed：" + LionGDPR.Completed);
            //Debug.Log("GDPR AdConsentGranted：" + LionGDPR.AdConsentGranted);
            //Debug.Log("GDPR AnalyticsConsentGranted：" + LionGDPR.AnalyticsConsentGranted);

            //游戏一开始进入 GDPR.OnOpen 函数，并处于完成状态，分析均已同意则不再锁定（因为此情况下，lionGDPR并不会执行 OnClosed 函数）
            if (LionGDPR.Completed && LionGDPR.AdConsentGranted && LionGDPR.AnalyticsConsentGranted && Time.time < 1.5f)
            {
                return;
            }

            PauseGame();
            buttons = transform.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.enabled = false;
            }

        };

        LionGDPR.OnClosed += () =>
        {
            ResumeGame();
            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    button.enabled = true;
                }
            }
        };
    }

    #endregion


    #region LevelControl
    private void GameWin()
    {
        if (GameProcess != GameProcess.OutGame)
        {
            GameProcess = GameProcess.OutGame;

            Messenger.Broadcast(StringMgr.GameWin);
            StartCoroutine(DelayShowWinUI());
        }
    }
    IEnumerator DelayShowWinUI()
    {
        yield return new WaitForSeconds(1.5f);

        if (SDKManager.Instance.ShowInterstitial(GameStatus.GameWin) == false)
        {
            UIPanelManager.Instance.PopPanel();
            UIPanelManager.Instance.PushPanel(UIPanelType.GameWinPanel);
        }
    }

    public void LoadNextLevel()
    {
        CurLevel = BonusLevel ? CurLevel : CurLevel + 1;
        GameSetting.IsReplay = false;
        
        //ab分组判定当前是否为有奖励关组
        if (SDKManager.Instance.HaveBonus && BonusLevelIndexList.Contains(CurLevel -1) && !BonusLevel)
        {
            LoadBonusLevel();
        }
        else
        {
            LoadLevel(CurLevel);
        }

    }

    public void GameFail()
    {
        if (GameProcess != GameProcess.OutGame)
        {
            Debug.LogError("Game Lose !");

            GameProcess = GameProcess.OutGame;
            UIPanelManager.Instance.PopPanel();
            UIPanelManager.Instance.PushPanel(UIPanelType.GameLosePanel);
        }
    }

    public void GameReplay()
    {
        LoadLevel(CurLevel);
    }

    /// <summary>
    /// 加载UI界面，计算关卡下标，加载场景
    /// </summary>
    /// <param name="curLevel"></param>
    public void LoadLevel(int curLevel)
    {
        catNdog = null;
        BonusLevel = false;

        var levelName = "Old Lv " + CurLevelIndex;

        if (SDKManager.Instance.OldLevel && OldScenesIndexList.Contains(CurLevelIndex))
        {
            //SceneManager.LoadScene(levelName); 
            LoadScene(levelName);

        }
        else
        {
            levelName = "Lv " + CurLevelIndex;
            //SceneManager.LoadScene(levelName);
            LoadScene(levelName);
        }

    }

    public void LoadOtherLevel(string levelName)
    {
        LoadScene(levelName);
    }

    private void LoadScene(string sceneName)
    {
#if UNITY_EDITOR

        SceneManager.LoadScene(sceneName);
#else
        _ = BundleManager.LoadScene("scene", sceneName);
#endif
    }



    Transform cat;
    Transform dog;

    private void LoadLevelSetting()
    {
        if (levelSetting == null)
        {
            return;
        }

        GotCoinCount = 0;
        WinConditionNum = 0;
        GameProcess = GameProcess.OutGame;

        //第一关拉近视角
        CameraControl.Instance.ChangeCamera(CurLevelIndex == 1 ? CameraType.CM_LookFocus : levelSetting.cameraType);
        

        //打开gamePlay面板
        UIPanelManager.Instance.PopPanel();
        UIPanelManager.Instance.PushPanel(UIPanelType.GamePlayPanel);

        //第二关打开评分系统
        if (CurLevel == 2 && PlayerPrefs.GetInt(StringMgr.HaveRate, 0) == 0)
        {
            PlayerPrefs.SetInt(StringMgr.HaveRate, 1);
#if UNITY_EDITOR || UNITY_ANDROID
            UIPanelManager.Instance.PushPanel(UIPanelType.RatePanel);
#elif UNITY_IOS
            Device.RequestStoreReview();
#endif
        }
    }

    #endregion


    #region GameProcessControl

    private GameProcess lastProcess;
    public void PauseGame()
    {
        lastProcess = GameProcess;

        GameProcess = GameProcess.PauseGame;
    }

    public void ResumeGame()
    {
        GameProcess = lastProcess;
    }

    public void StartGame()
    {
        GameProcess = GameProcess.InGame;

    }



    #endregion



    #region 特殊场景

    public void LoadModelShowScene()
    {
        SceneManager.LoadScene("ShopScene");
    }

    bool restart = true;
    /// <summary>
    /// 退出特殊场景
    /// </summary>
    /// <param name="restart">是否重新开始此关卡</param>
    public void UnloadSpecialScene(bool restart = true)
    {
        this.restart = restart;

        LoadLevel(CurLevel);
    }


    #endregion


    #region 奖励关相关

    /// <summary>
    /// 当前是否为奖励关
    /// </summary>
    [HideInInspector] public bool BonusLevel = false;
    [NonSerialized] [HideInInspector] public int BonusIndex = -1;
    [HideInInspector] public SkinInfo BonusSkin;
    [HideInInspector] public bool GotAllBonus;

    public List<BonusType> GotBonusList = new List<BonusType>();

    //在此处实现“加载奖励关”的逻辑
    private void LoadBonusLevel()
    {
        BonusIndex = BonusLevelIndexList.IndexOf(CurLevel - 1);
        BonusLevel = true;
        BonusSkin = null;
        GotBonusList.Clear();

        LoadOtherLevel("BonusLevel");
    }

    /// <summary>
    /// 金币随机
    /// </summary>
    public BonusType RandomBonusCoin()
    {
        var bonusType = (BonusType)UnityEngine.Random.Range(0, 3);
        
        GotBonusList.Add(bonusType);

        return bonusType;
    }

    /// <summary>
    /// 带皮肤随机
    /// </summary>
    public BonusType RandomAllBonus()
    {
        var index = UnityEngine.Random.Range(0, 2);

        if (index == 0)
        {
            return RandomBonusCoin();
        }
        else
        {
            BonusSkin = SkinManager.Instance.RandomLuckySkin();
            if (BonusSkin)
            {
                GotBonusList.Add(BonusType.LuckySkin);
                return BonusType.LuckySkin;
            }
            else
            {
                return RandomBonusCoin();
            }

        }

    }



    /*************  礼物  ****************/
    [HideInInspector] public bool FirstGotGift = false;

    /// <summary>
    /// 获取礼物次数（金鼠和礼盒，首次给金币）
    /// </summary>
    private int GotGiftTimes
    {
        get => PlayerPrefs.GetInt("GotGiftTimes", 1);
        set => PlayerPrefs.SetInt("GotGiftTimes", value);
    }
    public bool IsCoinGift;

    public void JudgeGiftType(bool HaveGotGift)
    {
        IsCoinGift = !HaveGotGift || GotGiftTimes % 2 == 1;
        if (!IsCoinGift)
        {
            BonusSkin = SkinManager.Instance.RandomLuckySkin();
            if (BonusSkin == null)
            {
                IsCoinGift = true;
            }
        }

        if (HaveGotGift)
        {
            GotGiftTimes += 1;
        }
    }


    #endregion

    #region 农场相关

    public void LoadFarmScene()
    {
        LoadOtherLevel("FarmScene");
    }



    #endregion



    #region 猫狗查询、隐藏


    private void FindCatNDog()
    {
        //渲染全层级，回避幸运皮肤界面的更改
        Camera.main.cullingMask = -1;

        catNdog = FindObjectOfType<ObiFixedUpdater>();
        if (catNdog)
        {
            cat = catNdog.transform.Find("Cat");
            dog = catNdog.transform.Find("Dog");
            catNdog.gameObject.SetActive(true);

            Messenger.Broadcast(StringMgr.CatNDogInit, cat.GetComponent<TouchMove>(), dog.GetComponent<TouchMove>());

            //获取猫狗身体
            GetChangeIndex();
        }

        //锁定视角（在猫狗身上）
        cinemachineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
        cinemachineTargetGroup.m_Targets[0].target = cat;
        cinemachineTargetGroup.m_Targets[1].target = dog;
    }


    public void HideCatNDog()
    {
        Messenger.Broadcast(StringMgr.BouthDeathLock);
        catNdog.gameObject.SetActive(false);
    }


    #endregion


    public void CleanData()
    {
        Debug.Log("清空数据。。。。。");
        //PlayerPrefs.DeleteAll();
        foreach (var skin in SkinManager.Instance.skinsInfo)
        {
            PlayerPrefs.DeleteKey(skin.ID);
        }
        GameSetting.CoinCount = 0;
        PlayerPrefs.DeleteKey(StringMgr.CurLevel);
        PlayerPrefs.DeleteKey(StringMgr.IsDebug);
        PlayerPrefs.DeleteKey("_Skin_Init_");
        PlayerPrefs.DeleteKey("lastLuckySkinIndex");
        PlayerPrefs.DeleteKey("FirstCoinReward");
        PlayerPrefs.DeleteKey("FootBallGuidLevel");
        PlayerPrefs.DeleteKey("IsReplay");
        PlayerPrefs.DeleteKey("HaveGotGift"); 
        PlayerPrefs.DeleteKey("HaveGotMouse");
        PlayerPrefs.DeleteKey("GotGiftTimes");
        PlayerPrefs.DeleteKey(StringMgr.HaveRate);

        CurLevel = 1;
    }

    #region DebugMode 判断

    private void DebugModeConfig()
    {
        var debugMode = PlayerPrefs.GetInt(StringMgr.IsDebug, -1);

        if (debugMode == -1)
        {
            var path = "";
#if UNITY_EDITOR || UNITY_IPHONE
            path = "file://" + Application.streamingAssetsPath + "/DebugMode.txt";
#else
        path =  Application.streamingAssetsPath + "/DebugMode.txt";
#endif
            StartCoroutine(DebugFileRead(path));
        }
        else
        {
            GameSetting.IsDebug = debugMode == 1;
        }
    }

    IEnumerator DebugFileRead(string path)
    {
        var request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        var debugMode = int.Parse(request.downloadHandler.text.Trim());
        GameSetting.IsDebug = debugMode == 1;

#if !UNITY_EDITOR
        PlayerPrefs.SetInt(StringMgr.IsDebug, debugMode);
#endif
    }


    #endregion


    #region GreenScene

    private Dictionary<Renderer, Texture> TempTextureDic = new Dictionary<Renderer, Texture>();
    Image BGImage;
    //绿幕
    public void ChangeGreenGround()
    {
        var obj = GameObject.Find("Ground");

        if (GameSetting.GreenGround)
        {
            TempTextureDic.Clear();
        }

        var Renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var Renderer in Renderers)
        {
            Renderer.material.color = GameSetting.GreenGround ? Color.green : Color.white;

            if (GameSetting.GreenGround)
            {
                TempTextureDic.Add(Renderer, Renderer.material.mainTexture);
                Renderer.material.mainTexture = null;
            }
            else
            {
                if (TempTextureDic.ContainsKey(Renderer))
                {
                    Renderer.material.mainTexture = TempTextureDic[Renderer];
                    TempTextureDic.Remove(Renderer);
                }
            }
        }
    }

    public void ChangeGreenBG()
    {
        BGImage.color = GameSetting.GreenBG ? Color.green : Color.white;
    }


    #endregion


    #region LoadABundle

    private async void LoadABundle()
    {
        await BundleManager.Init();

    }


    #endregion


    #region Other Part Init

    //显示lion分析工具
    public void ActivePerformanceTool(bool activeSelf)
    {
        if (Performance)
        {
            Performance.gameObject.SetActive(activeSelf);
        }

    }


    #endregion


    #region 主画布设定
    private Canvas MainCanvas;


    public void ChangeCullingToCatNDog()
    {
        Debug.Log("切换相机渲染模式");
        //只渲染目标层级
        Camera.main.cullingMask = LayerMask.GetMask("Default") | LayerMask.GetMask("UI") | LayerMask.GetMask("CatOrDog");

        //设为相机前方模式
        MainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        MainCanvas.worldCamera = Camera.main;
        MainCanvas.planeDistance = 300;

    }


    public void BackMainCanvasSetting()
    {
        MainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }



    #endregion


    /// <summary>
    /// 打折商店弹窗
    /// </summary>
    private void LoadLuckyShopPanel()
    {
        //在3、10、20后，弹出打折窗口
        var luckyShopLevel = PlayerPrefs.GetInt("luckyShopLevel", 1);
        int CurLevel = GameControl.Instance.CurLevel;
        if (!GameSetting.GotTheBundle && (CurLevel == 4 || CurLevel == 11 || CurLevel == 21))
        {
            if (luckyShopLevel < CurLevel)
            {
                PlayerPrefs.SetInt("luckyShopLevel", CurLevel);
                UIPanelManager.Instance.PushPanel(UIPanelType.LuckyShopPanel);
            }
        }
    }

}


public enum BonusType
{
    Coin_50,
    Coin_100,
    Coin_150,
    LuckySkin,
}
