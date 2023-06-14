using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour
{
    private static TouchController instance;
    public static TouchController Instance { get { return instance; } }

    [Header("Status")]
    public LayerMask layer;
    public float nodeDistance;
    public int maxRope;

    [Header("References")]
    public Image inkImage;
    public Rope ropePrefab;
    public List<Rope> listRope = new List<Rope>();
    public LineRenderer lineRenderer;

    private Rope currentRope;
    private List<Vector3> listTouchPoint = new List<Vector3>();
    private Vector3 lastPoint;
    private Vector3 currentPoint;
    private bool isTouchDown;

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    private void Update()
    {
        if (GameManager.Instance.typeGamePlay != GameManager.TypeGamePlay.DrawRope) return;

        UpdateTouch();
        UpdateLineRenderer();
    }

    private void UpdateTouch()
    {
        if (GameManager.Instance.isComplete) return;
        if (RopeMultiplyDotGP.Instance.phaseGame != RopeMultiplyDotGP.PhaseGame.DrawRope) return;

        if (Input.GetMouseButtonDown(0))
        {
            inkImage.fillAmount = 1.0f;
            currentRope = Instantiate(ropePrefab);
            currentRope.stt = listRope.Count;
            listRope.Add(currentRope);
            currentPoint = lastPoint = TouchPoint();
            listTouchPoint.Add(currentPoint);
            currentRope.SetRope();
            currentRope.GenerateRopeNode(currentPoint, nodeDistance, false);
            PenController.Instance.UpdatePosition(currentPoint);
            isTouchDown = true;
            return;
        }
        else if (Input.GetMouseButton(0) && isTouchDown)
        {
            currentPoint = TouchPoint();
            PenController.Instance.UpdatePosition(currentPoint);
            inkImage.fillAmount = (GetMaxInk() - currentRope.RopeNodes.Count) / GetMaxInk();

            if (currentRope.RopeNodes.Count > GetMaxInk())
            {
                RopeMultiplyDotGP.Instance.NextStep();
                lineRenderer.positionCount = 0;
                listTouchPoint.Clear();
                isTouchDown = false;
                return;
            }
        }
        else if (Input.GetMouseButtonUp(0) && isTouchDown)
        {
            if (listTouchPoint.Count < 10)
            {
                listRope.Remove(currentRope);
                Destroy(currentRope.gameObject);
            }
            else
            {
                RopeMultiplyDotGP.Instance.NextStep();
            }

            inkImage.fillAmount = 1.0f;
            lineRenderer.positionCount = 0;
            listTouchPoint.Clear();
            isTouchDown = false;
        }
        else
        {
            return;
        }

        float distance = Vector3.Distance(currentPoint, lastPoint);
        if(distance >= nodeDistance)
        {
            Vector3 direction = currentPoint - lastPoint;
            Vector3 newPoint = lastPoint + direction.normalized * nodeDistance;
            currentPoint = lastPoint = newPoint;
            listTouchPoint.Add(currentPoint);
            currentRope.GenerateRopeNode(currentPoint, nodeDistance,false);
            currentRope.DrawRope();
        }
    }

    public void PhaseMerge()
    {
        TouchController.Instance.inkImage.transform.parent.gameObject.SetActive(false);
        for (int i = 0; i < listRope.Count; i++) listRope[i].ActiveRope();
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = listTouchPoint.Count;
        lineRenderer.SetPositions(listTouchPoint.ToArray());
    }

    private Vector3 TouchPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, layer))
        {
            Vector3 result = hit.point;
            result.z = 0.0f;
            return result;
        }
        return Vector3.zero;
    }

    public void Refresh()
    {
        for(int i = 0; i < listRope.Count; i++)
        {
            if(listRope[i] != null)
            Destroy(listRope[i].gameObject);
        }

        listRope.Clear();
        listTouchPoint.Clear();
        lineRenderer.positionCount = 0;
        currentRope = null;
        currentPoint = lastPoint = Vector3.zero;
    }

    private float GetMaxInk()
    {
        if (RopeMultiplyDotGP.Instance.maxStep <= 2) return 160.0f;
        else if (RopeMultiplyDotGP.Instance.maxStep == 3) return 130.0f;
        else if (RopeMultiplyDotGP.Instance.maxStep == 4) return 110;
        else return 100.0f;
    }

}
