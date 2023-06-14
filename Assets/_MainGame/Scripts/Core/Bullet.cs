using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public Color myColor;
    public MeshRenderer meshRenderer;
    public Rigidbody rigid;
    public ReferenceManager.TypeColor typeColor;
    public Material[] m_Trails;

    public TrailRenderer trailRenderer;
    public int myID;

    public void SetColor(int _myID)
    {
        myID = _myID;
        typeColor = (ReferenceManager.TypeColor)myID;

        try
        {
            meshRenderer.material = ReferenceManager.Instance.GetMaterialColor(typeColor);
            myColor = ReferenceManager.Instance.GetColor(typeColor);
        }
        catch
        {
            meshRenderer.material = GameObject.Find("ReferenceManager").GetComponent<ReferenceManager>().GetMaterialColor(typeColor);
            myColor = GameObject.Find("ReferenceManager").GetComponent<ReferenceManager>().GetColor(typeColor);
        }
    }

    public void Shoot()
    {
        transform.position = FightBossGP.Instance.canonPoint.position;
        rigid.velocity = Vector3.zero;
        rigid.isKinematic = true;
        trailRenderer.material = m_Trails[myID];
        gameObject.SetActive(true);
        StartCoroutine(C_Shoot());
    }

    private IEnumerator C_Shoot()
    {
        Vector3 targetPosition = FightBossGP.Instance.boss.transform.position + Vector3.up * 6.0f;
        targetPosition.x += Random.Range(-3.0f, 3.0f);
        targetPosition.y += Random.Range(-3.0f, 3.0f);
        targetPosition.z += Random.Range(-3.0f, 3.0f);
        float distance = Vector3.Distance(transform.position, targetPosition) * 0.08f;
        if (distance < 1.0f) distance = 1.0f;
        float jump = Random.Range(2.0f, 5.0f);
        float time = Random.Range(0.4f, 0.6f) * distance;
        transform.DOJump(targetPosition, jump, 1, time).SetEase(Ease.Flash);
        yield return new WaitForSeconds(time);
        FightBossGP.Instance.boss.BossHitDamage();
        gameObject.SetActive(false);
    }
}
