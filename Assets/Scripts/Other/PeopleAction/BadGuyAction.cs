using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BadGuyAction : MonoBehaviour
{
    public BoyState curState;

    private Animator anim;
    private BoyAction theBoy;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();

    }

    private void Start()
    {
        switch (curState)
        {
            case BoyState.PlayWithSnow:
                anim.SetTrigger("PlayWithSnow");
                break;
            case BoyState.BullyState:
                anim.SetTrigger("Bully");
                break;
            default:
                break;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (theBoy != null || curState == BoyState.PlayWithSnow)
        {
            return;
        }


        //触碰攻击
        theBoy = other.GetComponent<BoyAction>();
        if (theBoy && theBoy.CurState == BoyState.FindSomeThing)
        {
            other.GetComponentInParent<Rigidbody>().isKinematic = true;
            other.transform.LookAt(transform);
            transform.LookAt(other.transform);

            anim.SetTrigger("Hit");
            StartCoroutine(HitTheBoy());

            Messenger.Broadcast(StringMgr.BouthDeathLock);
        }
    }
    private IEnumerator HitTheBoy()
    {
        yield return new WaitForEndOfFrame();

        float length = anim.GetNextAnimatorClipInfo(0).Length;
        yield return new WaitForSeconds(length);
        if (theBoy)
        {
            theBoy.BeHit();
        }
    }



    public void GoAway()
    {
        anim.SetBool("Walk", true);
        transform.rotation = Quaternion.identity;
        transform.DOMove(transform.position + Vector3.forward * 20, 2);

    }


}
