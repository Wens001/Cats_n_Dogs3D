using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerResoult : MonoBehaviour
{
    public TriggerResult triggerResult;
    public ParticleSystem[] VFXs;
    public Transform TargetPoint;
    bool haveTrigger = false;

    private List<GameObject> objects = new List<GameObject>();
    private void Awake()
    {
        if (triggerResult != TriggerResult.HumanFood)
        {
            var render = GetComponent<Renderer>();
            if (render) render.enabled = false;
        }

        Messenger.AddListener<int>(StringMgr.LevelInit, OnLevelInit);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<int>(StringMgr.LevelInit, OnLevelInit);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (GameControl.Instance.GameProcess != GameProcess.InGame || haveTrigger || objects.Contains(other.gameObject))
        {
            return;
        }

        switch (triggerResult)
        {
            case TriggerResult.TriggerWin:
                //热气球（充气移动
                Balloon balloon = other.GetComponentInParent<Balloon>();
                if (balloon)
                {
                    balloon.OnArrivedPoint();

                    haveTrigger = true;
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                    return;
                }
                     
                //热气球(升起跑酷
                BalloonSimapleUp balloonUp = other.GetComponentInParent<BalloonSimapleUp>();
                if (balloonUp)
                {
                    balloonUp.rigi.isKinematic = true;

                    haveTrigger = true;
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                    return;
                }

                //滑索
                Strop strop = other.GetComponentInParent<Strop>();
                if (strop)
                {
                    strop.otherHead.GetComponent<SimapleMove>().enabled = false;
                    strop.curHead.TryUnlockHead();
                    strop.enabled = false;

                    haveTrigger = true;
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                    AudioPlayControl.Instance.PauseAllClip();
                    return;
                }

                //猫狗
                if (other.GetComponent<TouchMove>())
                {
                    haveTrigger = true;
                    foreach (var vfx in VFXs)
                    {
                        vfx.Play();
                    }
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                    return;
                }
                break;

            case TriggerResult.DelayWin:
                if (other.GetComponent<TouchMove>())
                {
                    haveTrigger = true;
                    foreach (var vfx in VFXs)
                    {
                        vfx.Play();
                    }
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                    return;
                }
                break;

            //糖果
            case TriggerResult.TriggerCandyWin:
                if (other.name.ToLower().Contains("candy"))
                {
                    objects.Add(other.gameObject);

                    Messenger.Broadcast(StringMgr.GetWinCondition);
                }
                break;

            //稻草人关卡
            case TriggerResult.TriggerBarrowWin:
                BarrowControl barrow = other.GetComponentInParent<BarrowControl>();
                if (barrow)
                {
                    barrow.MoveScarecrows(transform);
                    Messenger.Broadcast(StringMgr.GetWinCondition);

                    haveTrigger = true;
                    gameObject.SetActive(false);
                }
                break;

            //船
            case TriggerResult.TriggerShipWin:
                Ship ship = other.GetComponentInParent<Ship>();
                if (ship)
                {
                    haveTrigger = true;
                    ship.rigi.isKinematic = true;

                    Messenger.Broadcast(StringMgr.BouthDeathLock);
                    Messenger.Broadcast(StringMgr.ShipArrive);
                    return;
                }
                break;

            //足球射门(或者触碰到“SlingShotBall”)
            case TriggerResult.TriggerFootballWin:
                if (other.GetComponentInParent<Ball>() || other.GetComponentInParent<SlingShotBall>())
                {
                    haveTrigger = true;

                    foreach (var vfx in VFXs)
                    {
                        vfx.Play();
                    }
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                }
                break;

            //校车等小孩
            case TriggerResult.TriggerBoyWin:
                BoyAction boy = other.GetComponentInParent<BoyAction>();
                if (boy)
                {
                    haveTrigger = true;

                    if (TargetPoint)
                    {
                        boy.GetComponentInChildren<Animator>().SetBool("Walk", true);
                        boy.GetComponent<Collider>().enabled = false;
                        boy.GetComponent<Rigidbody>().isKinematic = true;
                        boy.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(TargetPoint.position - boy.transform.position, Vector3.up));
                        //boy.transform.LookAt(TargetPoint);


                        boy.transform.DOMove(TargetPoint.position, 1f)
                            .OnComplete(()=> {
                                boy.transform.rotation = TargetPoint.rotation;
                                boy.GetComponentInChildren<Animator>().SetBool("Walk", false);
                                Messenger.Broadcast(StringMgr.GetWinCondition);

                            });
                    }
                    else
                    {
                        boy.GetComponentInChildren<Animator>().SetBool("Walk", false);
                        Messenger.Broadcast(StringMgr.GetWinCondition);
                    }
                }
                break;

            //触碰失败
            case TriggerResult.TriggerFail:
                if (other.GetComponentInParent<TouchMove>())
                {
                    GameControl.Instance.GameFail();
                }
                break;

            case TriggerResult.HumanFood:

                BoyAction boy1 = other.GetComponentInParent<BoyAction>();
                if (boy1)
                {
                    enabled = false;
                    boy1.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.position - boy1.transform.position, Vector3.up));
                    Messenger.Broadcast(StringMgr.BouthDeathLock);
                    boy1.GetWinHappy();
                }
                else if (other.GetComponentInParent<TouchMove>())
                {
                    AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.CatchClip);
                    gameObject.SetActive(false);
                    //GameControl.Instance.GameFail();

                    boy1 = FindObjectOfType<BoyAction>();
                    if (boy1)
                    {
                        boy1.SadAndGameFail();
                    }
                }
                break;

                //目标碰到特定物体时，执行
            case TriggerResult.TriggerWinThingWin:
                if (other.CompareTag("WinThing") && !objects.Contains(other.gameObject))
                {
                    objects.Add(other.gameObject);
                    Messenger.Broadcast(StringMgr.GetWinCondition);
                }

                break;


            default:
                break;
        }

    }


    private void OnLevelInit(int levelIndex)
    {
        haveTrigger = false;
        gameObject.SetActive(true);
    }    

    private IEnumerator DelayWin()
    {
        yield return new WaitForSeconds(.2f);
        Messenger.Broadcast(StringMgr.GetWinCondition);
    }


}
