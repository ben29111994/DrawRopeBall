using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public bool isTakeDamage;
    public bool isDead;
    public GameObject[] modelsArray;
    public Image healthBar;
    private Animator anim;
    private GameObject model;
    private int maxHealth;
    private float targetT;

    public int BossLevel
    {
        get
        {
            return PlayerPrefs.GetInt("BossLevel");
        }
        set
        {
            PlayerPrefs.SetInt("BossLevel", value);
        }
    }

    public int BossHealth
    {
        get
        {
            return PlayerPrefs.GetInt("BossHealth");
        }
        set
        {
            PlayerPrefs.SetInt("BossHealth", value);
        }
    }

    private void OnEnable()
    {
        isDead = false;
        healthBar.transform.parent.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if(healthBar != null) healthBar.transform.parent.gameObject.SetActive(false);
    }

    public void ActiveBoss()
    {
        SetModel();
        SetBossHealth();
        UpdateHealthBar();
        transform.position = Vector3.forward * 30.0f;
        gameObject.SetActive(true);
        StartCoroutine(C_ActiveBoss());
    }

    private IEnumerator C_ActiveBoss()
    {
        if (!isDead) anim.SetTrigger("Walk");

        while (transform.position.z > 5.0f)
        {
            while (isTakeDamage) yield return null;
            float speed = (FightBossGP.Instance.isDone) ? 12.0f : 4.0f;
            transform.Translate(Time.deltaTime * Vector3.back * speed);
            yield return null;
        }

        if(FightBossGP.Instance.isDone == false) FightBossGP.Instance.AutoShoot();
        while (FightBossGP.Instance.isDone == false)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);

        if (!isDead) anim.SetTrigger("Attack");
        UIManager.Instance.Show_WarningPopup();
        FightBossGP.Instance.cameraAnim.SetTrigger("Fall");
        BossHealth--;
        GameManager.Instance.Complete();
    }

    public void BossHitDamage()
    {
        if (isDead) return;
        UpdateHealthBar();    
        if (C2_TakeDamage != null) StopCoroutine(C2_TakeDamage);
        C2_TakeDamage = C_TakeDamage();
        StartCoroutine(C2_TakeDamage);
    }


    private IEnumerator C2_TakeDamage;
    private IEnumerator C_TakeDamage()
    {
        isTakeDamage = true;
        yield return new WaitForSeconds(0.6f);
        isTakeDamage = false;
    }

    private void UpdateHealthBar()
    {
        float t = FightBossGP.Instance.tBall;
        float h = Mathf.Lerp(0.0f, targetT, t);
        float c = (float)BossHealth / (float)maxHealth;
        float c2 = c - h;
        healthBar.fillAmount = c2;

        if(c2 <= 0)
        {
            Die();
        }
        else
        {
           if(!isDead) anim.SetTrigger("TakeDamage");
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        isDead = true;
        anim.SetTrigger("Die");
        BossHealth = 0;
        GameManager.Instance.Complete();
        BossLevel++;
    }

    private void SetBossHealth()
    {
        if (BossHealth <= 0) BossHealth = 4;
        maxHealth = 4;
        targetT = Random.Range(0.25f, 0.35f);
    }

    private void SetModel()
    {
        for (int i = 0; i < modelsArray.Length; i++) modelsArray[i].gameObject.SetActive(false);
        int index = BossLevel;
        if (BossLevel >= modelsArray.Length) index = Random.Range(0, modelsArray.Length);
        model = modelsArray[index].gameObject;
        model.SetActive(true);
        anim = model.GetComponent<Animator>();
    }
}
