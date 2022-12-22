using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThePairs
{

    [ExecuteInEditMode]
    // istenilen oyun ekraninin duzenlemesi icin yazilis bir script / runtime da kullanilmiyor
    public class GameBuilder : MonoBehaviour
    {


        public bool arrangeSprites = false, arangePos = false;

        public Sprite[] cardSprites;

        public Card[] cards;


        // Update is called once per frame
        void Update()
        {

            if (arrangeSprites)
            {
                arrangeSprites = false;
                ArrangeSprites();
            }

            if (arangePos)
            {
                arangePos = false;
                ArrangePositions(rowElemetNum);
            }

        }


        public void ArrangeSprites()
        {

            for (int i = 0; i < cards.Length; i += 2)
            {
                /*
                cards[i].name = "Card " + (i+1);
                cards[i + 1].name = "Card " + (i + 2);
                */
                cards[i].mFront.GetComponent<Image>().sprite = cardSprites[i / 2];
                cards[i + 1].mFront.GetComponent<Image>().sprite = cardSprites[i / 2];
            }

        }

        public int rowElemetNum;
        public Transform firstCardPos;
        public float distanceX, distanceY;

        public void ArrangePositions(int rowElementNum)
        {
            Vector3 firstPos = firstCardPos.position;

            float x = firstPos.x;
            float y = firstPos.y;
            for (int i = 0; i < cards.Length; i++)
            {
                if (i % rowElemetNum == 0 && i != 0)
                {
                    y -= distanceY;
                    x = firstPos.x;
                }
                cards[i].transform.position = new Vector3(x, y, 0);
                x += distanceX;
            }


        }


    }


}