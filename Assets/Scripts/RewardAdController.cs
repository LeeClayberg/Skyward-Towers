using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardAdController : MonoBehaviour, IUnityAdsListener, IUnityAdsLoadListener
{
    [SerializeField] string _androidGameId = "4235445";
    [SerializeField] string _IOsAdUnitId = "4235444";
    string _gameId;
    string _rewardVideoId;
    

    // Start is called before the first frame update
    void Start()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _IOsAdUnitId : _androidGameId;
        _rewardVideoId = (Application.platform == RuntimePlatform.IPhonePlayer) ? "Rewarded_iOS" : "Rewarded_Android";
        Advertisement.AddListener(this);
        Advertisement.Initialize(_gameId);
        LoadAd();
    }

    public void LoadAd()
    {
        Advertisement.Load(_rewardVideoId, this);
    }

    public void ShowAd()
    {
        StoreController.showingAd = true;
        Advertisement.Show(_rewardVideoId);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            StoreController.points += 25;
            PlayerPrefs.SetInt("_points", StoreController.points);
            PlayerPrefs.Save();
            StoreController.pointsToBeAdded += 25;
            StoreController.showingAd = false;
            StoreController.isAdReady = false;
            Advertisement.Load(_rewardVideoId, this);
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        StoreController.isAdReady = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log("Failed");
    }
}
