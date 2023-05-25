using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BalloonSimapleUp : MonoBehaviour
{
    public GameObject BrokenBalloonPrefab;

    private float UpSpeed = 35;
    private float HorizontalSpeed = 6;
    private float ForceRate;

    [HideInInspector] public Rigidbody rigi;

    //射线检测
    Transform RayOri;

    private void Awake()
    {
        rigi = transform.GetComponent<Rigidbody>();
        rigi.isKinematic = true;

        Messenger.AddListener(StringMgr.GameStart, OnGameStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameStart, OnGameStart);
    }



    private void Start()
    {
        Messenger.Broadcast(StringMgr.BouthDeathLock);

        RayOri = GetComponent<HingeJoint>().connectedBody.transform;

    }

    private void Update()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            CheckMouse();
        }
    }



    private void LateUpdate()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            rigi.velocity = new Vector3(rigi.velocity.x, CheckUpObstacle()? 0 : UpSpeed, rigi.velocity.z);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Thorn"))
        {
            if (BrokenBalloonPrefab)
            {
                Instantiate(BrokenBalloonPrefab, transform.position, transform.rotation).SetActive(true) ;
            }

            GetComponent<HingeJoint>().connectedBody = null;
            gameObject.SetActive(false);

            int num = 0;
            DOTween.To(() => num, x => num = x, 2, 2f)
                .OnComplete(() => {
                    GameControl.Instance.GameFail();
                });
        }
    }










    private bool CheckUpObstacle()
    {
        Debug.DrawRay(RayOri.transform.position, Vector3.up * 7F, Color.red, 1f);
        if (Physics.Raycast(RayOri.transform.position, Vector3.up, 7f, LayerMask.GetMask("Obstacle")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    #region 左右横移

    private float deltaDis = 50f;
    Vector3 lastPoints;
    Vector3 screenDir;
    Vector3 moveDir;
    private void CheckMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPoints = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition != lastPoints && Vector3.Distance(Input.mousePosition, lastPoints) >= deltaDis)
            {
                screenDir = Input.mousePosition - lastPoints;
                moveDir = CaluteMoveDirection(screenDir).normalized;

                lastPoints = Input.mousePosition;
            }
            //
            //else
            //{
            //    return;
            //}

            //Debug.DrawRay(RayOri.transform.position, moveDir * 4.5F, Color.red, 1f);
            if (Physics.Raycast(RayOri.transform.position, moveDir, 4.5F, LayerMask.GetMask("Obstacle")))
            {
                ForceRate = 2;
            }
            else
            {
                ForceRate = HorizontalSpeed;
            }
            rigi.AddForce(moveDir * GameSetting._force * ForceRate);
        }
    }

    private Vector3 CaluteMoveDirection(Vector3 screenDirection)
    {
        return (/*Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * screenDirection.y + */Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * screenDirection.x);
    }


    #endregion



    private void OnGameStart()
    {
        rigi.isKinematic = false;
    }




}
