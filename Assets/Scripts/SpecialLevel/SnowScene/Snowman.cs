using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowman : MonoBehaviour
{
    public List<GameObject> selfThings;

    private void Awake()
    {
        foreach (var thing in selfThings)
        {
            thing.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        SimapleLock locked = other.GetComponentInParent<SimapleLock>();
        if (locked)
        {
            for (int i = 0; i < selfThings.Count; i++)
            {
                if (selfThings[i].name.Equals(locked.name) && locked.curHead)
                {
                    locked.curHead.UnlockHead();
                    locked.gameObject.SetActive(false);

                    selfThings[i].gameObject.SetActive(true);

                    selfThings.RemoveAt(i);
                    break;
                }
            }

            if (selfThings.Count == 0)
            {
                Messenger.Broadcast(StringMgr.GetWinCondition);
            }

        }


        
    }







}
