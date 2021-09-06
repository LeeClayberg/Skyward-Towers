using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPiece : MonoBehaviour
{
    public static Color fadeToColor;
    Color startingColor;
    Color endingColor;
    float startTime;
    float endAfter;

    private void Start()
    {
        startingColor = gameObject.GetComponent<Renderer>().material.color;
        startTime = Time.time;
        endAfter = Random.value * 0.15f;
        if (StoreController.explosionEffect == 1)
        {
            endingColor = fadeToColor;
        }
        else if (StoreController.explosionEffect == 0)
        {
            endingColor = startingColor;
        }
        else if (StoreController.explosionEffect == 2)
        {
            endingColor = Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
        }
    }

    private void Update()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.Lerp(startingColor, endingColor, Mathf.Pow((Time.time - startTime) / endAfter, 4));
        if (Time.time - startTime > endAfter)
        {
            Destroy(gameObject);
        }
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (Vector3.Distance(rb.velocity, Vector3.zero) < 0.05f)
        {
            Destroy(gameObject);
        }
    }

    public void setWaitPeriod(float value)
    {
        endAfter = value;
    }
}
