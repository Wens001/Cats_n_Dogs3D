using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsPool : MonoBehaviour
{

    int endCount = 150;
    bool haveEnd = false;
    BoyAction boy;

    private void Start()
    {
        boy = FindObjectOfType<BoyAction>();
    }


    private void Update()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame && !haveEnd)
        {

            if (transform.childCount < endCount)
            {
                haveEnd = true;

                if (boy)
                {
                    //boy.gameObject.SetActive(true);
                    //boy.GetWinHappy();
                    boy.GetAlive();
                    Messenger.Broadcast(StringMgr.BouthDeathLock);

                }


                //Messenger.Broadcast(StringMgr.GetWinCondition);
            }
        }
    }





}
