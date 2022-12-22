using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace ThePairs
{

    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        private void Awake()
        {
            instance = this;
        }

        private CardController _cardController;
        private ScoreController _scoreController;
        private TimeController _timeController;
        private MovementController _movementController;

        private void Start()
        {
            _cardController = GetComponent<CardController>();
            _scoreController = GetComponent<ScoreController>();
            _timeController = GetComponent<TimeController>();
            _movementController = GetComponent<MovementController>();
        }


        #region Button Methods

        #region Buttons/MenuScreen

        public void MainMenu_PlayButton()
        {
            StartGame("NormalGameScreen");
        }

        public void MainMenu_PlayHardButton()
        {
            StartGame("HardGameScreen");
        }

        public void MainMenu_SettingsButton()
        {
            //SettingsScreen.SetActive(true);
        }

        public void MainMenu_QuitButton()
        {
            Application.Quit();
        }

        #endregion

        #endregion

        
        public void StartGame(string screenName)
        {
            SoundManager.Instance.PlayMusic();
            
            ScreenManager.Instance.LoadScreen(screenName);

            Destroy(_timeController);
            if (screenName == "NormalGameScreen")
            {
                _timeController = gameObject.AddComponent<TimeController>();
                TimeController.ResetTimeController(60);
            }

            CardController.ResetCardController();
            ScoreController.ResetScoreController();

            MovementController.ResetMovementContoller(6);

            GameObject cards = ScreenManager.Instance.CurrentScreen.transform.GetChild(0).gameObject;
            cards.SetActive(false);

            StartCoroutine(StartGameRoutine());
        }


        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        private IEnumerator StartGameRoutine()
        {
            if (ScreenManager.Instance.CurrentScreen.name == "HardGameScreen")
            {
                ScreenManager.Instance.OpenPopUp("HardHowToPlayPopUp");
            }
            else
            {
                ScreenManager.Instance.OpenPopUp("HowToPlayPopUp");
            }


            //blocking screenmanager
            ScreenManager.Instance.Bloked = true;

            Debug.Log("waiting to close HowToPlayPopUp");
            while (ScreenManager.Instance.IsPopUpOpened("HowToPlayPopUp") || ScreenManager.Instance.IsPopUpOpened("HardHowToPlayPopUp"))
            {
                yield return waitForEndOfFrame;
            }

            Debug.Log("HowToPlayPopUp is closed");


            StartCoroutine(StartCoundown());
            Debug.Log("RevealTheCards");

            bool revealTheCardsOver = false;

            ScreenManager.Instance.CurrentScreen.transform.GetChild(0).gameObject.SetActive(true);

            List<Card> cards = CardController.CurrentCards;

            float fadeOutTime = 3f / cards.Count;
            var images = new List<Image>();
            var transparentColor = new Color(255, 255, 255, 0);


            Card joker = null;
            Image jokerImage = null;
            foreach (var card in cards)
            {
                var temp = card.mFront.GetComponent<Image>();
                temp.color = transparentColor;
                images.Add(temp);
                if (card.isJoker)
                {
                    jokerImage = temp;
                    joker = card;
                }
            }

            for (int i = 0; i < images.Count; i++)
            {
                images[i].DOFade(1f, fadeOutTime);
                yield return new WaitForSeconds(fadeOutTime - 0.05f);
                if (images[i] == jokerImage)
                    joker.ShinningEffect.SetActive(true);
                if (i == images.Count - 1)
                {
                    //Debug.Log("hi");
                    yield return new WaitForSeconds(1.5f);
                    revealTheCardsOver = true;
                }
            }

            Debug.Log("waiting to close RevealTheCards");
            while (!revealTheCardsOver)
            {
                yield return waitForEndOfFrame;
            }

            Debug.Log("Cards are Closed");
            foreach (Card c in cards)
            {
                c.CloseTheCard();
                if (cards[cards.Count - 1].Equals(c))
                {
                    // all card closed so can start time
                    if (TimeController != null)
                        TimeController.State = TimeController.TimeState.Start;
                }
            }


            //release screenmanager
            ScreenManager.Instance.Bloked = false;
        }


        private IEnumerator StartCoundown()
        {
            int x = Screen.width / 2;
            int y = Screen.height / 2;
            GameObject countDown = Instantiate(Resources.Load<GameObject>("CountDown"), new Vector3(x, y, 0),
                Quaternion.identity);
            yield return new WaitForSeconds(3);
            Destroy(countDown);
        }


        public void ReloadCurrentGame()
        {
            StartGame(ScreenManager.Instance.CurrentScreen.name);
        }


        public void WinTheGame()
        {
            SoundManager.Instance.PlaySoundClip("SuccessClip");
            TimeController.State = TimeController.TimeState.Pause;
            if (ScreenManager.Instance.CurrentScreen.name == "HardGameScreen")
            {
                ScoreController.OnGameEnd();
                ScreenManager.Instance.OpenPopUp("VictoryPopUp");
            }
            else
            {
                StartCoroutine(ConvertRemainingTimeToScore());
            }
        }


        private IEnumerator ConvertRemainingTimeToScore()
        {
            while (TimeController.TimeRemaining > 0)
            {
                TimeController.TimeRemaining--;
                if (TimeController.TimeRemaining < 1)
                    TimeController.TimeRemaining = 0;
                TimeController.DisplayTime(TimeController.TimeRemaining);
                ScoreController.Score += 10;
                ScoreController.UpdateScoreTexts();
                SoundManager.Instance.PlaySoundClip("IncreaseClip");
                yield return new WaitForSeconds(0.2f);
                //Debug.Log(timeManager.timeRemaining);
            }

            ScoreController.OnGameEnd();
            ScreenManager.Instance.OpenPopUp("VictoryPopUp");
        }


        public void LoseTheGame()
        {
            ScoreController.OnGameEnd();
            SoundManager.Instance.PlaySoundClip("FailClip");

            SoundManager.Instance.StopMusic();

            ScreenManager.Instance.OpenPopUp("LosePopUp");
        }


        #region GetterSetter

        public static GameManager Instance
        {
            get => instance;
        }

        public CardController CardController
        {
            get => _cardController;
        }

        public ScoreController ScoreController
        {
            get => _scoreController;
        }

        public TimeController TimeController
        {
            get => _timeController;
        }

        public MovementController MovementController
        {
            get => _movementController;
        }

        #endregion
    }

}