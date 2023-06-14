using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTrigger : MonoBehaviour
{
    public Dot dot;
    public CircleCollider2D cir2D;

    private void Start()
    {
        cir2D.radius = 0.5f;
        StartCoroutine(C_SetRadiusDotTrigger());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsStatic()) return;

        if (collision.gameObject.CompareTag("Dot") || collision.gameObject.CompareTag("Rope"))
        {
            if (collision.gameObject.GetInstanceID() == dot.gameObject.GetInstanceID()) return;
            dot.SetCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            dot.Hide(-1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsStatic()) return;

        if (collision.gameObject.CompareTag("Dot") || collision.gameObject.CompareTag("Rope"))
        {
            if (collision.gameObject.GetInstanceID() == dot.gameObject.GetInstanceID()) return;
            dot.SetCollision(collision);
        }
    }

    private bool IsStatic()
    {
        if (RopeMultiplyDotGP.Instance.phaseGame != RopeMultiplyDotGP.PhaseGame.MergeDot) return true;
        
        for(int i = 0;i< TouchController.Instance.listRope.Count; i++)
        {
            if (!TouchController.Instance.listRope[i].isHidePolygonCol) return true;
        }

        return false;
    }

    private IEnumerator C_SetRadiusDotTrigger()
    {
        while (RopeMultiplyDotGP.Instance.phaseGame != RopeMultiplyDotGP.PhaseGame.MergeDot) yield return null;
        yield return new WaitForSeconds(1.0f);
        yield return new WaitForSeconds(0.2f);
        cir2D.radius = 0.55f;
    }
}