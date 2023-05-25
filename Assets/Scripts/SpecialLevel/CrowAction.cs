using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CrowAction : MonoBehaviour
{
    Animator anim;
    bool haveFly;

    private void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (haveFly)
        {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }
    }



    float speed = 15f;
    private void OnTriggerEnter(Collider other)
    {
        TouchMove touchMove = other.GetComponentInParent<TouchMove>();
        if (!haveFly && touchMove)
        {
            haveFly = true;
            anim.SetTrigger("FlyTrigger");
            Vector3 dir = (transform.position + Vector3.up * 3 - touchMove.transform.position);
            
            transform.localRotation = Quaternion.LookRotation(dir);
            
            //音效
            //AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CrowAudio);
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CrowAudio);

            Messenger.Broadcast(StringMgr.GetWinCondition);

        }
    }




}
