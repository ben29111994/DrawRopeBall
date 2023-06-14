using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeMultiplyDotGP : MonoBehaviour
{
    private static RopeMultiplyDotGP instance;
    public static RopeMultiplyDotGP Instance { get { return instance; } }

    [Header("Status")]
    public PhaseGame phaseGame;
    public int maxStep;
    public int drawStep;
    public int mergeStep;
    public int caculatorStep;

    [Header("References")]
    public List<Dot> listDot = new List<Dot>();
    public List<Obstacle> listObstacle = new List<Obstacle>();
    public GameObject levelPrefab;
    private GameObject levelObject;

    public enum PhaseGame
    {
        DrawRope,
        MergeDot,
        CaculatorDot
    }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    private void Refresh()
    {
        mergeStep= maxStep = drawStep = caculatorStep = 0;
        phaseGame = PhaseGame.DrawRope;
        listDot.Clear();
        listObstacle.Clear();
        GameManager.Instance.Refresh();
        PenController.Instance.Refresh();
        TouchController.Instance.Refresh();
        PoolManager.Instance.RefreshAll();
        if (levelObject != null) Destroy(levelObject);
    }

    public void ActiveGamePlay()
    {
        TouchController.Instance.inkImage.transform.parent.gameObject.SetActive(true);
        FightBossGP.Instance.cameraAnim.SetTrigger("Idle");
        GameManager.Instance.typeGamePlay = GameManager.TypeGamePlay.DrawRope;
        gameObject.SetActive(true);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localEulerAngles = Vector3.zero;
        Camera.main.transform.parent.localPosition = new Vector3(0.0f, 0.0f, -25.0f);
        Camera.main.transform.parent.localEulerAngles = Vector3.zero;
        FightBossGP.Instance.HideGamePlay();
        Refresh();
        int lvl = DataManager.Instance.LevelGame;
        if (lvl >= 60) lvl = Random.Range(0, 60);
        levelObject = Instantiate(levelPrefab);
        levelObject.name = "Level " + lvl;
        GenerateLevelEditor gle = levelObject.GetComponent<GenerateLevelEditor>();
        gle.levelIndex = lvl;
        gle.LoadData();

        for(int i = 0; i < levelObject.transform.GetChild(0).childCount; i++)
        {
            if (levelObject.transform.GetChild(0).GetChild(i).childCount > 0) maxStep++;
        }

        if (maxStep > 4) maxStep = 4;
    }

    public void NextStep()
    {
        drawStep++;

        PenController.Instance.Refresh(drawStep + 1);


        if (drawStep >= maxStep)
        {
            phaseGame = PhaseGame.MergeDot;
            PenController.Instance.ActivePen(false);
            TouchController.Instance.PhaseMerge();
        }
    }

    public void DoneMerge()
    {
        mergeStep++;
        if(mergeStep == maxStep)
        {
            Debug.Log("Done Merge => CaculateDot");
            phaseGame = PhaseGame.CaculatorDot;
            HideDotOutSide();
            HideObstacle();

            for (int i = 0; i < TouchController.Instance.listRope.Count;i++)
            {
                TouchController.Instance.listRope[i].CaculateDot();
            }
        }
    }

    public void DoneCaculator()
    {
        caculatorStep++;

        if(caculatorStep >= maxStep)
        {
            for (int i = 0; i < TouchController.Instance.listRope.Count; i++) TouchController.Instance.listRope[i].enabled = false;
            StartCoroutine(C_WaitAnimationPlusBall());
        }
    }

    public IEnumerator C_WaitAnimationPlusBall()
    {
        // do somethings like animation etc ... after hide gameplay
      //  for (int i = 0; i < TouchController.Instance.listRope.Count; i++) TouchController.Instance.listRope[i].ShowUIPopup();
        yield return new WaitForSeconds(0.5f);
        HideGamePlay();
    }

    public void HideDotOutSide()
    {
        for(int i = 0; i < listDot.Count; i++) if (!listDot[i].isBall && listDot[i].rope == null && listDot[i].gameObject.activeSelf) listDot[i].HideNoEffect();
    }

    public void HideObstacle()
    {
        for (int i = 0; i < listObstacle.Count; i++) listObstacle[i].gameObject.SetActive(false);
    }

    public void HideGamePlay()
    {
        FightBossGP.Instance.ActiveGamePlay(ListBallColor().ToArray());

        Refresh();
        PenController.Instance.ActivePen(false);
        gameObject.SetActive(false);
    }

    private List<int> ListBallColor()
    {
        List<int> listBallColor = new List<int>();
        for (int i = 0; i <= 8; i++) listBallColor.Add(0);
        for (int i = 0; i < TouchController.Instance.listRope.Count; i++)
        {
            listBallColor[TouchController.Instance.listRope[i].myID] += TouchController.Instance.listRope[i].ballAmount;
        }
        return listBallColor;
    }

    public void ResetLayerAllObjects()
    {
        for (int i = 0; i < listDot.Count; i++) listDot[i].ResetLayer();
        for (int i = 0; i < listObstacle.Count; i++) listObstacle[i].gameObject.layer = 0;
    }

}
