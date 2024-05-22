using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    public static ControlPlayer instance;
    Vector2 startPoint;
    Vector2 endPoint;

    void Awake()
    {
        instance = this;
    }
    public void FindDirection(ref Direct direct)
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            endPoint = Input.mousePosition;
            Vector2 tempVector = endPoint - startPoint;
            float xLenght = Mathf.Abs(tempVector.x);
            float yLenght = Mathf.Abs(tempVector.y);

            if (xLenght > yLenght)
            {
                if (tempVector.x > 0)
                {
                    direct = Direct.Right;
                }
                else
                {
                    direct = Direct.Left;
                }
            }
            else
            {
                if (tempVector.y > 0)
                {
                    direct = Direct.Forward;
                }
                else
                {
                    direct = Direct.Back;
                }
            }
        }
    }
}
public enum Direct
{
    Forward = 0,
    Back = 1,
    Right = 2,
    Left = 3,
    None = 4
}
