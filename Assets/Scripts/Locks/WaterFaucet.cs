using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFaucet : LockObjectBase
{
    public Transform waterParticle;
    public Sprite HintSprite;
    public Sprite FireSprite;

    Vector3 waterLocalPosition;
    Quaternion waterLocalRotation;
    HingeJoint _hingeJoint;

    private void Awake()
    {
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.AddListener<int>(StringMgr.LevelInit, SelfInit);
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);

        _hingeJoint = transform.GetComponent<HingeJoint>();
        waterLocalPosition = waterParticle.localPosition;
        waterLocalRotation = waterParticle.localRotation;

    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener<LockObjectBase>(StringMgr.UnlockHead, OnHeadUnlock);
        Messenger.RemoveListener<int>(StringMgr.LevelInit, SelfInit);
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }
 


    private void OnHeadLocked(LockObjectBase lockObject, TouchMove lockHead)
    {
        if (lockObject == this && curHead == null)
        {
            curHead = lockHead;
            lockHead.LockHead();

            waterParticle.SetParent(lockHead.otherHead.transform);
            waterParticle.localPosition = Vector3.zero;
            waterParticle.rotation = waterParticle.parent.rotation;

            /*******Test*******/
            if (_hingeJoint)
            {
                lockHead.rigi.constraints = RigidbodyConstraints.None;
                _hingeJoint.connectedBody = lockHead.rigi;
            }

            //提示气泡
            Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Cat, FireSprite);
            Messenger.Broadcast<LockObjectBase, TouchMove>(StringMgr.ShowBitIt, this, lockHead);
        }
    }
    

    private void OnHeadUnlock(LockObjectBase lockObject)
    {
        if (lockObject == this)
        {
            curHead = null;
            waterParticle.SetParent(transform);
            waterParticle.localPosition = waterLocalPosition;
            waterParticle.localRotation = waterLocalRotation;
            
            if (_hingeJoint)
            {
                _hingeJoint.connectedBody = null;
            }


            Messenger.Broadcast(StringMgr.HintBroadcast, suitHead, HintSprite);
            Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Cat);
        }
    }

    private void SelfInit(int levelIndex)
    {
        waterParticle.SetParent(transform);
        waterParticle.localPosition = waterLocalPosition;
        waterParticle.localRotation = waterLocalRotation;
    }

    private void OnGameStart()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, suitHead, HintSprite);
    }

}
