using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchingPuzzle
{
    public class GameBuilder{
        public static List<Slot> CreateSlots(RectTransform parent, int rowNumber
            , int columnNumber, Sprite sprite)
        {

            List<Slot> slotList = new List<Slot>();
            GameObject leftUpGO = new GameObject("leftUpRef");
            leftUpGO.transform.SetParent(parent);
            RectTransform leftUpRef = leftUpGO.AddComponent<RectTransform>();
            leftUpRef.sizeDelta = new Vector2(parent.sizeDelta.x / (columnNumber * 1f), parent.sizeDelta.y / (rowNumber * 1f));
            leftUpRef.anchoredPosition = new Vector3(-parent.sizeDelta.x / 2 + leftUpRef.sizeDelta.x / 2
                , parent.sizeDelta.y / 2 - leftUpRef.sizeDelta.y / 2, 0);

            GameObject slotsGO = new GameObject("Slots");
            slotsGO.transform.SetParent(parent);
            slotsGO.transform.position = leftUpRef.position;
            Slot[][] slots = new Slot[columnNumber][];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new Slot[rowNumber];
            }

            float deltaX = leftUpRef.rect.width;
            float deltaY = -leftUpRef.rect.height;

            float currentX = leftUpRef.transform.position.x, currentY = leftUpRef.transform.position.y;

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    GameObject go = new GameObject("Slot" + (j + (i * columnNumber)));
                    slots[j][i] = go.AddComponent<Slot>();
                    slots[j][i].transform.SetParent(slotsGO.transform);
                    slots[j][i].transform.position = new Vector3(currentX, currentY, 0);
                    slots[j][i].RectTransform.sizeDelta = leftUpRef.sizeDelta;
                    slots[j][i].Image.sprite = sprite;
                    currentX += deltaX;
                    slotList.Add(slots[j][i]);

                }
                currentX = leftUpRef.transform.position.x;
                currentY += deltaY;

            }

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    SetNeighbors(slots, slots[j][i], j, i, columnNumber, rowNumber);
                }
            }


            GameObject.Destroy(leftUpGO);



            return slotList;

        }



        public static void SetNeighbors(Slot[][] slots, Slot slot, int x, int y, int numberOfRows, int numberOfCols)
        {
            int upNeighborX = x, upNeighborY = y - 1;
            int downNeighborX = x, downNeighborY = y + 1;
            int leftNeighborX = x - 1, leftNeighborY = y;
            int rightNeighborX = x + 1, rightNeighborY = y;

            if (IsBetweenRange(upNeighborX, 0, numberOfRows) && IsBetweenRange(upNeighborY, 0, numberOfCols))
            {
                slot.UpNeighbor = slots[upNeighborX][upNeighborY];
            }
            if (IsBetweenRange(downNeighborX, 0, numberOfRows) && IsBetweenRange(downNeighborY, 0, numberOfCols))
            {
                slot.DownNeighbor = slots[downNeighborX][downNeighborY];
            }
            if (IsBetweenRange(leftNeighborX, 0, numberOfRows) && IsBetweenRange(leftNeighborY, 0, numberOfCols))
            {
                slot.LeftNeighbor = slots[leftNeighborX][leftNeighborY];
            }
            if (IsBetweenRange(rightNeighborX, 0, numberOfRows) && IsBetweenRange(rightNeighborY, 0, numberOfCols))
            {
                slot.RightNeighbor = slots[rightNeighborX][rightNeighborY];
            }
        }


        public static bool IsBetweenRange(float val, float value1, float value2)
        {

            if (val >= value1 && val < value2)
                return true;
            return false;

        }


    }
}

