using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoverData : MonoBehaviour
{
    public int cost;
    public bool openCover;
    public bool isOpen;
    public int playerPrefsIndex;

    float speed = 80;
    float total;

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("Cover").Find("Price").GetComponent<TextMeshPro>().text = cost.ToString();
    }

    void Update()
    {
        if (openCover && !isOpen)
        {
            if (total < 100)
            {
                transform.Find("Cover").Rotate(-speed * Time.deltaTime, 0, 0);
                total += speed * Time.deltaTime;
            }
            else
            {
                isOpen = true;
            }
        }
    }

    public void setOpen()
    {
        if (isOpen)
        {
            transform.Find("Cover").Rotate(-100, 0, 0);
        }
    }
}
