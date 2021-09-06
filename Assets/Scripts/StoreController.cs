using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreController : StoreType
{
    public static int points;
    public static int explosionEffect;
    public static int levelNumberEffect;
    public static int stringLights;

    public static int pointsToBeAdded;

    public static bool showingAd;
    public static bool isAdReady;

    public float movementSpeed;
    public string buttonTag;
    public string playerPrefsStart;
    public static bool resetPlayerPrefs;

    public MainStoreController prev;

    float startTime;
    //float adStartTime;
    bool playSound;
    bool addPoints;

    string[] colorOptions = { "blue", "red", "yellow", "green", "pink", "orange", "purple", "rainbow" };

    void Start()
    {
        // Points
        if (!PlayerPrefs.HasKey("_points") || resetPlayerPrefs)
        {
            PlayerPrefs.SetInt("_points", 0);
        }

        // String Lights Type
        if (!PlayerPrefs.HasKey(playerPrefsStart + "selected") || resetPlayerPrefs)
        {
            PlayerPrefs.SetString(playerPrefsStart + "selected", "Reg");
        }

        // Bought Items
        foreach (string color in colorOptions)
        {
            if (!PlayerPrefs.HasKey(playerPrefsStart + color) || resetPlayerPrefs)
            {
                PlayerPrefs.SetInt(playerPrefsStart + color, 0);
            }
        }

        PlayerPrefs.Save();

        // Retreive data
        points = PlayerPrefs.GetInt("_points");

        // Selected 
        Transform selected = transform.Find(PlayerPrefs.GetString(playerPrefsStart + "selected"));
        selectedOption(selected, false);

        // Covers
        transform.Find("ButtonCover (0)").GetComponent<CoverData>().setOpen();
        for (int a = 0; a < colorOptions.Length; a++)
        {
            string num = (a + 1).ToString();
            transform.Find("ButtonCover (" + num + ")").GetComponent<CoverData>().isOpen = PlayerPrefs.GetInt(playerPrefsStart + colorOptions[a]) == 1;
            transform.Find("ButtonCover (" + num + ")").GetComponent<CoverData>().setOpen();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(addPoints && !showingAd)
        {
            if (pointsToBeAdded > 0)
            {
                AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[14];
                a_s.PlayOneShot(a_s.clip, 0.025f);
                pointsToBeAdded -= 1;
            }
            else
            {
                addPoints = false;
            }
        }

        // Too little points
        transform.Find("Points").GetComponent<TextMeshPro>().text = "Points: " + (points - pointsToBeAdded).ToString();
        if (Time.time - startTime < 0.2f)
        {
            transform.Find("Points").GetComponent<TextMeshPro>().color = Color.red;
        }
        else
        {
            transform.Find("Points").GetComponent<TextMeshPro>().color = Color.white;
        }

        // Remote Movement
        if (toggleRemote && !RemoteController.remoteOut && (prev == null || !prev.remoteOut) && !FindObjectOfType<IAPStoreController>().remoteOut)
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
                if (Vector3.Distance(transform.localPosition, new Vector3(0, -2.55f, 10)) < 0.05f)
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
                    prev.toggleRemote = true;
                    toggleRemote = true;
                }
                else if (hit.transform.name == "ShopButton" && hit.transform.parent == transform)
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    IAPStoreController.prevStore = this;
                    transform.parent.Find("controllerPurchase").GetComponent<IAPStoreController>().toggleRemote = true;
                    toggleRemote = true;
                }
                else if (hit.transform.name == "AdButton" && hit.transform.parent == transform)
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    GameObject.Find("AdController").GetComponent<RewardAdController>().ShowAd();
                    addPoints = true;
                }
                else if (hit.transform.name.Contains("Cover") && hit.transform.parent != null && !hit.transform.parent.GetComponent<CoverData>().isOpen)
                {
                    if (hit.transform.parent.GetComponent<CoverData>().cost <= points)
                    {
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[7];
                        a_s.PlayOneShot(a_s.clip, 0.1f);
                        hit.transform.parent.GetComponent<CoverData>().openCover = true;
                        points -= hit.transform.parent.GetComponent<CoverData>().cost;
                        PlayerPrefs.SetInt(playerPrefsStart + colorOptions[hit.transform.parent.GetComponent<CoverData>().playerPrefsIndex], 1);
                        PlayerPrefs.SetInt("_points", points);
                        PlayerPrefs.Save();
                    }
                    else
                    {
                        startTime = Time.time;
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
                        a_s.PlayOneShot(a_s.clip, 0.1f);
                    }
                }
                selectedOption(hit.transform, true);
            }
        }
    }

    public void setButtonColors(GameObject clicked)
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag(buttonTag))
        {
            g.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f);
        }
        clicked.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
    }

    private void selectedOption(Transform tf, bool playSound)
    {
        if (tf.name == "Reg" && tf.parent == transform)
        {
            if (playSound)
            {
                AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                a_s.PlayOneShot(a_s.clip, 0.05f);
            }
            if (gameObject.name == "storeExplosion")
            {
                explosionEffect = 0;
            }
            else if (gameObject.name == "storeLights")
            {
                stringLights = 0;
            }
            else if (gameObject.name == "storeLevelNums")
            {
                levelNumberEffect = 0;
            }
            tf.GetComponent<ClickMove>().clicked = true;
            PlayerPrefs.SetString(playerPrefsStart + "selected", tf.name);
            PlayerPrefs.Save();
            setButtonColors(tf.gameObject);
        }
        else if (tf.name.Contains("Option") && tf.parent == transform)
        {
            if (playSound)
            {
                AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                a_s.PlayOneShot(a_s.clip, 0.05f);
            }
            if (gameObject.name == "storeExplosion")
            {
                DestroyPiece.fadeToColor = tf.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().color;
                explosionEffect = 1;
            }
            else if (gameObject.name == "storeLights")
            {
                BackgroundController.lightColor = tf.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().color;
                stringLights = 1;
            }
            else if (gameObject.name == "storeLevelNums")
            {
                FloorInfo.levelNumberColor = tf.Find("Canvas").Find("Image").gameObject.GetComponent<Image>().color;
                levelNumberEffect = 1;
            }
            tf.GetComponent<ClickMove>().clicked = true;
            PlayerPrefs.SetString(playerPrefsStart + "selected", tf.name);
            PlayerPrefs.Save();
            setButtonColors(tf.gameObject);
        }
        else if (tf.name == "Rainbow" && tf.parent == transform)
        {
            if (playSound)
            {
                AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                a_s.PlayOneShot(a_s.clip, 0.05f);
            }
            if (gameObject.name == "storeExplosion")
            {
                explosionEffect = 2;
            }
            else if (gameObject.name == "storeLights")
            {
                stringLights = 2;
            }
            else if (gameObject.name == "storeLevelNums")
            {
                levelNumberEffect = 2;
            }
            tf.GetComponent<ClickMove>().clicked = true;
            PlayerPrefs.SetString(playerPrefsStart + "selected", tf.name);
            PlayerPrefs.Save();
            setButtonColors(tf.gameObject);
        }
    }
}
