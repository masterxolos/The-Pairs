using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;

namespace SlidingPuzzle
{
    public class Slot : MonoBehaviour
    {

        private Image image;
        private Button button;

        [SerializeField]
        private Piece correctPiece;
        [SerializeField]
        private Piece currentPiece;

        [SerializeField]
        private Slot upNeighbor, downNeighbor, leftNeighbor, rightNeighbor;

        private List<Slot> neighbors;

        [SerializeField]
        private GameManager _gameManager;


        private void Awake()
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
            }
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }
        }

        private void Start()
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
            button.onClick.AddListener(SlotSelected);

            neighbors = new List<Slot>();
            neighbors.Add(upNeighbor);
            neighbors.Add(downNeighbor);
            neighbors.Add(leftNeighbor);
            neighbors.Add(rightNeighbor);

        }


        private void OnEnable()
        {
            cinkoUsed = false;
        }

        private bool blinking;
        private Tween blink;
        private void Blink()
        {
            if (blinking)
                return;
            blinking = true;
            blink = image.DOFade(1, 0.75f).OnComplete(() =>
            {
                blink = image.DOFade(0, 0.25f).OnComplete(() => { blinking = false; });
                
            });
        }


        private static Slot secondSelected;
        private void SlotSelected()
        {
            _gameManager = GameManager.Instance;
            Debug.Log(gameObject.name + " Selected");

            if (_gameManager.IsHintActive == true)
            {
                StartCoroutine(ShowHint(gameObject.GetComponent<Slot>()));
            }
            else if(currentPiece == null && routineRunning==false)
            {
                Debug.Log("There is no piece in here and not second select");
                return;
            }
            else if (routineRunning)
            {
                secondSelected = this;
                if (this.CurrentPiece != null)
                {
                    StartCoroutine(SlotSelectedRoutine());
                }

            }
            else
            {
                StartCoroutine(SlotSelectedRoutine());
            }

            EventSystem.current.SetSelectedGameObject(null);

            

        }



        private static bool routineRunning;
        private IEnumerator SlotSelectedRoutine()
        {
            yield return new WaitForEndOfFrame();
            routineRunning = true;


            // first looking neighbors if a neighbor is empty send piece to there and exit coroutine
            foreach (Slot neighbor in neighbors)
            {
                if (neighbor != null && neighbor.CurrentPiece == null)
                {
                    SendPieceToAnotherSlot(neighbor);
                    neighbor.ControlCinko();
                    ControlCinko();
                    routineRunning = false;
                    secondSelected = null;
                    ControlWin();
                    yield break;
                }
            }


            // secondly looking pieceCarryRemainder if it's 0 exit coroutine
            if (GameManager.Instance.PieceCarryRemainder <= 0 )
            {
                routineRunning = false;
                secondSelected = null;
                yield break;
            }

            
            currentPiece.OpenSelectedEffect();
            currentPiece.transform.SetAsLastSibling();

            GameManager.Instance.EmptySlot.Blink();
            // wait for select another slot
            while (secondSelected == null)
            {
                
                yield return new WaitForEndOfFrame();
            }
            currentPiece.CloseSelectedEffect();

            //Selected slot empty so we must send currentPiece to secondSelected slot and exit coroutine
            if (secondSelected.CurrentPiece == null)
            {
                GameManager.Instance.PieceCarryRemainder--;
                _gameManager.UpdateInfoTexts();
                SendPieceToAnotherSlot(secondSelected);
                secondSelected.ControlCinko();
                ControlCinko();
                routineRunning = false;
                secondSelected = null;
                ControlWin();
                yield break;
            }

            routineRunning = false;
            secondSelected = null;
            
            yield break;

        }


        private void SendPieceToAnotherSlot(Slot slot)
        {
            
            if(slot.CurrentPiece != null || CurrentPiece == null)
            {
                return;
            }
            slot.CurrentPiece = this.currentPiece;
            slot.CurrentPiece.transform.SetAsLastSibling();
            slot.CurrentPiece.transform.DOMove(slot.transform.position, 0.4f);
            this.currentPiece = null;
            GameManager.Instance.CurrentMovement++;
            SoundManager.Instance.PlaySoundClip("Sliding");
            _gameManager.UpdateInfoTexts();
            Debug.Log("Current movement : "+GameManager.Instance.CurrentMovement);
            
            
        }


        [SerializeField]
        private bool cinkoUsed;
        private void ControlCinko()
        {
            if (cinkoUsed || CurrentPiece != CorrectPiece)
            {
                return;
            }

            Slot left = LeftNeighbor;
            Slot right = RightNeighbor;
            //Debug.Log(name + " current piece : " + currentPiece + " correct piece : " + correctPiece);
            while (left != null)
            {
                //Debug.Log(left + " current piece : " + left.CurrentPiece + " correct piece : " + left.CorrectPiece);
                if (left.CurrentPiece != left.CorrectPiece)
                {
                    return;
                }
                left = left.LeftNeighbor;
            }
            while (right != null)
            {
                //Debug.Log(right + " current piece : " + right.CurrentPiece + " correct piece : " + right.CorrectPiece);
                if (right.CurrentPiece != right.CorrectPiece)
                {
                    return;
                }
                right = right.RightNeighbor;
            }

            left = LeftNeighbor;
            right = RightNeighbor;
            Debug.Log("Cinko");
            SoundManager.Instance.PlaySoundClip("Cinko");
            Debug.Log(name);

            this.cinkoUsed = true;
            this.currentPiece?.StartShinningEffect(1.6f);

            while (left != null)
            {
                Debug.Log(left.name);
                left.CinkoUsed = true;
                left.CurrentPiece?.StartShinningEffect(1.6f);
                left = left.LeftNeighbor;
            }

            while (right != null)
            {
                Debug.Log(right.name);
                right.CinkoUsed = true;
                right.CurrentPiece?.StartShinningEffect(1.6f);
                right = right.RightNeighbor;
            }


            GameManager.Instance.TimeController.TimeRemaining += 20;



        }



        private void ControlWin()
        {

            List<Slot> slots = GameManager.Instance.Slots;

            for (int i = 0; i < slots.Count; i++)
            {
                if(slots[i].CorrectPiece != slots[i].CurrentPiece)
                {
                    return;
                }
            }

            GameManager.Instance.WinTheGame();

        }

        private IEnumerator ShowHint(Slot slot)
        {
            _gameManager.IsHintActive = false;
            if (CorrectPiece != CurrentPiece && GameManager.Instance.HintRemainder>0)
            {
                foreach (var _slot in _gameManager.Slots)
                {
                    if (slot.currentPiece == _slot.correctPiece)
                    {
                        if (currentPiece == null)
                        {
                            _gameManager.IsHintActive = false;
                            yield break;
                        }
                        GameManager.Instance.HintRemainder--;

                        GameObject cloneImage = new GameObject("Clone");
                        cloneImage.transform.SetParent(currentPiece.transform.parent);
                        cloneImage.transform.position = currentPiece.transform.position;
                        Image image = cloneImage.AddComponent<Image>();
                        image.sprite = currentPiece.Sprite;
                        image.rectTransform.sizeDelta = currentPiece.RectTransform.sizeDelta;
                        image.transform.SetAsLastSibling();

                        image.transform.DOMove(_slot.transform.position, 0.4f);
                        yield return new WaitForSeconds(0.6f);
                        image.DOFade(0, 0.5f);
                        yield return new WaitForSeconds(0.6f);
                        Destroy(cloneImage);
                    }
                }
            }
            else if(CorrectPiece != CurrentPiece && GameManager.Instance.HintRemainder <= 0)
            {
                // hint hakkı bitti
            }
            else
            {
                CurrentPiece?.SpawnAlreadyCorrectAnimation(1f);
            }
            _gameManager.UpdateInfoTexts();
        }


        #region GetterSetter
        public Piece CurrentPiece { get => currentPiece; set => currentPiece = value; }
        public Piece CorrectPiece { get => correctPiece; set => correctPiece = value; }

        public RectTransform RectTransform
        {

            get => image.rectTransform;
        }
        public Slot UpNeighbor { get => upNeighbor; set => upNeighbor = value; }
        public Slot DownNeighbor { get => downNeighbor; set => downNeighbor = value; }
        public Slot LeftNeighbor { get => leftNeighbor; set => leftNeighbor = value; }
        public Slot RightNeighbor { get => rightNeighbor; set => rightNeighbor = value; }
        public List<Slot> Neighbors
        {

            get => neighbors;

        }
        public bool CinkoUsed { get => cinkoUsed; set => cinkoUsed = value; }

        #endregion


    }


}