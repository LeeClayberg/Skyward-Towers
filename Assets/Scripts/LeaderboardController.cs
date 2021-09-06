using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
#endif


public class LeaderboardController : MonoBehaviour
{
    [Header("IOS Ids")]
    public string iosNormalId;
    public string iosHardcoreId;
    [Header("Andriod Ids")]
    public string andriodNormalId;
    public string andriodHardcoreId;

    bool loginSuccessful;

    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
#endif
    }

    void Start()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
#else
        Social.localUser.Authenticate(ProcessAuthentication);
#endif
    }

    void ProcessAuthentication(bool success)
    {
        if (success)
        {
            Debug.Log("Authenticated");
            loginSuccessful = true;
            HighScoreTracker.highScoreNormal = PlayerPrefs.GetInt("_high_score_normal");
            SubmitScore(GameType.Normal, HighScoreTracker.highScoreNormal);
            HighScoreTracker.highScoreHardcore = PlayerPrefs.GetInt("_high_score_hardcore");
            SubmitScore(GameType.Hardcore, HighScoreTracker.highScoreHardcore);
        }
        else
        {
            Debug.Log("Failed to authenticate");
        }
    }

    public void SubmitScore(GameType gametype, int score)
    {
        string leaderboardId;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            leaderboardId = gametype == GameType.Normal ? iosNormalId : iosHardcoreId;
        }
        else
        {
            leaderboardId = gametype == GameType.Normal ? andriodNormalId : andriodHardcoreId;
        }

        if (loginSuccessful)
        {
            Social.ReportScore(score, leaderboardId, SubmitScoreCallback);
        }
        else
        {
            Social.localUser.Authenticate((bool success) => {
                if (success)
                {
                    Debug.Log("Authenticated");
                    loginSuccessful = true;
                    Social.ReportScore(score, leaderboardId, SubmitScoreCallback);
                }
                else
                {
                    Debug.Log("Failed to submit");
                }
            });
        }
    }

    void SubmitScoreCallback(bool success)
    {
        if (success)
        {
            Debug.Log("Successfully submitted");
        }
        else
        {
            Debug.Log("Failed to submit");
        }
    }

    public void OnClick()
    {
        Social.ShowLeaderboardUI();
    }
}