using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MatchingPuzzle
{
    public class GameManager : MonoBehaviour
    {


        private static GameManager instance;


        private void Awake()
        {
            instance = this;
        }


        [SerializeField]
        private Sprite gridSprite;

        [SerializeField]
        private Sprite[] foodSprites;

        [SerializeField]
        private GameObject boiler;

        private List<Slot> slots;
        private List<Piece> pieceList;
        public bool joker;
        
        public Texture2D cursorTexture;
        public CursorMode cursorMode = CursorMode.Auto;
        public Vector2 hotSpot = Vector2.zero;


        //public RectTransform table;
        private void Start()
        {

            slots = new List<Slot>();
            slots.AddRange(GameObject.FindObjectsOfType<Slot>());

            pieceList = new List<Piece>();
            pieceList.AddRange(GameObject.FindObjectsOfType<Piece>());


            foreach (Piece piece in pieceList)
            {
                piece.PieceColor = GetRandomColor();
            }

            /*
            pieceList = new List<Piece>();

            int rows=10, cols=9;
            // create and add slots
            slots = GameBuilder.CreateSlots(table, rows, cols, gridSprite);


            GameObject piecesGO = new GameObject("Pieces");
            piecesGO.transform.SetParent(table);
            // create and add pieces
            for (int i = 0; i < slots.Count; i++)
            {

                GameObject temp = new GameObject("Piece " + i);
                temp.transform.SetParent(piecesGO.transform);
                Piece piece = temp.AddComponent<Piece>();
                piece.sprite = gridSprite;
                piece.PieceColor = GetRandomColor();
                piece.Image.rectTransform.sizeDelta = slots[i].Image.rectTransform.sizeDelta;
                temp.transform.position = slots[i].transform.position;

                slots[i].CurrentPiece = piece;
                pieceList.Add(piece);
            }

            GameObject foodsGO = new GameObject("Foods");
            foodsGO.transform.SetParent(table);
            // yemekler ekleniyor
            for (int i = 1; i < cols; i+=2)
            {
                GameObject temp = new GameObject("Food " + (i/2));
                temp.transform.SetParent(foodsGO.transform);
                Food food = temp.AddComponent<Food>();
                food.sprite = foodSprites[(i/2)];
                food.Image.rectTransform.sizeDelta = slots[i].Image.rectTransform.sizeDelta;
                food.Slot = slots[i];
                temp.transform.position = slots[i].transform.position + new Vector3(0, slots[i].Image.rectTransform.sizeDelta.y, 0);
            }
            */


        }


        public void Joker()
        {
            
            joker = ScoreManager.Instance.UseJoker();
            if (joker)
            {
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            }
            
        }

        public void JokerUsed()
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
            SoundManager.Instance.PlaySoundClip("Hammer");
            
        }


        public PieceColor GetRandomColor()
        {
            return (PieceColor)Random.Range(1, Enum.GetValues(typeof(PieceColor)).Length);
        }

        public static GameManager Instance { get => instance; set => instance = value; }
        public GameObject Boiler { get => boiler; set => boiler = value; }
    }


}
