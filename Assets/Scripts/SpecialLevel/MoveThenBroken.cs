using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(Rigidbody))]
public class MoveThenBroken : MonoBehaviour
{
    private Rigidbody rigi;
    private bool haveBroken = false;

    private List<Transform> children = new List<Transform>();
    private List<Rigidbody> childrenRigi = new List<Rigidbody>();

    private void Awake()
    {
        rigi = GetComponent<Rigidbody>();

        children = transform.GetComponentsInChildren<Transform>().ToList();
        children.RemoveAt(0);
    }


    private void Update()
    {
        if (!haveBroken && GameControl.Instance.GameProcess == GameProcess.InGame && rigi.velocity.magnitude > 2)
        {
            haveBroken = true;
            GetComponent<Collider>().enabled = false;
            rigi.isKinematic = true;
            StartCoroutine(StartBroken());
            return;
        }

        if (haveBroken)
        {
            for (int i = 0; i < childrenRigi.Count; i++)
            {
                if (childrenRigi[i].velocity.y == 0)
                {
                    childrenRigi[i].GetComponent<Collider>().enabled = false;
                    childrenRigi.RemoveAt(i);
                    i--;
                }
            }
        }

    }


    private IEnumerator StartBroken()
    {
        yield return new WaitForEndOfFrame();

        foreach (var child in children)
        {
             child.GetComponent<Collider>().enabled = true;

            var _rigi = child.gameObject.AddComponent<Rigidbody>();
            childrenRigi.Add(_rigi);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2);
        Messenger.Broadcast(StringMgr.GetWinCondition);

    }




}
