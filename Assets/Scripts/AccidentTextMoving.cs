using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccidentTextMoving : MonoBehaviour
{
    public static bool startSignal;
    public static bool endSignal;
    public float curve;
    public float speedMultiplier;

    // Update is called once per frame
    void Update() {
        if (startSignal)
        {
            float speedChange = curve * Mathf.Pow(transform.localPosition.x - 2, 4) + 0.5f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.right, Time.deltaTime * speedChange * speedMultiplier);
            if (transform.localPosition.x > 58.0f && !endSignal)
            {
                transform.localPosition = new Vector3(-58.0f, 12.0f, 0);
            }
        }
    }
}
