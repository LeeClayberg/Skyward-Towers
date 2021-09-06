using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyChanger : MonoBehaviour
{
    public int levelsToChange;

    Color start = new Color(0.40625f, 0.609375f, 0.6875f);
    Color lightBlack = new Color(0.06667f, 0.06667f, 0.06667f);

    float startHeight = 132;
    Transform starParent;

    void Start()
    {
        ResetSky();
    }

    // Update is called once per frame
    void Update()
    {
        Transform crane = GameObject.Find("Crane").transform;
        if (crane.position.y > startHeight)
        {
            float interValue = 1.0f / levelsToChange * (crane.position.y - startHeight);
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(start, lightBlack, interValue));
            ChangeStarAlpha(interValue);
        }
    }

    void ChangeStarAlpha(float alpha)
    {
        foreach (ParticleSystemRenderer ps in starParent.GetComponentsInChildren<ParticleSystemRenderer>())
        {
            ps.material.color = new Color(ps.material.color.r, ps.material.color.b, ps.material.color.g, alpha);
        }
    }

    public void ResetSky()
    {
        RenderSettings.skybox.SetColor("_Tint", new Color(0.40625f, 0.609375f, 0.6875f));
        starParent = GameObject.Find("Crane").transform.Find("MainCamera").Find("Stars");
        ChangeStarAlpha(0.0f);
    }
}
