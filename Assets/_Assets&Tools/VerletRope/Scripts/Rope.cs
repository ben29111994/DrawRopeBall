using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Rope : MonoBehaviour
{
    [Header("Status")]
    public int ballAmount;
    public int myStep;
    public int myID;
    public int stt;
    public LayerMask layer;
    public float sumDotRadius;
    public bool isActive;
    public bool isSwing;
    public bool isGravity;
    public bool isScaleDown;
    public bool isHidePolygonCol;
    public Color ropeColor;
    private bool isSimulate;
    private bool isFree;

    [Header("References")]
    public TextMeshProUGUI numberRopeText;
    public LayerMask layerRope;
    public RopeNode ropeNodePrefab;
    PolygonCollider2D polygonCollider2D;
    EdgeCollider2D edgeCollider2D;
    LineRenderer LineRenderer;
    Vector3[] LinePositions;

    public List<Dot> listDot = new List<Dot>();
    public List<Obstacle> listObstacle = new List<Obstacle>();
    public List<RopeNode> RopeNodes = new List<RopeNode>();
    private float NodeDistance;
    private int TotalNodes;

    int LayerMask = 1;
    ContactFilter2D ContactFilter;    
    RaycastHit2D[] RaycastHitBuffer = new RaycastHit2D[10];
    Collider2D[] ColliderHitBuffer = new Collider2D[10];

    Vector3 Gravity = new Vector2(0f, -5f);
    Vector2 Node1Lock;

    void Awake()
    {
        ContactFilter = new ContactFilter2D
        {
            layerMask = LayerMask,
            useTriggers = false,
        };

        LineRenderer = this.GetComponent<LineRenderer>();
        polygonCollider2D = this.transform.GetChild(0).GetComponent<PolygonCollider2D>();
        edgeCollider2D = this.GetComponent<EdgeCollider2D>();

       // polygonCollider2D.enabled = false;
        edgeCollider2D.enabled = false;
    }


    void Update()
    {
        if (!isSimulate)
            return;

        DrawRope();
    }

    private void FixedUpdate()
    {
        UpdateCollider2D();

        if (!isSimulate)
            return;

        UpdateRopeNodeStep();
    }

    private void UpdateCollider2D()
    {
        List<Vector2> listRopeNodesPoint = RopeNodePoint();
        listRopeNodesPoint.Add(GetPivotRopeNodes());
        polygonCollider2D.pathCount = 1;
        polygonCollider2D.points = listRopeNodesPoint.ToArray();

        List<Vector2> listRopeNodesPoint2 = RopeNodePoint();
        if (listRopeNodesPoint2.Count <= 0) return;
        listRopeNodesPoint2.Add(listRopeNodesPoint2[0]);
        edgeCollider2D.points = listRopeNodesPoint2.ToArray();
    }

    public void AddDot(Dot dot)
    {
        listDot.Add(dot);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isScaleDown) return;
        if (isHidePolygonCol) return;

        if (collision.CompareTag("DotTrigger"))
        {
            DotTrigger dotTrigger = collision.GetComponent<DotTrigger>();
            Dot dot = dotTrigger.dot;
            if (listDot.Contains(dot)) return;
            if (dot.rope != null) return;
            AddDot(dot);
            dot.SetRope(this);
        }
        //else if (collision.CompareTag("Obstacle"))
        //{
        //    Obstacle obstacle = collision.GetComponent<Obstacle>();
        //    listObstacle.Add(obstacle);
        //}
    }

    public void ActiveScaleDown()
    {
        StartCoroutine(C_MoveDot());
    }

    private IEnumerator C_MoveDot()
    {
        yield return new WaitForSeconds(0.6f);
        // RopeMultiplyDotGP.Instance.ResetLayerAllObjects();
        for (int i = 0; i < listObstacle.Count; i++) listObstacle[i].gameObject.layer = 0;

        isScaleDown = true;
        yield return new WaitForSeconds(0.1f);
        edgeCollider2D.enabled = true;
        for (int i = 0; i < listDot.Count; i++) listDot[i].transform.localScale *=  0.98f;
        yield return null;
        RopeMultiplyDotGP.Instance.HideDotOutSide();
        yield return null;
        StartCoroutine(C_ScaleDown());
        yield return new WaitForSeconds(0.2f);
        isHidePolygonCol = true;
    }

    private IEnumerator C_ScaleDown()
    {
        Vector2 a = RopeNodes[0].transform.position;

        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        // optimize performance game , wait previous rope 
        while (stt > 0)
        {
            int h = 0;

            for (int i = 0; i < stt; i++)
            {
                if (TouchController.Instance.listRope[i].RopeNodes.Count > 80)
                {
                    h++;
                }
            }

            if (h == 0 || RopeNodes.Count<80) break;
            yield return null;
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(true);

        while (true)
        {
            // remove head
            RopeNode ropeNode = RopeNodes[RopeNodes.Count - 1];
            ropeNode.gameObject.SetActive(false);
            RopeNodes.Remove(ropeNode);
            TotalNodes = RopeNodes.Count;
            LinePositions = new Vector3[RopeNodes.Count];

            // remove tail
            ropeNode = RopeNodes[0];
            ropeNode.gameObject.SetActive(false);
            RopeNodes.Remove(ropeNode);
            TotalNodes = RopeNodes.Count;
            LinePositions = new Vector3[RopeNodes.Count];

            // set position head and tail
            Node1Lock = ropeNode.transform.position;
            //Node1Lock = a;

            // remove n dot
            if (IsHaveDifferentDotInRope() && RopeNodes.Count > 2) ;
            else if (RopeNodes.Count > GetLeftDotCount()) ;
            else break;

            if(RopeNodes.Count > 20)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                for (int i = 0; i < listObstacle.Count; i++) listObstacle[i].isMove = true;
                yield return new WaitForSeconds(0.2f);
            }       
        }

        while (IsHaveDifferentDotInRope())
        {
            if (listObstacle.Count > 0 || RopeNodes.Count <= 1) break;
            yield return null;
        }

        for (int i = 0; i < listObstacle.Count; i++) listObstacle[i].isMove = false;
        for (int i = 0; i < listDot.Count; i++) listDot[i].gameObject.layer = 6;

        yield return new WaitForSeconds(0.6f);
        RopeMultiplyDotGP.Instance.DoneMerge();
    }

    public void ShowUIPopup()
    {
        if (ballAmount <= 0) return;
        numberRopeText.transform.parent.parent.transform.position = GetPivotRopeNodes() - Vector3.forward * 2.0f;
        numberRopeText.transform.parent.GetChild(4).GetComponent<Image>().color = ReferenceManager.Instance.GetColor((ReferenceManager.TypeColor)myID);
        numberRopeText.text = ballAmount.ToString();
        StartCoroutine(C_ShowUIPOpup());
    }

    private IEnumerator C_ShowUIPOpup()
    {
        numberRopeText.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        numberRopeText.transform.parent.gameObject.SetActive(false);
    }

    private int GetLeftDotCount()
    {
        sumDotRadius = GetSumDotRadius();
        int r = (int)sumDotRadius;
        float tongdientich = 0.0f;
        for(int i = 0; i < listDot.Count;i++) tongdientich += (listDot[i].transform.localScale.x * listDot[i].transform.localScale.x) * 3.14f;  // tong dien tich
        float h1 = 2.0f * (Mathf.Sqrt(tongdientich) / Mathf.Sqrt(3.14f));
        float ratio = 0.45f;
        if (listDot.Count > 8) ratio = 0.4f;
        else if (listDot.Count > 14) ratio = 0.35f;
        else if (listDot.Count > 20) ratio = 0.3f;
        float h2 = 8 + h1 * 8 * ratio;
        int leftDotCount = (int)h2;
        return leftDotCount;
    }

    private bool IsHaveDifferentDotInRope()
    {
        List<int> listDotID = new List<int>();
        for (int i = 0; i < listDot.Count; i++)
        {
            int _dotID = listDot[i].myID;
            if (!listDotID.Contains(_dotID) && _dotID != 0)
            {
                listDotID.Add(_dotID);
            }
        }

        if (listObstacle.Count > 0) return true;

        return listDotID.Count <= 1 ? false : true;
    }

    public void CaculateDot()
    {
        int multiplyAmount = 0;
        int plus = 0;
        int manus = 0;
        int dotAmount = 0;
        myID = 8;

        for (int i = 0; i < listDot.Count; i++)
        {
            Multiply multiply = listDot[i].GetComponent<Multiply>();
            if (multiply != null)
            {
                if(multiply.caculateIndex == 0)
                {
                    plus += multiply.multiplyNumber;
                }
                else if(multiply.caculateIndex == 1)
                {
                    manus += multiply.multiplyNumber;
                }
                else
                {
                    multiplyAmount += multiply.multiplyNumber;
                }
            }
            else
            {
                dotAmount++;
                myID = listDot[i].myID;
            }
        }

        for (int i = 0; i < listDot.Count; i++) listDot[i].HideNoEffect();
        listDot.Clear();

        if (multiplyAmount == 0) multiplyAmount = 1;
        ballAmount = (dotAmount * multiplyAmount) + plus - manus;
        if (ballAmount < 0) ballAmount = 0;
        transform.GetChild(0).gameObject.SetActive(false);
        BallController.Instance.SpawnBall(ballAmount, myID, GetPivotRopeNodes(), this);
        LineRenderer.material = ReferenceManager.Instance.GetMaterialColor(myID);

        isFree = true;
    }

    public void SpawnToScaleUpRope()
    {
        Vector3 pos = RopeNodes.Count % 2 == 0 ? RopeNodes[0].transform.position : RopeNodes[RopeNodes.Count - 1].transform.position;
        GenerateRopeNode(pos, NodeDistance,true);
    }

    public List<Vector2> RopeNodePoint()
    {
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < RopeNodes.Count; i++)
        {
            result.Add(RopeNodes[i].transform.position);
        }

        return result;
    }

    public Vector3 GetNearestRopeNode(Dot dot)
    {
        float distance = 999;
        Vector3 result = Vector3.zero;

        for(int i = 0; i < RopeNodes.Count; i++)
        {
            float d = Vector3.Distance(dot.transform.position, RopeNodes[i].transform.position);
            if(distance > d)
            {
                distance = d;
                result = RopeNodes[i].transform.position;
            }
        }

        return result;
    }

    public Vector3 GetPivotRopeNodes()
    {
        Vector3 pivot = Vector3.zero;
        for (int i = 0; i < RopeNodes.Count; i++) pivot += RopeNodes[i].transform.position;
        if(RopeNodes.Count != 0) pivot /= RopeNodes.Count;
        return pivot;
    }
   
    public Vector3 GetPivotDot()
    {
        Vector3 pivot = Vector3.zero;
        for (int i = 0; i < listDot.Count; i++) pivot += listDot[i].transform.position;
        if(listDot.Count != 0) pivot /= listDot.Count;
        return pivot;
    }

    public Vector3 GetDotHightestPosY()
    {
        Vector3 result = new Vector3(0.0f,-999.0f,0.0f);
        for (int i = 0; i < listDot.Count; i++) if (result.y < listDot[i].transform.position.y) result = listDot[i].transform.position;
        return result;
    }

    public float GetSumDotRadius()
    {
        float result = 0.0f;
        for (int i = 0; i < listDot.Count; i++) result += listDot[i].transform.localScale.x;
        return result;
    }

    private void UpdateRopeNodeStep()
    {
        Simulate();

        for (int i = 0; i < 6; i++)
        {
            ApplyConstraint();

            if (i % 2 == 1)
                AdjustCollisions();
        }

        DrawRope();
    }

    public void ActiveRope()
    {
        AutoSpawnLastRopeNode();
        HideAllRopeNode();

        Node1Lock = RopeNodes[0].transform.position;

        isActive = true;
        isGravity = false;
        isSimulate = true;

        ActiveScaleDown();
    }

    private void HideAllRopeNode()
    {
        for (int i = 0; i < RopeNodes.Count; i++) RopeNodes[i].spr.enabled = false;
    }

    public void SetRope()
    {
        myStep = RopeMultiplyDotGP.Instance.drawStep;
        myID = 8;
        ropeColor = ReferenceManager.Instance.GetColor(myID);
        LineRenderer.material = ReferenceManager.Instance.GetMaterialColor(myID);
    }

    public void GenerateRopeNode(Vector3 position,float nodeDistance,bool isHideDot)
    {
        NodeDistance = nodeDistance;

        RopeNode node = Instantiate(ropeNodePrefab);
        node.transform.SetParent(transform);
        node.transform.position = position;
        node.PreviousPosition = position;
        node.SetColor(ropeColor);
        node.spr.enabled = !isHideDot;
        RopeNodes.Add(node);
        LinePositions = new Vector3[RopeNodes.Count];
        TotalNodes = RopeNodes.Count;
        UpdateRopeNodeRotation();
    }

    private void UpdateRopeNodeRotation()
    {
        int a, b = 0;

        for (int i = 0;i < RopeNodes.Count; i++)
        {
            if(i < RopeNodes.Count - 1)
            {
                a = i;
                b = i + 1;
            }
            else
            {
                a = i;
                b = 0;
            }

            Vector3 dir = RopeNodes[a].transform.position - RopeNodes[b].transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            RopeNodes[a].transform.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
        }
    }

    private void AutoSpawnLastRopeNode()
    {
        while (true)
        {
            Vector3 lastNode = RopeNodes[RopeNodes.Count - 1].transform.position;
            Vector3 dir = RopeNodes[0].transform.position - lastNode;
            Vector3 newNode = lastNode + dir.normalized * NodeDistance;
            Vector3 dir2 = RopeNodes[0].transform.position - newNode;

            float distance = Vector3.Distance(RopeNodes[0].transform.position, newNode);
            float dot = Vector3.Dot(dir,dir2);

            if (dot > 0.0f)
            {
                GenerateRopeNode(newNode, NodeDistance,true);
            }
            else
            {
                newNode = RopeNodes[0].transform.position;
                GenerateRopeNode(newNode, NodeDistance,true);
                break;
            }
        }
    }

    private void Simulate()
    {
        // step each node in rope
        for (int i = 0; i < TotalNodes; i++)
        {            
            // derive the velocity from previous frame
            Vector3 velocity = RopeNodes[i].transform.position - RopeNodes[i].PreviousPosition;
            if (!isSwing) velocity = Vector3.zero;
            RopeNodes[i].PreviousPosition = RopeNodes[i].transform.position;

            Gravity = (isGravity) ? new Vector2(0.0f, -5.0f) : Vector2.zero;
            // calculate new position
            Vector3 newPos = RopeNodes[i].transform.position + velocity;
            newPos += Gravity * Time.fixedDeltaTime;
            Vector3 direction = RopeNodes[i].transform.position - newPos;
                        
            // cast ray towards this position to check for a collision
            int result = -1;
            result = Physics2D.CircleCast(RopeNodes[i].transform.position, RopeNodes[i].transform.localScale.x / 2f, -direction.normalized, ContactFilter, RaycastHitBuffer, direction.magnitude);

            if (result > 0)
            {
                for (int n = 0; n < result; n++)
                {                    
                    if (RaycastHitBuffer[n].collider.gameObject.layer == layer)
                    {
                        Vector2 collidercenter = new Vector2(RaycastHitBuffer[n].collider.transform.position.x, RaycastHitBuffer[n].collider.transform.position.y);
                        Vector2 collisionDirection = RaycastHitBuffer[n].point - collidercenter;
                        // adjusts the position based on a circle collider
                        Vector2 hitPos = collidercenter + collisionDirection.normalized * (RaycastHitBuffer[n].collider.transform.localScale.x / 2f + RopeNodes[i].transform.localScale.x / 2f);
                        newPos = hitPos;
                        break;              //Just assuming a single collision to simplify the model
                    }
                }
            }

            RopeNodes[i].transform.position = newPos;
        }
    }
    
    private void AdjustCollisions()
    {
        // Loop rope nodes and check if currently colliding
        for (int i = 0; i < TotalNodes - 1; i++)
        {
            RopeNode node = this.RopeNodes[i];

            int result = -1;
            result = Physics2D.OverlapCircleNonAlloc(node.transform.position, node.transform.localScale.x / 2f, ColliderHitBuffer, layer);

            if (result > 0)
            {
                for (int n = 0; n < result; n++)
                {
                    if (ColliderHitBuffer[n].gameObject.layer != 8)
                    {
                        // Adjust the rope node position to be outside collision
                        Vector3 collidercenter = ColliderHitBuffer[n].transform.position;
                        Vector3 collisionDirection = node.transform.position - collidercenter;

                        Vector3 hitPos = collidercenter + collisionDirection.normalized * ((ColliderHitBuffer[n].transform.localScale.x / 2f) + (node.transform.localScale.x / 2f));
                        node.transform.position = hitPos;
                        break;
                    }
                }
            }
        }    
    }

    private void ApplyConstraint()
    {
        // Check if the first node is clamped to the scene or is follwing the mouse
     
        if(isFree == false)
        {
            RopeNodes[0].transform.position = Node1Lock;
            RopeNodes[RopeNodes.Count - 1].transform.position = Node1Lock;
        }
        else
        {
            RopeNodes[RopeNodes.Count - 1].transform.position = RopeNodes[0].transform.position;
        }


        for (int i = 0; i < TotalNodes - 1; i++)
        {
            RopeNode node1 = this.RopeNodes[i];
            RopeNode node2 = this.RopeNodes[i + 1];

            // Get the current distance between rope nodes
            float currentDistance = (node1.transform.position - node2.transform.position).magnitude;
            float difference = Mathf.Abs(currentDistance - NodeDistance);
            Vector2 direction = Vector2.zero;
           
            // determine what direction we need to adjust our nodes
            if (currentDistance > NodeDistance)
            {
                direction = (node1.transform.position - node2.transform.position).normalized;
            }
            else if (currentDistance < NodeDistance)
            {
                direction = (node2.transform.position - node1.transform.position).normalized;
            }

            // calculate the movement vector
            Vector3 movement = direction * difference;

            // apply correction
            node1.transform.position -= (movement * 0.5f);
            node2.transform.position += (movement * 0.5f);
        }
    }

    public void DrawRope()
    {
        for (int n = 0; n < TotalNodes; n++)
        {
            LinePositions[n] = new Vector3(RopeNodes[n].transform.position.x, RopeNodes[n].transform.position.y, 0);
        }

        LineRenderer.positionCount = LinePositions.Length;
        LineRenderer.SetPositions(LinePositions);
    }

}
