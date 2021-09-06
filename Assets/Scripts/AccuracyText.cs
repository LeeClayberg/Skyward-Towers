using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyText : MonoBehaviour
{
    public float playLength;
    public float multiplier;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime < playLength)
        {
            float func = (-((Time.time - startTime) / playLength) + 0.5f) * multiplier;
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.up * func, Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
