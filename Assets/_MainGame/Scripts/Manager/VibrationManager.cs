using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibrationManager : MonoBehaviour
{
    private static VibrationManager instance;
    public static VibrationManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;

        if (PlayerPrefs.GetInt("f_vibration") == 0)
        {
            OnClick_SetVibration();
            PlayerPrefs.SetInt("f_vibration", 1);
        }

#if UNITY_IOS
      MMNViOS.iOSInitializeHaptics();
#else

#endif
    }

    [Header("Vibration")]
    public bool isVibration;
    public bool canVibarion;
    public Sprite vibrationSpr;
    public Sprite notvibrationSpr;
    public Image vibrationBtn;

    private void Start()
    {
        int v = PlayerPrefs.GetInt("vibration");
        canVibarion = (v == 0) ? false : true;
        SetButtonSprite(canVibarion);
    }

    public void OnClick_SetVibration()
    {
        canVibarion = !canVibarion;
        int v = (canVibarion) ? 1 : 0;
        SetButtonSprite(canVibarion);
        PlayerPrefs.SetInt("vibration", v);
    }

    private void SetButtonSprite(bool enabledVibarion)
    {
        vibrationBtn.sprite = (enabledVibarion) ? vibrationSpr : notvibrationSpr;
    }

    public void Vibration()
    {
        if (canVibarion == false) return;
        if (isVibration) return;

        StartCoroutine(C_Vibarion());
    }

    private IEnumerator C_Vibarion()
    {
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);

        isVibration = true;
        yield return new WaitForSecondsRealtime(0.25f);
        isVibration = false;
    }
}
