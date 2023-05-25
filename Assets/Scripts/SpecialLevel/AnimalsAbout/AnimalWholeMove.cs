using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalWholeMove : MonoBehaviour
{
    public AnimalType selfType;
    public Transform targetPoint;
    public Transform TempPoint;
    public float speed;

    private Rigidbody rigi;
    private Animator[] animators;

    private void Awake()
    {
        rigi = GetComponent<Rigidbody>();
        animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            anim.SetFloat("AnimalType", Convert.ToInt32(selfType));
        }

        Messenger.AddListener(StringMgr.OpenTheValve, OnOpenTheDoor);
        Messenger.AddListener(StringMgr.ShipArrive, OnShipArrive);

    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.OpenTheValve, OnOpenTheDoor);
        Messenger.RemoveListener(StringMgr.ShipArrive, OnShipArrive);

    }



    private void Update()
    {
        if (GameControl.Instance.GameProcess != GameProcess.InGame)
        {
            return;
        }

        if (walk)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.position) < .2f)
            {
                walk = false;
                Messenger.Broadcast(StringMgr.GetWinCondition);
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<Animator>().Play(TempPoint == null ? "Idle A" : "Munch/Peck");
                }
            }
        }

        if (rigi)
        {
            if (rigi.velocity.magnitude > .5f)
            {
                foreach (var anim in animators)
                {
                    anim.SetBool("Walk", true);
                }

            }
            else
            {
                rigi.velocity = Vector3.zero; 
                foreach (var anim in animators)
                {
                    anim.SetBool("Walk", false);
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (rigi)
        {
            rigi.velocity = Vector3.ClampMagnitude(rigi.velocity, GameSetting.MaxSpd);

            Vector3 dir = Vector3.ProjectOnPlane(rigi.velocity, Vector3.up);
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 30);
            }
        }
    }


    #region CallBack

    bool walk;
    private void OnOpenTheDoor()
    {
        Messenger.Broadcast(StringMgr.BouthDeathLock);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().Play("Walk");
        }
        walk = true;

    }

    private void OnShipArrive()
    {
        transform.position = TempPoint.position;
        transform.rotation = Quaternion.identity;
        walk = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().Play("Walk");
        }

        //
        CameraControl.Instance.ChangeCamera(CameraType.CM_LookFocus);
    }


    #endregion





}
