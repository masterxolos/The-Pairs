using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HowToPlay : MonoBehaviour
{
    public GameObject hand;
    public GameObject img1;
    public GameObject img2;
    public GameObject emptyArea;

    public Vector3 pos;
    public Vector3 pos2;
    void Start()
    {
        FirstMoves();
    }


    public void FirstMoves()
    {
        hand.transform.DOMove(img1.transform.position, 0.5f).OnComplete(() =>
        {
            hand.transform.DOScale(0.9f, 0.2f).OnComplete(() =>
            {
                hand.transform.DOScale(1f, 0.2f);
                pos = img1.transform.position;
                img1.transform.DOMove(emptyArea.transform.position, 0.5f).OnComplete(() =>
                {
                    hand.transform.DOMove(img2.transform.position, 0.5f).OnComplete(() =>
                    {
                        hand.transform.DOScale(0.9f, 0.2f).OnComplete(() =>
                        {
                            hand.transform.DOScale(1f, 0.2f);
                            pos2 = img2.transform.position;
                            img2.transform.DOMove(pos, 0.5f).OnComplete(() =>
                            {
                                pos = img2.transform.position;
                                SecondMoves();
                            });
                        });
                    });
                });
            });
        });
    }

    public void SecondMoves()
    {
        hand.transform.DOMove(img2.transform.position, 0.5f).OnComplete(() =>
        {
            hand.transform.DOScale(0.9f, 0.2f).OnComplete(() =>
            {
                hand.transform.DOScale(1f, 0.2f);
                pos = img2.transform.position;
                img2.transform.DOMove(pos2, 0.5f).OnComplete(() =>
                {
                    hand.transform.DOMove(img1.transform.position, 0.5f).OnComplete(() =>
                    {
                        hand.transform.DOScale(0.9f, 0.2f).OnComplete(() =>
                        {
                            hand.transform.DOScale(1f, 0.2f);
                            pos2 = img1.transform.position;
                            img1.transform.DOMove(pos, 0.5f).OnComplete(() =>
                            {
                                FirstMoves();
                            });
                        });
                    });
                });
            });
        });
    }
}
