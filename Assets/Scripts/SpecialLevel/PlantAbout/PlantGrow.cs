using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrow : MonoBehaviour
{
    public Sprite hintSprite;

    AudioSource audioSource;
    bool endGrow;
    float plantScale;
    float targetScale = 3;
    Transform plant;
    MyTimer stopGrowTimer = new MyTimer(.3f);

    Rigidbody rigi;

    private void Awake()
    {
        //transform.localScale = Vector3.one * 1.5f;
        plantScale = transform.localScale.x;
        plant = transform.GetChild(0);
        audioSource = transform.GetComponent<AudioSource>();
        rigi = transform.GetComponent<Rigidbody>();

        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }

    bool playAudio = false;
    private void Update()
    {
        stopGrowTimer.OnUpdate(Time.deltaTime);
        if (stopGrowTimer.IsFinish && GameControl.Instance.GameProcess == GameProcess.InGame && playAudio)
        {
            audioSource.Pause();
            playAudio = false;
        }

        if (rigi && rigi.velocity.magnitude > .5f && !hideHint)
        {
            hideHint = true;
            Messenger.Broadcast(StringMgr.otherHideHint, gameObject);

        }
    }

    bool hideHint;






    private void OnParticleCollision(GameObject other)
    {
        if (!endGrow)
        {
            GrowUp();
        }
    }



    float extent = .01f;
    bool vibration = false;

    private void GrowUp()
    {
        if (plant.localScale.x < targetScale)
        {
            stopGrowTimer.ReStart();
            if (!playAudio)
            {
                playAudio = true;
                audioSource.Play();
            }


            plantScale += extent;
            plant.localScale = Vector3.one * plantScale;

            //震动
            vibration = !vibration;
            if (vibration)
            {
                ShakeControl.Instance.ShotLightShake();
            }

            if (plant.localScale.x >= targetScale && !endGrow)
            {
                endGrow = true;
                Debug.Log("生长完成，发送信号");
                Messenger.Broadcast(StringMgr.GetWinCondition);
            }
        }
    }


    private void OnGameStart()
    {
        if (transform.localScale.x == targetScale)
        {
            //Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Default, hintSprite);
            Messenger.Broadcast(StringMgr.otherHintBroadcast, gameObject, hintSprite);
        }

    }


}
