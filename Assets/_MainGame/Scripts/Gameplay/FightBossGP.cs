using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FightBossGP : MonoBehaviour
{
    private static FightBossGP instance;
    public static FightBossGP Instance { get { return instance; } }

    public bool isDone;
    public int[] ballColorAmount;
    public int maxBall;
    public int leftBall;
    public float tBall;

    [Header("References")]
    public TextMeshProUGUI turnText;
    public Animator cameraAnim;
    public Animator canonAnim;
    public Transform canonPoint;
    public Boss boss;
    public BulletButton[] bulletButton;
    private GameObject bulletButtonParent;

    private void Awake()
    {
        instance = (instance == null) ? this : instance;

        bulletButtonParent = bulletButton[0].transform.parent.parent.gameObject;
    }

    private void Update()
    {
        UpdateTurnText();
    }

    private void UpdateTurnText()
    {
        int sumBullet = 0;
        for (int i = 0; i < bulletButton.Length; i++)
        {
            sumBullet += bulletButton[i].ballAmount;
        }

        turnText.text = (sumBullet <= 0) ? "BOSS TURN" : "YOUR TURN";
    }

    public void ActiveGamePlay(int[] _ballColorAmount)
    {
        isDone = false;
        turnText.gameObject.SetActive(true);
        turnText.text = "YOUR TEXT";
        FightBossGP.Instance.cameraAnim.SetTrigger("Idle");
        GameManager.Instance.typeGamePlay = GameManager.TypeGamePlay.FightBoss;
        Camera.main.transform.localPosition = new Vector3(0.0f, 8.0f, -21.0f);
        Camera.main.transform.localEulerAngles = new Vector3(5.0f, 0.0f, 0.0f);
        Camera.main.transform.parent.localPosition = Vector3.zero;
        bulletButtonParent.SetActive(false);
        boss.gameObject.SetActive(false);
        gameObject.SetActive(true);
        ballColorAmount = _ballColorAmount;
        maxBall = 0;
        //health no bullet
        for (int i = 0; i < bulletButton.Length; i++) bulletButton[i].ResetData();
        for (int i = 0; i < bulletButton.Length; i++)
        {
            bulletButton[i].SetBallAmount(ballColorAmount[i + 1]);
        }
        for (int i = 0; i < bulletButton.Length; i++) maxBall += bulletButton[i].ballAmount;
        leftBall = 0;
        tBall = (float)leftBall / (float)maxBall;
        if (maxBall == 0)
        {
            tBall = 1;
            isDone = true;
        }
        UIManager.Instance.Show_FightBossPopup();
        StartCoroutine(C_Delay());
    }

    private IEnumerator C_Delay()
    {
        yield return new WaitForSeconds(2.0f);
        bulletButtonParent.SetActive(true);
        boss.ActiveBoss();
    }

    public void HideGamePlay()
    {
        turnText.gameObject.SetActive(false);
        bulletButtonParent.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Shoot(int ballID)
    {
        GameObject go = (GameObject)PoolManager.Instance.GetObject(PoolManager.NameObject.Bullet);
        Bullet bullet = go.GetComponent<Bullet>();
        bullet.SetColor(ballID);
        bullet.Shoot();
        canonAnim.SetTrigger("Shoot");
        ballColorAmount[ballID]--;

        leftBall++;
        tBall = (float)leftBall / (float)maxBall;
        if(tBall >= 1.0f)
        {
            isDone = true;
            bulletButtonParent.gameObject.SetActive(false);
        }
    }

    public void AutoShoot()
    {
        StartCoroutine(C_AutoShoot());
    }

    private IEnumerator C_AutoShoot()
    {
        for (int i = 0; i < bulletButton.Length; i++) bulletButton[i].isHold = true;
        for (int i = 0; i < bulletButton.Length; i++)
        {
            while (bulletButton[i].ballAmount > 0) yield return null;
        }
        for (int i = 0; i < bulletButton.Length; i++) bulletButton[i].isHold = false;
    }
}
