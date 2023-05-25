using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Nest : MonoBehaviour
{
    public AnimalType suitAnimal;
    public Transform TargetPoint;

    public AudioClip AnimalAudio;

    private void Awake()
    {
        transform.GetComponent<MeshRenderer>().enabled = false;
        Messenger.AddListener<int>(StringMgr.LevelInit, OnGameInit);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<int>(StringMgr.LevelInit, OnGameInit);
    }




    List<AnimalAction> animals = new List<AnimalAction>();
    private void OnTriggerEnter(Collider other)
    {
        //动物回窝
        AnimalAction animal = other.GetComponent<AnimalAction>();
        if (animal)
        {
            if (animal.selfType == suitAnimal)
            {
                if (!animals.Contains(animal))
                {
                    animals.Add(animal);
                    Messenger.Broadcast(StringMgr.GetWinCondition);

                    if (TargetPoint)
                    {
                        animal.transform.DOMove(TargetPoint.position, .8f)
                        .OnComplete(() => { animal.transform.LookAt(Vector3.right); });
                        
                        //动物叫声
                        switch (suitAnimal)
                        {
                            case AnimalType.Duck:
                                AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.DuckShoutClip);
                                break;
                            case AnimalType.Chick:
                                AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.ChickShoutClip);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                GameControl.Instance.GameFail();
            }

            return;
        }

        //整体动物回窝
        AnimalWholeMove wholeAnimal = other.GetComponent<AnimalWholeMove>();
        if (wholeAnimal)
        {
            if (wholeAnimal.selfType == suitAnimal)
            {
                Messenger.Broadcast(StringMgr.GetWinCondition);
                if (TargetPoint)
                {
                    wholeAnimal.transform.DOMove(TargetPoint.position, .8f)
                    .OnComplete(() => { wholeAnimal.transform.LookAt(Vector3.right); });

                    //动物叫声
                    switch (suitAnimal)
                    {
                        case AnimalType.Duck:
                            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.DuckShoutClip);
                            break;
                        case AnimalType.Chick:
                            AudioPlayControl.Instance.PlayClip(AudioPlayControl.Instance.ChickShoutClip);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                GameControl.Instance.GameFail();
            }

        }


        //运送南瓜
        PlantGrow plant = other.GetComponentInParent<PlantGrow>();
        if (plant)
        {
            Messenger.Broadcast(StringMgr.GetWinCondition);

        }


    }


    private void OnGameInit(int levelIndex)
    {
        animals.Clear();

    }


}
