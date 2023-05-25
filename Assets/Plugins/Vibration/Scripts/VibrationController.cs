using System;
using UnityEngine;
using MoreMountains.NiceVibrations;


public class VibrationController : MonoBehaviour
{
    public static VibrationController Instance { get; private set; }

    public bool vibrate { get; set; }

    private void Awake()
    {
        Instance = this;

        MMVibrationManager.iOSInitializeHaptics();
    }

    private void OnDestroy()
    {
		MMVibrationManager.iOSReleaseHaptics();
    }

    public void Impact()
    {
        if (!vibrate) { return; }
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }
}
