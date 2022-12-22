using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace ThePairs
{

    public class MovementController : MonoBehaviour
    {


        [SerializeField] private TextMeshProUGUI movementText;
        private int _movementLeft;
        private void UpdateMovementText()
        {
            movementText.text = _movementLeft.ToString();
        }

        public void ResetMovementContoller(int maxMovement)
        {
            _movementLeft = maxMovement;
            UpdateMovementText();
        }

        public void OnIncorrectMatch()
        {
            _movementLeft--;
            if (_movementLeft <= 0)
            {
                GameManager.Instance.LoseTheGame();
            }
            UpdateMovementText();
        }

        public void OnCorrectMatch()
        {
            switch (GameManager.Instance.CardController.CurrentCards.Count)
            {
                case > 28:
                    _movementLeft += 6;
                    break;
                case > 26:
                    _movementLeft += 3;
                    break;
                case >= 12:
                    _movementLeft += 2;
                    break;
                case > 0:
                    _movementLeft++;
                    break;
            }
            UpdateMovementText();
        }



    }


}