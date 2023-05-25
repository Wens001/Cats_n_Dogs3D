using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 礼盒
/// </summary>
public class GiftAction : MonoBehaviour
{
    public List<LockObjectBase> GiftRopes;
    public Transform ropesTrans;
    public Transform BoxCover;


    [Header("GiftShow")]
    public Transform GiftPoint;
    public ParticleSystem vfx;
    public Sprite HintSprite;

    private bool HaveGotGift
    {
        get => PlayerPrefs.GetInt("HaveGotGift", 0) == 1;
        set => PlayerPrefs.SetInt("HaveGotGift", value ? 1 : 0);
    }


    private bool HaveHold;
    private bool HaveOpen;
    private float JudgeDis = 5;

    private void Awake()
    {
        AddSelfToObiUpdate();
    }

    private void OnEnable()
    {
        Messenger.AddListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnSomethingLocked);
    }

    private void Start()
    {
        //ab分组为奖励关，且未重玩——显示礼盒
        gameObject.SetActive(!GameSetting.IsReplay && SDKManager.Instance.HaveBonus);

        ropesTrans.gameObject.SetActive(!HaveGotGift); 
        if (!HaveGotGift)
        {
            GameControl.Instance.FirstGotGift = true;
            HaveGotGift = true;
        }
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<LockObjectBase, TouchMove>(StringMgr.LockHead, OnSomethingLocked);
    }


    private void Update()
    {
        if (HaveHold && !HaveOpen)
        {
            for (int i = 0; i < GiftRopes.Count; i++)
            {
                var dis = Vector3.Distance(transform.position, GiftRopes[i].transform.position);
                if (dis > JudgeDis)
                {
                    OpenTheGift();
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<TouchMove>())
        {
            GetComponent<Collider>().enabled = false;
            OpenTheGift();
        }
    }



    private void AddSelfToObiUpdate()
    {
        var obiUpdate = FindObjectOfType<Obi.ObiFixedUpdater>();
        var selfSolver = transform.GetComponent<Obi.ObiSolver>();

        if (!obiUpdate.solvers.Contains(selfSolver))
        {
            obiUpdate.solvers.Add(selfSolver);
        }

    }


    private void OnSomethingLocked(LockObjectBase lockObj, TouchMove head)
    {
        if (GiftRopes.Contains(lockObj))
        {
            HaveHold = true;
        }
    }




    private void OpenTheGift()
    {
        HaveOpen = true;
        HaveHold = false;
       

        GameControl.Instance.JudgeGiftType(HaveGotGift);
       


        //隐藏飘带
        foreach (var locked in GiftRopes)
        {
            if (locked.curHead)
            {
                locked.curHead.UnlockHead();
                locked.gameObject.SetActive(false);
            }
        }
        GiftPoint.localPosition = Vector3.up;
        ropesTrans.gameObject.SetActive(false);
        //BoxCover.gameObject.SetActive(false);
        BoxCover.gameObject.AddComponent<Rigidbody>().AddForce((Vector3.up + transform.forward) * 10, ForceMode.Impulse);
        vfx.Play();


        Messenger.Broadcast(StringMgr.ShowGiftImage, GiftPoint, GameControl.Instance.IsCoinGift ? HintSprite : GameControl.Instance.BonusSkin.Icon);


        GiftPoint.DOLocalMoveY(transform.localPosition.y + 3, 1)
            .OnComplete(()=> {
                UIPanelManager.Instance.PushPanel(UIPanelType.GetGiftPanel);
                Messenger.Broadcast(StringMgr.HideGiftImage);
            });
    }




}
