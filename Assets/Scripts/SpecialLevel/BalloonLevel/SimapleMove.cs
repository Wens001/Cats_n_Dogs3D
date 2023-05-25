using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// 赋予物体在 XOZ 平面上的移动
/// </summary>
public class SimapleMove : MonoBehaviour
{

    /// <summary>
    /// 以对猫狗施加的力为基础
    /// </summary>
    [HideInInspector] public float ForceRate
    {
        get => forceRate;
        set
        {
            forceRate = Mathf.Clamp(value, 0, value);
        }
    }
    [HideInInspector] public SimapleMoveType moveWay = SimapleMoveType.None;
    [HideInInspector] public CatOrDog controlType = CatOrDog.Cat;
    [HideInInspector] public bool FollowRotation;

    private float forceRate = 1;
    private Rigidbody rigi;
    private float deltaDis = 50f;
    private bool isHold = false;
    private GamePlayPanel playPanel;

    Vector3 screenDir;
    Vector3 moveDir;
    Vector3[] lastPoints = new Vector3[2];

    private void Awake()
    {
        rigi = transform.GetComponent<Rigidbody>();
        playPanel = FindObjectOfType<GamePlayPanel>();
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
        if (isHold)
        {
            rigi.velocity = Vector3.ClampMagnitude(rigi.velocity, GameSetting.MaxSpd);
            if (FollowRotation)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rigi.velocity, Vector3.up));
            }
        }
        else
        {
            rigi.velocity = Vector3.Lerp(rigi.velocity, Vector3.zero, Time.deltaTime * 30);
            rigi.angularVelocity = Vector3.Lerp(rigi.angularVelocity, Vector3.zero, Time.deltaTime * 30);
        }

    }




    private void CheckMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isHold = true;
            lastPoints[0] = Input.mousePosition;

            //显示摇杆
            playPanel.OnMouseClickDown(controlType);
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition != lastPoints[0] && Vector3.Distance(Input.mousePosition, lastPoints[0]) >= deltaDis)
            {
                screenDir = Input.mousePosition - lastPoints[0];
                moveDir = CaluteMoveDirection(screenDir).normalized;

                rigi.AddForce(moveDir * GameSetting._force * ForceRate);
            }


            playPanel.OnMouseHold(controlType);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isHold = false;

            playPanel.OnMouseClickUp(controlType);
        }
    }


    private Vector3 CaluteMoveDirection(Vector3 screenDirection)
    {
        switch (moveWay)
        {
            case SimapleMoveType.MoveX:
                return Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * screenDirection.x;

                //break;
            case SimapleMoveType.MoveZ:
                return Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * screenDirection.y;

                //break;


            case SimapleMoveType.None:
            default:

                return (Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * screenDirection.y + Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * screenDirection.x);
                //break;
        }


       
    }

}
