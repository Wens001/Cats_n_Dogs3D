using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SnowBrush))]
public class SnowBall : MonoBehaviour
{
    public float BallScale = 3;
    public float targetScale = 5;
    [Range(.01f, 1)]
    public float ChangeScale = .1f;


    private SnowBrush snowBrush;
    private float InitSize;
    private Rigidbody rigi;
    private MyTimer ChangeTimer = new MyTimer(.1f);
    private bool GetWin = false;

    private void Awake()
    {
        transform.localScale = Vector3.one * BallScale;

        rigi = transform.GetComponent<Rigidbody>();
        snowBrush = transform.GetComponent<SnowBrush>();
        InitSize = snowBrush.BrushSize.x;

    }




    private void Update()
    {
        var magnitude = Vector3.ProjectOnPlane(rigi.velocity, Vector3.up).magnitude;
        if (magnitude > 3f)
        {
            ChangeTimer.OnUpdate(Time.deltaTime);
            if (!GetWin && ChangeTimer.IsFinish)
            {
                ChangeTimer.ReStart();

                BallScale += ChangeScale;
                transform.localScale = Vector3.one * BallScale;
                snowBrush.BrushSize = Vector2.one * (InitSize + BallScale * 2);

                if (BallScale >= targetScale)
                {
                    GetWin = true;
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                    rigi.isKinematic = true;
                }
            }
        }

    }



}
