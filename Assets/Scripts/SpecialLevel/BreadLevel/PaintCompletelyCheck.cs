using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PaintIn3D;
using PaintIn3D.Examples;


/// <summary>
/// 涂果酱关卡
/// </summary>
[RequireComponent(typeof(P3dChangeCounter))]
[RequireComponent(typeof(P3dMaterialCloner))]
public class PaintCompletelyCheck : MonoBehaviour
{
    public bool Inverse;
    public Sprite HintSprite;

    private List<P3dChangeCounter> SelfChange = new List<P3dChangeCounter>();
    private float ratio;
    private bool PaintCompletely = false;
    private Material material;

    private float WinRate = .85f;

    private void Awake()
    {
        SelfChange.Add(GetComponent<P3dChangeCounter>());
        EnableHint();

        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);

    }


    private void Update()
    {
        if (PaintCompletely)
        {
            return;
        }

        ratio = P3dChangeCounter.GetRatio(SelfChange);
        if (Inverse)
        {
            ratio = 1 - ratio;
        }

        //涂抹完成
        if (ratio > WinRate)
        {
            PaintCompletely = true;

            //完整显示
            GetComponent<Renderer>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);

            CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);
            StartCoroutine(DelayWin());
        }
    }

    IEnumerator DelayWin()
    {
        yield return new WaitForSeconds(.8f);
        Messenger.Broadcast(StringMgr.GetWinCondition);
    }


    //区域提示
    private void EnableHint()
    {
        material = transform.GetComponent<Renderer>().material;


        float num = .8f;
        DOTween.To(() => num, x => num = x, .3f, 1f)
            .OnUpdate(() =>
            {
                Color color = material.color;
                color.a = num;
                material.color = color;
            })
            .SetLoops(-1, LoopType.Yoyo);


    }


    //提示
    private void OnGameStart()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Default, HintSprite);
        StartCoroutine(DelayHideHint());

    }

    private IEnumerator DelayHideHint()
    {
        yield return new WaitForSeconds(2f);
        Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Default);

    }

}
