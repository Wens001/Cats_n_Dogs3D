using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPool : MonoBehaviour
{
    public bool WaitForPlug;
    public GameObject Plug;

    [Range(.1f, 1f)]
    public float FillSpeed = .5f;
    public Transform Water;

    private float FillExtent = 0.1f;
    private MyTimer OnceFillTimer = new MyTimer(.06f);
    //泳池加水
    float lowPoint = -4.6f;
    float highPoint = .5f;
    bool filled = false;


    private void Awake()
    {
        Plug.SetActive(!WaitForPlug);
        if (Water) 
            Water.localPosition = new Vector3(Water.localPosition.x, lowPoint, Water.localPosition.z);
       
    }

    private void Update()
    {
        if (!WaitForPlug && !filled && GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            OnceFillTimer.OnUpdate(Time.deltaTime);
        }
    }




    private void OnTriggerEnter(Collider other)
    {
        SimapleLock plug = other.GetComponentInParent<SimapleLock>();
        if (plug && plug.gameObject.name.Equals("Plug"))
        {
            plug.gameObject.SetActive(false);
            plug.curHead.UnlockHead();

            Plug.SetActive(true);

            Messenger.Broadcast(StringMgr.GetWinCondition);
        }



    }


    private void OnParticleCollision(GameObject other)
    {
        if (OnceFillTimer.IsFinish && !filled)
        {
            OnceFillTimer.ReStart();


            Water.localPosition += Vector3.up * FillSpeed * FillExtent;
            if (Water.localPosition.y >= highPoint)
            {
                filled = true;
                Messenger.Broadcast(StringMgr.GetWinCondition);
            }
        }
    }


}
