using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DirtCleanCheck : MonoBehaviour
{
    private int dirtsCount;
    private ParticleSystem vfx;

    private void Awake()
    {
        Messenger.AddListener(StringMgr.DirtClean, OnDirtClean);

        dirtsCount = FindObjectsOfType<DirtAction>().Length;

        vfx = transform.GetComponentInChildren<ParticleSystem>();
        vfx.Stop();

    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.DirtClean, OnDirtClean);
    }


    private void OnDirtClean()
    {
        dirtsCount -= 1;
        if (dirtsCount <= 0)
        {
            //gameObject.SetActive(true);

            vfx.Play();
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CleanUpAudio);
        }
    }




}
