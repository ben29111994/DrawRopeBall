using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletButton : MonoBehaviour
{
    public bool isShoot;
    public bool isHold;
    public int myID;
    public int ballAmount;
    public TextMeshProUGUI ballText;


    public void OnClick_Down()
    {
        isHold = true;
    }
    public void OnClick_Up()
    {
        isHold = false;
    }

    private void OnEnable()
    {
        isShoot = false;
        isHold = false;
    }

    private void Update()
    {
        if (isHold)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (isShoot || ballAmount <= 0) return;
        StartCoroutine(C_Shoot());
    }

    private IEnumerator C_Shoot()
    {
        isShoot = true;
        FightBossGP.Instance.Shoot(myID);
        UpdateBallAmount();
        yield return null;
        yield return null;
        isShoot = false;
    }

    public void UpdateBallAmount()
    {
        ballAmount--;
        ballText.text = "x" + ballAmount.ToString();
    }

    public void ResetData()
    {
        ballAmount = 0;
        ballText.text = "x" + ballAmount.ToString();
    }

    public void SetBallAmount(int number)
    {
        ballAmount += number;
        ballText.text = "x" + ballAmount.ToString();
    }
}
