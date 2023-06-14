using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    private static ReferenceManager instance;
    public static ReferenceManager Instance { get { return instance; } }

    public Transform cameraMain;
    public Transform player;

    public Color[] colorArray;
    public Material[] mColorArray;

    public enum TypeColor
    {
        White,
        Blue,
        Orange,
        Green,
        Pink,
        Red,
        Black
    }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    public Color GetColor(TypeColor typeColor)
    {
        return colorArray[(int)typeColor];
    }

    public Color GetColor(int typeColor)
    {
        return colorArray[typeColor];
    }

    public Material GetMaterialColor(TypeColor typeColor)
    {
        return mColorArray[(int)typeColor];
    }

    public Material GetMaterialColor(int typeColor)
    {
        return mColorArray[typeColor];
    }
}
