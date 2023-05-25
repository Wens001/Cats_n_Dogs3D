using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisable : MonoBehaviour
{
    public CatOrDog suitType;

    private void OnTriggerEnter(Collider other)
    {
        TouchMove head = other.GetComponent<TouchMove>();
        if (head && suitType == head.selfType)
        {
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CatchClip);

            gameObject.SetActive(false);
        }
    }

}
