using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 滑索关卡
/// </summary>
public class Strop : LockObjectBase
{
    public Transform rope;
    public GameObject obstacle;


    public float speed = 16;
    private HingeJoint _hingeJoint;

    private void Awake()
    {
        _hingeJoint = transform.GetComponent<HingeJoint>();

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


    bool startMove;
    private void Update()
    {
        if (startMove && GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            transform.Translate(-rope.right * speed * Time.deltaTime, Space.World);

            //另一端头下沉
            if (otherHead)
            {
                otherHead.rigi.AddForce(Vector3.down * 80f);
            }
        }
    }


    #region CallBack


    private void OnHeadLocked(LockObjectBase lockObject, TouchMove lockHead)
    {
        if (lockObject == this && curHead == null)
        {
            lockHead.LockHead();
            curHead = lockHead;

            lockHead.rigi.constraints = RigidbodyConstraints.None;
            _hingeJoint.connectedBody = lockHead.rigi;

            if (obstacle)
            {
                obstacle.SetActive(false);
            }
            StartCoroutine(DelayMove());

            otherHead.InAir = true;
            //另一边头添加移动操纵脚本
            SimapleMove simapleMove = lockHead.otherHead.gameObject.AddComponent<SimapleMove>();
            simapleMove.ForceRate = 3;
            simapleMove.moveWay = SimapleMoveType.MoveX;
            simapleMove.controlType = otherHead.selfType;

            Messenger.Broadcast(StringMgr.BouthDeathLock);

        }
    }
    private IEnumerator DelayMove()
    {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.2f);
        startMove = true;
        if (GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            CameraControl.Instance.ChangeCamera(CameraType.CM_StropView);
            AudioPlayControl.Instance.PlayLongClip(AudioPlayControl.Instance.StropArriveAudio);
        }
    }


    private void OnHeadUnlock(LockObjectBase lockObject)
    {
        if (lockObject == this)
        {
            _hingeJoint.connectedBody = null;
            curHead = null;
        }
    }


    private void OnGameStart()
    {
        if (startMove)
        {
            CameraControl.Instance.ChangeCamera(CameraType.CM_StropView);
            AudioPlayControl.Instance.PlayLongClip(AudioPlayControl.Instance.StropArriveAudio);
        }
    }





    #endregion


}
