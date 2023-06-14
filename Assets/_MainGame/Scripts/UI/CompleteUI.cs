using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompleteUI : MonoBehaviour
{
    private int currentCoinEarn;
    public int CurrentCoinEarn
    {
        get
        {
            return currentCoinEarn;
        }
        set
        {
            currentCoinEarn = value;
            coinEarnText.text = "$" + currentCoinEarn.ToString();
        }
    }

    public GameObject coinEarnObject;
    public GameObject nextButton;
    public GameObject nextImageButton;
    public TextMeshProUGUI coinEarnText;

    public GameObject moneyIMG;
    public GameObject gemObject;
    public Image strokeImg;
    public Image iconImg;
    public Sprite[] strokeArray;
    public Sprite[] iconArray;
    public Text percentText;

    public void OnEnable()
    {
        StartCoroutine(C_Active());
    }

    private IEnumerator C_Active()
    {
        coinEarnObject.SetActive(true);
        gemObject.SetActive(false);
        nextImageButton.SetActive(false);
        nextButton.SetActive(false);
        moneyIMG.SetActive(true);
        CurrentCoinEarn = UIManager.Instance.coinEarn = Random.Range(150, 300);
        //    int targetWeapon = DataManager.Instance.CurrentUnlockWeapon + 1;

        //if (targetWeapon >= iconArray.Length)
        //{
        //    iconImg.gameObject.transform.parent.gameObject.SetActive(false);
        //    nextButton.SetActive(true);
        //    // unlock all already
        //    yield break;
        //}

        //int currentPercent = DataManager.Instance.PercentUnlockWeapon;
        //int targetPercent = currentPercent + GetPercent();
        //float t1 = (float)currentPercent / 100.0f;
        //float t2 = (float)targetPercent / 100.0f;
        //if (t2 >= 1.0f) t2 = 1.0f;

        //iconImg.sprite = iconArray[targetWeapon];
        //strokeImg.sprite = strokeArray[targetWeapon];
        //    iconImg.color = colors[CurrentUnlockTurret];
        //strokeImg.SetNativeSize();
        //iconImg.SetNativeSize();

        //iconImg.fillAmount = t1;
        //percentText.text = t1.ToString() + "%";

        //float t = 0.0f;
        //while (t < 1.0f)
        //{
        //    t += Time.deltaTime * 0.5f;

        //    float t3 = Mathf.Lerp(t1, t2, t);
        //    iconImg.fillAmount = t3;
        //    percentText.text = (Mathf.Round((t3 * 100) * 10) / 10).ToString() + "%";

        //    yield return null;
        //}

        //DataManager.Instance.PercentUnlockWeapon = targetPercent;
        //if (DataManager.Instance.PercentUnlockWeapon >= 100)
        //{
        //    DataManager.Instance.PercentUnlockWeapon = 0;
        //    DataManager.Instance.CurrentUnlockWeapon++;
        //}
        yield return new WaitForSeconds(0.4f);
        gemObject.SetActive(true);
    }

    public void Show_ContinueButton()
    {
        coinEarnObject.SetActive(false);
        nextButton.SetActive(true);
        nextImageButton.SetActive(true);
    }

    private int GetPercent()
    {
        int lvl = DataManager.Instance.LevelGame;
        if(lvl <= 1)
        {
            return 101;
        }
        else if (lvl <= 4)
        {
            return Random.Range(55, 60);
        }
        else if (lvl <= 8)
        {
            return Random.Range(35, 38);
        }
        else if (lvl <= 12)
        {
            return Random.Range(26, 30);
        }
        else
        {
            return Random.Range(21, 24);
        }
    }

    public void OnClick_Next()
    {
        UIManager.Instance.OnClick_Continue();
        gameObject.SetActive(false);
    }


    public void OnClick_NextUpgrade()
    {
        gameObject.SetActive(false);
    //    UIManager.Instance.OnClick_LoadScene();
    }
}