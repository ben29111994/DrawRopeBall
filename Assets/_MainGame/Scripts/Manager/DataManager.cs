using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance { get { return instance; } }

    [Header("References")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI coinText;
    public Animation coinAnim;

    public int LevelGame
    {
        get
        {
            return PlayerPrefs.GetInt("LevelGame");
        }
        set
        {
            PlayerPrefs.SetInt("LevelGame",value);
            levelText.text = "Lvl " + (value + 1);
        }
    }

    public int Coin
    {
        get
        {
            return PlayerPrefs.GetInt("Coin");
        }
        set
        {
            PlayerPrefs.SetInt("Coin", value);
            coinText.text = "" + CoinFixedText(value);
            coinAnim.Play("CoinCollect",PlayMode.StopAll);
        }
    }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;

        LevelGame += 0;
        Coin += 0;
    }

    private string CoinFixedText(int number)
    {
        if (number < 1000)
        {
            return number.ToString();
        }
        else
        {
            int a = number / 1000;
            int b = number % 1000;
            int c = b / 10;

            if (c == 0)
            {
                return a + "K";
            }
            else
            {
                return a + "." + c + "K";
            }
        }
    }
}
