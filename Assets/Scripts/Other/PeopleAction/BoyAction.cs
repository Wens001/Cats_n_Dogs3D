using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 男孩相关控制脚本
/// </summary>
public class BoyAction : MonoBehaviour
{
    public BoyState CurState;

    public List<GameObject> OtherThings = new List<GameObject>();

    //起床
    private TouchMove DogHead;
    
    [Header("提示")]
    public Sprite HintSprite;
    public Sprite WantSprite;
    public Transform HintPoint;
    public Sprite TempHintSprite;

    [Header("泳池")]
    public GameObject SwimRing;
    private float InitPositionY;

    [Header("滑梯")]
    public GameObject Upper;
    public GameObject Down;
    public Transform tempTarget;
    public Transform theTarget;
    [Header("移动")]
    public float speed = 8;


    [Header("Ice Cream")]
    public List<GameObject> IceCreamBalls;

    [Header("高尔夫")]
    public GameObject BallPrefab;
    public float ForceRate = 1;

    [Header("受伤")]
    public GameObject Gauze;


    int CoinCount;
    Animator anim;
    Rigidbody rigi;
    bool startWalk = false;
    MyTimer WakeUpTimer = new MyTimer(3f);
    MyTimer shoutTimer = new MyTimer(.5f);
    private MyTimer DrownTimer = new MyTimer(10f);

    private void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();
        rigi = GetComponentInChildren<Rigidbody>();
    }

    private void OnEnable()
    {
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
        Messenger.AddListener(StringMgr.BouthDeathLock, OnBouthLocked);
        Messenger.AddListener(StringMgr.ShowTheResult, ShowTheResult);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);
        Messenger.RemoveListener(StringMgr.BouthDeathLock, OnBouthLocked);
        Messenger.RemoveListener(StringMgr.ShowTheResult, ShowTheResult);

    }


    private void Start()
    {
        var heads = FindObjectsOfType<TouchMove>();
        foreach (var head in heads)
        {
            if (head.selfType == CatOrDog.Dog)
            {
                DogHead = head;
                break;
            }
        }

        switch (CurState)
        {
            //case BoyState.Sleeping:
            //    return;
            //    break;
            case BoyState.BeHurt:
                anim.SetTrigger("BeHurt");
                Gauze.SetActive(false);
                break;


            case BoyState.WaitForSock:
                anim?.SetTrigger("Sit");
                foreach (var sock in OtherThings)
                {
                    sock.SetActive(false);
                }
                break;

            case BoyState.WaitForFood:
                foreach (var sock in OtherThings)
                {
                    sock.SetActive(false);
                }
                break;
            //泳池浮沉
            case BoyState.WaitForHelp:
                SwimRing.SetActive(false);
                InitPositionY = transform.position.y;
                transform.DOMoveY(transform.position.y - 3f, 1f).SetLoops(-1, LoopType.Yoyo);

                break;

            case BoyState.EatCoin:
                anim.SetTrigger("GetAlive");
                SimapleMove move = gameObject.AddComponent<SimapleMove>();
                move.FollowRotation = true;
                move.ForceRate = 1.5f;
                Messenger.Broadcast(StringMgr.BouthDeathLock);
                CoinCount = FindObjectsOfType<CoinAction>().Length;
                //this.enabled = false;

                break;

            case BoyState.WaitForIceCream:
                anim.SetTrigger("WaitForIce");
                foreach (var ball in IceCreamBalls)
                {
                    ball.SetActive(false);
                }
                break;

            case BoyState.PlayWhitShot:
                StartCoroutine(HitBall());
                break;

            case BoyState.BullyState:
                anim.SetTrigger("Scared");
                break;

            default:
                break;
        }

    }


    private void Update()
    {
        if (GameControl.Instance.GameProcess != GameProcess.InGame)
        {
            return;
        }

        //叫醒  赶跑
        if ((CurState == BoyState.Sleeping && !weakUp) || CurState == BoyState.BullyState && !BadGuyAway)
        {
            shoutTimer.OnUpdate(Time.deltaTime);
            if (shoutTimer.IsFinish)
            {
                shoutTimer.ReStart();
                DogHead.AnimalShout();
            }

            if (ShoutUp)
            {
                WakeUpTimer.OnUpdate(Time.deltaTime);
                if (WakeUpTimer.IsFinish)
                {
                    if (CurState == BoyState.Sleeping)
                    {
                        OnGetUp();
                    }
                    if (CurState == BoyState.BullyState)
                    {
                        ShoutBadGuyAway();
                    }
                }
            }
        }

        //沉没计时
        if (CurState == BoyState.WaitForHelp)
        {
            DrownTimer.OnUpdate(Time.deltaTime);
            if (DrownTimer.IsFinish)
            {
                DrownTimer.ReStart();
                DrownInPool();

                Messenger.Broadcast(StringMgr.otherHideHint, HintPoint == null ? gameObject : HintPoint.gameObject);
            }
            return;
        }

        //泳池吃金币
        if (CurState == BoyState.EatCoin && GameControl.Instance.GotCoinCount == CoinCount)
        {
            GetComponent<SimapleMove>().enabled = false;
            Messenger.Broadcast(StringMgr.GetWinCondition);
        }

        //推着走
        if (CurState == BoyState.GotoSchool || CurState == BoyState.FindSomeThing)
        {
            MoveByCatNDog();
        }

        //滑梯
        if (CurState == BoyState.PlayOnSlide)
        {
            if (startWalk)
            {
                transform.position = Vector3.MoveTowards(transform.position, theTarget.position, speed * .8f * Time.deltaTime);

                if (Vector3.Distance(transform.position, theTarget.position) < 1)
                {
                    startWalk = false;
                    transform.rotation = theTarget.rotation;
                    anim.SetBool("Walk", false);

                    //滑下去
                    transform.position = Upper.transform.position;
                    transform.rotation = Quaternion.LookRotation(Down.transform.position - Upper.transform.position);
                    StartSlide = true;
                    anim.SetTrigger("Slide");

                    return;
                }
            }

            if (StartSlide)
            {
                transform.position = Vector3.MoveTowards(transform.position, Down.transform.position, speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, Down.transform.position) < 2)
                {
                    StartSlide = false;

                    anim.SetTrigger("Happy");
                    transform.rotation = Quaternion.LookRotation(Vector3.back);
                    transform.position = transform.position - Vector3.up * transform.position.y + transform.forward * 5;

                    StartCoroutine(SetResultAfterNextAnimation());

                }
            }
        }

        //前行
        if (!stopMove && GameControl.Instance.GameProcess == GameProcess.InGame && CurState == BoyState.moveFoward)
        {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }

    }



    bool ShoutUp = false;
    List<TouchMove> heads = new List<TouchMove>();
    private void OnTriggerEnter(Collider other)
    {
        TouchMove head = other.GetComponentInParent<TouchMove>();
        if (head && !heads.Contains(head))
        {
            heads.Add(head);
            //狗头进入计时范围
            if (head.selfType == CatOrDog.Dog)
            {
                ShoutUp = true;
            }

            //泳池获救
            if (CurState == BoyState.WaitForHelp && heads.Count == 2)
            {
                GetHelpInPool();
                return;
            }
        }

        //穿衣，拿筷
        SimapleLock simapleLock = other.GetComponentInParent<SimapleLock>();
        if (simapleLock)
        {
            switch (CurState)
            {
                case BoyState.WaitForSock:
                    PutOnLittleThing(simapleLock);
                    break;
                case BoyState.WaitForFood:
                    PutOnThingInOrder(simapleLock);
                    break;
                default:
                    break;
            }
            return;
        }


        //拿足球
        if (CurState == BoyState.FindSomeThing && other.GetComponentInParent<Ball>())
        {
            Messenger.Broadcast(StringMgr.GetWinCondition);
            anim.SetBool("Walk", false);
            rigi.isKinematic = true;
        }

        //冰淇淋
        if (CurState == BoyState.WaitForIceCream && other.gameObject.name.Contains("Ice_CreamBall"))
        {
            if (IceCreamBalls.Count > 0)
            {
                other.gameObject.SetActive(false);
                IceCreamBalls[0].SetActive(true);
                IceCreamBalls.RemoveAt(0);
                if (IceCreamBalls.Count == 0)
                {
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                }
            }
        }

        //远足 （触碰障碍）
        if (CurState == BoyState.moveFoward)
        {
            if (other.CompareTag("Thorn"))
            {
                OnTriggerObstacle();
            }

            if (other.GetComponentInParent<TriggerResoult>())
            {
                GetWinHappy();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TouchMove head = other.GetComponentInParent<TouchMove>();
        if (head && heads.Contains(head))
        {
            heads.Remove(head);
            if (head.selfType == CatOrDog.Dog)
            {
                ShoutUp = false;
            }
        }
    }



    #region 泳池

    private void GetHelpInPool()
    {
        transform.DOKill();
        transform.DOMoveY(InitPositionY, 1f);

        SwimRing.SetActive(true);
        CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);

        GameControl.Instance.HideCatNDog();
        GetAlive();
    }

    public void GetAlive()
    {
        Messenger.Broadcast(StringMgr.otherHideHint, HintPoint == null ? gameObject : HintPoint.gameObject);

        Debug.Log("获救");
        anim.SetTrigger("GetAlive");
        StartCoroutine(SetResultAfterNextAnimation());
    }


    private void DrownInPool()
    {
        transform.DOKill();
        anim.SetTrigger("Drown");
        Messenger.Broadcast(StringMgr.BouthDeathLock);

        CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);

        StartCoroutine(SetResultAfterNextAnimation(false));
    }

    #endregion


    #region 起床

    bool weakUp = false;
    private void OnGetUp()
    {
        weakUp = true;
        anim.SetTrigger("GetUp");
        StartCoroutine(SetResultAfterNextAnimation());

        Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Dog);
    }

    #endregion

    #region 赶走坏人

    bool BadGuyAway = false;
    private void ShoutBadGuyAway()
    {
        BadGuyAway = true;
        foreach (var obj in FindObjectsOfType<BadGuyAction>())
        {
            //obj.gameObject.SetActive(false);
            obj.GoAway();
        }

        Messenger.Broadcast(StringMgr.GetWinCondition);
    }



    #endregion


    #region 穿上小物件，不分顺序

    private void PutOnLittleThing(SimapleLock sock)
    {
        if (OtherThings.Count == 0)
        {
            return;
        }

        OtherThings[0].SetActive(true);
        OtherThings.RemoveAt(0);
        sock.curHead.UnlockHead();
        sock.gameObject.SetActive(false);

        if (OtherThings.Count == 0)
        {
            if (CurState == BoyState.WaitForSock)
            {
                CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);

                anim.SetTrigger("Jump");
                StartCoroutine(SetResultAfterNextAnimation());
            }
        }
    }

    #endregion

    #region 分顺序穿上小物件
    private void PutOnThingInOrder(SimapleLock LittleThing)
    {
        if (OtherThings.Count == 0)
        {
            return;
        }

        for (int i = 0; i < OtherThings.Count; i++)
        {
            if (OtherThings[i].gameObject.name.Equals(LittleThing.gameObject.name))
            {
                OtherThings[i].SetActive(true);
                OtherThings.RemoveAt(i);
                i--;
                LittleThing.curHead.UnlockHead();
                LittleThing.gameObject.SetActive(false);


                if (OtherThings.Count == 0)
                {
                    if (CurState == BoyState.WaitForFood)
                    {
                        Messenger.Broadcast(StringMgr.GetWinCondition);
                    }
                }

                break;
            }
        }
    }
    #endregion

    #region 推着走

    private void MoveByCatNDog()
    {
        if (rigi)
        {
            if (rigi.velocity.magnitude > .5f)
            {
                anim.SetBool("Walk", true);

            }
            else
            {
                rigi.velocity = Vector3.zero;
                anim.SetBool("Walk", false);
            }
        }


        //rigi.velocity = Vector3.ClampMagnitude(rigi.velocity, GameSetting.MaxSpd);

        Vector3 dir = Vector3.ProjectOnPlane(rigi.velocity, Vector3.up);
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 30);
        }
    }



    #endregion


    #region 被打
    public void BeHit()
    {
        anim.SetTrigger("BeHit");

        StartCoroutine(SetResultAfterNextAnimation(false));
    }

    #endregion


    #region 失败哭泣和胜利欢呼

    public void GetWinHappy()
    {
        anim.SetTrigger("Happy");

        StartCoroutine(SetResultAfterNextAnimation());
    }

    public void SadAndGameFail()
    {
        anim.SetBool("Cry", true);
        StartCoroutine(SetResultAfterNextAnimation(false));
    }

    #endregion

    private IEnumerator HitBall()
    {
        anim.SetTrigger("HitBall");
        BallPrefab.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();
        float length = anim.GetNextAnimatorClipInfo(0).Length;
        yield return new WaitForSeconds(length);

        GameObject ball = Instantiate(BallPrefab, BallPrefab.transform.position, BallPrefab.transform.rotation);
        Rigidbody ballRigi = ball.GetComponent<Rigidbody>();
        ballRigi.isKinematic = false;
        ballRigi.AddForce(transform.forward * GameSetting._force * ForceRate, ForceMode.Impulse);

        BallPrefab.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);
        StartCoroutine(HitBall());
    }



    /// <summary>
    /// 等下个动作播完后
    /// </summary>
    /// <param name="GetWin"></param>
    /// <returns></returns>
    private IEnumerator SetResultAfterNextAnimation(bool GetWin = true)
    {
        GameControl.Instance.PauseGame();
        Messenger.Broadcast(StringMgr.otherHideHint, HintPoint == null ? gameObject : HintPoint.gameObject);
        yield return new WaitForEndOfFrame();

        float length = Mathf.Min(anim.GetNextAnimatorStateInfo(0).length, 3);
        yield return new WaitForSeconds(length);
        if (GetWin)
        {
            Messenger.Broadcast(StringMgr.GetWinCondition);
        }
        else
        {
            GameControl.Instance.GameFail();
        }
    }

    #region CallBack

    private void OnGameStart()
    {
        int num = 0;

        switch (CurState)
        {
            case BoyState.Sleeping:
                Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Dog, HintSprite);
                break;

            case BoyState.WaitForHelp:
                DrownTimer.ReStart();
                //提示，救生圈
                Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Cat, HintSprite);
                Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Dog, TempHintSprite);
               
                break;
            case BoyState.playInBallsPool:
                //提示，吃球；
                Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Default, HintSprite);
                break;
            case BoyState.moveFoward:
                anim.SetBool("Walk", true);
                break;

            default:
                break;
        }


        Messenger.Broadcast(StringMgr.otherHintBroadcast, HintPoint == null ? gameObject : HintPoint.gameObject, WantSprite); 

        DOTween.To(() => num, x => num = x, 95, 5)
                    .OnComplete(() => {
                        Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Default);
                        Messenger.Broadcast(StringMgr.otherHideHint, HintPoint == null ? gameObject : HintPoint.gameObject);
                    });

    }


    bool StartSlide = false;
    private void OnBouthLocked()
    {
        //滑梯
        if (CurState == BoyState.PlayOnSlide)
        {
            //transform.position = Upper.transform.position;
            //transform.rotation = Quaternion.LookRotation(Down.transform.position - Upper.transform.position);
            //StartSlide = true;
            //anim.SetTrigger("Slide");

            transform.position = tempTarget.position;
            transform.rotation = tempTarget.rotation;
            anim.SetBool("Walk", true);
            startWalk = true;

            CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);
        }

        //秋千
        if (CurState == BoyState.PlayOnSwing)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.right);
            CameraControl.Instance.ChangeCamera(CameraType.CM_StropView);

            StartCoroutine(DelayPlayWithSwing());

        }

        //无事，达成胜利条件
        if (CurState == BoyState.Nothing)
        {
            Messenger.Broadcast(StringMgr.GetWinCondition);
        }
    }


    private void ShowTheResult()
    {
        switch (CurState)
        {
            case BoyState.BeHurt:
                Gauze.SetActive(true);
                Messenger.Broadcast(StringMgr.GetWinCondition);

                break;
            
            default:
                break;
        }


    }


    #endregion


    #region 荡秋千

    IEnumerator DelayPlayWithSwing()
    {
        Obi.ObiRope rope = FindObjectOfType<Obi.ObiRope>();
        rope.stretchingScale = 1.5f;

        yield return new WaitForSeconds(3f);
        var catNdog = FindObjectOfType<Obi.ObiSolver>();
        transform.position = catNdog.transform.Find("MiddlePoint").position;


        anim.SetTrigger("Swing");
        StartCoroutine(SetResultAfterNextAnimation());

    }





    #endregion

    #region 前行（跑酷）

    bool stopMove = false;
    private void OnTriggerObstacle()
    {
        anim.SetTrigger("Fall");
        stopMove = true;

        StartCoroutine(SetResultAfterNextAnimation(false));
    }

    #endregion

    
}


public enum BoyState
{
    Sleeping = 0,
    WaitForSock,
    WaitForFood,
    WaitForHelp,
    EatCoin,
    WaitForIceCream,
    GotoSchool,
    FindSomeThing,
    PlayOnSlide,
    PlayOnSwing,
    PlayWhitShot,
    playInBallsPool,
    PlayWithSnow,
    /// <summary>
    /// 欺凌
    /// </summary>
    BullyState,
    moveFoward,
    BeHurt,


    Nothing = -1,
}