using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ThePairs
{
    public class CardController : MonoBehaviour
    {

        [SerializeField]
        private Sprite _currentSkin;

        private List<Card> _currentCards;

        public List<Card> CurrentCards { get => _currentCards; }
        public Sprite CurrentSkin { get => _currentSkin; }

        public void ResetCardController()
        {
            _openCard1 = null;
            _openCard2 = null;
            ReadCurrentCards(ScreenManager.Instance.CurrentScreen);
            RandomizeCards();
            ChangeCardsBG(_currentSkin);
            currentCase = CASES.NoOpenCard;
        }


        private void ReadCurrentCards(GameObject level)
        {
            _currentCards = level.GetComponentsInChildren<Card>().ToList();

            RandomizeCards();
            foreach (Card card in _currentCards)
            {
                card.ResetTheCard();
            }
        }

        private void RandomizeCards()
        {

            List<Sprite> sprites = new List<Sprite>();
            for (int i = 0; i < _currentCards.Count; i++)
            {
                Image temp = _currentCards[i].mFront.GetComponent<Image>();
                sprites.Add(temp.sprite);
            }

            int j = 0;
            int rand;
            while (sprites.Count > 0)
            {
                rand = Random.Range(0, sprites.Count);
                _currentCards[j].mFront.GetComponent<Image>().sprite = sprites[rand];
                _currentCards[j].ResetTheCard();
                j++;
                sprites.RemoveAt(rand);
            }


        }


        private Card _openCard1, _openCard2;

        private CASES currentCase;
        public void SelectCard(Card card)
        {

            if (ScreenManager.Instance.CurrentPopUp != null)
                return;

            if (card.visibleSide == Card.VisibleSide.Front ||
                card == _openCard1 || card == _openCard2)
                return;

            switch (currentCase)
            {
                case CASES.NotReady:
                    Debug.Log("not ready");
                    //not ready do nothing

                    return;
                case CASES.NoOpenCard:

                    Debug.Log(card.name + " opened ! ");
                    card.OpenTheCard();

                    if (card.isJoker)
                    {
                        SoundManager.Instance.PlaySoundClip("JokerCardFlipClip");
                    }
                    else
                    {
                        SoundManager.Instance.PlaySoundClip("CardFlipClip");
                    }


                    _openCard1 = card;
                    currentCase = CASES.OneOpenCard;

                    if (_currentCards.Count == 1)
                    {
                        Debug.Log("last card");
                        CurrentCards.Remove(card);
                        GameManager.Instance.WinTheGame();
                        Debug.Log("Game Over");
                    }


                    break;
                case CASES.OneOpenCard:

                    if (_openCard1 == null)
                    {
                        currentCase = CASES.NoOpenCard;
                        return;
                    }
                    Debug.Log(card.name + " opened ! ");
                    if (card.isJoker)
                    {
                        SoundManager.Instance.PlaySoundClip("JokerCardFlipClip");
                    }
                    else
                    {
                        SoundManager.Instance.PlaySoundClip("CardFlipClip");
                    }
                    _openCard2 = card;



                    if (_openCard1.isJoker && _openCard2.isJoker)
                    {
                        _openCard2.OpenTheCard();
                    }
                    else if (_openCard1.isJoker)
                    {
                        _openCard2.OpenTheCard();
                        CurrentCards.Remove(_openCard2);
                        _openCard2.CorrectAnimation(1f);

                        Card joker = _openCard1;
                        _currentCards.Remove(joker);
                        joker.CorrectAnimation(1f);

                        _openCard1 = FindPair(_openCard2);
                        _openCard1.OpenTheCard();

                    }
                    else if (_openCard2.isJoker)
                    {

                        Card joker = _openCard2;
                        _currentCards.Remove(joker);
                        joker.CorrectAnimation(1f);
                        joker.OpenTheCard();

                        _openCard2 = FindPair(_openCard1);
                        _openCard2.OpenTheCard();

                    }
                    else
                    {

                        _openCard2.OpenTheCard();
                    }





                    if (_openCard1.IsSameWith(_openCard2))
                    {
                        StartCoroutine(CorrectPair());
                    }
                    else
                    {
                        StartCoroutine(IncorrectPair());
                    }

                    break;
            }

        }

        private IEnumerator CorrectPair()
        {
            currentCase = CASES.NotReady;
            Debug.Log(_openCard1.name + " and " + _openCard2.name + " are same ! ");

            GameManager.Instance.ScoreController.OnCorrectMatch();
            if (ScreenManager.Instance.CurrentScreen.name.Equals("HardGameScreen"))
                GameManager.Instance.MovementController.OnCorrectMatch();

            // wait for card opening
            yield return new WaitUntil(() => !_openCard2.opening && !_openCard1.opening);

            // right sound effect
            SoundManager.Instance.PlaySoundClip("CorrectClip");

            float duration = 1f;
            _openCard1.CorrectAnimation(duration);
            _openCard2.CorrectAnimation(duration);
            yield return new WaitForSeconds(duration / 3);

            _currentCards.Remove(_openCard1);
            _currentCards.Remove(_openCard2);



            if (IsGameOver() == 1)
            {
                // Game Over can change current level
                GameManager.Instance.WinTheGame();
                Debug.Log("Game Over");

            }
            else if (IsGameOver() == 2)
            {
                GameManager.Instance.ScoreController.Score += 10;
                CurrentCards[0].OpenTheCard();
                SoundManager.Instance.PlaySoundClip("JokerCardFlipClip");

                yield return new WaitForSeconds(0.5f);
                GameManager.Instance.WinTheGame();

                Debug.Log("Game Over");
            }
            else
            {
                // do nothing
            }
            Debug.Log(_currentCards.Count);
            currentCase = CASES.NoOpenCard;

        }

        private IEnumerator IncorrectPair()
        {
            currentCase = CASES.NotReady;
            Debug.Log(_openCard1.name + " and " + _openCard2.name + " are not same ! ");

            //GameManager.Instance.OnIncorrectMatch();

            GameManager.Instance.ScoreController.OnIncorrectMatch();



            // wait for card opening
            yield return new WaitUntil(() => !_openCard2.opening && !_openCard1.opening);

            // wrong sound effect
            SoundManager.Instance.PlaySoundClip("IncorrectClip");

            float duration = 1f;
            _openCard1.InCorrectAnimation(duration);
            _openCard2.InCorrectAnimation(duration);
            // wait for animation
            yield return new WaitForSeconds(duration + 0.1f);


            _openCard1?.CloseTheCard();
            _openCard2?.CloseTheCard();
            // wait for card closing
            yield return new WaitUntil(()
                => ((_openCard2 == null || !_openCard2.closing) && (_openCard1 == null || !_openCard1.closing))
            );
            Debug.Log("over");
            _openCard1 = null;
            _openCard2 = null;
            currentCase = CASES.NoOpenCard;
            if (ScreenManager.Instance.CurrentScreen.name.Equals("HardGameScreen"))
                GameManager.Instance.MovementController.OnIncorrectMatch();


        }


        private int IsGameOver()
        {
            //1 tane kart kaldıysa joker olmak zorunda
            if (_currentCards.Count == 1)
            {
                return 2;
            }
            //0 kart kaldıysa joker kullanılmış ve oyun bitmiş
            if (_currentCards.Count == 0)
            {
                return 1;
            }
            //1den fazla kart var
            else
            {
                return 0;
            }
        }


        public void ChangeCardsBG(Sprite sprite)
        {

            _currentSkin = sprite;
            if (ScreenManager.Instance.CurrentScreen.name.Equals("MainMenuScreen"))
            {
                return;
            }
            foreach (Card card in _currentCards)
            {
                card.mBack.GetComponent<Image>().sprite = _currentSkin;
            }

        }


        private Card FindPair(Card card)
        {

            for (int i = 0; i < _currentCards.Capacity; i++)
            {
                if (card.IsSameWith(_currentCards[i]) && card != _currentCards[i])
                {
                    return _currentCards[i];
                }
            }
            return null;
        }


        #region enums
        private enum CASES
        {

            NoOpenCard, OneOpenCard, NotReady

        }

        #endregion



    }


}