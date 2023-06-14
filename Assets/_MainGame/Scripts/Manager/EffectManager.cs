using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance;
    public static EffectManager Instance { get { return instance; } }

    public enum EffectType
    {
        Splash,
        CubeBreak
    }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    public void SpawnEffect(PoolManager.NameObject nameObjectEffect,Vector3 pos)
    {
        GameObject effectGO = PoolManager.Instance.GetObject(nameObjectEffect) as GameObject;
        effectGO.transform.position = pos;
        effectGO.SetActive(true);
    }

    public void SpawnEffect(PoolManager.NameObject nameObjectEffect, Vector3 pos,Color color)
    {
        GameObject effectGO = PoolManager.Instance.GetObject(nameObjectEffect) as GameObject;
        effectGO.transform.position = pos;
        effectGO.SetActive(true);
        effectGO.GetComponent<IColorable>().SetColor(color);
    }
}
