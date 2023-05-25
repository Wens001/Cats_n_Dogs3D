using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheManAction : MonoBehaviour
{
    public GameObject WorkClothes;
    public GameObject WorkShoes;
    public GameObject HomeClothes;
    public GameObject HomeShoes;
    public GameObject NewsPaper;

    public Transform SofaTrans;


    private Animator anim;

    private void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();

        HomeClothes.SetActive(false);
        HomeShoes.SetActive(false);
        NewsPaper.SetActive(false);
    }





    private void OnTriggerEnter(Collider other)
    {
        SimapleLock simaple = other.GetComponentInParent<SimapleLock>();
        if (simaple)
        {
            if (simaple.gameObject.name.Equals("Clothes"))
            {
                HomeClothes.SetActive(true);
                WorkClothes.SetActive(false);
            }

            if (simaple.gameObject.name.Equals("Shoes"))
            {
                HomeShoes.SetActive(true);
                WorkShoes.SetActive(false);
            }

            simaple.gameObject.SetActive(false);
            simaple.curHead.UnlockHead();


            if (HomeShoes.activeSelf && HomeClothes.activeSelf)
            {
                //Messenger.Broadcast(StringMgr.GetWinCondition);
                CameraControl.Instance.ChangeCamera(CameraType.CM_LookForward);
                StartCoroutine(GotoWatchPaper());

            }
        }

        if (other.gameObject.name.Equals("Letter"))
        {
            other.gameObject.SetActive(false);
            NewsPaper.SetActive(true);
            Messenger.Broadcast(StringMgr.GetWinCondition);

        }
    }



    IEnumerator GotoWatchPaper()
    {
        anim.SetTrigger("Sit");

        yield return new WaitForEndOfFrame();
        NewsPaper.SetActive(true);
        transform.position = SofaTrans.position;
        transform.rotation = SofaTrans.rotation;

        yield return new WaitForSeconds(1.5f);
        Messenger.Broadcast(StringMgr.GetWinCondition);


    }







}
