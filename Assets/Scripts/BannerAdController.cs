using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAdController : MonoBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.TOP_CENTER;
    [SerializeField] string _androidGameId = "4235445";
    [SerializeField] string _IOsAdUnitId = "4235444";
    string _gameId;
    string _bannerId;


    // Start is called before the first frame update
    void Start()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _IOsAdUnitId : _androidGameId;
        _bannerId = (Application.platform == RuntimePlatform.IPhonePlayer) ? "Banner_iOS" : "Banner_Android2";
        Advertisement.Initialize(_gameId);
        // Set the banner position:
        Advertisement.Banner.SetPosition(_bannerPosition);
        // Show banner
        LoadBanner();
    }

    // Implement a method to call when the Load Banner button is clicked:
    public void LoadBanner()
    {
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_bannerId);
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(_bannerId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show(_bannerId);
    }
}