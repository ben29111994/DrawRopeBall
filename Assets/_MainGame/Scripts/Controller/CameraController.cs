using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public RectTransform[] topUI;

    private void Start()
    {
        SetCorrectCamera();
    }

    private void SetCorrectCamera()
    {
        float ratio = Camera.main.aspect;

        if (ratio >= 0.74) // 3:4
        {
            Camera.main.fieldOfView = 60;
        }
        else if (ratio >= 0.56) // 9:16
        {
            Camera.main.fieldOfView = 60;
        }
        else if (ratio >= 0.45) // 9:19
        {
            Camera.main.fieldOfView = 65;

            foreach (RectTransform r in topUI)
            {
                Vector2 current = r.anchoredPosition;
                current.y -= 150.0f;
                r.anchoredPosition = current;
            }
        }
    }
}
