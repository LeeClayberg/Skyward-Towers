using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clamp : MonoBehaviour
{
    public Vector3 endPosition;
    public Vector3 startPosition;

    bool opening;
    static float speed = 3.0f;

    public bool fullyOpened;

    // Update is called once per frame
    void Update()
    {
        if (opening)
        {
            if (Vector3.Distance(transform.localPosition, endPosition) < 0.01f)
            {
                fullyOpened = true;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, endPosition, Time.deltaTime * speed);
            }
        }
    }

    public void Reset()
    {
        transform.localPosition = startPosition;
        opening = false;
        fullyOpened = false;
    }

    public void Open()
    {
        opening = true;
    }
}
