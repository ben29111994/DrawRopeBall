using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Rigidbody2D rigid;
    public bool isMove;
    public Rope rope;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (!isMove || rope == null) return;
        if (rope.listDot.Count <= 0) return;

        rigid.velocity = (rope.GetPivotRopeNodes() - transform.position).normalized * 3.0f;
    }

    private void Init()
    {
        RopeMultiplyDotGP.Instance.listObstacle.Add(this);
    }



}
