using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAction : MonoBehaviour
{
    public ParticleSystem vfx;
    bool once = false;

    private void OnTriggerEnter(Collider other)
    {
        if (once)
        {
            return;
        }

        if (other.GetComponentInParent<BalloonSimapleUp>()
            || other.GetComponentInParent<SimapleMove>()
            || other.GetComponentInParent<TouchMove>()
            || other.GetComponentInParent<SlingShotBall>())
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<Renderer>().enabled = false;

            GameControl.Instance.GotCoinCount += 1;
            once = true;

            //音效
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CoinAudio);
            //AudioPlayControl.Instance.PlayClipOnce(AudioPlayControl.Instance.CoinAudio);

            //特效
            if (vfx)
            {
                vfx.Play();
            }
        }

    }
}
