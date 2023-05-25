using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

//邮递员
public class PostManAction : MonoBehaviour
{
    public GameObject HandleLetter;
    public GameObject OutLetter;
    public Sprite HintSprite;
    public Transform HintPoint;

    private float speed = 5;
    private Animator anim;

    TouchMove Dog;
    MyTimer ShoutTimer = new MyTimer(.5f);

    private void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();
        OutLetter.SetActive(false);
        anim.SetBool("FearState", true);

        foreach (var head in FindObjectsOfType<TouchMove>())
        {
            if (head.selfType == CatOrDog.Dog)
            {
                Dog = head;
                break;
            }
        }
    }

    private void OnEnable()
    {
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnHeadLocked);

    }

    private void Start()
    {
        Messenger.Broadcast(StringMgr.otherHintBroadcast, HintPoint == null? gameObject : HintPoint.gameObject, HintSprite);
    }


    private void Update()
    {
        if (GameControl.Instance.GameProcess != GameProcess.InGame)
        {
            return;
        }

        if (walkState)
        {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

            var distance = Vector3.Distance(transform.position, OutLetter.transform.position);
            if (distance < 5)
            {
                walkState = false;
                StartCoroutine(WaitForPutLetter());
            }
        }

        ShoutTimer.OnUpdate(Time.deltaTime);
        if (ShoutTimer.IsFinish && !stopShout)
        {
            ShoutTimer.ReStart();
            Dog.AnimalShout();
        }
    }




    bool stopShout = false;
    bool walkState = false;
    private void OnHeadLocked(LockObjectBase lockObj, TouchMove head)
    {
        if (head.selfType == CatOrDog.Dog)
        {
            anim.SetBool("FearState", false);
            anim.SetTrigger("Walk");
            walkState = true;
            stopShout = true;
            CameraControl.Instance.ChangeCamera(CameraType.CM_LookForward);

            //anim.SetTrigger("PutLetterState");
            var point = new Vector3(OutLetter.transform.position.x, transform.position.y, OutLetter.transform.position.z);
            transform.LookAt(point);


            Messenger.Broadcast(StringMgr.otherHideHint, HintPoint == null ? gameObject : HintPoint.gameObject);
        }
    }


    IEnumerator WaitForPutLetter()
    {
        anim.SetTrigger("PutLetterState");
        yield return new WaitForEndOfFrame();

        float dur = anim.GetNextAnimatorClipInfo(0).Length;
        yield return new WaitForSeconds(dur);

        HandleLetter.SetActive(false);
        OutLetter.SetActive(true);
        Messenger.Broadcast(StringMgr.GetWinCondition);

    }


}

