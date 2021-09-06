using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoController : MonoBehaviour
{
    public static bool remoteOut;
    public static bool toggleRemote;

    public float movementSpeed;

    bool playSound;

    // Update is called once per frame
    void Update()
    {
        // Remote Movement
        if (toggleRemote && !RemoteController.remoteOut)
        {
            if (!playSound)
            {
                AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[8];
                a_s.PlayOneShot(a_s.clip, 0.05f);
                playSound = true;
            }
            float speedFunc = 0.75f * Mathf.Pow(-transform.localPosition.x + 0.5f, 1);
            if (remoteOut)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.left, Time.deltaTime * speedFunc * movementSpeed);
                if (transform.localPosition.x < -7)
                {
                    remoteOut = false;
                    toggleRemote = false;
                    playSound = false;
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.right, Time.deltaTime * speedFunc * movementSpeed);
                if (Vector3.Distance(transform.localPosition, new Vector3(0, -3.0f, 10)) < 0.05f)
                {
                    remoteOut = true;
                    toggleRemote = false;
                    playSound = false;
                }
            }
        }

        // Clicking on buttons
        if (Input.GetMouseButtonDown(0) && remoteOut && !toggleRemote)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.name == "LeftButton" && hit.transform.parent == transform)
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    RemoteController.toggleRemote = true;
                    toggleRemote = true;
                }
            }
        }
    }
}
