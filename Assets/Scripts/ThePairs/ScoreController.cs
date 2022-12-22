using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ThePairs
{

    public class ScoreController : MonoBehaviour
    {


        

        private int _score;
        [SerializeField]
        private TextMeshProUGUI mainMenuHighScoreText, mainMenuLastScoreText, winScreenHighScoreText, winScreenYourScoreText, failScreenYourScoreText;

        private TextMeshProUGUI gameScreenScoreText;

        private int _combo = 1;
        private bool isJockerUsed = false;


        private void Start()
        {
            UpdateScoreTexts();
        }
        public void ResetScoreController()
        {
            _score = 0;
            UpdateScoreTexts();
            gameScreenScoreText = GameObject.Find("GameScoreText").GetComponent<TextMeshProUGUI>();
            _combo = 1;

        }


        public int HighScore
        {
            get
            {
                return PlayerPrefs.GetInt("HighScore", 0);
            }
            set
            {
                int temp = HighScore;
                if (value > temp)
                {
                    PlayerPrefs.SetInt("HighScore", value);
                }
            }
        }
        public int LastScore
        {
            get
            {
                return PlayerPrefs.GetInt("LastScore", 0);
            }
            set
            {
                PlayerPrefs.SetInt("LastScore", value);
            }
        }

        public int Score { get => _score; set => _score = value; }
        public void OnCorrectMatch()
        {
            if (ScreenManager.Instance.CurrentScreen.name.Equals("HardGameScreen"))
            {
                _combo = 1;
            }
            int earnedScore = _combo * 10;
            _score += earnedScore;
            ShowEarnedScoreOnScreen(earnedScore);
            UpdateScoreTexts();
            Debug.Log("Current Combo:" + _combo);
            _combo += 1;
        }

        public void OnIncorrectMatch()
        {
            _combo = 1;
            Debug.Log("Current Combo:" + _combo);
        }

        [SerializeField]
        private TMP_FontAsset _font;
        private void ShowEarnedScoreOnScreen(float earnedScore)
        {
            var emptyGameObject = new GameObject();
            var canvasGameObject = GameObject.Find("Canvas");
            int x = Screen.width / 2;
            int y = Screen.height / 2;
            var textGameObject = Instantiate(emptyGameObject, new Vector3(x, y, 0), Quaternion.identity);
            textGameObject.transform.parent = canvasGameObject.transform;
            var textMeshPro = textGameObject.AddComponent<TextMeshProUGUI>();
            textMeshPro.fontSize = 100;
            textMeshPro.font = _font;
            textMeshPro.transform.localScale = textMeshPro.transform.localScale * 3;
            textMeshPro.margin = new Vector4(-50, -50, -50, -50);
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.text = earnedScore.ToString(CultureInfo.CurrentCulture);
            textMeshPro.DOFade(0, 1f).OnComplete(() =>
            {
                Destroy(textGameObject);
                Destroy(emptyGameObject);
            }
            );
        }

        public void OnGameEnd()
        {
            HighScore = Score;
            LastScore = Score;
            UpdateScoreTexts();
        }

        public void UpdateScoreTexts()
        {
            if (gameScreenScoreText != null)
                gameScreenScoreText.text = Score.ToString();

            mainMenuHighScoreText.text = HighScore.ToString();
            mainMenuLastScoreText.text = LastScore.ToString();


            winScreenHighScoreText.text = HighScore.ToString();
            winScreenYourScoreText.text = LastScore.ToString();

            failScreenYourScoreText.text = LastScore.ToString();
        }





    }

}

