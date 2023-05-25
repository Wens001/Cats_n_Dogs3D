using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Balloon : LockObjectBase
{

    public Sprite selfHintSprite;
    public Sprite OtherHintSprite;
    public AudioClip BalloonAudio;

    private float targetHeight = 20;
    private Rigidbody rigi;
    private HingeJoint _hingeJoint;
    private SkinnedMeshRenderer meshRenderer;
    private CameraType lastCMtype;

    private void Awake()
    {
        _hingeJoint = transform.GetComponent<HingeJoint>();
        rigi = transform.GetComponent<Rigidbody>();
        meshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();

        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
        Messenger.AddListener(StringMgr.BouthDeathLock, OnDeathLock);

    }


    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);
        Messenger.RemoveListener(StringMgr.BouthDeathLock, OnDeathLock);

    }

    /// <summary>
    /// 到达结束地点判定
    /// </summary>
    public void OnArrivedPoint()
    {
        curHead.InAir = otherHead.InAir = false;
        curHead.TryUnlockHead();

        GetComponent<SimapleMove>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        CameraControl.Instance.ChangeCamera(CameraType.None);

        StartCoroutine(DelayChangeCamera());

    }
    private IEnumerator DelayChangeCamera()
    {
        yield return new WaitForEndOfFrame();

        CameraControl.Instance.ChangeCamera(lastCMtype);
        transform.DOMoveY(targetHeight * 3, 2f);
    }


    #region CallBack

    private void OnHeadLocked(LockObjectBase lockObject, TouchMove lockHead)
    {
        if (lockObject == this && curHead == null)
        {
            lockHead.LockHead();
            curHead = lockHead;

            /*******Test*******/
            lockHead.rigi.constraints = RigidbodyConstraints.None;
            _hingeJoint.connectedBody = lockHead.rigi;

            //提示气泡
            Messenger.Broadcast<LockObjectBase, TouchMove>(StringMgr.ShowBitIt, this, lockHead);

            //另一个头空闲时，提示要咬的东西
            if (!lockHead.otherHead.IsLocked)
            {
                Messenger.Broadcast(StringMgr.HintBroadcast, otherHead.selfType, OtherHintSprite);

            }
            //另个头已经咬住时， deathLock
            else
            {
                Messenger.Broadcast(StringMgr.BouthDeathLock);

            }
        }
    }

    private void OnHeadUnlock(LockObjectBase lockObject)
    {
        if (lockObject == this)
        {
            curHead = null;
            _hingeJoint.connectedBody = null;



        }
    }

    private void OnGameStart()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Cat, selfHintSprite);

    }


    float FillTime = 3f;
    private void OnDeathLock()
    {
        int num = 0;
        DOTween.To(() => num, x => num = x, 95, FillTime)
            .OnUpdate(() =>{
                meshRenderer.SetBlendShapeWeight(0, num);
            })
            .OnComplete(()=> {
                meshRenderer.SetBlendShapeWeight(0, 100);

                OnBalloonFly();
            });

        meshRenderer.transform.DOScale(1.5f, FillTime);
        transform.DOMoveY(targetHeight, FillTime);
        transform.DORotate(Vector3.zero, FillTime);

        //充气音效
        AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.FillBalloonAudio);
    }


    #endregion

    /// <summary>
    /// 充气完成，准备起飞
    /// </summary>
    private void OnBalloonFly()
    {
        //transform.DOMoveY(targetHeight, FillTime);
        //transform.DORotate(Vector3.zero, FillTime);
        lastCMtype = CameraControl.Instance.GetCurCamera();
        CameraControl.Instance.ChangeCamera(CameraType.CM_Bowling);

        curHead.InAir = otherHead.InAir = true;
        otherHead.TryUnlockHead();

        _hingeJoint.axis = Vector3.zero;

        //充气完成，赋予简单移动
        SimapleMove simapleMove = gameObject.AddComponent<SimapleMove>();
        simapleMove.ForceRate = .6f;
        simapleMove.controlType = curHead.selfType;


        rigi.isKinematic = false;
        rigi.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        //显示摇杆
        Messenger.Broadcast(StringMgr.ShowControlImage, curHead.selfType);
    }




}
