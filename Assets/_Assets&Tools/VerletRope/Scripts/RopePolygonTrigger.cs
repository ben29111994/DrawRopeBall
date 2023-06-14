using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePolygonTrigger : MonoBehaviour
{
    public Rope rope;
    public List<Dot> listDot2 = new List<Dot>();
    public List<Obstacle> listObstacle2 = new List<Obstacle>();

    private void Awake()
    {
        rope = transform.parent.GetComponent<Rope>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DotTrigger"))
        {
            DotTrigger dotTrigger = collision.GetComponent<DotTrigger>();
            Dot dot = dotTrigger.dot;
            if (dot.rope != null) return;
            if (rope.listDot.Contains(dot)) return;
            if (dot.rope != null) return;
            rope.AddDot(dot);
            dot.SetRope(rope);
            listDot2.Add(dot);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            Obstacle obstacle = collision.GetComponent<Obstacle>();
            obstacle.rope = rope;
            listObstacle2.Add(obstacle);
            rope.listObstacle.Add(obstacle);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (RopeMultiplyDotGP.Instance.phaseGame == RopeMultiplyDotGP.PhaseGame.CaculatorDot) return;
  
        if (collision.CompareTag("DotTrigger"))
        {
            DotTrigger dotTrigger = collision.GetComponent<DotTrigger>();
            Dot dot = dotTrigger.dot;
            if (rope.listDot.Contains(dot))
            {
                rope.listDot.Remove(dot);
                dot.rope = null;

                if (rope.isHidePolygonCol == false)
                {
                    
                }
                else
                {
                    dot.Hide(-1);
                }

            }

            if (listDot2.Contains(dot))
            {
                listDot2.Remove(dot);
            }
        }
        else if (collision.CompareTag("Obstacle"))
        {
            Obstacle obstacle = collision.GetComponent<Obstacle>();
            obstacle.rope = null;
            listObstacle2.Remove(obstacle);
            rope.listObstacle.Remove(obstacle);
        }
    }

}
