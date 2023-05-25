using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class EatAndBigger : MonoBehaviour
{
     public int TargetScale = 30;
     public int Extent = 2;


    private CatOrDog headType;
    private Transform Parent;
    private ObiRopeExtrudedRenderer obiRenderer;
    
    private static int scaleSize = 10;
    static List<GameObject> objs = new List<GameObject>();

    private void Awake()
    {
        scaleSize = 10;
        objs.Clear();


        Parent = transform.parent;
        obiRenderer = Parent.GetComponentInChildren<ObiRopeExtrudedRenderer>();
        headType = GetComponent<TouchMove>().selfType;
    }


    private void OnTriggerEnter(Collider other)
    {
        var suitHead = false;
        switch (headType)
        {
            case CatOrDog.Cat:
                suitHead = other.CompareTag(StringMgr.Tags.EatThing_Cat);
                break;
            case CatOrDog.Dog:
                suitHead = other.CompareTag(StringMgr.Tags.EatThing_Dog);
                break;
        }


        if (suitHead && !objs.Contains(other.gameObject))
        {
            objs.Add(other.gameObject);
            other.gameObject.SetActive(false);
            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CatchClip);

            scaleSize += Extent;
            Parent.localScale = Vector3.one * scaleSize * .1f;
            Parent.position += Vector3.up;

            obiRenderer.thicknessScale = 1 / (scaleSize * .1f);

            if (scaleSize.Equals(TargetScale))
            {
                Messenger.Broadcast(StringMgr.GetWinCondition);

            }
        }
    }







}
