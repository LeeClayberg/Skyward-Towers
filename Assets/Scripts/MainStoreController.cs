using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainStoreController : StoreType
{
    public float movementSpeed;
    public StoreType explosionStore;
    public StoreType lightsStore;
    public StoreType levelNumStore;
    public StoreType abilityStore;

    bool playSound;
    bool addPoints;

    // Update is called once per frame
    void Update()
    {
        transform.Find("Points").GetComponent<TextMeshPro>().text = "Points: " + (StoreController.points - StoreController.pointsToBeAdded).ToString();
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

        // Remote Movement
        if (toggleRemote && !RemoteController.remoteOut && !explosionStore.remoteOut && !lightsStore.remoteOut && !levelNumStore.remoteOut && !abilityStore.remoteOut && !FindObjectOfType<IAPStoreController>().remoteOut)
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
                    RemoteController.toggleRemote = true;
                    toggleRemote = true;
                }
                else if (hit.transform.name == "ExplosionEffects")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    toggleRemote = true;
                    explosionStore.toggleRemote = true;
                }
                else if (hit.transform.name == "FestiveLights")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    toggleRemote = true;
                    lightsStore.toggleRemote = true;
                }
                else if (hit.transform.name == "LevelNumbers")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    toggleRemote = true;
                    levelNumStore.toggleRemote = true;
                }
                else if (hit.transform.name == "Abilities")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    //AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
                    //a_s.PlayOneShot(a_s.clip, 0.1f);
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    toggleRemote = true;
                    abilityStore.toggleRemote = true;
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
            }
        }
    }
}
