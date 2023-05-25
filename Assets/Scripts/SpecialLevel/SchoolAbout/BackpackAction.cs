using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackpackAction : MonoBehaviour
{
    public List<GameObject> books = new List<GameObject>();
    public Sprite HintSprite;


    private void Awake()
    {
        foreach (var book in books)
        {
            book.SetActive(false);

        }

        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (books.Count == 0)
        {
            return;
        }

        SimapleLock book = other.GetComponentInParent<SimapleLock>();
        if (book && book.lockedType == LockedType.PaperClass)
        {
            book.curHead.UnlockHead();
            book.gameObject.SetActive(false);
            books[0].SetActive(true);

            books.RemoveAt(0);


            if (books.Count == 0)
            {
                Messenger.Broadcast(StringMgr.GetWinCondition);
            }
        }



    }



    private void OnGameStart()
    {
        Messenger.Broadcast(StringMgr.otherHintBroadcast, gameObject, HintSprite);

        int num = 0;
        DOTween.To(() => num, x => num = x, 95, 5)
                   .OnComplete(() => {
                       Messenger.Broadcast(StringMgr.otherHideHint, gameObject);
                   });
    }




}
