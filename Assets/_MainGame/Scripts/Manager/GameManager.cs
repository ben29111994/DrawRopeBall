using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public enum TypeGamePlay
    {
        DrawRope,
        FightBoss
    }

    [Header("Status Game")]
    public TypeGamePlay typeGamePlay;
    public bool isComplete;

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
        InitPlugin();
    }
    
    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ShakeManager.Instance.ShakeCamera(1.0f);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Complete();
        }
    }

    public void InitPlugin()
    {
        Application.targetFrameRate = 60;


#if UNITY_IOS

#else

#endif
    }

    public void Refresh()
    {
        isComplete = false;
    }

    public void LevelUp()
    {
        DataManager.Instance.LevelGame++;
    }

    public void LevelDown()
    {
        if (DataManager.Instance.LevelGame > 0) DataManager.Instance.LevelGame--;
    }
    
    public void Complete()
    {
        if (isComplete) return;

        isComplete = true;
        StartCoroutine(C_Complete());
    }

    private IEnumerator C_Complete()
    {
        LevelUp();
        yield return new WaitForSeconds(1.0f);
        UIManager.Instance.Show_CompleteUI();
    }

    public void Fail()
    {
        if (isComplete) return;

        isComplete = true;
        StartCoroutine(C_Fail());
    }

    private IEnumerator C_Fail()
    {
        yield return null;
    }


}
