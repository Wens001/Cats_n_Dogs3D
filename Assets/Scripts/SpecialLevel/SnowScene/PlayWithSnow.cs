using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWithSnow : MonoBehaviour
{
    public GameObject SnowBall;
    public float Force = 10;

    private Animator anim;

    private void Awake()
    {
        SnowBall?.SetActive(false);
        anim = GetComponent<Animator>();
    }


    #region 动画中调用

    public void GetTheSnowBall()
    {
        SnowBall.SetActive(true);
    }

    public void ThrowTheSnowBall()
    {
        SnowBall.SetActive(false);

        GameObject ball = Instantiate(SnowBall, SnowBall.transform.position, SnowBall.transform.rotation);
        ball.SetActive(true);
        ball.transform.localScale = Vector3.one;
        var ballRigi = ball.GetComponent<Rigidbody>();
        ballRigi.isKinematic = false;
        ballRigi.AddForce(transform.forward * Force, ForceMode.Impulse);

        Destroy(ball, 5);
    }

    #endregion



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("WinThing"))
        {
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.SnowballHitClip);
            GetComponent<Collider>().enabled = false;
            anim.SetTrigger("BeHit");
            StartCoroutine(DelaySetWin());
        }
    }
    IEnumerator DelaySetWin()
    {
        yield return new WaitForEndOfFrame();

        float length = Mathf.Min(anim.GetNextAnimatorStateInfo(0).length, 2);
        yield return new WaitForSeconds(length);
       
        Messenger.Broadcast(StringMgr.GetWinCondition);
       

    }



}
