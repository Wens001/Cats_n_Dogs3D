using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenBalloon : MonoBehaviour
{
    public float speed = 20;
    public List<Transform> brokens = new List<Transform>();

    private void Awake()
    {
        foreach (var broken in brokens)
        {
            broken.SetParent(null);
        }
    }


    private void Update()
    {
        transform.Translate(-transform.forward * speed * Time.deltaTime, Space.World);
    }

}
