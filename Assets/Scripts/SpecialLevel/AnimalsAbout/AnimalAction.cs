using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Obi;
using DG.Tweening;
using System;

/// <summary>
/// 鸡鸭的运送逻辑
/// </summary>
public class AnimalAction : MonoBehaviour
{
    public AnimalType selfType;
    public Sprite hintSprite;

    Rigidbody rigi;
    Animator anim;


    private void Awake()
    {
        rigi = transform.GetComponent<Rigidbody>();
        anim = transform.GetComponent<Animator>();
        anim.SetFloat("AnimalType", Convert.ToInt32(selfType));
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);
    }



    bool haveHideHint = false;
    private void Update()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            if (rigi.velocity.magnitude > .5f)
            {
                anim.SetBool("Walk", true);

                if (!haveHideHint)
                {
                    haveHideHint = true;
                    Messenger.Broadcast(StringMgr.otherHideHint, gameObject);
                }
            }
            else
            {
                rigi.velocity = Vector3.zero;
                anim.SetBool("Walk", false);
            }
        }
    }


    private void LateUpdate()
    {
        rigi.velocity = Vector3.ClampMagnitude(rigi.velocity, GameSetting.MaxSpd);

        Vector3 dir = Vector3.ProjectOnPlane(rigi.velocity, Vector3.up);
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 30);
        }
    }


    private void OnGameStart()
    {
        Messenger.Broadcast(StringMgr.otherHintBroadcast, gameObject, hintSprite);
    }

}
