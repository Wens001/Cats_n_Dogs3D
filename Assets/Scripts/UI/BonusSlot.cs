using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusSlot : MonoBehaviour
{
    public Transform CoinTrans;
    public Text CoinText;
    public Image BonusSkinImage;

    public BonusType curSlotType;

    private int coinNum;
    private bool isCoinBonus;

    public void SlotInit(BonusType bonusType)
    {
        curSlotType = bonusType;
        isCoinBonus = bonusType != BonusType.LuckySkin;

        if (isCoinBonus)
        {
            coinNum = int.TryParse(bonusType.ToString().Split('_')[1], out int temp) ? temp : 50;
            CoinText.text = coinNum.ToString();
        }
        else
        {
            BonusSkinImage.sprite = GameControl.Instance.BonusSkin.Icon;
        }

        CoinTrans.gameObject.SetActive(isCoinBonus);
        BonusSkinImage.gameObject.SetActive(!isCoinBonus);
    }


    public void GotTheBonus(Text coinText, bool haveFlyCoin)
    {
        if (isCoinBonus)
        {
            if (haveFlyCoin)
            {
                Messenger.Broadcast(StringMgr.FlyOtherCoins, transform.position, coinText, coinNum);
            }
            else
            {
                Messenger.Broadcast(StringMgr.FlyCoins, transform.position, coinText, coinNum);
            }

            //GameSetting.CoinCount += coinNum;
        }
        else
        {
            SkinManager.Instance.GetNewSkin(GameControl.Instance.BonusSkin);
        }
    }



    
}
