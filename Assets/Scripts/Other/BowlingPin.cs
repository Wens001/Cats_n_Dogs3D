using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    private Vector3 initPosition;
    private Collider _collider;
    private Rigidbody rigi;
    bool notCollide = true;
    bool notFall = true;

    private void Awake()
    {
        initPosition = transform.position;
        _collider = transform.GetComponentInChildren<Collider>();
        rigi = transform.GetComponent<Rigidbody>();

        Messenger.AddListener<int>(StringMgr.LevelInit, OnLevelInit);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<int>(StringMgr.LevelInit, OnLevelInit);
    }


    private void Update()
    {
        if (notCollide && (Vector3.Angle(transform.up, Vector3.up) > 50 || transform.position.y < -1))
        {
            notCollide = false;

            Messenger.Broadcast(StringMgr.GetWinCondition);
        }

        if (!notCollide && notFall && rigi.velocity.magnitude < 2)
        {
            notFall = false;
            _collider.isTrigger = true;
            this.enabled = false;
        }

    }

    private void OnLevelInit(int levelIndex)
    {
        notFall = notCollide = true;
        _collider.isTrigger = false;
        rigi.velocity = Vector3.zero;
        rigi.angularVelocity = Vector3.zero;
        transform.position = initPosition;
        transform.rotation = Quaternion.identity;
        this.enabled = true;

    }



}
