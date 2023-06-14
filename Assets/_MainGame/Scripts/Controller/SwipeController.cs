using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    private static SwipeController instance;
    public static SwipeController Instance { get { return instance; } }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    private float magnitudeSwipe = 25.0f;
    private bool isSwipe;
    private Vector2 currentTouchPosition;
    private Vector2 lastTouchPosition;
    private Vector2 deltaTouchPosition;

    private void Update()
    {
        UpdateSwipe();
    }

    private void UpdateSwipe()
    {
        bool touchBegan = false;
        bool touchMoved = false;
        bool touchEnded = false;



#if UNITY_EDITOR
        touchBegan = Input.GetMouseButtonDown(0);
        touchMoved = Input.GetMouseButton(0);
        touchEnded = Input.GetMouseButtonUp(0);
#elif UNITY_IOS
        if(Input.touchCount > 0)
        {
            touchBegan = Input.touches[0].phase == TouchPhase.Began;
            touchMoved = Input.touches[0].phase == TouchPhase.Moved;
            touchEnded = Input.touches[0].phase == TouchPhase.Ended;
        }
#endif

        if (touchBegan)
        {
            currentTouchPosition = lastTouchPosition = Input.mousePosition;
        }
        else if (touchMoved && isSwipe == false)
        {
            currentTouchPosition = Input.mousePosition;
            deltaTouchPosition = currentTouchPosition - lastTouchPosition;
            lastTouchPosition = currentTouchPosition;

            if (deltaTouchPosition.magnitude >= magnitudeSwipe)
            {
                isSwipe = true;

                float x = deltaTouchPosition.x;
                float y = deltaTouchPosition.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    if (x < 0)
                    {
                        SwipeLeft();
                    }
                    else
                    {
                        SwipeRight();
                    }
                }
                else
                {
                    if (y < 0)
                    {
                        SwipeDown();
                    }
                    else
                    {
                        SwipeUp();
                    }
                }
            }
        }
        else if (touchEnded)
        {
            currentTouchPosition = lastTouchPosition = deltaTouchPosition = Vector2.zero;
            isSwipe = false;
        }
    }

    private void SwipeLeft()
    {
        // Debug.Log("swipe left");
    }

    private void SwipeRight()
    {
        //    Debug.Log("swipe right");
    }

    private void SwipeUp()
    {
        //  Debug.Log("swipe up");
    }

    private void SwipeDown()
    {
        // Debug.Log("swipe down");
    }
}
