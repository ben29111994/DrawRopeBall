using UnityEngine;

public class RopeNode : MonoBehaviour
{
    public Vector3 PreviousPosition;
    public SpriteRenderer spr;

    public void SetColor(Color color)
    {
        color.a = 0.0f;
        spr.color = color;
    }
}