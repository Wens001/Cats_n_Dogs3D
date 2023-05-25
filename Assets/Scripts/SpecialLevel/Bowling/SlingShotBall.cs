using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotBall : MonoBehaviour
{
    private float MaxMoveRadius = 3;
    private float MoveRate = 0.01f;
    private bool beShoot = false;
    private bool willDestroy = false;
    private bool velocityCheck = false;

    public GameObject dirGuid;
    public bool ViewFollow = false;

    private Vector3 lastScreenPoint;
    private Vector3 lastPosition;
    Rigidbody rigi;
    private void Awake()
    {
        rigi = transform.GetComponent<Rigidbody>();
        rigi.isKinematic = true;

        lastPosition = transform.position;
    }

    Ray ray;
    bool beHold;

    private void Update()
    {
        if (GameControl.Instance.GameProcess == GameProcess.InGame)
        {
            OnMouseClickDown();
            OnMouseHold();
            OnMouseClickUp();
        }
    }


    private void OnMouseClickDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //旧关卡时，需检测点到球上
            //if (SDKManager.Instance.OldLevel)
            //{
            //    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (!Physics.Raycast(ray, 200f, LayerMask.GetMask("Ball")))
            //    {
            //        return;
            //    }
            //}

            if (!beShoot)
            {
                lastScreenPoint = Input.mousePosition;
                beHold = true;
                if (dirGuid)
                {
                    dirGuid.SetActive(true);
                }

                Messenger.Broadcast(StringMgr.HideHint, CatOrDog.Default);
            }
        }
    }


    Vector3 moveDir;
    private void OnMouseHold()
    {
        if (beHold && Input.GetMouseButton(0))
        {
            if (beShoot || Input.mousePosition.y > lastScreenPoint.y)
            {
                return;
            }

            moveDir = CaluteMoveDirection(Input.mousePosition - lastScreenPoint) * MoveRate;
            transform.position = Vector3.MoveTowards(transform.position, lastPosition + Vector3.ClampMagnitude(moveDir, MaxMoveRadius), 5 * Time.deltaTime);

            if (dirGuid && moveDir != Vector3.zero)
            {
                dirGuid.transform.position = transform.position;
                dirGuid.transform.rotation = Quaternion.LookRotation(-moveDir);
            }
        }
    }

    private void OnMouseClickUp()
    {
        if (beHold && Input.GetMouseButtonUp(0))
        {
            Vector3 dir = (lastPosition - transform.position);
            if (beShoot || dir == Vector3.zero)
            {
                return;
            }

            beShoot = true;
            beHold = false;
            rigi.isKinematic = false;

            //旧关卡时，最小力很小
            //dir = SDKManager.Instance.OldLevel ? 
            //    (dir.magnitude < .5f ? dir.normalized * .5f : dir)
            //    : (dir.magnitude < 1.5f ? dir.normalized * 2f : dir);

            dir = dir.magnitude < 1.5f ? dir.normalized * 2f : dir;

            //汽车弹射
            if (ViewFollow)
            {
                Camera.main.GetComponent<Cinemachine.CinemachineBrain>().m_DefaultBlend.m_Style = Cinemachine.CinemachineBlendDefinition.Style.Cut;
                lastCamera = CameraControl.Instance.GetCurCamera();
                CameraControl.Instance.ChangeCamera(CameraType.CM_TopView);
                CameraControl.Instance.LookAtSomething(transform);

                rigi.AddForce(dir * 1500);
            }
            else
                rigi.AddForce(dir * 500);

            //震动
            ShakeControl.Instance.LightShake();

            if (dirGuid)
            {
                dirGuid.transform.LookAt(Vector3.left);
                dirGuid.SetActive(false);
            }
        }
    }

    private CameraType lastCamera;
    public void CheckSelf()
    {
        if (beShoot && !velocityCheck && rigi.velocity.magnitude > .3f)
        {
            velocityCheck = true;
            return;
        }

        //停止移动，或者掉落
        if (velocityCheck && !willDestroy && (rigi.velocity.magnitude <= .3f || rigi.velocity.y < -5f))
        {
            willDestroy = true;
            //gameObject.SetActive(false);
            Messenger.Broadcast(StringMgr.ShootAgain);

            if (ViewFollow)
            {
                CameraControl.Instance.ChangeCamera(lastCamera);
            }
        }
    }


    /// <summary>
    /// 将在屏幕上的移动，投影到相机对应的前方和右方
    /// </summary>
    /// <param name="screenDirection"></param>
    /// <returns></returns>
    private Vector3 CaluteMoveDirection(Vector3 screenDirection)
    {
        return (Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up)) * screenDirection.y + Camera.main.transform.right * screenDirection.x;
    }
}
