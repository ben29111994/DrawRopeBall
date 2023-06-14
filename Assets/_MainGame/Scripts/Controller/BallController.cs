using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private static BallController instance;
    public static BallController Instance { get { return instance; } }


    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    public void SpawnBall(int amount,int ID,Vector3 pos,Rope rope)
    {
        StartCoroutine(C_SpawnBall(amount, ID, pos,rope));
    }

    private IEnumerator C_SpawnBall(int amount, int ID, Vector3 pos, Rope rope)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject dotGO = (GameObject)PoolManager.Instance.GetObject(PoolManager.NameObject.Dot);
            Dot dot = dotGO.GetComponent<Dot>();
            dot.ActiveBall(pos, ID);
            yield return null;
            yield return null;

            if (i % 6 == 0)  rope.SpawnToScaleUpRope();
        }

        float ratio = 1.0f + (float)amount / 50.0f;
        float timeDelay = 1.0f * ratio;
        rope.ShowUIPopup();
        yield return new WaitForSeconds(timeDelay);
        RopeMultiplyDotGP.Instance.DoneCaculator();
    }
}
