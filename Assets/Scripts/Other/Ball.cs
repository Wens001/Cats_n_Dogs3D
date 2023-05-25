using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Sprite hintImage;

    int FootBallGuidLevel { 
        get => PlayerPrefs.GetInt("FootBallGuidLevel", -1); 
        set { PlayerPrefs.SetInt("FootBallGuidLevel", value); } 
    }
    Rigidbody rigi;
    [HideInInspector] public bool arrivedTarget = false;



    private void Awake()
    {
        rigi = transform.GetComponent<Rigidbody>();

        Messenger.AddListener<int>(StringMgr.LevelInit, BallInit);
        Messenger.AddListener(StringMgr.GameStart, BroadcastHint);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<int>(StringMgr.LevelInit, BallInit);
        Messenger.RemoveListener(StringMgr.GameStart, BroadcastHint);

    }

    bool haveHide = false;
    private void OnTriggerEnter(Collider other)
    {
        //隐藏提示图标
        if (!haveHide && other.GetComponentInParent<TouchMove>())
        {
            haveHide = true;
            Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Default);
        }
    }


    #region CallBack

    private void BallInit(int LevelIndex)
    {
        gameObject.SetActive(true);
        transform.rotation.Normalize();
        rigi.constraints = RigidbodyConstraints.None;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void BroadcastHint()
    {
        Messenger.Broadcast(StringMgr.HintBroadcast, CatOrDog.Default, hintImage);

        //第一次或者重复玩显示引导
        //if (FootBallGuidLevel == -1 || GameControl.Instance.CurLevel == FootBallGuidLevel)
        //{
        //    FootBallGuidLevel = GameControl.Instance.CurLevel;
        //    UIPanelManager.Instance.PushPanel(UIPanelType.DoubleHandGuidPanel);
        //}
    }

    #endregion

}
