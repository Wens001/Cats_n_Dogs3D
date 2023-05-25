using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [HideInInspector] public Rigidbody rigi;

    private void Awake()
    {
        rigi = GetComponent<Rigidbody>();

    }

    private void OnEnable()
    {
        Messenger.AddListener(StringMgr.BouthLock, OnBouthLock);
        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.BouthLock, OnBouthLock);
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);
    }



    /// <summary>
    /// 为船施加力
    /// </summary>
    /// <param name="offset">-1，船桨在左；1，船桨在右</param>
    public void AddForce(int offset)
    {
        //Vector3 dir = transform.forward + (offset < 0 ? transform.right : -transform.right);
        //rigi.AddForce(dir * GameSetting._force * 80);

        rigi.AddForce(transform.forward * GameSetting._force * 100);

    }

    private void LateUpdate()
    {
        rigi.velocity = Vector3.ClampMagnitude(rigi.velocity, GameSetting.MaxSpd * 2);
    }


    #region CallBack

    private void OnBouthLock()
    {
        if (rigi == null)
        {
            Debug.Log("ship 脚本中rigi报空");
            return;
        }

        rigi.isKinematic = false;

        transform.Find("Obstacle").gameObject.SetActive(false);
        StartCoroutine("HideHint"); 
    }

    IEnumerator HideHint()
    {
        yield return new WaitForSeconds(2);

        Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Default);
    }


    private void OnGameStart()
    {
        //Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Default, hintSprite);

    }

    #endregion


}
