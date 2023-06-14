using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenController : MonoBehaviour
{
    private static PenController instance;
    public static PenController Instance { get { return instance; } }

    public TypeAnimation typeAnimation;
    public Transform model;
    public Animator anim;

    public Material[] m_penArray;

    public enum TypeAnimation
    {
        Idle,
        Draw
    }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }


    public void Refresh()
    {
        model.transform.position = new Vector3(-70.0f,0.0f, 0.0f);
        anim.GetComponent<Renderer>().material = m_penArray[1];
        SetAnimation(TypeAnimation.Idle);
        ActivePen(true);
    }

    public void Refresh(int penColor)
    {
        if (penColor >= m_penArray.Length) return;
        anim.GetComponent<Renderer>().material = m_penArray[penColor];
        model.transform.position = new Vector3(-70.0f, 0.0f, 0.0f);
        SetAnimation(TypeAnimation.Idle);
    }

    public void SetAnimation(TypeAnimation _typeAnimation)
    {
        if (typeAnimation == _typeAnimation) return;
        typeAnimation = _typeAnimation;
        string theName = typeAnimation.ToString();
        anim.SetTrigger(theName);
    }

    public void UpdatePosition(Vector3 pos)
    {
        SetAnimation(TypeAnimation.Draw);
        pos.z = model.transform.position.z;
        model.transform.position = pos;
    }

    public void ActivePen(bool _isActove)
    {
        gameObject.SetActive(_isActove);
    }
}
