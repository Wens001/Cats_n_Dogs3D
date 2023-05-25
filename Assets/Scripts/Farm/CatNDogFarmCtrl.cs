using CallBackDelegate;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Messaging;
//using UnityEditor.VersionControl;
using UnityEngine;

public class CatNDogFarmCtrl : MonoBehaviour
{
    public TouchMove dogHead;
    public TouchMove catHead;
    Ray ray;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out hit, 200))
        //    {
        //        BothHeadMove(hit.point);
        //    }
        //}
    }

    void SetHeadMove(TouchMove _head,Vector3 _targetPos,Callback callback=null) {
        Vector3 dir = _targetPos - _head.transform.position;
        dir.y = 0;
        float moveTime = dir.magnitude / (GameSetting.MaxSpd+2);
        _head.rigi.DORotate(Quaternion.LookRotation(dir.normalized).eulerAngles, moveTime>= 0.5f?0.5f:moveTime);
        _head.rigi.DOMove(_targetPos, moveTime).OnComplete(()=> {
            if (callback != null)
                callback();
        });
    }
    public void DogHeadMove(Vector3 _targetPos, Callback callback = null) {
        SetHeadMove(dogHead, _targetPos, callback);
    }
    public void CatHeadMove(Vector3 _targetPos, Callback callback = null)
    {
        SetHeadMove(catHead, _targetPos, callback);
    }

    public void BothHeadMove(Vector3 _targetPos, Callback callback = null) {       
        DogHeadMove(_targetPos + new Vector3(2.2f, 0, 0), callback);
        CatHeadMove(_targetPos + new Vector3(-2.2f, 0, 0));
    }
  
    public void GoToWatering(Vector3 _targetPos, Callback callback = null)
    {
        _targetPos.z -= 2;
        _targetPos.y += 0.4f;
        dogHead.rigi.isKinematic = true;
        callback += () => { dogHead.rigi.isKinematic = false; };
        BothHeadMove(_targetPos,callback);
    }

    public void GoToFei(Vector3 _targetPos, Callback callback = null)
    {
        _targetPos.z -= 2;
        dogHead.rigi.isKinematic = true;
        callback += () => { dogHead.rigi.isKinematic = false; };
        BothHeadMove(_targetPos, callback);
    }

    public void SetHeadsIK(bool _isKinematic) {
        dogHead.rigi.isKinematic = _isKinematic;
        catHead.rigi.isKinematic = _isKinematic;
    }

    public void SetBothLock(bool isLock) {
        if (isLock)
        {
            dogHead.rigi.constraints = RigidbodyConstraints.FreezeRotation;
            catHead.rigi.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else {
            dogHead.rigi.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            catHead.rigi.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
            
    }

    public void DoKillAnim() {
        dogHead.rigi.DOKill();
        catHead.rigi.DOKill();
    }
}
