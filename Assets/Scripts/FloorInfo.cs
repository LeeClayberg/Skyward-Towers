using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FloorInfo : MonoBehaviour
{
    public static Color levelNumberColor;
    public static Color lastColor;
    public static float colorIncrement = 0.003921f;
    public static int highestFloor;
    public int floorNumber;
    public float windowFadeLength;
    public float freezeChangeLength;
    public float boomHeight;
    public int accuracy;
    public GameObject accuracyExcellent;
    public GameObject accuracyPerfect;
    public GameObject accuracyUnsteady;
    public ParticleSystem drippingParticles;
    public bool frozen;

    bool moving;
    Rigidbody rb;
    bool stopped;
    public bool fullyStopped;
    Color numberColor;
    Color extraWindowColor;
    Color outsideColor;
    Color roofColor;
    List<Renderer> renderers;
    float startTime;
    float freezeStartTime;
    Transform FloorParent;
    Vector3 lastFloorLocation;
    int num;
    float hitStartTime;
    Renderer outsideRenderer;
    Renderer roofRenderer;
    Renderer extraWindowRenderer;

    void Start()
    {
        if (StoreController.levelNumberEffect == 0)
        {
            numberColor = new Color(0.23f, 0.37f, 0.42f);
        }
        else if (StoreController.levelNumberEffect == 1)
        {
            numberColor = levelNumberColor;
        }
        else
        {
            if (floorNumber == 1)
            {
                numberColor = UnityEngine.Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
                lastColor = numberColor;
            }
            else
            {
                float h, s, v;
                Color.RGBToHSV(lastColor, out h, out s, out v);
                h -= 0.2f;
                if (h < 0.0f)
                {
                    h += 1.0f;
                }
                numberColor = Color.HSVToRGB(h, s, v);
                lastColor = numberColor;
            }
        }
        renderers = new List<Renderer>(transform.Find("Number").GetComponentsInChildren<Renderer>());
        renderers.Add(transform.Find("FirstLayer-Windows").GetComponent<Renderer>());
        outsideRenderer = transform.Find("FirstLayer").GetComponent<Renderer>();
        outsideColor = outsideRenderer.material.color;
        roofRenderer = transform.Find("FirstLayer-Top").GetComponent<Renderer>();
        roofColor = roofRenderer.material.color;
        extraWindowRenderer = transform.Find("FirstLayer-Windows2").GetComponent<Renderer>();
        extraWindowColor = extraWindowRenderer.material.color;
        FloorParent = GameObject.Find("Dropped Floors").transform;
        boomHeight = float.PositiveInfinity;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CraneMovement.misplacedFloor)
        {
            if (!frozen)
            {
                if (StoreController.levelNumberEffect == 2)
                {
                    float h, s, v;
                    Color.RGBToHSV(numberColor, out h, out s, out v);
                    h += colorIncrement;
                    if (h > 1.0f)
                    {
                        h = 0;
                    }
                    numberColor = Color.HSVToRGB(h, s, v);
                }
                freezeStartTime = Time.time;
            }
            else
            {
                numberColor = Color.Lerp(numberColor, new Color(0.26f, 0.49f, 0.58f), (Time.time - freezeStartTime) / freezeChangeLength);
                extraWindowColor = Color.Lerp(extraWindowColor, new Color(0.26f, 0.49f, 0.58f), (Time.time - freezeStartTime) / freezeChangeLength);
                roofColor = Color.Lerp(roofColor, new Color(0.40f, 0.64f, 0.73f), (Time.time - freezeStartTime) / freezeChangeLength);
                outsideColor = Color.Lerp(outsideColor, new Color(0.60f, 0.76f, 0.82f), (Time.time - freezeStartTime) / freezeChangeLength);
                if (Time.time - freezeStartTime > 1.0f) {
                    drippingParticles.Play();
                }
            }
            startTime = Time.time;
        }
        else
        {
            var e = drippingParticles.emission;
            e.rateOverTime = 0;
            numberColor = Color.Lerp(numberColor, new Color(0.23f, 0.37f, 0.42f), (Time.time - startTime) / windowFadeLength);
            extraWindowColor = Color.Lerp(extraWindowColor, new Color(0.23f, 0.37f, 0.42f), (Time.time - startTime) / windowFadeLength);
            roofColor = Color.Lerp(roofColor, new Color(0.30f, 0.30f, 0.30f), (Time.time - startTime) / windowFadeLength);
            outsideColor = Color.Lerp(outsideColor, new Color(0.61f, 0.61f, 0.61f), (Time.time - startTime) / windowFadeLength);
        }
        foreach (Renderer ren in renderers)
        {
            ren.material.color = numberColor;
        }
        outsideRenderer.material.color = outsideColor;
        roofRenderer.material.color = roofColor;
        extraWindowRenderer.material.color = extraWindowColor;

        rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Started drop
            if (rb.velocity.y < -5 && !moving)
            {
                moving = true;
                hitStartTime = Time.time;
            }
            // Floor full stop
            if (rb.velocity.y > -0.01f && Vector3.Distance(rb.angularVelocity, Vector3.zero) < 0.03f && moving && !fullyStopped && !CraneMovement.misplacedFloor)
            {
                // Accuracy Indictator
                if (FloorParent.childCount > 1)
                {
                    Transform lastSegment = FloorParent.GetChild(FloorParent.childCount - 2).transform;
                    lastFloorLocation = lastSegment.position + 2 * lastSegment.up;
                    num = lastSegment.GetComponent<FloorInfo>().floorNumber;
                }
                else
                {
                    lastFloorLocation = new Vector3(0, 12, 0);
                    num = 0;
                }
                if (num == floorNumber - 1)
                {
                    if (Vector3.Distance(transform.position, lastFloorLocation) < 0.15f)
                    {
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[10];
                        a_s.PlayOneShot(a_s.clip, 0.4f);
                        Instantiate(accuracyPerfect, transform.position + transform.up * 1.5f, transform.rotation, transform);
                        StoreController.points += CraneMovement.gameType == GameType.Hardcore ? 6 : 3;
                        StoreController.pointsToBeAdded += CraneMovement.gameType == GameType.Hardcore ? 6 : 3;
                        EndScreenController.accuracyPerfect += 1;
                        accuracy = 2;
                        Debug.Log("Perfect");
                    }
                    else if (Vector3.Distance(transform.position, lastFloorLocation) < 0.45f)
                    {
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[11];
                        a_s.PlayOneShot(a_s.clip, 0.4f);
                        Instantiate(accuracyExcellent, transform.position + transform.up * 1.5f, transform.rotation, transform);
                        StoreController.points += CraneMovement.gameType == GameType.Hardcore ? 4 : 2;
                        StoreController.pointsToBeAdded += CraneMovement.gameType == GameType.Hardcore ? 4 : 2;
                        EndScreenController.accuracyExcellent += 1;
                        accuracy = 1;
                        Debug.Log("Excellent");
                    }
                    else if (Time.time - hitStartTime > 2.0f && !CraneMovement.misplacedFloor)
                    {
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[12];
                        a_s.PlayOneShot(a_s.clip, 0.4f);
                        if (Vector3.Angle(transform.up, Vector3.up) < 90)
                        {
                            Instantiate(accuracyUnsteady, transform.position + transform.up * 1.5f, transform.rotation, transform);
                        }
                        else
                        {
                            Instantiate(accuracyUnsteady, transform.position + transform.up * 0.5f, transform.rotation * Quaternion.Euler(180, 0, 0), transform);
                        }
                        StoreController.points += CraneMovement.gameType == GameType.Hardcore ? 2 : 1;
                        StoreController.pointsToBeAdded += CraneMovement.gameType == GameType.Hardcore ? 2 : 1;
                        EndScreenController.accuracyUnsteady += 1;
                        accuracy = 2;
                        Debug.Log("Unsteady");
                    }
                }
                boomHeight = Mathf.Max(gameObject.GetComponent<Collider>().bounds.max.y + 16.0f, 28.0f);
                fullyStopped = true;
            }
            // Floor came to a good-enough stop
            if (rb.velocity.y > -4 && moving && !stopped)
            {
                // Accuracy Indictator (maybe change)
                boomHeight = Mathf.Max(gameObject.GetComponent<Collider>().bounds.max.y + 16.0f, 28.0f);
                transform.parent = FloorParent.transform;
                stopped = true;
            }
            if (CraneMovement.gameType == GameType.Normal && rb.velocity == Vector3.zero && rb.angularVelocity == Vector3.zero && moving)
            {
                Destroy(rb);
            }
        }
    }

    // Floor was misplaced causing the game to end
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("here");
        if (!CraneMovement.misplacedFloor)
        {
            if (CraneMovement.gameType == GameType.Normal)
            {
                EndScreenController.score = floorNumber - 1;
            }
            else
            {
                EndScreenController.score = fullyStopped ? FloorParent.childCount : floorNumber - 1;
            }
            GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[4].Play();
            StoreController.points += EndScreenController.score * (CraneMovement.gameType == GameType.Hardcore ? 2 : 1);
            StoreController.pointsToBeAdded += EndScreenController.score * (CraneMovement.gameType == GameType.Hardcore ? 2 : 1);
            PlayerPrefs.SetInt("_points", StoreController.points);
            PlayerPrefs.Save();
            HighScoreTracker.updateScore(EndScreenController.score);
            if (AbilityController.remoteOut)
            {
                AbilityController.toggleRemote = true;
            }
            GameObject.Find("Leaderboards").GetComponent<LeaderboardController>().SubmitScore(CraneMovement.gameType, EndScreenController.score);
            GameObject.Find("controllerEndScreen").GetComponent<EndScreenController>().UpdateSummary();
        }
        CraneMovement.misplacedFloor = true;
        AccidentTextMoving.startSignal = true;
    }

    void OnCollisionEnter(Collision col)
    {
        AudioSource a_s = transform.GetComponent<AudioSource>();
        if (!CraneMovement.blow)
        {
            a_s.PlayOneShot(a_s.clip, Mathf.Min(1.0f, 0.6f * col.relativeVelocity.magnitude / 20.0f));
        }
    }
}
