using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Fire : MonoBehaviour
{
    public AudioClip outFireClip;
    bool hurt;


    private void OnEnable()
    {
        Messenger.AddListener<int>(StringMgr.LevelInit, OnLevelInit);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<int>(StringMgr.LevelInit, OnLevelInit);
    }


    MyTimer FireRestrike = new MyTimer(.8f);
    private void Update()
    {
        FireRestrike.OnUpdate(Time.deltaTime);
        if (FireRestrike.IsFinish && hurt && transform.localScale.x < 3)
        {
            FireScale = Mathf.Clamp(FireScale + extent, 0, 3);
            transform.localScale = Vector3.one * FireScale;
        }

    }


    private void OnParticleCollision(GameObject other)
    {
        if (hurt)
        {
            OutFire();
        }
    }


    float FireScale = 3;
    float targetScale = .5f;
    float extent = 0.05f;
    bool vibration = false;
    private void OutFire()
    {
        if (transform.localScale.x > targetScale)
        {
            FireScale -= extent;
            transform.localScale = Vector3.one * FireScale;
            FireRestrike.ReStart();
            //震动
            vibration = !vibration;
            if (vibration)
            {
                ShakeControl.Instance.ShotLightShake();
            }

            //灭火成功
            if (transform.localScale.x <= targetScale && hurt)
            {
                //音效
                AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.outFireClip);

                hurt = false;
                Debug.Log("灭火成功，发送信号");
                Messenger.Broadcast(StringMgr.GetWinCondition);

                gameObject.SetActive(false);
            }
        }
    }

    private void OnLevelInit(int levelIndex)
    {
        FireInit();
    }

    private void FireInit()
    {
        transform.DOKill();
        FireScale = 3;
        hurt = true;
        transform.localScale = Vector3.one * FireScale;
        gameObject.SetActive(true);
    }



}
