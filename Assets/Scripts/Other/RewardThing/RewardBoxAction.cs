using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class RewardBoxAction : MonoBehaviour
{
    public Transform InKey;

    [Header("GiftShow")]
    public Transform HintPoint;
    public Sprite HintSprite;


    private Animator anim;
    private bool HaveOpen = false;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        InKey.gameObject.SetActive(false);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (HaveOpen)
        {
            return;
        }

        SimapleLock key = other.GetComponentInParent<SimapleLock>();
        if (key)
        {
            key.curHead.UnlockHead();
            key.gameObject.SetActive(false);

            OpenBox();
            HaveOpen = true;
        }

    }


    private void OpenBox()
    {
        InKey.gameObject.SetActive(true);
        anim.SetTrigger("OpenBox");

        var bonusType = BonusLevelMgr.Instance.RandomBonus();
        var sprite = bonusType == BonusType.LuckySkin ? GameControl.Instance.BonusSkin.Icon : HintSprite;
        Messenger.Broadcast(StringMgr.ShowGiftImage, HintPoint, sprite);

        HintPoint.localPosition = Vector3.up * 3;
        HintPoint.DOLocalMoveY(HintPoint.localPosition.y + 3, 1f)
            .OnComplete(() => Messenger.Broadcast(StringMgr.OpenRewardBoxOver));
        
    }










}
