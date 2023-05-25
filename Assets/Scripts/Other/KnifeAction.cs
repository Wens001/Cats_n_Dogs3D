using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 单纯控制大刀动作
/// </summary>
public class KnifeAction : MonoBehaviour
{
    public bool simpleMove = false;
    [Header("直线移动")]
    public float TargetDis = 20;

    [Header("扇形挥动")]
    public float TargetEuler = 30;
    public float DurTime = 1;


    private void Start()
    {
        if (simpleMove)
        {
            transform.GetChild(0).DOMove(transform.localPosition + transform.right * TargetDis, DurTime).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            transform.GetChild(0).DOLocalRotate(Vector3.forward * TargetEuler, DurTime).SetLoops(-1, LoopType.Yoyo);
        }


    }








}
