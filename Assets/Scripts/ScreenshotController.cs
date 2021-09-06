using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotController : MonoBehaviour
{
    public string filename;
    public int counter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ScreenCapture.CaptureScreenshot(filename + "(" + counter.ToString() + ").png");
            counter++;
        }
    }
}
