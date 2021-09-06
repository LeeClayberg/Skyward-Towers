using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityStoreController : StoreType
{
    public static int speedResets;
    public static int freezes;
    public static int extraLives;
    public static int maxNumberOfEachAbility = 3;

	public float movementSpeed;

	public MainStoreController prev;

	float startTime;
	bool playSound;
	bool addPoints;

    TextMeshPro tmp;

    public static bool AnyAbilities()
    {
        return speedResets > 0 || freezes > 0 || extraLives > 0;
    }

    void Start()
	{
        // Abilities
        if (!PlayerPrefs.HasKey("_speed_resets") || StoreController.resetPlayerPrefs)
        {
            PlayerPrefs.SetInt("_speed_resets", 0);
        }
        if (!PlayerPrefs.HasKey("_freezes") || StoreController.resetPlayerPrefs)
        {
            PlayerPrefs.SetInt("_freezes", 0);
        }
        if (!PlayerPrefs.HasKey("_extra_lives") || StoreController.resetPlayerPrefs)
        {
            PlayerPrefs.SetInt("_extra_lives", 0);
        }
        tmp = transform.Find("Points").GetComponent<TextMeshPro>();

        speedResets = PlayerPrefs.GetInt("_speed_resets");
        freezes = PlayerPrefs.GetInt("_freezes");
        extraLives = PlayerPrefs.GetInt("_extra_lives");

        foreach(AbilityData ad in gameObject.GetComponentsInChildren<AbilityData>())
        {
            ad.fillText();
        }
        FindObjectOfType<AbilityController>().Setup();
    }

    // Update is called once per frame
    void Update()
    {
        // Purchase failed
        if (addPoints && !StoreController.showingAd)
        {
            if (StoreController.pointsToBeAdded > 0)
            {
                AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[14];
                a_s.PlayOneShot(a_s.clip, 0.025f);
                StoreController.pointsToBeAdded -= 1;
            }
            else
            {
                addPoints = false;
            }
        }

        // Too little points
        tmp.text = "Points: " + (StoreController.points - StoreController.pointsToBeAdded).ToString();
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
                Debug.Log(hit.transform.name);
                if (hit.transform.name == "LeftButton" && hit.transform.parent == transform)
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    prev.toggleRemote = true;
                    toggleRemote = true;
                    FindObjectOfType<AbilityController>().Setup();
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
                else if (hit.transform.name.Contains("Ability"))
                {
                    AbilityData ad = hit.transform.GetComponent<AbilityData>();
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    if (ad.cost <= StoreController.points)
                    {
                        if (hit.transform.name.Contains("SpeedReset"))
                        {
                            if (speedResets < maxNumberOfEachAbility)
                            {
                                speedResets += 1;
                                ad.UpdateTextCount();
                                StoreController.points -= ad.cost;
                                PlayerPrefs.SetInt("_points", StoreController.points);
                                PlayerPrefs.SetInt("_speed_resets", speedResets);
                                PlayerPrefs.Save();
                            }
                            else
                            {
                                ad.triggerToManyFlash();
                            }
                        }
                        else if (hit.transform.name.Contains("Frozen"))
                        {
                            if (freezes < maxNumberOfEachAbility)
                            {
                                freezes += 1;
                                ad.UpdateTextCount();
                                StoreController.points -= ad.cost;
                                PlayerPrefs.SetInt("_points", StoreController.points);
                                PlayerPrefs.SetInt("_freezes", freezes);
                                PlayerPrefs.Save();
                            }
                            else
                            {
                                ad.triggerToManyFlash();
                            }
                        }
                        else if (hit.transform.name.Contains("ExtraLives"))
                        {
                            if (extraLives < maxNumberOfEachAbility)
                            {
                                extraLives += 1;
                                ad.UpdateTextCount();
                                StoreController.points -= ad.cost;
                                PlayerPrefs.SetInt("_points", StoreController.points);
                                PlayerPrefs.SetInt("_extra_lives", extraLives);
                                PlayerPrefs.Save();
                            }
                            else
                            {
                                ad.triggerToManyFlash();
                            }
                        }
                    }
                    else
                    {
                        startTime = Time.time;
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
                        a_s.PlayOneShot(a_s.clip, 0.1f);
                    }
                }
            }
        }
    }


}
