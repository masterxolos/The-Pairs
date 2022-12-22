using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

namespace MatchingPuzzle
{
    public class Piece : MonoBehaviour, ICanvasRaycastFilter
    {
        [SerializeField]
        private PieceColor pieceColor;

        public Sprite sprite;

        private Image image;

        private bool waitingToDestroy;

        private bool falling;


        public GameObject linePrefab;
        public GameObject linePrefabUpDown;

        private Image lineLeft;
        private Image lineRight;
        private Image lineDown;
        private Image lineUp;

        private void Awake()
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
            }

            lineLeft = Instantiate(linePrefab, transform).GetComponent<Image>();
            lineLeft.transform.position += new Vector3(-27f,0,0);

            lineRight = Instantiate(linePrefab, transform).GetComponent<Image>();
            lineRight.transform.position += new Vector3(25.6f,0,0);
            
            lineDown = Instantiate(linePrefabUpDown, transform).GetComponent<Image>();
            lineDown.transform.position += new Vector3(0,-27.81f,0);

            lineUp = Instantiate(linePrefabUpDown, transform).GetComponent<Image>();
            lineUp.transform.position += new Vector3(0,25.6f,0);
        }

        private void Start()
        {
            image = GetComponent<Image>();
            image.sprite = sprite;
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return false;
        }



        Tweener shake;
        public void Shake()
        {
            if (shake != null && shake.active)
                return;
            shake = transform.DOShakePosition(0.5f, new Vector3(2f, 2f, 0), 10, 50);
        }


        private bool isShining;
        Tweener shining;
        public void ShinningEffect()
        {

            if (shining != null && shining.active)
                return;

            isShining = true;
            shining = Image.DOFade(0f, 0.25f).OnComplete(delegate
            {
                shining = Image.DOFade(1f, 0.25f).OnComplete(()=> { isShining = false; }) ;
            }
            );

        }

        private bool lineActive;
        Tweener lineTweenerDown;
        Tweener lineTweenerLeft;
        Tweener lineTweenerRight;
        Tweener lineTweenerUp;

        public void ShowLine(string side)
        {

            switch (side)
            {
                case "left":
                    if (lineTweenerLeft != null && lineTweenerLeft.active)
                        return;

                    lineActive = true;
                    lineTweenerLeft = lineLeft.DOFade(1f, 0.25f).OnComplete(delegate
                        {
                            lineTweenerLeft = lineLeft.DOFade(0f, 0.25f).OnComplete(() => { lineActive = false; });
                        }
                    );
                    break;
                
                case "right":
                    if (lineTweenerRight != null && lineTweenerRight.active)
                        return;

                    lineActive = true;
                    lineTweenerRight = lineRight.DOFade(1f, 0.25f).OnComplete(delegate
                        {
                            lineTweenerRight = lineRight.DOFade(0f, 0.25f).OnComplete(() => { lineActive = false; });
                        }
                    );
                    break;
                
                case "up":
                    if (lineTweenerUp != null && lineTweenerUp.active)
                        return;

                    lineActive = true;
                    lineTweenerUp = lineUp.DOFade(1f, 0.25f).OnComplete(delegate
                        {
                            lineTweenerUp = lineUp.DOFade(0f, 0.25f).OnComplete(() => { lineActive = false; });
                        }
                    );
                    break;
                
                case "down":
                    if (lineTweenerDown != null && lineTweenerDown.active)
                        return;

                    lineActive = true;
                    lineTweenerDown = lineDown.DOFade(1f, 0.25f).OnComplete(delegate
                        {
                            lineTweenerDown = lineDown.DOFade(0f, 0.25f).OnComplete(() => { lineActive = false; });
                        }
                    );
                    break;
            }
            
        }



        public RectTransform RectTransform
        {
            get => image.rectTransform;
        }

        public Sprite Sprite
        {
            get { return image.sprite; }
            set { image.sprite = value; }
        }

        public Image Image { get => image; set => image = value; }

        private bool firstSettingOfPieceColor=true;
        public PieceColor PieceColor { 
            get => pieceColor;
            set{ 
                pieceColor = value;
                if (pieceColor == PieceColor.Red)
                {
                    if (firstSettingOfPieceColor)
                    {
                        image.color = Color.red;
                        firstSettingOfPieceColor = false;
                    }
                    else
                    {
                        image.DOColor(Color.red, 0.25f);
                    }

                }
                else if(pieceColor == PieceColor.Green)
                {
                    if (firstSettingOfPieceColor)
                    {
                        image.color = Color.green;
                        firstSettingOfPieceColor = false;
                    }
                    else
                    {
                        image.DOColor(Color.green, 0.25f);
                    }
                }
                else if (pieceColor == PieceColor.Yellow)
                {
                    if (firstSettingOfPieceColor)
                    {
                        image.color = Color.yellow;
                        firstSettingOfPieceColor = false;
                    }
                    else
                    {
                        image.DOColor(Color.yellow, 0.25f);
                    }
                }
                else if (pieceColor == PieceColor.Blue)
                {
                    if (firstSettingOfPieceColor)
                    {
                        image.color = Color.blue;
                        firstSettingOfPieceColor = false;
                    }
                    else
                    {
                        image.DOColor(Color.blue, 0.25f);
                    }
                }
                else
                {
                    image.DOFade(0, 0.25f);
                }

            }
        }

        public bool Falling { get => falling; set => falling = value; }
        public bool WaitingToDestroy { get => waitingToDestroy; set => waitingToDestroy = value; }
        public bool IsShining { get => isShining; set => isShining = value; }
        public bool LineActive { get => lineActive; set => lineActive = value; }
    }

}

public enum PieceColor
{
    None,Green,Yellow,Red,Blue
}
