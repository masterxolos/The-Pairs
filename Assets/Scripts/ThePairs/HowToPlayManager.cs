using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ThePairs
{
    public class HowToPlayManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> cardsFront;
        [SerializeField] private List<GameObject> cardsBack;
        [SerializeField] private GameObject hand;
        private float mTimeClose = 0.4f;

        // Start is called before the first frame update
        void OnEnable()
        {

            for (int i = 0; i < cardsBack.Count; i++)
            {
                cardsBack[i].GetComponent<Image>().sprite = GameManager.Instance.CardController.CurrentSkin;
                cardsBack[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            for (int i = 0; i < cardsFront.Count; i++)
            {
                cardsFront[i].transform.rotation = Quaternion.Euler(0, 90, 0);
            }

            StartCoroutine(HowToPlay());

        }



        IEnumerator HowToPlay()
        {
            while (true)
            {
                //burada yanlış eşleştirme yapıyor
                hand.GameObject().SetActive(true);
                hand.transform.position = cardsFront[0].transform.position;

                //ilk kart dönüyor

                hand.transform.DOScale(0.93f, 0.2f).OnComplete(delegate
                {
                    cardsBack[0].transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
                    {
                        cardsFront[0].transform.DORotate(new Vector3(0, 0, 0), mTimeClose);

                    });
                    hand.transform.DOScale(1f, 0.2f);
                });

                yield return new WaitForSeconds(1);
                hand.transform.DOMove(cardsFront[1].transform.position, 1);
                yield return new WaitForSeconds(1);

                //ikinci kart dönüyor

                hand.transform.DOScale(0.93f, 0.2f).OnComplete(delegate
                {
                    hand.transform.DOScale(1f, 0.2f);
                    cardsBack[2].transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
                    {
                        cardsFront[1].transform.DORotate(new Vector3(0, 0, 0), mTimeClose);
                        hand.GameObject().SetActive(false);
                    });
                });

                yield return new WaitForSeconds(1);

                //ikisi de kapanıyor
                cardsFront[0].transform.DOShakePosition(1f, new Vector3(2f, 2f, 0), 10, 50).OnComplete(delegate
                {
                    cardsFront[0].transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
                    {
                        cardsBack[0].transform.DORotate(new Vector3(0, 0, 0), mTimeClose);
                    });
                });

                cardsFront[1].transform.DOShakePosition(1f, new Vector3(2f, 2f, 0), 10, 50).OnComplete(delegate
                {
                    cardsFront[1].transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
                    {
                        cardsBack[2].transform.DORotate(new Vector3(0, 0, 0), mTimeClose);
                    });
                });


                yield return new WaitForSeconds(2f);


                //burada doğru eşleştirme yapıyor
                hand.GameObject().SetActive(true);
                hand.transform.position = cardsFront[0].transform.position;

                //ilk kart dönüyor
                cardsBack[0].transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
                {
                    cardsFront[0].transform.DORotate(new Vector3(0, 0, 0), mTimeClose);
                    hand.transform.DOScale(0.93f, 0.2f).OnComplete(delegate
                    {
                        hand.transform.DOScale(1f, 0.2f);
                    });
                });


                yield return new WaitForSeconds(1);
                hand.transform.DOMove(cardsFront[2].transform.position, 1);
                yield return new WaitForSeconds(1);

                //ikinci kart dönüyor
                cardsBack[3].transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
                {
                    cardsFront[2].transform.DORotate(new Vector3(0, 0, 0), mTimeClose);
                    hand.GameObject().SetActive(false);
                });
                hand.transform.DOScale(0.93f, 0.2f).OnComplete(delegate
                {
                    hand.transform.DOScale(1f, 0.2f);
                });

                yield return new WaitForSeconds(3);

                //ikisi de kapanıyor
                cardsFront[0].transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
                {
                    cardsBack[0].transform.DORotate(new Vector3(0, 0, 0), mTimeClose);
                });

                cardsFront[2].transform.DORotate(new Vector3(0, 90, 0), mTimeClose).OnComplete(delegate
                {
                    cardsBack[3].transform.DORotate(new Vector3(0, 0, 0), mTimeClose);
                });
                yield return new WaitForSeconds(1);
            }
        }
    }


}