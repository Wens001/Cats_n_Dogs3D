using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchesManager : MonoBehaviour
{
    [SerializeField] private float deltaDis = 50f;
    [HideInInspector] public bool bouthDeathLock = false;
    bool InEditor;

    TouchMove CatHead;
    TouchMove DogHead;
    [HideInInspector] private GamePlayPanel gamePlayPanel;
    public GamePlayPanel GamePlayPanelM { 
        get => gamePlayPanel;
        set => gamePlayPanel = value;
    }


    private void Awake()
    {
#if UNITY_EDITOR
        InEditor = true;
#else
        InEditor = false;
#endif
    }

    private void OnEnable()
    {
        Messenger.AddListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, OnCatNDogInit);
        Messenger.AddListener(StringMgr.BouthDeathLock, OnBouthDeathLock);
        Messenger.AddListener<int>(StringMgr.LevelInit, OnLevelInit);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<TouchMove, TouchMove>(StringMgr.CatNDogInit, OnCatNDogInit);
        Messenger.RemoveListener(StringMgr.BouthDeathLock, OnBouthDeathLock);
        Messenger.RemoveListener<int>(StringMgr.LevelInit, OnLevelInit);
    }


    private void Update()
    {
        if (CatHead == null || DogHead == null)
        {
            return;
        }

        if (GameControl.Instance.GameProcess != GameProcess.InGame || bouthDeathLock)
        {
            CatHead.StopMove();
            DogHead.StopMove();
            return;
        }

        if (InEditor)
        {
            CheckMouse();
        }
        else
        {
            if (Input.touchCount == 1)
            {
                CheckTouch(Input.GetTouch(0));
            }
            else if(Input.touchCount > 1)
            {
                CheckTouch(Input.GetTouch(0));
                CheckTouch(Input.GetTouch(1));
            }
            else
            {
                CatHead.StopMove();
                DogHead.StopMove();
            }
        }
    }

    private void FixedUpdate()
    {
        if (GamePlayPanelM == null)
        {
            return;
        }
        //提示气泡跟随
        if (CatHead != null && DogHead != null)
        {
            GamePlayPanelM.CatOrDogWantFollow(CatHead, DogHead);
        }
    }


    #region CheckMove检测触屏移动

    Vector3 screenDir;
    Vector3 moveDir;
    Vector3[] lastPoints = new Vector3[2];

    private void CheckTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            if (touch.fingerId > 1)
            {
                return;
            }

            if (touch.position.x < Screen.width * .5f)
            {
                if (DogHead.holdFingerID == -1)
                {
                    GamePlayPanelM.OnTouchEnter(CatOrDog.Dog, touch);
                    DogHead.holdFingerID = touch.fingerId;
                    DogHead.isHold = true;
                    DogHead.AnimalShout();
                    DogHead.TryUnlockHead();
                }
            }
            else
            {
                if (CatHead.holdFingerID == -1)
                {
                    GamePlayPanelM.OnTouchEnter(CatOrDog.Cat, touch);
                    CatHead.holdFingerID = touch.fingerId;
                    CatHead.isHold = true;
                    CatHead.AnimalShout();
                    CatHead.TryUnlockHead();
                }
            }

            lastPoints[touch.fingerId] = touch.position;
        }


        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            if ((Vector3)touch.position != lastPoints[touch.fingerId] && Vector3.Distance(touch.position, lastPoints[touch.fingerId]) >= deltaDis)
            {
                screenDir = (Vector3)touch.position - lastPoints[touch.fingerId];
                //moveDir = CaluteMoveDirection(screenDir);
                moveDir = CalculateMoveDir.CalculateDirWithCameraXOZ(screenDir);
                moveDir = Vector3.ClampMagnitude(moveDir, 2);
            }

            if (DogHead.holdFingerID == touch.fingerId)
            {
                GamePlayPanelM.OnTouchHold(CatOrDog.Dog, touch);

                DogHead.DoMove(moveDir);

            }
            else if (CatHead.holdFingerID == touch.fingerId)
            {
                GamePlayPanelM.OnTouchHold(CatOrDog.Cat, touch);

                CatHead.DoMove(moveDir);
            }

        }


        if (touch.phase == TouchPhase.Ended)
        {
            if (DogHead.holdFingerID == touch.fingerId)
            {
                GamePlayPanelM.OnTouchEnd(CatOrDog.Dog);
                DogHead.holdFingerID = -1;
                DogHead.isHold = false;
            }
            else if (CatHead.holdFingerID == touch.fingerId)
            {
                GamePlayPanelM.OnTouchEnd(CatOrDog.Cat);
                CatHead.holdFingerID = -1;
                CatHead.isHold = false;

            }

            lastPoints[touch.fingerId] = Vector3.zero;
        }


    }

    private void CheckMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < Screen.width * .5f)
            {
                DogHead.isHold = true;
                DogHead.AnimalShout();
                DogHead.TryUnlockHead();

                GamePlayPanelM.OnMouseClickDown(CatOrDog.Dog);
            }
            else
            {
                CatHead.isHold = true;
                CatHead.AnimalShout();
                CatHead.TryUnlockHead();

                GamePlayPanelM.OnMouseClickDown(CatOrDog.Cat);
            }

            lastPoints[0] = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition != lastPoints[0] && Vector3.Distance(Input.mousePosition, lastPoints[0]) >= deltaDis)
            {
                screenDir = Input.mousePosition - lastPoints[0];

                //moveDir = CaluteMoveDirection(screenDir);
                moveDir = CalculateMoveDir.CalculateDirWithCameraXOZ(screenDir);

                moveDir = Vector3.ClampMagnitude(moveDir, 2);
            }

            if (CatHead.isHold)
            {
                GamePlayPanelM.OnMouseHold(CatOrDog.Cat);

                CatHead.DoMove(moveDir);
            }
            else if (DogHead.isHold)
            {
                GamePlayPanelM.OnMouseHold(CatOrDog.Dog);

                DogHead.DoMove(moveDir);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            CatHead.isHold = false;
            DogHead.isHold = false;

            GamePlayPanelM.OnMouseClickUp(CatOrDog.Dog);
            GamePlayPanelM.OnMouseClickUp(CatOrDog.Cat);
        }
        else
        {
            CatHead.StopMove();
            DogHead.StopMove();
        }

    }

    #endregion


    #region CallBack
    private void OnCatNDogInit(TouchMove cat, TouchMove dog)
    {
        CatHead = cat;
        DogHead = dog;
    }

    private void OnLevelInit(int levelIndex)
    {
        bouthDeathLock = false;

    }

    private void OnBouthDeathLock()
    {
        bouthDeathLock = true;

    }

    #endregion


  
}
