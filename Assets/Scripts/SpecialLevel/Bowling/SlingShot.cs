using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 当特定的两个木桩被咬住时，进入弹弓模式
/// </summary>
public class SlingShot : MonoBehaviour
{
    public GameObject ballPrefab;
    public Sprite hintSprite;
    public List<LockObjectBase> unlocks = new List<LockObjectBase>();
    public List<GameObject> ballsList = new List<GameObject>();
    [Header("瞄准线")]
    public GameObject dirGuid;
    //碰碰车
    public bool NeverFail = false;

    public bool FollowBall = false;


    SlingShotBall curBall;



    private void Awake()
    {
        if (dirGuid)
        {
            dirGuid.SetActive(false);
        }
        if (ballPrefab && ballPrefab.activeSelf)
        {
            ballPrefab.SetActive(false);
        }


        Messenger.AddListener<int>(StringMgr.LevelInit, SlingShotInit);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<int>(StringMgr.LevelInit, SlingShotInit);

    }


    private void OnEnable()
    {
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnPlayerBeLocked);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnUnlock);
        Messenger.AddListener(StringMgr.ShootAgain, SpawnBall);
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);

    }

    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnPlayerBeLocked);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnUnlock);
        Messenger.RemoveListener(StringMgr.ShootAgain, SpawnBall);
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }


    private void Update()
    {
        //检测保龄球移动，静止时重新生成球
        if (GameControl.Instance.GameProcess == GameProcess.InGame && curBall)
        {
            curBall.CheckSelf();
        }

    }



    private void SlingShotInit(int levelIndex)
    {
        if (curBall)
        {
            Destroy(curBall.gameObject);
        }

        gameObject.SetActive(true);
        locks.Clear(); 
        curBall = null;
        foreach (var ball in usedBallsList)
        {
            if (ball)
            {
                ball.gameObject.SetActive(false);
            }
        }
        usedBallsList.Clear();
        readyPerfect = false;
    }


    bool readyPerfect = false;
    private List<LockObjectBase> locks = new List<LockObjectBase>();
    /// <summary>
    /// 监听木桩，特定木桩均被锁住时，生成球
    /// </summary>
    /// <param name="lockObject"></param>
    /// <param name="head"></param>
    private void OnPlayerBeLocked(LockObjectBase lockObject, TouchMove head)
    {
        if (unlocks.Contains(lockObject) && !locks.Contains(lockObject))
        {
            locks.Add(lockObject);

            if (locks.Count == 2)
            {
                //生成弹珠
                SpawnBall();
                //Messenger.Broadcast(StringMgr.DeathLock);
                //Messenger.Broadcast(StringMgr.BallShotGuide, curBall);
                //StartCoroutine(BroadcastHint());
                readyPerfect = true;
            }
        }
    }

    private IEnumerator BroadcastHint()
    {
        yield return new WaitForEndOfFrame();
        Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Default, hintSprite);
    }

    private void OnUnlock(LockObjectBase lockObject)
    {
        if (locks.Contains(lockObject))
        {
            locks.Remove(lockObject);
            if (locks.Count < 2 && curBall)
            {
                curBall.gameObject.SetActive(false);
                curBall = null;
            }
        }
    }


    private List<GameObject> usedBallsList = new List<GameObject>();
    private void SpawnBall()
    {
        if (ballsList.Count == 0 && GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            if (NeverFail)
            {
                Messenger.Broadcast(StringMgr.GetWinCondition);
            }
            else
            {
                GameControl.Instance.GameFail();
            }
            return;
        }


        GameObject obj = Instantiate(ballPrefab);
        obj.transform.SetParent(transform.parent);
        obj.transform.localPosition = transform.localPosition;
        obj.transform.rotation.Normalize();
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);

        curBall = obj.AddComponent<SlingShotBall>();
        curBall.dirGuid = dirGuid;
        curBall.ViewFollow = FollowBall;
        usedBallsList.Add(obj);


        if (ballsList[0] != null)
        {
            ballsList[0].SetActive(false);
        }
        ballsList.RemoveAt(0);
    }


    int SlingShotGuidLevel
    {
        get => PlayerPrefs.GetInt("SlingShotGuidLevel", -1);
        set { PlayerPrefs.SetInt("SlingShotGuidLevel", value); }
    }
    private void OnGameStart()
    {
        if (readyPerfect)
        {
            Messenger.Broadcast(StringMgr.BouthDeathLock);
            StartCoroutine(BroadcastHint());


            //第一次或者重复玩显示引导
            if (GameControl.Instance.levelSetting.guidType == GuidType.SlingShotGuid)
            {
                Messenger.Broadcast(StringMgr.BallShotGuide, curBall);
            }

            //if (SlingShotGuidLevel == -1 || GameControl.Instance.CurLevel == SlingShotGuidLevel)
            //{
            //    SlingShotGuidLevel = GameControl.Instance.CurLevel;
            //    Messenger.Broadcast(StringMgr.BallShotGuide, curBall);
            //}

        }
    }

}
