using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShakeManager : MonoBehaviour
{
    private static ShakeManager instance;
    public static ShakeManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    public bool isShakeCamera;

    public void ShakeCamera(float _ratio = 1.0f)
    {
        if (isShakeCamera) return;

        StartCoroutine(C_ShakeCamera(_ratio));
    }

    private IEnumerator C_ShakeCamera(float _ratio = 1.0f)
    {
        int strength = (int)(2.0f * _ratio);
        int vatio = (int)(20.0f * _ratio);
        ReferenceManager.Instance.cameraMain.DOShakeRotation(.6f, strength, vatio).SetUpdate(true);
        isShakeCamera = true;
        yield return new WaitForSeconds(0.6f);
        isShakeCamera = false;
    }
}
