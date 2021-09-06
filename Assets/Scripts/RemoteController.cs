using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : MonoBehaviour
{
    public static bool remoteOut = true;
    public static bool toggleRemote;

    public float movementSpeed;
    public MainStoreController mainStore;
    public HighScoreTracker highScoreTracker;

    bool playSound;

    // Update is called once per frame
    void Update()
    {
        if (remoteOut && !toggleRemote)
        {
            StoreController.pointsToBeAdded = 0;
        }

        // Remote Movement
        if (toggleRemote && !mainStore.remoteOut && !InfoController.remoteOut && !EndScreenController.remoteOut)
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
                if (Vector3.Distance(transform.localPosition, new Vector3(0, -1.8f, 10)) < 0.05f)
                {
                    remoteOut = true;
                    toggleRemote = false;
                    playSound = false;
                }
            }
            if (Mathf.Abs(transform.localPosition.x - 2.0f) < 0.4f)
            {
                highScoreTracker.UpdateBillboard();
            }
        }

        // Clicking on buttons
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100) && !toggleRemote)
            {
                if (hit.transform.name == "Normal")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    CraneMovement.isPlaying = true;
                    CraneMovement.gameType = GameType.Normal;
                    toggleRemote = true;
                    AbilityController.toggleRemote |= AbilityStoreController.AnyAbilities();
                    EndScreenController.Reset();
                }
                else if (hit.transform.name == "Hardcore")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    CraneMovement.isPlaying = true;
                    CraneMovement.gameType = GameType.Hardcore;
                    toggleRemote = true;
                    AbilityController.toggleRemote |= AbilityStoreController.AnyAbilities();
                    EndScreenController.Reset();
                }
                else if (hit.transform.name == "Store")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    toggleRemote = true;
                    mainStore.toggleRemote = true;
                }
                else if (hit.transform.name == "Leaderboard")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    GameObject.Find("Leaderboards").GetComponent<LeaderboardController>().OnClick();
                }
                else if (hit.transform.name == "Info")
                {
                    hit.transform.GetComponent<ClickMove>().clicked = true;
                    AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
                    a_s.PlayOneShot(a_s.clip, 0.1f);
                    toggleRemote = true;
                    InfoController.toggleRemote = true;
                }
            }
        }
    }
}
