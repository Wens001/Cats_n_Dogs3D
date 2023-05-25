using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运西瓜
/// </summary>
public class MoveThingByHead : MonoBehaviour
{
    public GameObject TheThing;

    private SimapleLock thingStore;
    private TouchMove startHead;


    private void Awake()
    {
        thingStore = transform.GetComponent<SimapleLock>();

        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener(StringMgr.BouthDeathLock, OnBouthHeadLocked);

    }

    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener(StringMgr.BouthDeathLock, OnBouthHeadLocked);
    }




    private void OnHeadLocked(LockObjectBase lockObj, TouchMove head)
    {
        if (thingStore == lockObj)
        {
            startHead = head;
        }
    }


    private void OnBouthHeadLocked()
    {
        StartCoroutine(MoveThing());
        CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);
    }


    private IEnumerator MoveThing()
    {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.5f);

        while (true && GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            GameControl.Instance.EatThenOut(startHead.selfType, 1.2f);

            yield return new WaitForSeconds(1f);

            GameObject Obj = Instantiate(TheThing, startHead.otherHead.transform.position, startHead.otherHead.transform.rotation);
            Obj.SetActive(true);
            //Obj.GetComponent<Rigidbody>().AddForce((Obj.transform.forward + Obj.transform.up) * GameSetting._force * .5f, ForceMode.Impulse);

        }
    }








}
