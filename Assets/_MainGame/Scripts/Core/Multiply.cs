using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Multiply : MonoBehaviour
{
    [Header("Status")]
    public string caculate;
    public int caculateIndex;
    public int multiplyNumber;
    public bool isRandom;

    [Header("References")]
    public TextMeshPro multiplyText;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    public void ResetVariable()
    {
        SetCaculate();

        if (isRandom || multiplyNumber == 0 || multiplyNumber == 1)
        {
            multiplyNumber = GetRandomNumber();
        }

        multiplyText.text = caculate + multiplyNumber;
    }

    public void SetMultiplyNumber(int _multiplyNumber)
    {
        multiplyNumber = _multiplyNumber;
        SetCaculate();
        multiplyText.text = caculate + multiplyNumber;
    }

    private int GetRandomNumber()
    {
        if (caculateIndex == 0)
        {
            return Random.Range(8, 28);
        }
        else if (caculateIndex == 1)
        {
            return Random.Range(2, 15);
        }
        else if (caculateIndex == 2)
        {
            return Random.Range(2, 7);
        }

        return 1;
    }

    private void SetCaculate()
    {
        if (caculateIndex == 0)
        {
            caculate = "+";
        }
        else if (caculateIndex == 1)
        {
            caculate = "-";
        }
        else if (caculateIndex == 2)
        {
            caculate = "x";
        }

    }
}