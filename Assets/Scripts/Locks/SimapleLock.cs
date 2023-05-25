using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimapleLock : LockObjectBase
{
    public LockedType lockedType;

    [Header("提示")]
    public ParticleSystem HintVfx;
    public Sprite HintSprite;
    public GameObject HintTrans;

    private HingeJoint _hingeJoint;
    private Rigidbody rigi;

    private void Awake()
    {
        _hingeJoint = transform.GetComponentInChildren<HingeJoint>();
        rigi = transform.GetComponentInChildren<Rigidbody>();

    }

    private void OnEnable()
    {
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }


    #region 咬住
    private void OnHeadLocked(LockObjectBase lockObj, TouchMove head)
    {
        if (lockObj != this)
        {
            return;
        }

        if (HintVfx)
        {
            HintVfx.gameObject.SetActive(false);
        }

        switch (lockedType)
        {
            case LockedType.TableLamp:
                OnTableLampLock(head);
                break;
            case LockedType.SimapleFollow:
                if (curHead == null)
                {
                    SimapleFollow(head);
                }
                else return;
                break;

            case LockedType.DogHouse:
                if (head.selfType == CatOrDog.Dog)
                {
                    foreach (var collider in transform.GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }

                    StartCoroutine(DogBackKennel(head));
                }
                break;

            case LockedType.PaperClass:
                if (curHead == null)
                {
                    curHead = head;
                    if (head.selfType == CatOrDog.Cat)
                        SimapleFollow(head);
                    else
                        //StartCoroutine(BreakPaperClass(head));
                        BreakPaperClass(head, gameObject);
                }

                break;

            case LockedType.MailBox:
                if (curHead == null)
                {
                    curHead = head;
                    StartCoroutine(GetMail(head));
                }
                break;

            case LockedType.IceCreamTruck:

                LockIceCreamCar(head);
                break;

            case LockedType.RenderFollow:
                GetRenderTool(head);
                break;

            case LockedType.DoubleLock:
                OnChildSlideLocked(head);
                break;

            default:
                break;
        }


    }

    private void HingeJointLock(TouchMove head)
    {
        if (_hingeJoint)
        {
            head.rigi.constraints = RigidbodyConstraints.None;
            head.rigi.isKinematic = false;

            _hingeJoint.connectedBody = head.rigi;
        }
    }

    private void FollowHead(TouchMove head)
    {
        transform.SetParent(head.transform);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;

        if (rigi)
        {
            var obiRigi = transform.GetComponent<Obi.ObiRigidbody>();
            if (obiRigi)
            {
                Destroy(obiRigi);
            }
            Destroy(rigi);
        }
    }



    private void OnTableLampLock(TouchMove head)
    {
        head.LockHead();

        HingeJointLock(head);
        Messenger.Broadcast(StringMgr.BouthDeathLock);

        var light = FindObjectOfType<Light>();
        float num = light.intensity;
        DOTween.To(() => num, x => num = x, 1.1f, 1f)
            .OnUpdate(() => {
                light.intensity = num;
            })
            .OnComplete(() => {
                //Messenger.Broadcast(StringMgr.GetUp);
                Messenger.Broadcast(StringMgr.GetWinCondition);
            });

    }

    private void SimapleFollow(TouchMove head)
    {
        curHead = head;
        FollowHead(head);

        head.LockHead(false);
        head.isOtherMoving = true;
    }
    //狗回窝
    private IEnumerator DogBackKennel(TouchMove head)
    {
        yield return new WaitForEndOfFrame();

        head.deathLock = true;
        head.LockHead();
        HingeJointLock(head);

        Messenger.Broadcast(StringMgr.BouthDeathLock);
    }

    //拿邮件
    private IEnumerator GetMail(TouchMove head)
    {
        yield return new WaitForEndOfFrame();

        head.deathLock = true;
        head.LockHead();
        HingeJointLock(head);

        yield return new WaitForSeconds(.2f);
        head.deathLock = false;
        head.TryUnlockHead();

        var letter = transform.Find("Letter");
        letter.SetParent(head.transform);
        letter.localPosition = Vector3.zero;
        letter.localRotation = Quaternion.identity;

        if (head.selfType == CatOrDog.Cat)
        {
            Messenger.Broadcast(StringMgr.GetWinCondition);
        }
        else
        {
            //StartCoroutine(BreakPaperClass(head));
            curHead = head;
            BreakPaperClass(head, letter.gameObject);
        }
    }
    //冰淇淋车
    private void LockIceCreamCar(TouchMove head)
    {
        if (curHead == null)
        {
            curHead = head;
            head.LockHead();
            head.deathLock = true;
            HingeJointLock(head);
        }
    }

    //画笔
    private void GetRenderTool(TouchMove head)
    {
        if (curHead == null)
        {
            SimapleFollow(head);
            head.transform.GetComponent<PaintIn3D.P3dHitCollisions>().Layers = -1;
        }
    }


    //滑梯(双嘴咬住
    private void OnChildSlideLocked(TouchMove head)
    {
        if (curHead == null)
        {
            curHead = head;
            head.LockHead();
            HingeJointLock(head);

            if (head.otherHead.IsLocked)
            {
                Messenger.Broadcast(StringMgr.BouthDeathLock);
            }
        }
    }


    #endregion


    #region 松嘴

    private void OnHeadUnlock(LockObjectBase lockObj)
    {
        if (lockObj != this)
        {
            return;
        }

        switch (lockedType)
        {
            case LockedType.MailBox:
                OnMailBoxUnlock();
                break;

            case LockedType.DoubleLock:
                OnChildSlideUnlock();
                break;
            default:
                break;
        }

    }

    private void HingeJointUnlock()
    {
        if (_hingeJoint)
        {
            _hingeJoint.connectedBody = null;
            //Destroy(_hingeJoint);
            rigi.isKinematic = true;
        }
    }

    private void OnMailBoxUnlock()
    {
        HingeJointUnlock();

        Destroy(this);
    }

    //滑梯
    private void OnChildSlideUnlock()
    {
        HingeJointUnlock();
        curHead = null;

    }

    #endregion


    private void OnGameStart()
    {
        if (HintTrans)
        {
            Messenger.Broadcast(StringMgr.otherHintBroadcast, HintTrans, HintSprite);
        }
    }


    //纸张类破坏
    private void BreakPaperClass(TouchMove head, GameObject paper)
    {
        GameControl.Instance.FailByDog = true;
        head.AnimalShout();
        enabled = false;
        foreach (var render in paper.transform.GetComponentsInChildren<Renderer>())
        {
            render.enabled = false;
        }

        Transform breaks = paper.transform.Find("Breaks");
        if (breaks)
        {
            breaks.position = curHead.transform.position + Vector3.up;
            breaks.gameObject.SetActive(true);
            //Debug.Log(breaks.name);
            for (int i = 0; i < breaks.childCount; i++)
            {
                var theBroken = breaks.GetChild(i).gameObject;
                //theBroken.AddComponent<BoxCollider>();
                theBroken.AddComponent<MeshCollider>().convex = true;
                theBroken.AddComponent<Rigidbody>();

            }
        }





        head.ShakeHead();
        Messenger.Broadcast(StringMgr.BouthDeathLock);

        //yield return new WaitForSeconds(.5f);

        int num = 0;
        DOTween.To(() => num, x => num = x, 2, 2f)
            .OnComplete(() =>
            {
                GameControl.Instance.GameFail();
            });

    }



}


public enum LockedType
{
    /// <summary>
    /// 台灯
    /// </summary>
    TableLamp,
    /// <summary>
    /// 袜子
    /// </summary>
    SimapleFollow,
    DogHouse,
    PaperClass,
    MailBox,
    /// <summary>
    /// 冰淇淋车
    /// </summary>
    IceCreamTruck,
    RenderFollow,
    DoubleLock,

}
