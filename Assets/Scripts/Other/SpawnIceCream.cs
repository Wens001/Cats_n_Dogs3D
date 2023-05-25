using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(SimapleLock))]
public class SpawnIceCream : MonoBehaviour
{
    [Header("特效")]
    public ParticleSystem vfx;

    [Header("喷东西")]
    public GameObject IceCreamBall;
    public float Force = 10;

    private TouchMove curHead;
    private LockObjectBase lockBase;



    private void Awake()
    {
        lockBase = GetComponent<LockObjectBase>();

        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
        //enabled = false;
    }


    private void OnDestroy()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }


    private void Update()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            EatBreakTimer.OnUpdate(Time.deltaTime);
        }
    }


    private MyTimer EatBreakTimer = new MyTimer(.2f);
    private void OnTriggerEnter(Collider other)
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame && other.gameObject.name.Contains("Sphere"))
        {
            if (other.transform.parent != null && other.transform.parent.name.Equals("Balls"))
            {
                EatBall(other.gameObject);
            }
        }
    }


    /// <summary>
    /// 吃球
    /// </summary>
    /// <param name="ball"></param>
    private void EatBall(GameObject ball)
    {
        ball.SetActive(false);
        ball.transform.SetParent(null);

        GameControl.Instance.EatAndStore(.01f);
        if (EatBreakTimer.IsFinish)
        {
            EatBreakTimer.ReStart();
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CatchClip);
        }

    }





    private void OnHeadLocked(LockObjectBase lockObj, TouchMove head)
    {
        if (curHead == null && lockBase == lockObj)
        {
            curHead = head;

            if (IceCreamBall && GameControl.Instance.GameProcess == GameProcess.InGame)
            {
                StartCoroutine(SpawnIceCreamBall());
                CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);
            }


            if (vfx)
            {
                MoveVfxToHead();
            }
        }
    }

    private void OnGameStart()
    {
        if (curHead == null)
        {
            return;
        }


        if (IceCreamBall)
        {
            StartCoroutine(SpawnIceCreamBall());
        }
    }



    private IEnumerator SpawnIceCreamBall()
    {
        while (curHead)
        {
            GameControl.Instance.EatThenOut(curHead.selfType);

            yield return new WaitForSeconds(1f);

            GameObject ball = Instantiate(IceCreamBall, curHead.otherHead.transform.position, curHead.otherHead.transform.rotation);
            ball.SetActive(true);
            ball.GetComponent<Rigidbody>().AddForce((ball.transform.forward + ball.transform.up) * Force, ForceMode.Impulse);
            Destroy(ball, 5);

        }
    }


    private void MoveVfxToHead()
    {
        vfx.transform.SetParent(curHead.otherHead.transform);

        vfx.transform.localRotation = Quaternion.identity;
        vfx.transform.DOLocalRotate(Vector3.left * 15, .1f);
        //vfx.transform.localRotation = new Quaternion(15, 0, 0, 1);

        vfx.transform.localPosition = Vector3.zero;
    }



}
