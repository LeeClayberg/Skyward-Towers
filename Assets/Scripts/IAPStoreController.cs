using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPStoreController : StoreType
{
    public static StoreType prevStore;

    public float movementSpeed;

    float startTime;
    bool playSound;
    TextMeshPro tmp;
    int pointAddRate;

    void Start()
    {
        tmp = transform.Find("Points").GetComponent<TextMeshPro>();
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            transform.Find("RestorePurchases").gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Purchase Failed
        transform.Find("Points").GetComponent<TextMeshPro>().text = "Points: " + (StoreController.points - StoreController.pointsToBeAdded).ToString();
        if (Time.time - startTime < 0.2f)
        {
            transform.Find("Points").GetComponent<TextMeshPro>().color = Color.red;
        }
        else
        {
            transform.Find("Points").GetComponent<TextMeshPro>().color = Color.white;
        }

        // Remote Movement
        if (toggleRemote && !RemoteController.remoteOut && (prevStore == null || !prevStore.remoteOut))
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
                    prevStore.toggleRemote = true;
                    toggleRemote = true;
                }
                else if (hit.transform.name == "RestorePurchases" && hit.transform.parent == transform)
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    hit.transform.GetComponent<CustomIAPButton>().OnClick();

                }
                else if (hit.transform.name.Contains("Option")) {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.05f);
                    hit.transform.GetComponent<CustomIAPButton>().OnClick();
                }
            }
        }
    }

    public void OnPurchaseComplete(Product product)
    {
        switch (product.definition.id)
        {
            case "com.leeclayberg.skywardtowers.points300":
                StoreController.points += 300;
                StoreController.pointsToBeAdded += 300;
                break;
            case "com.leeclayberg.skywardtowers.points1000":
                StoreController.points += 1000;
                StoreController.pointsToBeAdded += 1000;
                break;
            case "com.leeclayberg.skywardtowers.points2500":
                StoreController.points += 2500;
                StoreController.pointsToBeAdded += 2500;
                break;
            case "com.leeclayberg.skywardtowers.points6000":
                StoreController.points += 6000;
                StoreController.pointsToBeAdded += 6000;
                break;
        }
        PlayerPrefs.SetInt("_points", StoreController.points);
        PlayerPrefs.Save();
        pointAddRate = (StoreController.pointsToBeAdded + 157) / 100;
        InvokeRepeating("addPoints", 0, 0.01f);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase of product " + product.definition.id + " failed due to " + reason);
        //startTime = Time.time;
    }

    void addPoints()
    {
        if (StoreController.pointsToBeAdded > 0)
        {
            AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[14];
            a_s.PlayOneShot(a_s.clip, 0.025f);
            StoreController.pointsToBeAdded -= pointAddRate;
        }
        else
        {
            CancelInvoke();
        }
        tmp.text = "Points: " + (StoreController.points - StoreController.pointsToBeAdded).ToString();
    }
}
