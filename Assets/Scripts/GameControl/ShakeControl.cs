using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class ShakeControl : Singleton<ShakeControl>
{
    public void ShotLightShake()
    {
        if (GameSetting.ShakeSwitch)
        {
            MMVibrationManager.Haptic(HapticTypes.Selection);
        }
    }


    public void LightShake()
    {
        if (GameSetting.ShakeSwitch)
        {
            MMVibrationManager.Vibrate();
        }
    }

}
