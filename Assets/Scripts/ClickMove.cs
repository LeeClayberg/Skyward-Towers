using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMove : MonoBehaviour
{
    public bool clicked;
    public float retractAmount;
    public Vector3 direction = Vector3.down;

    bool started;
    float timeStart;
    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (clicked)
        {
            if (!started)
            {
                transform.localPosition = transform.localPosition + direction * retractAmount;
                timeStart = Time.time;
                started = true;
            }
            if (Time.time - timeStart > 0.10f)
            {
                transform.localPosition = startPosition;
                clicked = false;
                started = false;
            }
        }
    }
}
