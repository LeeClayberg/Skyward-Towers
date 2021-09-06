using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EndScreenController : MonoBehaviour
{
    public static int score;
    public static int accuracyExcellent;
    public static int accuracyPerfect;
    public static int accuracyUnsteady;

    public static bool remoteOut;
    public static bool toggleRemote;

    public float movementSpeed;
    public float delay;
    public float appearSpeed;
    public HighScoreTracker highScoreTracker;

    public TextMeshPro[] summaryText;

    bool playSound;
    int counter;
    float lastTime;
    TextMeshPro tmp;
    int pointAddRate;

    void Start()
    {
        tmp = transform.Find("Points").GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        tmp.text = "Points: " + (StoreController.points - StoreController.pointsToBeAdded).ToString();
        if (remoteOut) {
            if (Time.time - lastTime > delay && counter < summaryText.Length)
            {
                AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[13];
                a_s.PlayOneShot(a_s.clip, 0.004f);
                counter += 1;
                lastTime = Time.time;
            }
            for(int i = 0; i < counter; i++)
            {
                summaryText[i].color = new Color(summaryText[i].color.r, summaryText[i].color.g, summaryText[i].color.b, Mathf.Min(summaryText[i].color.a + appearSpeed, 1.0f));
            }
            if (summaryText[summaryText.Length - 1].color.a > 0.99f)
            {
                if (StoreController.pointsToBeAdded > 0)
                {
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[14];
                    a_s.PlayOneShot(a_s.clip, 0.05f);
                    StoreController.pointsToBeAdded -= Math.Min(pointAddRate, StoreController.pointsToBeAdded);
                }
            }
        }

        // Remote Movement
        if (toggleRemote && !RemoteController.remoteOut)
        {
            if (!playSound)
            {
                AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[8];
                a_s.PlayOneShot(a_s.clip, 0.05f);
                playSound = true;
            }
            float speedFunc = 0.75f * Mathf.Pow(transform.localPosition.x + 0.5f, 1);
            if (remoteOut)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.right, Time.deltaTime * speedFunc * movementSpeed);
                if (transform.localPosition.x > 7)
                {
                    remoteOut = false;
                    toggleRemote = false;
                    playSound = false;
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.left, Time.deltaTime * speedFunc * movementSpeed);
                if (Vector3.Distance(transform.localPosition, new Vector3(0, -2.55f, 10)) < 0.05f)
                {
                    remoteOut = true;
                    toggleRemote = false;
                    playSound = false;
                    lastTime = Time.time;
                }
            }
            if (Mathf.Abs(transform.localPosition.x - 2.0f) < 0.4f)
            {
                highScoreTracker.UpdateBillboard();
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

    public void UpdateSummary()
    {
        counter = 0;
        foreach (TextMeshPro tmp2 in summaryText)
        {
            tmp2.color = new Color(tmp2.color.r, tmp2.color.g, tmp2.color.b, 0);
        }
        int total = (score + accuracyUnsteady + (2 * accuracyExcellent) + (3 * accuracyPerfect)) * (CraneMovement.gameType == GameType.Hardcore ? 2 : 1);
        summaryText[0].text = "Score (<#" + (CraneMovement.gameType == GameType.Hardcore ? "DD0101>Hardcore" : "008418>Normal") + "</color>)";
        summaryText[1].text = score.ToString();
        summaryText[3].text = "<#FF00D7>Unsteady:</color> " + accuracyUnsteady.ToString();
        summaryText[4].text = "<#0088DD>Excellent:</color> " + accuracyExcellent.ToString();
        summaryText[5].text = "<#DDA100>Perfect:</color> " + accuracyPerfect.ToString();
        summaryText[7].text = total.ToString();
        pointAddRate = (StoreController.pointsToBeAdded + 57) / 20;
    }

    public static void Reset()
    {
        score = 0;
        accuracyExcellent = 0;
        accuracyPerfect = 0;
        accuracyUnsteady = 0;
    }
}