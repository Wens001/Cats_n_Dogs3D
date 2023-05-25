using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDead : MonoBehaviour
{
    public float lifeTime = 5;
    private float curTime;
    public bool isDestroy;


    private void OnEnable()
    {
        curTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime < lifeTime)
        {
            curTime += Time.deltaTime;
        }
        else {
            if (isDestroy)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
