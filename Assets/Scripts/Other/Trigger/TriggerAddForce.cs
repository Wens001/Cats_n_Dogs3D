using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerAddForce : MonoBehaviour
{
    [Header("参数控制")]
    public float RestTime = 2.5f;
    public float WindTime = 3;
    public float ForceRate = 1.5F;

    [Header("特效控制")]
    public Fan[] fans;
    public GameObject[] VFXs;
    private List<TouchMove> headList = new List<TouchMove>();
    //private float WindForce => GameSetting._force;

    AudioSource audioSource;

    private void Awake()
    {
        isWindTime = true;

        windTimer = new MyTimer(WindTime);
        restTimer = new MyTimer(RestTime);

        windTimer.ReStart();
        restTimer.SetFinish();
        audioSource = transform.GetComponent<AudioSource>();

        GetComponent<Renderer>().enabled = false;
    }

    MyTimer VibrationTimer = new MyTimer(.2f);
    private void Update()
    {
        WindAction();

        if (GameControl.Instance.GameProcess == GameProcess.InGame && isWindTime && headList.Count > 0)
        {
            foreach (var head in headList)
            {
                head.AddExternalForce(GameSetting._force * ForceRate, transform.right);
            }

            VibrationTimer.OnUpdate(Time.deltaTime);
            if (VibrationTimer.IsFinish)
            {
                VibrationTimer.ReStart();
                //震动
                ShakeControl.Instance.ShotLightShake();
            }
           
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        TouchMove head = other.GetComponent<TouchMove>();
        if (head && !headList.Contains(head))
        {
            headList.Add(head);
            head.InAir = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        TouchMove head = other.GetComponent<TouchMove>();
        if (head && headList.Contains(head))
        {
            headList.Remove(head);
            head.InAir = false;

        }
    }

    float fanSpeed = 1000;
    MyTimer restTimer;
    MyTimer windTimer;
    bool isWindTime = false;
    private void WindAction()
    {
        //休息时间
        if (!restTimer.IsFinish)
        {
            restTimer.OnUpdate(Time.deltaTime);

            //开始刮风
            if (restTimer.IsFinish)
            {
                isWindTime = true;
                windTimer.ReStart();

                foreach (var vfx in VFXs)
                {
                    vfx.SetActive(true);
                }

                fanSpeed = 0;
                DOTween.To(() => fanSpeed, x => fanSpeed = x, 1000, .8f);

                //音效
                audioSource.Play();
            }
        }

        //刮风时间
        if (!windTimer.IsFinish)
        {
            windTimer.OnUpdate(Time.deltaTime);

            //停止刮风
            if (windTimer.IsFinish)
            {
                //休息时间为零的时候，继续刮风
                if (restTimer.DurationTime == 0)
                {
                    windTimer.ReStart();
                    return;
                }

                isWindTime = false;
                restTimer.ReStart();

                foreach (var vfx in VFXs)
                {
                    vfx.SetActive(false);
                }

                fanSpeed = 1000;
                DOTween.To(() => fanSpeed, x => fanSpeed = x, 0, 1f);
            }
        }

        //控制风扇
        foreach (var fan in fans)
        {
            fan.FanRotate(fanSpeed);
        }
    }


}
