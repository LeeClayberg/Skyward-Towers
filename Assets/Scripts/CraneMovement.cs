using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public enum GameType { Normal, Hardcore };

public class CraneMovement : MonoBehaviour
{
    public static GameType gameType;
    public static bool misplacedFloor;
    public static bool isPlaying;
    public static bool blow;
    public static int TowerHeight;

    [Header("Testing")]
    public bool isTesting;

    [Header("Speeds")]
    public float Horizontalspeed;
    public float Verticalspeed;
    public float DetonatorMoveSpeed;
    public float DetonatorPlungerSpeed;
    public float MoveDownSpeed;
    public float SkipMoveSpeed;
    public static int floorNumber = 1;

    [Header("Game Objects")]
    public GameObject floor;
    public GameObject boomFloor;
    public GameObject explosionExtra;
    public GameObject[] numbers;
    public GameObject detonator;

    [Header("Exclude Clicks")]
    public string[] nonclickables;

    bool stopped;
    bool dropped = true;
    bool detonatorUp;
    bool soundUp;
    bool clickedDetonator;
    bool isDelay;
    bool soundDown;
    bool detonator_mute;
    bool blowFirst;
    bool hasSkipped;
    public bool closeSkip;
    GameObject fallingFLoors;
    GameObject droppedFloors;
    GameObject explodedFloors;
    GameObject destroyQueue;
    GameObject currentlyExploding;
    GameObject buttonHolder;
    GameObject skipCover;
    Transform trolley;

    Clamp clamp1;
    Clamp clamp2;

    public float maxCraneHeight = 28.0f;
    float timeDelayStart;
    float skipDelayStart;
    float resetSpeedStart;
    float lastBoom;
    float rotateCounter;

    float FLOOR_NUMBER_LENGTH = 2.4f;
    float FLOOR_NUMBER_SPACING_CONSTANT = 0.1f;
    float EXTRA_SCALE_FACTOR = 50.0f;

    AudioSource crane_moving;
    AudioSource alarm;
    AudioSource detonator_sound;
    bool justStarted = true;
    float horizontalSpeedWhenReset;
    float verticalSpeedWhenReset;
    bool resetSpeed;

    void Reset()
    {
        // Reset Variables
        Horizontalspeed = 8;
        Verticalspeed = 0.4f;
        floorNumber = 1;
        stopped = false;
        dropped = true;
        misplacedFloor = false;
        detonatorUp = false;
        clickedDetonator = false;
        blow = false;
        isDelay = false;
        maxCraneHeight = 28.0f;
        justStarted = true;
        crane_moving.volume = 0.15f;
        soundUp = false;
        soundDown = false;
        detonator_mute = false;
        blowFirst = false;
        closeSkip = false;
        hasSkipped = false;
        lastBoom = 0.0f;
        rotateCounter = 0.0f;
        currentlyExploding = null;
        FloorInfo.highestFloor = 0;
        // Reset Message
        AccidentTextMoving.startSignal = false;
        AccidentTextMoving.endSignal = false;
        transform.Find("Accident Text").localPosition = new Vector3(-45, 12, 0);
        // Reset directions
        trolley.Find("directionsText").GetComponent<MeshRenderer>().enabled = false;
        // Reset trolley and crane
        transform.position = new Vector3(0, 28, 0);
        trolley.localPosition = new Vector3(0, 2, 0);
        // Reset detonator
        detonator.transform.Find("Plunger").localPosition = new Vector3(0, 8.2f, 0);
        // Remove all detonated floors
        foreach (Transform child in GameObject.Find("Exploded Floors").transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        buttonHolder = GameObject.Find("ButtonHolder");
        skipCover = GameObject.Find("SkipCover");
        trolley = transform.Find("Trolley");
        clamp1 = trolley.Find("Clamp1").GetComponent<Clamp>();
        clamp2 = trolley.Find("Clamp2").GetComponent<Clamp>();
        fallingFLoors = new GameObject("Falling Floors");
        droppedFloors = new GameObject("Dropped Floors");
        explodedFloors = new GameObject("Exploded Floors");
        destroyQueue = new GameObject("Destroy Queue");
        crane_moving = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[2];
        alarm = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[4];
        detonator_sound = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[9];
    }

    // Update is called once per frame
    void Update()
    {
        // Color changer (move somewhere eventually)
        float h, s, v;
        Color.RGBToHSV(FloorInfo.lastColor, out h, out s, out v);
        h += FloorInfo.colorIncrement;
        if (h > 1.0f)
        {
            h -= 1.0f;
        }
        FloorInfo.lastColor = Color.HSVToRGB(h, s, v);

        if (isPlaying)
        {
            if (justStarted)
            {
                crane_moving.Play();
                justStarted = false;
            }
            if (!misplacedFloor)
            {
                // Send trolley back left
                if (trolley.localPosition.x > 57.0f)
                {
                    trolley.localPosition = new Vector3(-57.0f, trolley.localPosition.y, trolley.localPosition.z);
                    if (dropped)
                    {
                        createFloor();
                        clamp1.Reset();
                        clamp2.Reset();
                        if (floorNumber == 1)
                        {
                            trolley.Find("directionsText").GetComponent<MeshRenderer>().enabled = true;
                        }
                        Horizontalspeed = Mathf.Min(Horizontalspeed + 0.65f, 70.0f);
                        Verticalspeed = Mathf.Min(Verticalspeed + 0.05f, 6.6f);
                    }
                }
                if (resetSpeed)
                {
                    if (Time.time - resetSpeedStart < 1)
                    {
                        Horizontalspeed = Mathf.Lerp(horizontalSpeedWhenReset, 8.0f, Time.time - resetSpeedStart);
                        verticalSpeedWhenReset = Mathf.Lerp(horizontalSpeedWhenReset, 0.4f, Time.time - resetSpeedStart);
                    }
                    else
                    {
                        Horizontalspeed = 8;
                        Verticalspeed = 0.4f;
                        resetSpeed = false;
                    }
                }
                // Move trolley side-to-side
                if (!stopped)
                {
                    trolley.position = Vector3.Lerp(trolley.position, trolley.position + Vector3.right * Horizontalspeed, Time.deltaTime);
                }
                // Get new crane height
                if (droppedFloors.GetComponentsInChildren<Collider>().Length > 0)
                {
                    maxCraneHeight = Mathf.Max(droppedFloors.GetComponentsInChildren<Collider>().Select(floorCollider => floorCollider.bounds.max.y).ToArray().Max() + 16.0f, 28.0f);
                }
                else
                {
                    maxCraneHeight = 28.0f;
                }
                // Move crane up
                if (transform.position.y < maxCraneHeight)
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up, Time.deltaTime * Verticalspeed);
                }
                // Drop floor
                Transform droppedFloor = trolley.Find("Floor");
                if ((Input.GetMouseButtonDown(0) || (isTesting && Mathf.Abs(trolley.position.x) < 0.5f)) && droppedFloor != null)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    bool hitSomething = Physics.Raycast(ray, out hit, 100);
                    if (!hitSomething || !nonclickables.Contains(hit.transform.name))
                    {
                        stopped = true;
                        // Remove clamps
                        crane_moving.mute = true;
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[1];
                        a_s.Play();
                        clamp1.Open();
                        clamp2.Open();
                        // Move floor out of crane and drop
                        droppedFloor.parent = fallingFLoors.transform;
                        Rigidbody rb = droppedFloor.gameObject.AddComponent<Rigidbody>();
                        rb.mass = 1000;
                        rb.useGravity = true;
                        rb.interpolation = RigidbodyInterpolation.Interpolate;
                        droppedFloor.name = floorNumber.ToString();
                        dropped = true;
                        floorNumber += 1;
                    }
                }
                if (clamp1.fullyOpened || clamp2.fullyOpened)
                {
                    stopped = false;
                    crane_moving.mute = false;
                }
                if (fallingFLoors.transform.childCount > 0)
                {
                    Transform firstFloor = fallingFLoors.transform.GetChild(0);
                    if (firstFloor.position.y < trolley.Find("directionsText").position.y - 1)
                    {
                        trolley.Find("directionsText").GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }
            else
            {
                if (Vector3.Distance(trolley.position, new Vector3(0, trolley.position.y, 0)) > 0.02f)
                {
                    if (trolley.localPosition.x > 57.0)
                    {
                        trolley.localPosition = new Vector3(-57.0f, trolley.localPosition.y, trolley.localPosition.z);
                    }
                    if (trolley.position.x > 0 || Vector3.Distance(trolley.position, new Vector3(0, trolley.position.y, 0)) > Time.deltaTime * Horizontalspeed)
                    {
                        trolley.position = Vector3.Lerp(trolley.position, trolley.position + Vector3.right, Time.deltaTime * Horizontalspeed);
                    }
                    else
                    {
                        trolley.position = Vector3.Lerp(trolley.position, new Vector3(0, trolley.position.y, 0), Time.deltaTime * Horizontalspeed);
                        crane_moving.volume = (1 - Vector3.Distance(trolley.position, new Vector3(0, trolley.position.y, 0)) / Time.deltaTime * Horizontalspeed) * 0.15f;
                    }
                }
                else
                {
                    crane_moving.Stop();
                    if (!AbilityController.remoteOut)
                    {
                        if (detonator.transform.localPosition.y < -13.5 && !detonatorUp)
                        {
                            if (!soundUp)
                            {
                                detonator_sound.Play();
                                soundUp = true;
                            }
                            detonator.transform.position = Vector3.Lerp(detonator.transform.position, detonator.transform.position + Vector3.up, Time.deltaTime * DetonatorMoveSpeed);
                        }
                        else
                        {
                            detonatorUp = true;
                            if (!detonator_mute)
                            {
                                detonator_sound.mute = true;
                                detonator_mute = true;
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonDown(0) && detonatorUp)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        if (hit.transform.tag == "Detonator")
                        {
                            clickedDetonator = true;
                        }
                    }
                }
                if (clickedDetonator)
                {
                    Transform detonatorPlunger = detonator.transform.Find("Plunger");
                    if (detonatorPlunger.localPosition.y > 6.25f)
                    {
                        detonatorPlunger.position = Vector3.Lerp(detonatorPlunger.position, detonatorPlunger.position + Vector3.down, Time.deltaTime * DetonatorPlungerSpeed);
                    }
                    else
                    {
                        blow = true;
                    }
                }
                if (blow)
                {
                    // Blow up top floor
                    if (!blowFirst)
                    {
                        Transform currentFloor = trolley.Find("Floor");
                        if (currentFloor != null)
                        {
                            clamp1.Open();
                            clamp2.Open();
                            blowUpFloor(currentFloor);
                            lastBoom = Time.time;
                        }
                        else
                        {
                            lastBoom = Time.time - 2;
                        }
                        skipDelayStart = Time.time;
                        maxCraneHeight = droppedFloors.GetComponentsInChildren<FloorInfo>().Select(floorInfo => floorInfo.boomHeight).ToArray().Max();
                        blowFirst = true;
                    }
                    // Move crane down
                    Transform floors = GameObject.Find("Dropped Floors").transform;
                    if (floors.childCount > 0)
                    {
                        Transform child = floors.GetChild(floors.childCount - 1);
                        if (transform.position.y > maxCraneHeight)
                        {
                            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down, Time.deltaTime * MoveDownSpeed);
                        }
                        else
                        {
                            // Add time delay
                            if (currentlyExploding == null)
                            {
                                maxCraneHeight = droppedFloors.GetComponentsInChildren<FloorInfo>().Select(floorInfo => floorInfo.boomHeight).ToArray().Max();
                                blowUpFloor(child);
                                lastBoom = Time.time;
                            }
                        }
                    }
                    else
                    {
                        if (transform.position.y > 30.0f)
                        {
                            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down, Time.deltaTime * MoveDownSpeed);
                        }
                        else
                        {
                            transform.position = Vector3.Lerp(transform.position, new Vector3(0, 28, 0), Time.deltaTime * MoveDownSpeed);
                            closeSkip = true;
                        }
                    }
                    if (Time.time - skipDelayStart > 1)
                    {
                        int dir = closeSkip ? 1 : -1;
                        if ((rotateCounter < 50 && !closeSkip) || (rotateCounter > 0 && closeSkip))
                        {
                            float rotateAmount = Time.deltaTime * SkipMoveSpeed * dir;
                            rotateCounter -= rotateAmount;
                            buttonHolder.transform.RotateAround(buttonHolder.transform.position, buttonHolder.transform.up, rotateAmount);
                            // Calculate Cover
                            skipCover.transform.localRotation = Quaternion.Euler(-90.0f + calcCoverRotation(rotateCounter), 10, 0);
                        }
                        // On skip click
                        if (Input.GetMouseButtonDown(0) && rotateCounter > 30)
                        {
                            RaycastHit hit;
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out hit, 100))
                            {
                                if (hit.transform.name == "SkipButton")
                                {
                                    hit.transform.GetComponent<ClickMove>().clicked = true;
                                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                                    a_s.PlayOneShot(a_s.clip, 0.1f);
                                    skipExplosions();
                                    closeSkip = true;
                                }
                            }
                        }
                    }
                }
                if (GameObject.Find("Dropped Floors").transform.childCount == 0 && clickedDetonator && !isDelay && Mathf.Abs(transform.position.y - 28.0f) < 0.01f)
                {
                    timeDelayStart = Time.time;
                    isDelay = true;
                    AccidentTextMoving.endSignal = true;
                    alarm.Stop();
                }
                if (Time.time - timeDelayStart > 0.75f && isDelay)
                {
                    if (detonator.transform.localPosition.y > -18.5)
                    {
                        if (!soundDown)
                        {
                            detonator_sound.mute = false;
                            soundDown = true;
                        }
                        detonator.transform.position = Vector3.Lerp(detonator.transform.position, detonator.transform.position + Vector3.down, Time.deltaTime * DetonatorMoveSpeed * 0.75f);
                    }
                    else
                    {
                        if (!EndScreenController.toggleRemote)
                        {
                            detonator_sound.Stop();
                            EndScreenController.toggleRemote = true;
                            isPlaying = false;
                            Reset();
                        }
                    }
                }
            }
        }
        if (destroyQueue.transform.childCount > 0)
        {
            Transform child = destroyQueue.transform.GetChild(destroyQueue.transform.childCount - 1);
            Destroy(child.gameObject);
        }
    }

    void createFloor()
    {
        GameObject f = Instantiate(floor, Vector3.zero, Quaternion.Euler(0, 90, 0), trolley).gameObject;
        f.transform.localPosition = new Vector3(0, 0, -0.5f);
        f.name = "Floor";
        f.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        f.GetComponent<FloorInfo>().floorNumber = floorNumber;
        dropped = false;
        floorNumber3D(f, new Vector3(2.5f, 1.0f, 0), Quaternion.Euler(0, -90, 0));
        floorNumber3D(f, new Vector3(-2.5f, 1.0f, 0), Quaternion.Euler(0, 90, 0));
    }

    GameObject floorNumber3D(GameObject flr, Vector3 position, Quaternion rotation)
    {
        char[] digits = floorNumber.ToString().ToCharArray();
        int gaps = digits.Length - 1;
        float spacing = FLOOR_NUMBER_SPACING_CONSTANT * (1 / digits.Length);
        float letterSize = (FLOOR_NUMBER_LENGTH - (gaps * spacing)) / digits.Length;
        float startPosition = -(FLOOR_NUMBER_LENGTH / 2.0f) + (letterSize / 2);
        float iterationLength = spacing + letterSize;
        float cappedLetterSize = Mathf.Min(letterSize, 0.95f);
        GameObject number = new GameObject("Number");
        for (int i = 0; i < digits.Length; i++)
        {
            int value = (int)char.GetNumericValue(digits[i]);
            GameObject digit = Instantiate(numbers[value], Vector3.zero, Quaternion.Euler(0, 180, 0), number.transform).gameObject;
            digit.transform.localScale = new Vector3(cappedLetterSize * EXTRA_SCALE_FACTOR, cappedLetterSize * EXTRA_SCALE_FACTOR, cappedLetterSize * EXTRA_SCALE_FACTOR);
            digit.transform.localPosition = new Vector3(startPosition + iterationLength * i, 0, 0);
        }
        number.transform.parent = flr.transform;
        number.transform.localPosition = position;
        number.transform.localRotation = rotation;
        return number;
    }

    public void blowUpFloor(Transform floorObj)
    {
        Vector3 position = floorObj.position;
        Quaternion rotation = floorObj.rotation;
        // Explode
        Destroy(floorObj.gameObject);
        currentlyExploding = Instantiate(boomFloor, position, rotation, explodedFloors.transform);
    }

    float calcCoverRotation(float a)
    {
        float l = 0.6f;
        float w = 0.1f;
        float b = Mathf.Cos(a * Mathf.Deg2Rad) * l;
        float c = l - b + w;
        float d = Mathf.Sin(a * Mathf.Deg2Rad) * l;
        float e = Mathf.Acos(l / (l + w)) * Mathf.Rad2Deg;
        return a < e ? Mathf.Max(Mathf.Atan2(d, c) * Mathf.Rad2Deg, 0) : 90.0f - e;
    }

    void skipExplosions()
    {
        if (!hasSkipped)
        {
            Transform floors = GameObject.Find("Dropped Floors").transform;
            if (floors.childCount > 7)
            {
                while (floors.childCount > 7)
                {
                    Transform child = floors.GetChild(floors.childCount - 1);
                    child.gameObject.SetActive(false);
                    child.parent = destroyQueue.transform;
                }
                if (floors.childCount > 0)
                {
                    Transform child = floors.GetChild(floors.childCount - 1);
                    maxCraneHeight = droppedFloors.GetComponentsInChildren<FloorInfo>().Select(floorInfo => floorInfo.boomHeight).ToArray().Max();
                    transform.position = new Vector3(transform.position.x, maxCraneHeight, transform.position.z);
                }
                lastBoom = Time.time - 2;
                GameObject.Find("SkyChanger").GetComponent<SkyChanger>().ResetSky();
            }
            hasSkipped = true;
        }
    }

    public void resetCraneSpeed()
    {
        resetSpeed = true;
        horizontalSpeedWhenReset = Horizontalspeed;
        verticalSpeedWhenReset = Verticalspeed;
        resetSpeedStart = Time.time;
        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[16];
        a_s.PlayOneShot(a_s.clip, 0.75f);
    }
}
