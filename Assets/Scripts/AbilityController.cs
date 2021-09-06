using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityController : MonoBehaviour
{
	public static bool remoteOut;
    public static bool toggleRemote;

	public float movementSpeed;
    public TextMeshPro[] counts;
    float[] startTimes;
    float noneDelay;
    bool allOut;

	bool playSound;

    private void Start()
    {
        startTimes = new float[3];
    }

    // Update is called once per frame
    void Update()
	{
        // Error color
        for (int i = 0; i < counts.Length; i++)
        {
            if (Time.time - startTimes[i] < 0.2f)
            {
                counts[i].color = Color.red;
            }
            else
            {
                counts[i].color = Color.white;
            }
        }

        // Delay close
        if (allOut && Time.time - noneDelay > 2.0f && remoteOut)
        {
            toggleRemote = true;
            allOut = false;
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
			float speedFunc = 0.75f * Mathf.Pow(-transform.localPosition.y -4.6f, 1);
			if (remoteOut)
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.down, Time.deltaTime * speedFunc * movementSpeed);
				if (transform.localPosition.y < -11.6f)
				{
					remoteOut = false;
					toggleRemote = false;
					playSound = false;
				}
			}
			else
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + Vector3.up, Time.deltaTime * speedFunc * movementSpeed);
				if (Vector3.Distance(transform.localPosition, new Vector3(0, -4.6f, 10)) < 0.05f)
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
				if (hit.transform.name == "SpeedReset")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    if (AbilityStoreController.speedResets > 0)
                    {
                        AbilityStoreController.speedResets -= 1;
                        counts[0].text = AbilityStoreController.speedResets.ToString();
                        PlayerPrefs.SetInt("_speed_resets", AbilityStoreController.speedResets);
                        PlayerPrefs.Save();
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                        a_s.PlayOneShot(a_s.clip, 0.05f);
                        syncWithStore();
                        // Reset Speed
                        FindObjectOfType<CraneMovement>().resetCraneSpeed();
                        if (!AbilityStoreController.AnyAbilities())
                        {
                            noneDelay = Time.time;
                            allOut = true;
                        }
                    }
                    else
                    {
                        startTimes[0] = Time.time;
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
                        a_s.PlayOneShot(a_s.clip, 0.1f);
                    }
                }
                else if (hit.transform.name == "Freeze")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    if (AbilityStoreController.freezes > 0)
                    {
                        Transform droppedFloors = GameObject.Find("Dropped Floors").transform;
                        if (droppedFloors.childCount > 0 && !droppedFloors.GetChild(droppedFloors.childCount - 1).GetComponent<FloorInfo>().frozen)
                        {
                            AbilityStoreController.freezes -= 1;
                            counts[1].text = AbilityStoreController.freezes.ToString();
                            PlayerPrefs.SetInt("_freezes", AbilityStoreController.freezes);
                            PlayerPrefs.Save();
                            AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                            a_s.PlayOneShot(a_s.clip, 0.05f);
                            syncWithStore();
                            // Remove rigidbodies
                            foreach (Transform tm in GameObject.Find("Dropped Floors").transform)
                            {
                                tm.GetComponent<FloorInfo>().frozen = true;
                                Destroy(tm.GetComponent<Rigidbody>());
                            }
                            AudioSource a_s2 = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[15];
                            a_s2.PlayOneShot(a_s2.clip, 1.75f);
                            if (!AbilityStoreController.AnyAbilities())
                            {
                                noneDelay = Time.time;
                                allOut = true;
                            }
                        }
                        else
                        {
                            AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
                            a_s.PlayOneShot(a_s.clip, 0.1f);
                        }
                    }
                    else
                    {
                        startTimes[1] = Time.time;
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
                        a_s.PlayOneShot(a_s.clip, 0.1f);
                    }
                }
                else if (hit.transform.name == "ExtraLife")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    if (AbilityStoreController.extraLives > 0)
                    {
                        Transform fallingFloors = GameObject.Find("Falling Floors").transform;
                        Transform droppedFloors = GameObject.Find("Dropped Floors").transform;
                        if (fallingFloors.childCount + droppedFloors.childCount > 0)
                        {
                            AbilityStoreController.extraLives -= 1;
                            counts[2].text = AbilityStoreController.extraLives.ToString();
                            PlayerPrefs.SetInt("_extra_lives", AbilityStoreController.extraLives);
                            PlayerPrefs.Save();
                            AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                            a_s.PlayOneShot(a_s.clip, 0.05f);
                            syncWithStore();
                            Transform lastFloor;
                            if (fallingFloors.childCount > 0)
                            {
                                lastFloor = fallingFloors.GetChild(fallingFloors.childCount - 1);
                            }
                            else
                            {
                                lastFloor = droppedFloors.GetChild(droppedFloors.childCount - 1);
                            }
                            FindObjectOfType<CraneMovement>().blowUpFloor(lastFloor);
                            if (!AbilityStoreController.AnyAbilities())
                            {
                                noneDelay = Time.time;
                                allOut = true;
                            }
                        }
                        else
                        {
                            AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
                            a_s.PlayOneShot(a_s.clip, 0.1f);
                        }
                    }
                    else
                    {
                        startTimes[2] = Time.time;
                        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
                        a_s.PlayOneShot(a_s.clip, 0.1f);
                    }
                }
            }
		}
	}

    public void Setup()
    {
        counts[0].text = AbilityStoreController.speedResets.ToString();
        counts[1].text = AbilityStoreController.freezes.ToString();
        counts[2].text = AbilityStoreController.extraLives.ToString();
    }

    void syncWithStore()
    {
        foreach(AbilityData ad in FindObjectsOfType<AbilityData>())
        {
            ad.fillText();
        }
    }
}
