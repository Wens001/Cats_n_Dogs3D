using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinFlyPanel : MonoBehaviour
{
    public Transform CoinsTrans;

    private bool flyOver = false;

    private void Awake()
    {
        gameObject.SetActive(false);

        Messenger.AddListener<Vector3, Text, int>(StringMgr.FlyCoins, OnFlyCoins);
        Messenger.AddListener<Vector3, Text, int>(StringMgr.FlyOtherCoins, OnOtherCoinFly);
        Messenger.AddListener<Vector3, Text, int>(StringMgr.FlyFarmCoins, OnFarmCoinFly);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<Vector3, Text, int>(StringMgr.FlyCoins, OnFlyCoins);
        Messenger.RemoveListener<Vector3, Text, int>(StringMgr.FlyOtherCoins, OnOtherCoinFly);
        Messenger.RemoveListener<Vector3, Text, int>(StringMgr.FlyFarmCoins, OnFarmCoinFly);
    }



    private void OnFlyCoins(Vector3 oriPoint, Text MoneyText, int coinNum)
    {
        CoinsInit(CoinsTrans);
        flyOver = false;
        gameObject.SetActive(true);
        CoinsTrans.position = oriPoint;

        for (int i = 0; i < CoinsTrans.childCount; i++)
        {
            var child = CoinsTrans.GetChild(i);
            //散开
            child.DOLocalMove(child.localPosition * 3, .5f)
                .OnComplete(() =>
                {
                    //移到金币显示区域
                    child.DOMove(MoneyText.transform.parent.position, 1f)
                    .OnComplete(() =>
                    {
                        if (!flyOver)
                        {
                            flyOver = true;
                            CoinsTrans.gameObject.SetActive(false);

                            var theCoinCount = GameSetting.CoinCount;
                            GameSetting.CoinCount += coinNum;
                            DOTween.To(() => theCoinCount, x => theCoinCount = x, GameSetting.CoinCount, .5f)
                            .OnUpdate(() => {
                                MoneyText.text = theCoinCount.ToString();
                            })
                            .OnComplete(() => {
                                MoneyText.text = theCoinCount.ToString();

                                FlyOver();
                            });
                        }
                    });
                });
        }

    }

    private void OnOtherCoinFly(Vector3 oriPoint, Text MoneyText, int coinNum)
    {
        var tempCoinsTrans = Instantiate(CoinsTrans.gameObject, CoinsTrans.parent).transform;
        CoinsInit(tempCoinsTrans);
        tempCoinsTrans.position = oriPoint;
        gameObject.SetActive(true);
        bool haveFly = false;
        for (int i = 0; i < tempCoinsTrans.childCount; i++)
        {
            var child = tempCoinsTrans.GetChild(i);
            //散开
            child.DOLocalMove(child.localPosition * 3, .5f)
                .OnComplete(() =>
                {
                    //移到金币显示区域
                    child.DOMove(MoneyText.transform.parent.position, 1f)
                    .OnComplete(() => {
                        if (haveFly)
                        {
                            return;
                        }
                        haveFly = true;

                        tempCoinsTrans.gameObject.SetActive(false);

                        var theCoinCount = GameSetting.CoinCount;
                        GameSetting.CoinCount += coinNum;
                        DOTween.To(() => theCoinCount, x => theCoinCount = x, GameSetting.CoinCount, .5f)
                        .OnUpdate(() => {
                            MoneyText.text = theCoinCount.ToString();
                        })
                        .OnComplete(() => {
                            MoneyText.text = theCoinCount.ToString();                           
                        });

                    });
                });
        }
    }
    private void OnFarmCoinFly(Vector3 oriPoint, Text MoneyText, int coinNum)
    {
        var tempCoinsTrans = Instantiate(CoinsTrans.gameObject, CoinsTrans.parent).transform;
        CoinsInit(tempCoinsTrans);
        tempCoinsTrans.position = oriPoint;
        gameObject.SetActive(true);
        bool haveFly = false;
        Vector3 _pos = Camera.main.WorldToScreenPoint(MoneyText.transform.parent.position);
        //Camera.main.ViewportToWorldPoint(MoneyText.transform.parent.position);
        for (int i = 0; i < tempCoinsTrans.childCount; i++)
        {
            var child = tempCoinsTrans.GetChild(i);
            //散开
            child.DOLocalMove(child.localPosition * 3, .5f)
                .OnComplete(() =>
                {
                    //移到金币显示区域
                    child.DOMove(_pos, 1f)
                    .OnComplete(() =>
                    {
                        if (haveFly)
                        {
                            return;
                        }
                        haveFly = true;

                        tempCoinsTrans.gameObject.SetActive(false);

                        var theCoinCount = GameSetting.CoinCount;
                        GameSetting.CoinCount += coinNum;
                        DOTween.To(() => theCoinCount, x => theCoinCount = x, GameSetting.CoinCount, .5f)
                        .OnUpdate(() =>
                        {
                            MoneyText.text = theCoinCount.ToString();
                        })
                        .OnComplete(() =>
                        {
                            MoneyText.text = theCoinCount.ToString();
                            FlyOver();
                        });

                    });
                });
        }
    }


    private void FlyOver()
    {
        gameObject.SetActive(false);
        //GameControl.Instance.LoadNextLevel();
        Messenger.Broadcast(StringMgr.FlyCoinsOver);
    }



    private void CoinsInit(Transform _coinsTrans)
    {
        for (int i = 0; i < _coinsTrans.childCount; i++)
        {
            var child = _coinsTrans.GetChild(i);
            child.localPosition = new Vector2(Random.Range(-50f, 50), Random.Range(-50f, 50));
        }

        _coinsTrans.gameObject.SetActive(true);
    }


}
