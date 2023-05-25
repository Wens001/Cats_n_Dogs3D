using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TouchMove : MonoBehaviour
{
    public Touch touch;
    public CatOrDog selfType;
    public TouchMove otherHead;
    public ParticleSystem shoutVfx;

    [HideInInspector] public int holdFingerID = -1;
    [HideInInspector] public bool InAir = false;
    [HideInInspector] public bool isHold = false;
    [HideInInspector] public bool deathLock = false;
    [HideInInspector] public Rigidbody rigi;
    [HideInInspector] public bool IsLocked { get; private set; }

    private void Awake()
    {
        rigi = transform.GetComponent<Rigidbody>();
        lockCD.SetFinish();
        grassLevel = FindObjectOfType<GrassControl>() != null;

    }

    private void OnEnable()
    {
        Messenger.AddListener(StringMgr.BouthLock, OnUseOtherMove);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.BouthLock, OnUseOtherMove);

    }




    RaycastHit[] hits;
    bool grassLevel;        //判断是否是稻田画关卡
    MyTimer lockCD = new MyTimer(.5f);
    private void Update()
    {
        if (GameControl.Instance.GameProcess != GameProcess.InGame)
        {
            return;
        }

        lockCD.OnUpdate(Time.deltaTime);

        //稻田画关卡
        if (grassLevel && selfType == CatOrDog.Cat)
        {
            Vector3 rayDir = otherHead.transform.position - transform.position;

            hits = Physics.SphereCastAll(transform.position, .3f, rayDir.normalized, rayDir.magnitude, LayerMask.GetMask("Wheat"));
            foreach (var hit in hits)
            {
                hit.transform.GetComponent<Collider>().enabled = false;
                hit.transform.SetParent(null);

                hit.transform.DORotate(Vector3.forward * 90, .6f)
                    .OnComplete(() => {
                        hit.transform.gameObject.SetActive(false);
                    });
            }
        }


    }

    private void LateUpdate()
    {
        rigi.velocity = Vector3.ClampMagnitude(rigi.velocity, GameSetting.MaxSpd);

        if (!IsLocked)
        {
            transform.localRotation = new Quaternion(0, transform.localRotation.y, 0, transform.localRotation.w);

            //if (InAir)
            //{
            //    transform.localRotation = new Quaternion(transform.localRotation.y, transform.localRotation.y, 0, transform.localRotation.w);
            //}
        }
    }

    #region 其他

    public void AddExternalForce(float force, Vector3 dir)
    {
        if (/*!isHold && */!IsLocked)
        {
            rigi.AddForce(dir * force);
        }
    }

    public void AnimalShout()
    {
        //音效
        switch (selfType)
        {
            case CatOrDog.Cat:
                AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CatShoutClip);
                break;
            case CatOrDog.Dog:
                AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.DogShoutClip);
                break;
            default:
                break;
        }

        //特效
        if (shoutVfx)
        {
            shoutVfx.Play();
        }
    }


    MyTimer shockUpTimer = new MyTimer(3f);
    /// <summary>
    /// 胜利时，跃起，喊叫
    /// </summary>
    private void ShoutUp()
    {
        UnlockHead();
        //transform.LookAt(Vector3.up);

        rigi.AddForce(Vector3.up * 20f, ForceMode.Impulse);
        //transform.position += Vector3.up;

        //rigi.centerOfMass = transform.position + transform.forward * 1.5f;

        //AnimalShout();
        //shockUpTimer.OnUpdate(Time.deltaTime);
        //if (shockUpTimer.IsFinish && selfType == AnimalType.Cat)
        //{
        //    rigi.AddForce(Vector3.up * 20f);
        //    //transform.localPosition += Vector3.up;
        //    shockUpTimer.ReStart();
        //}
    }

    public void ShakeHead()
    {
        StartCoroutine(ShakeHeadByForce());
    }
    private IEnumerator ShakeHeadByForce()
    {
        float torqueForce = GameSetting._force;

        yield return new WaitForEndOfFrame();
        rigi.AddRelativeTorque(transform.up * torqueForce, ForceMode.Impulse);

        yield return new WaitForSeconds(.1f);
        rigi.AddRelativeTorque(-transform.up * torqueForce * 2, ForceMode.Impulse);

        yield return new WaitForSeconds(.1f);
        rigi.AddRelativeTorque(transform.up * torqueForce * 2, ForceMode.Impulse);

        yield return new WaitForSeconds(.1f);
        rigi.AddRelativeTorque(-transform.up * torqueForce * 2, ForceMode.Impulse);

        yield return new WaitForSeconds(.1f);
        rigi.AddRelativeTorque(transform.up * torqueForce * 2, ForceMode.Impulse);

        yield return new WaitForSeconds(.1f);
        rigi.AddRelativeTorque(-transform.up * torqueForce, ForceMode.Impulse);

    }


    #endregion


    #region 锁定相关

    LockObjectBase lockObject;
    //触碰，锁定
    private void OnTriggerEnter(Collider other)
    {
        if (!IsLocked && lockCD.IsFinish && other.CompareTag(StringMgr.Tags.LockObject))
        {
            lockObject = other.GetComponentInParent<LockObjectBase>();
            if (lockObject && (lockObject.suitHead == CatOrDog.Default || lockObject.suitHead == selfType))
            {
                Messenger.Broadcast(StringMgr.LockHead, lockObject, this);
            }
        }
    }

    public void LockHead(bool lockTrue = true)
    {
        IsLocked = true;
        if (lockObject)
        {
            lockObject.curHead = this;
            lockObject.otherHead = otherHead;
        }

        if (lockTrue)
        {
            FreezeHead();
        }

        //咬住音效
        AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CatchClip);
        //震动
        ShakeControl.Instance.LightShake();
    }


    public void TryUnlockHead()
    {
        //holdToUnlockTimer.OnUpdate(Time.deltaTime);
        if (IsLocked && !deathLock && !isOtherMoving)
        {
            UnlockHead();
        }
    }
    public void UnlockHead()
    {
        //发信
        Messenger.Broadcast<LockObjectBase>(StringMgr.UnlockHead, lockObject);

        IsLocked = false;
        if (lockObject)
        {
            lockObject.curHead = null;
            lockObject.otherHead = null;
            lockObject = null;
        }

        lockCD.ReStart();
        FreeHead();
    }
    //咬住时，锁定
    private void FreezeHead(bool allFreeze = false)
    {
        //冻结位置
        if (allFreeze)
        {
            //rigi.constraints = RigidbodyConstraints.FreezeAll;
            rigi.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            //rigi.constraints = RigidbodyConstraints.FreezePosition;
            rigi.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePosition;
        }

        rigi.angularDrag = .15f;
    }
    //平常移动时的状态
    private void FreeHead()
    {
        //重新锁住转向
        Quaternion rotation = transform.rotation;
        transform.rotation = Quaternion.LookRotation(new Vector3(0, rotation.y, 0));

        rigi.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rigi.angularDrag = .05f;
    }

    #endregion


    #region 移动相关


    /// <summary>
    /// 移动函数
    /// </summary>
    /// <param name="dir"></param>
    public void DoMove(Vector3 dir)
    {
        //dir = CaluteRealDir(dir);
        if (dir != Vector3.zero/* && !IsLocked*/)
        {
            //Debug.DrawRay(transform.position, dir * 5, Color.red, 1f);

            rigi.AddForce(dir * GameSetting._force * 2);

            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 20);
            //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
            //transform.localRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, 0, transform.localRotation.w);
        }
    }


    Vector3 targetSpd;
    public void StopMove()
    {
        if (InAir)
        {
            return;
        }

        //rigi.velocity = Vector3.Lerp(rigi.velocity, Vector3.zero, Time.deltaTime * 60);

        targetSpd = new Vector3(0, rigi.velocity.y, 0);
        rigi.velocity = Vector3.Lerp(rigi.velocity, targetSpd, Time.deltaTime * 60);
    }

    #region 查探前方地形（弃）

    RaycastHit point1, point2;
    private Vector3 CaluteRealDir(Vector3 dir)
    {
        Physics.Raycast(transform.position, Vector3.down, out point1, 5f, LayerMask.GetMask(StringMgr.Layer.Ground));
        Physics.Raycast(transform.position, (Vector3.down + dir).normalized, out point2, 5f, LayerMask.GetMask(StringMgr.Layer.Ground));



        return (point2.point - point1.point).normalized;
    }

    #endregion

    #endregion


    #region CallBack

    [HideInInspector] public bool isOtherMoving;
    private void OnUseOtherMove()
    {
        isOtherMoving = true;
    }

    #endregion

}
