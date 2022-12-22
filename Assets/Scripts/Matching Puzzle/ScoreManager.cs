using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private static ScoreManager instance;
    [SerializeField] private TextMeshProUGUI _scoreTextBox;

    private void Awake()
    {
        instance = this;
    }


    private int scoreMultiple = 0;

    private void LateUpdate()
    {
        if(scoreMultiple != 0)
        {

            score += (int)Mathf.Pow(2, scoreMultiple);
            UpdateScore();
            scoreMultiple = 0;

        }
    }

    public void IncreaseScore()
    {
        scoreMultiple++;
        //score += 2;
        //UpdateScore();
    }

    public bool UseJoker()
    {
        if(score >= 1000)
        {
            score -= 1000;
            UpdateScore();
            return true;
        }
        return false;
    }

    private void UpdateScore()
    {
        _scoreTextBox.text = score.ToString();
    }


    public static ScoreManager Instance { get => instance; set => instance = value; }

}
