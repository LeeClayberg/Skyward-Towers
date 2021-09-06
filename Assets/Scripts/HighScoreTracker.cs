using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTracker : MonoBehaviour
{
    public Color normalColor;
    public Color hardcoreColor;

    public static int highScoreNormal;
    public static int highScoreHardcore;

    // Start is called before the first frame update
    void Start()
    {
        // High Scores
        if (!PlayerPrefs.HasKey("_high_score_normal"))
        {
            PlayerPrefs.SetInt("_high_score_normal", 0);
        }

        if (!PlayerPrefs.HasKey("_high_score_hardcore"))
        {
            PlayerPrefs.SetInt("_high_score_hardcore", 0);
        }

        PlayerPrefs.Save();

        highScoreNormal = PlayerPrefs.GetInt("_high_score_normal");
        highScoreHardcore = PlayerPrefs.GetInt("_high_score_hardcore");

        UpdateBillboard();
    }

    public static void updateScore(int score)
    {
        if (CraneMovement.gameType == GameType.Hardcore)
        {
            if (score > highScoreHardcore)
            {
                highScoreHardcore = score;
                PlayerPrefs.SetInt("_high_score_hardcore", score);
            }
        }
        else
        {
            if (score > highScoreNormal)
            {
                highScoreNormal = score;
                PlayerPrefs.SetInt("_high_score_normal", score);
            }
        }
        PlayerPrefs.Save();
    }

    public void UpdateBillboard()
    {
        if (CraneMovement.gameType == GameType.Hardcore)
        {
            transform.Find("Score").GetComponent<TextMeshPro>().text = highScoreHardcore.ToString();
            transform.Find("Canvas").Find("Banner1").GetComponent<Image>().color = hardcoreColor;
            transform.Find("Canvas").Find("Banner2").GetComponent<Image>().color = hardcoreColor;
        }
        else
        {
            transform.Find("Score").GetComponent<TextMeshPro>().text = highScoreNormal.ToString();
            transform.Find("Canvas").Find("Banner1").GetComponent<Image>().color = normalColor;
            transform.Find("Canvas").Find("Banner2").GetComponent<Image>().color = normalColor;
        }
    }
}
