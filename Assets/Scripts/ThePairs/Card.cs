using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace ThePairs
{

    public class Card : MonoBehaviour
    {



        public GameObject mFront;
        public GameObject mBack;
        public VisibleSide visibleSide = VisibleSide.Front;
        public bool isJoker;
        [SerializeField]
        private GameObject _shinningEffect;

        private float mTimeOpen = 0.15f;
        private float mTimeClose = 0.075f;



        private string _ID;

        public string ID { get => _ID; set => _ID = value; }
        public GameObject ShinningEffect { get => _shinningEffect; set => _shinningEffect = value; }

        private void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(delegate { GameManager.Instance.CardController.SelectCard(this); });
        }

        public bool IsSameWith(Card card)
        {
            if (card.ID.Equals(ID))
                return true;
            else
                return false;
        }




        public bool opening;
        public void OpenTheCard()
        {

            opening = true;
            mFront.SetActive(true);
            mBack.transform.DORotate(new Vector3(0, 90, 0), mTimeOpen).OnComplete(delegate
            {
                mFront.transform.DORotate(new Vector3(0, 0, 0), mTimeOpen).OnComplete(delegate
                {
                    Invoke("ChangeStateFront", 0.05f);
                    mBack.SetActive(false);
                });


            });




        }

        public bool closing;

        public void CloseTheCard()
        {
            closing = true;
            mBack.SetActive(true);
            mFront.transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
            {
                mBack.transform.DORotate(new Vector3(0, 0, 0), mTimeClose).OnComplete(delegate
                {
                    Invoke("ChangeStateBack", 0.05f);
                    mFront.SetActive(false);
                });


            });

        }


        private void ChangeStateFront()
        {

            visibleSide = VisibleSide.Front;
            opening = false;


        }
        private void ChangeStateBack()
        {
            visibleSide = VisibleSide.Back;
            closing = false;

        }



        public void ResetTheCard()
        {

            ID = mFront.GetComponent<Image>().sprite.name;
            visibleSide = VisibleSide.Front;
            mFront.SetActive(true);
            mBack.SetActive(true);

            ShinningEffect.SetActive(false);

            if (visibleSide == VisibleSide.Front)
            {
                mFront.transform.eulerAngles = Vector3.zero;
                mBack.transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else
            {
                mFront.transform.eulerAngles = new Vector3(0, 90, 0);
                mBack.transform.eulerAngles = Vector3.zero;
            }

            if (ID.Equals("icon_1"))
            {
                isJoker = true;
            }
            else
            {
                isJoker = false;
            }


            //Invoke("CloseTheCard", 3f);

        }





        #region Card Animations

        public void InCorrectAnimation(float duration)
        {

            mFront.transform.DOShakePosition(duration, new Vector3(2f, 2f, 0), 10, 50);

        }

        public void CorrectAnimation(float duration)
        {

            mFront.GetComponent<Image>().DOFade(0.5f, duration);
            //mFront.transform.DOScale(new Vector3(0, 0, 0), duration);
            ShinningEffect.SetActive(false);
            //Debug.Log("card "+isJoker);
        }




        #endregion

        public enum VisibleSide
        {
            Front,
            Back
        }


    }


}