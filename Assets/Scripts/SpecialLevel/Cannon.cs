using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("大炮参数")]
    public Transform Shooter;
    public GameObject weaponPrefab;
    public float AtkTime;

    private MyTimer AtkTimer;
    //private bool BeDestroy;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        AtkTimer = new MyTimer(AtkTime);
        AtkTimer.SetFinish();
    }

    private void Update()
    {
        AtkTimer.OnUpdate(Time.deltaTime);
        if (AtkTimer.IsFinish)
        {
            FarAttack();
            AtkTimer.ReStart();
        }

        if (GameControl.Instance.GameProcess == GameProcess.InGame && transform.position.y < -5)
        {
            gameObject.SetActive(false);
            Messenger.Broadcast(StringMgr.GetWinCondition);
        }

    }


    public void FarAttack()
    {
        if (weaponPrefab != null)
        {
            GameObject arrow = Instantiate(weaponPrefab, weaponPrefab.transform.position, weaponPrefab.transform.rotation);
            //arrow.transform.localPosition = Shooter.transform.localPosition;
            
            arrow.AddComponent<Rigidbody>().AddForce((arrow.transform.forward + Vector3.up * .4f) * 1000);
            Destroy(arrow, 3);
        }
    }

   
}
