using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;


namespace ThePairs
{
    public class TimeController : MonoBehaviour
    {


        private float _maxTime;
        private float _timeRemaining;
        private TextMeshProUGUI timeText;
        private TimeState _state;


        private void Awake()
        {
            _state = TimeState.Pause;
        }

        public void ResetTimeController(float maxTime)
        {

            _maxTime = maxTime;
            _state = TimeState.Pause;
            timeText = GameObject.Find("TimeText").GetComponent<TextMeshProUGUI>();
            _timeRemaining = _maxTime;
            DisplayTime(_timeRemaining);
            timeText.color = Color.black;
            timeText.transform.localScale = new Vector3(1, 1, 0);
        }

        void Update()
        {
            if (_state == TimeState.Start && ScreenManager.Instance.CurrentPopUp == null)
            {
                //Debug.Log("a");
                if (_timeRemaining - Time.deltaTime > 0)
                {
                    _timeRemaining -= Time.deltaTime;
                    DisplayTime(_timeRemaining);
                }
                else
                {
                    Debug.Log("The time is out");
                    _timeRemaining = 0;
                    _state = TimeState.Pause;
                    GameManager.Instance.LoseTheGame();
                    //burada thepairs.gamemanager de çağırmak lazım yoksa diğer oyunda çalışmayacak
                }

            }

        }

        public void DisplayTime(float time)
        {

            float minutes = Mathf.FloorToInt(time / 60);
            
            float seconds = Mathf.FloorToInt(time % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            //Debug.Log(timeText.text);
            if (_state != TimeState.Pause)
                LowTimeAlert(minutes, seconds);

        }


        private float lastSec;

        public TimeState State { get => _state; set => _state = value; }
        public float TimeRemaining { get => _timeRemaining; set => _timeRemaining = value; }

        private void LowTimeAlert(float minutes, float seconds)
        {
            if (minutes == 0 && seconds <= 10 && lastSec != seconds)
            {
                Debug.Log(Time.time);
                timeText.color = Color.red;
                if (seconds % 2 == 0)
                    timeText.transform.DOScale(new Vector3(2, 2, 2), 0.5f);
                else
                    timeText.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                SoundManager.Instance.PlaySoundClip("TickClip");

                lastSec = seconds;
            }

        }


        public enum TimeState
        {
            Start,
            Pause
        }


    }
}