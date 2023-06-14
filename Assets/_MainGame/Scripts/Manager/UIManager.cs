using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    public int coinEarn;

    [Header("References")]
    public GameObject MainMenuUI;
    public GameObject InGameUI;
    public GameObject CompleteUI;
    public GameObject FailUI;
    public GameObject SettingUI;
    public GameObject LoadingUI;
    public GameObject fightBossPopup;
    public GameObject warningPopup;

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    private void Start()
    {
        Show_LoadingUI();
    }

    public void Show_LoadingUI()
    {
        StartCoroutine(C_LoadingUI());
    }

    private IEnumerator C_LoadingUI()
    {
        MainMenuUI.SetActive(false);
        InGameUI.SetActive(false);
        CompleteUI.SetActive(false);
        FailUI.SetActive(false);
        SettingUI.SetActive(false);

        LoadingUI.SetActive(true);
        yield return new WaitForSecondsRealtime(2.0f);
        LoadingUI.SetActive(false);
        Show_MainMenuUI();
        RopeMultiplyDotGP.Instance.ActiveGamePlay();
    }

    public void Show_FightBossPopup()
    {
        StartCoroutine(C_Show_FightBossPopup());
    }
    private IEnumerator C_Show_FightBossPopup()
    {
        fightBossPopup.SetActive(true);
        yield return new WaitForSecondsRealtime(2.0f);
        fightBossPopup.SetActive(false);
    }

    public void Show_WarningPopup()
    {
        StartCoroutine(C_Show_WarningPopup());
    }
    private IEnumerator C_Show_WarningPopup()
    {
        warningPopup.SetActive(true);
        yield return new WaitForSecondsRealtime(1.0f);
        warningPopup.SetActive(false);
    }

    public void Show_MainMenuUI()
    {
        MainMenuUI.SetActive(true);
        InGameUI.SetActive(true);
        CompleteUI.SetActive(false);
        FailUI.SetActive(false);
    }

    public void Show_InGameUI()
    {
        MainMenuUI.SetActive(false);
        InGameUI.SetActive(true);
        CompleteUI.SetActive(false);
        FailUI.SetActive(false);
    }

    public void Show_CompleteUI()
    {
        MainMenuUI.SetActive(false);
        InGameUI.SetActive(false);
        CompleteUI.SetActive(true);
        FailUI.SetActive(false);
    }

    public void Show_FailUI()
    {
        MainMenuUI.SetActive(false);
        InGameUI.SetActive(false);
        CompleteUI.SetActive(false);
        FailUI.SetActive(true);
    }

    public void OnClick_Continue()
    {
        Show_LoadingUI();
    }

    public void OnClick_TryAgain()
    {
        Show_LoadingUI();
    }

    public void OnClick_Next()
    {
        GameManager.Instance.LevelUp();
        FightBossGP.Instance.boss.BossHealth--;
        if (FightBossGP.Instance.boss.BossHealth <= 0)
        {
            FightBossGP.Instance.boss.BossHealth = 4;
            FightBossGP.Instance.boss.BossLevel++;
        }
        Show_LoadingUI();
    }

    public void OnClick_Previous()
    {
        GameManager.Instance.LevelUp();
        FightBossGP.Instance.boss.BossHealth++;
        if (FightBossGP.Instance.boss.BossHealth >= 5)
        {
            FightBossGP.Instance.boss.BossHealth = 1;
            if (FightBossGP.Instance.boss.BossLevel > 0) FightBossGP.Instance.boss.BossLevel--;
        }
        Show_LoadingUI();
    }
}
