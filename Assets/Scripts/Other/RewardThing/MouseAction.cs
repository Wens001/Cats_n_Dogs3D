using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DG.Tweening;

public class MouseAction : MonoBehaviour
{
    public Transform PointsTrans;

    [Header("金鼠相关")]
    public Transform Mouse;
    public float MouseSpd = 10;
    public float MouseIdleTime = 1;
    [Tooltip("持续时间,-1为无穷")]
    public float HoldTime = -1;

    private Animator anim;
    private MyTimer IdleTimer;
    private MyTimer HoldTimer;
    private List<Transform> points;
    private bool idleState;
    private Vector3 dir;

    private int pointIndex = 0;
    private int PointIndex
    {
        get => pointIndex;
        set
        {
            pointIndex = value == points.Count ? 0 : value;
        }
    }

    private bool HaveGotMouse
    {
        get => PlayerPrefs.GetInt("HaveGotMouse", 0) == 1;
        set => PlayerPrefs.SetInt("HaveGotMouse", value ? 1 : 0);
    }


    private void Awake()
    {
        points = PointsTrans.GetComponentsInChildren<Transform>().ToList();
        points.RemoveAt(0);

        IdleTimer = new MyTimer(MouseIdleTime);
        HoldTimer = HoldTime > 0 ? new MyTimer(HoldTime) : null;

        Mouse.position = points[0].position;
        Mouse.rotation = Quaternion.LookRotation(points[1].position - points[0].position);

        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        //ab分组为奖励关，且未重玩——显示金鼠
        gameObject.SetActive(!GameSetting.IsReplay && SDKManager.Instance.HaveBonus);
        if (gameObject.activeSelf)
        {
            AudioPlayControl.Instance.PlayLongClip(AudioPlayControl.Instance.MouseClip);
        }
    }


    private void OnDisable()
    {
        AudioPlayControl.Instance.PauseClip(AudioPlayControl.Instance.MouseClip);
    }

    private void Update()
    {
        if (!idleState)
        {
            Mouse.Translate(dir * MouseSpd * Time.deltaTime, Space.World);
            CheckMouseMove();
        }
        else
        {
            IdleTimer.OnUpdate(Time.deltaTime);
            if (IdleTimer.IsFinish)
            {
                MouseStartMove();
            }
        }

        //逃脱计时
        if (GameControl.Instance.GameProcess == GameProcess.InGame && (HoldTimer != null && !HoldTimer.IsFinish))
        {
            HoldTimer.OnUpdate(Time.deltaTime);
            if(HoldTimer.IsFinish)
            {
                StartCoroutine(MouseEscape());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TouchMove head = other.GetComponentInParent<TouchMove>();
        if (head)
        {
            GameControl.Instance.JudgeGiftType(HaveGotMouse);
            if (!HaveGotMouse)
            {
                GameControl.Instance.FirstGotGift = true;
                HaveGotMouse = true;
            }

            //音效
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CatchClip);

            UIPanelManager.Instance.PushPanel(UIPanelType.GetGiftPanel);
            gameObject.SetActive(false);
        }
    }




    private void MouseStartMove()
    {
        idleState = false;
        anim.SetBool("Walk", true);

        PointIndex += 1;
        dir = (points[PointIndex].position - Mouse.position).normalized;
        Mouse.rotation = Quaternion.LookRotation(dir);
    }

    private void CheckMouseMove()
    {
        if (Vector3.Distance(Mouse.position, points[PointIndex].position) <= MouseSpd * .03f)
        {
            MouseStopMove();
        }
    }

    private void MouseStopMove()
    {
        idleState = true;
        anim.SetBool("Walk", false);

        IdleTimer.ReStart();
    }

    /// <summary>
    /// 金鼠逃脱
    /// </summary>
    private IEnumerator MouseEscape()
    {
        anim.SetTrigger("Escape");

        yield return new WaitForEndOfFrame();

        var length = anim.GetNextAnimatorClipInfo(0).Length;
        yield return new WaitForSeconds(length);

        //播放动画
        gameObject.SetActive(false);
    }

}
