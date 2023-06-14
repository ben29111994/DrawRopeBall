using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GemObject : MonoBehaviour
{
    public GameObject target;
    private List<GameObject> listGem = new List<GameObject>();
    private Vector3 startPos;
    private CompleteUI completeUI;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) listGem.Add(transform.GetChild(i).gameObject);
        startPos = listGem[0].transform.position;
    }

    private void OnEnable()
    {
        completeUI = UIManager.Instance.CompleteUI.GetComponent<CompleteUI>();
        StartCoroutine(C_Animation());
    }

    private IEnumerator C_Animation()
    {
        int coinEarn= (int)(UIManager.Instance.coinEarn / listGem.Count);
       
        for (int i = 0; i < listGem.Count; i++)
        {
            GameObject go = listGem[i];
            go.transform.position = startPos;
            go.transform.localScale = Vector3.one;
            float _time = Random.Range(0.4f, 0.5f);
            float _jump = Random.Range(-4.0f, 4.0f);
            go.transform.DOJump(target.transform.position, _jump, 1, _time).SetEase(Ease.InOutSine);
            go.transform.DOScale(Vector3.one * 0.6f, _time).SetEase(Ease.InOutSine).OnComplete(() => OnCompelte(go,coinEarn));
            completeUI.CurrentCoinEarn -= coinEarn;

            if(i == listGem.Count - 1)
            {
                completeUI.CurrentCoinEarn = 0;
                completeUI.coinEarnObject.SetActive(false);
            }
            yield return new WaitForSeconds(Random.Range(0.02f, 0.04f));
        }

        yield return new WaitForSeconds(0.6f);
        completeUI.Show_ContinueButton();
    }

    private void OnCompelte(GameObject go,int coinEarn)
    {
        DataManager.Instance.Coin += coinEarn;
        go.transform.localScale = Vector3.zero;
    }
}
