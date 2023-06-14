using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableByDistance : DisableSystem
{
    [SerializeField] private float DistanceDisable = 1.0f;
    [SerializeField] private AnchorType anchorType;
    private Vector3 AnchorPoint;

    public enum AnchorType
    {
        MyStartPosition,
        Camera,
        Player
    }

    private void OnEnable()
    {
        InitAnchorPoint();
    }

    private void Update()
    {
        UpdateAnchor();
        UpdateStep();
    }

    private void InitAnchorPoint()
    {
        if(anchorType == AnchorType.MyStartPosition) AnchorPoint = transform.position;
    }

    private void UpdateAnchor()
    {
        switch (anchorType)
        {
            case AnchorType.MyStartPosition:
   
                break;
            case AnchorType.Camera:
                AnchorPoint = ReferenceManager.Instance.cameraMain.position;
                break;
            case AnchorType.Player:
                AnchorPoint = ReferenceManager.Instance.player.position;
                break;
        }
    }

    private void UpdateStep()
    {
        float distance = Vector3.Distance(transform.position, AnchorPoint);
        if(distance >= DistanceDisable)
        {
            DisableGameObject(gameObject);
        }
    }

    protected override void DisableGameObject(GameObject go)
    {
        base.DisableGameObject(go);
    }
}
