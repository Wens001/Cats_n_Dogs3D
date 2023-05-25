using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTimeMgr : MonoBehaviour
{
    private static FarmTimeMgr instance;
    public static FarmTimeMgr Instance {
        get {
            if (instance == null) {
                GameObject mgr = new GameObject("FarmTimeMgr");
                instance = mgr.AddComponent<FarmTimeMgr>();
                DontDestroyOnLoad(mgr);
            }
            return instance;
        }
    }
    //成熟时间
    public DateTime[] harvestTimes;
    //是否在成长
    public bool[] isGrows;
    //是否发送过成熟通知
    public bool[] hasCall;
    private static bool hasHarvest;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        harvestTimes = new DateTime[ConfigFarm.MaxFarmNum];
        isGrows = new bool[ConfigFarm.MaxFarmNum];
        hasCall = new bool[ConfigFarm.MaxFarmNum];
        for (int i = 0; i < ConfigFarm.MaxFarmNum; i++) {
            hasCall[i] = false;
            isGrows[i] = GameSetting.GetIsGrow(i);
            harvestTimes[i] = TimeTool.StringToDateTime(GameSetting.GetHarvestTime(i));
        }
        hasHarvest = false;
    }

    private void Start()
    {
        StartCoroutine(CheckMaturity());
    }

    IEnumerator CheckMaturity() {
        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        TimeSpan span;
        bool _hasHarvest = false;
        while (true)
        {
            _hasHarvest = false;
            for (int i = 0; i < ConfigFarm.MaxFarmNum;i++) {
                if (isGrows[i]) {
                    if (!hasCall[i])
                    {
                        span = DateTime.Now - harvestTimes[i];
                        if (span.TotalSeconds > 0)
                        {
                            hasCall[i] = true;
                            _hasHarvest = true;
                            Messenger.Broadcast(ConfigFarm.CanHarvest);
                            _hasHarvest = true;
                        }
                        yield return waitTime;
                    }
                    else {
                        _hasHarvest = true;
                        yield return null;
                    }                   
                }
                else
                    yield return null;
            }
            hasHarvest = _hasHarvest;
        }
    }

    /// <summary>
    /// 是否有植物成熟
    /// </summary>
    /// <returns></returns>
    public static bool HasHarvest() {
        return hasHarvest;
    }
   
    public void Init()
    {
        //纯粹是为了实现预加载
    }

    //播种
    public void UpdateHarvestTimes(int _farmId, DateTime _nextTime,bool _isGrow) {
        harvestTimes[_farmId] = _nextTime;
        isGrows[_farmId] = _isGrow;
        hasCall[_farmId] = false;
    }
    //收获
    public void ToHarvest(int _farmId)
    {
        isGrows[_farmId] = false;
        hasCall[_farmId] = false;
    }
}
