using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Status")]
    public Color myColor;
    public int myID;
    public ReferenceManager.TypeColor typeColor;
    public bool isMove;
    public bool isCollision;
    public bool isSetRope;
    public bool isBall;
    public bool isHide;

    [Header("References")]
    public Rigidbody2D rigid2d;
    public MeshRenderer meshRenderer;
    public Rope rope;


    private void OnEnable()
    {
        if (!isBall)
            RopeMultiplyDotGP.Instance.listDot.Add(this);
    }

    private void Start()
    {
        ResetColor();
    }

    private void FixedUpdate()
    {
        if (!isMove) return;
        if (rope == null) return;

        Vector3 pivotA = rope.GetPivotDot();
        Vector3 pivotB = rope.GetPivotRopeNodes();
        Vector3 pivotC = (pivotA + pivotB) / 2.0f;
        rigid2d.velocity = (pivotC - transform.position).normalized * 3f;

    }

    public void ActiveBall(Vector3 position,int ID)
    {
        isBall = true;
        position.x += Random.Range(-0.2f, 0.2f);
        position.y += Random.Range(-0.2f, 0.2f);
        transform.position = position;
        transform.localScale = Vector3.one * 0.8f;
        myID = ID;
        typeColor = (ReferenceManager.TypeColor)System.Enum.ToObject(typeof(ReferenceManager.TypeColor), ID);
        ResetColor();
        gameObject.SetActive(true);
    }

    public void SetRope(Rope _rope)
    {
        rope = _rope;
    }

    public void Hide(int ignoreID)
    {
        if (isHide) return;
        if (myID == ignoreID) return;
        EffectManager.Instance.SpawnEffect(PoolManager.NameObject.Effect_Splash, transform.position, myColor);
        if (rope != null) rope.listDot.Remove(this);
        isHide = true;
        gameObject.SetActive(false);
    }

    public void ResetLayer()
    {
        // reset layer = 0 to avoid rope circle cast
        gameObject.layer = 0;
    }

    public void ResetTrueLayer()
    {
        gameObject.layer = 6;
    }

    public void HideNoEffect()
    {
        gameObject.SetActive(false);
    }

    public void SetCollision(Collider2D collision)
    {
        if (isBall) return;
        else if (rope == null) return;
        
        isCollision = true;
        isMove = true;

        if (myID == 0) return;

        Dot colDot = collision.gameObject.GetComponent<Dot>();
        Rope colRope = collision.gameObject.GetComponent<Rope>();

        if(colDot != null)
        {
            if (colDot.isHide) return;
         //   else if (colDot.rope == null) return;
         //   else if (myID == colDot.rope.myID) return;

            int collisionID = colDot.myID;

            if (myID != collisionID)
            {
                colDot.Hide(0);
                Hide(0);
            }

        }
        else if(colRope != null)
        {
            // hide when collision with rope
            //int collisionID = colRope.myID;

            //if (myID != collisionID)
            //{
            //    colDot.Hide(0);
            //    Hide(0);
            //}
        }
    }

    [NaughtyAttributes.Button]
    public void ResetColor()
    {
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
}
