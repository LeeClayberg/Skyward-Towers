using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityData : MonoBehaviour
{
	public int cost;

    float startTime;

    private void Update()
    {
        if (Time.time - startTime < 0.2f)
        {
            transform.GetChild(0).Find("AmountText").GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            transform.GetChild(0).Find("AmountText").GetComponent<TextMeshProUGUI>().color = Color.white;
        }
    }

    public void UpdateTextCount()
    {
        fillText();
        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[6];
        a_s.PlayOneShot(a_s.clip, 0.05f);
    }

    public void fillText()
    {
        if (transform.name.Contains("SpeedReset"))
        {
            transform.GetChild(0).Find("AmountText").GetComponent<TextMeshProUGUI>().text =
                AbilityStoreController.speedResets.ToString() + "/" + AbilityStoreController.maxNumberOfEachAbility.ToString();
        }
        else if (transform.name.Contains("Frozen"))
        {
            transform.GetChild(0).Find("AmountText").GetComponent<TextMeshProUGUI>().text =
                AbilityStoreController.freezes.ToString() + "/" + AbilityStoreController.maxNumberOfEachAbility.ToString();
        }
        else if (transform.name.Contains("ExtraLives"))
        {
            transform.GetChild(0).Find("AmountText").GetComponent<TextMeshProUGUI>().text =
                AbilityStoreController.extraLives.ToString() + "/" + AbilityStoreController.maxNumberOfEachAbility.ToString();
        }
    }

    public void triggerToManyFlash()
    {
        startTime = Time.time;
        AudioSource a_s = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[5];
        a_s.PlayOneShot(a_s.clip, 0.1f);
    }
}
