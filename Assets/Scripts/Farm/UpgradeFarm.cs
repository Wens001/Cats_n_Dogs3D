using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFarm : MonoBehaviour
{
    private void OnMouseDown()
    {
        if(FarmMgr.CanTouch && !FarmMgr.isGuide)
            Messenger.Broadcast(ConfigFarm.ShowUpgradePanel);
    }
}
