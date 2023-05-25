using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterValve : LockObjectBase
{
    public ValveType valveType;
    public Sprite HintSprite;
    public Quaternion TargetRotation = Quaternion.identity;
    public ParticleSystem WaterVfx;

    HingeJoint _hingeJoint;
    Rigidbody hingeRigi;

    private void Awake()
    {
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.AddListener<int>(StringMgr.LevelInit, SelfInit);
        Messenger.AddListener(StringMgr.GameStart, BroadcastHint);

        _hingeJoint = transform.GetChild(0).GetComponent<HingeJoint>();
        hingeRigi = _hingeJoint.GetComponent<Rigidbody>();

    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.RemoveListener<int>(StringMgr.LevelInit, SelfInit);
        Messenger.RemoveListener(StringMgr.GameStart, BroadcastHint);

    }

    bool getCondition = false;
    private void Update()
    {
        //判断是否打开
        if (!getCondition && islock && Quaternion.Angle(_hingeJoint.transform.localRotation, TargetRotation) < 10)
        {
            getCondition = true;
            _hingeJoint.transform.localRotation = TargetRotation;
            hingeRigi.isKinematic = true;

            if (WaterVfx)
            {
                WaterVfx.gameObject.SetActive(true);
            }
            if (valveType == ValveType.WaterValve)
            {
                Messenger.Broadcast(StringMgr.GetWinCondition);
            }
            else
            {
                Messenger.Broadcast(StringMgr.OpenTheValve);
                curHead.UnlockHead();
                Destroy(this);
            }
        }
    }



    bool islock;
    private void OnHeadLocked(LockObjectBase lockObject, TouchMove _lockHead)
    {
        if (lockObject == this && curHead == null)
        {
            _lockHead.LockHead();

            /*******Test*******/
            _lockHead.rigi.constraints = RigidbodyConstraints.None;
            _hingeJoint.connectedBody = _lockHead.rigi;
            hingeRigi.isKinematic = false;
            curHead = _lockHead;
            islock = true;

            //提示气泡
            Messenger.Broadcast(StringMgr.HideHint, suitHead);
            Messenger.Broadcast<LockObjectBase, TouchMove>(StringMgr.ShowBitIt, this, curHead);

        }
    }

    private void OnHeadUnlock(LockObjectBase lockObject)
    {
        if (lockObject == this)
        {
            curHead = null;
            _hingeJoint.connectedBody = null;
            hingeRigi.isKinematic = true;
            islock = false;

            Messenger.Broadcast(StringMgr.HintBroadcast, suitHead, HintSprite);
            Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Cat);
        }
    }

    private void SelfInit(int levelIndex)
    {
        transform.rotation = Quaternion.identity;
    }

    private void BroadcastHint()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, suitHead, HintSprite);
    }

}


public enum ValveType
{
    WaterValve,
    Door,

}
