using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.SceneManagement;

namespace SlidingPuzzle
{

    public class GameManager : MonoBehaviour
    {

        private static GameManager instance;
        private TimeController _timeController;
        private ScoreController _scoreController;

        public GameObject mainMenu;
        public GameObject tutorial;

        public Button hintButton;
        
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            _timeController = GetComponent<TimeController>();
            _scoreController = GetComponent<ScoreController>();
            UpdateInfoTexts();
        }


        [SerializeField]
        private int movementMax = 60;
        private int currentMovement;

        [SerializeField]
        private int pieceCarryMax = 5;
        private int pieceCarryRemainder = 5; 


        [SerializeField]
        private int hintMax = 10;
        private int hintRemainder = 10; 
        private bool _isHintActive = false;
        [SerializeField]
        private TextMeshProUGUI _remainingHintText;

        [SerializeField] private TextMeshProUGUI _movementsText;
        [SerializeField] private TextMeshProUGUI _pieceCarryText;

        [SerializeField]
        private List<Slot> slots3x3;
        [SerializeField]
        private List<Piece> pieces3x3;

        [SerializeField]
        private List<Slot> slots4x4;
        [SerializeField]
        private List<Piece> pieces4x4;


        [SerializeField]
        private List<Slot> slots;
        [SerializeField]
        private List<Piece> pieces;
        [SerializeField]
        private GameObject piecesGameObject;

        public void UpdateInfoTexts()
        {
            
            _remainingHintText.text = hintRemainder.ToString();
            _movementsText.text = currentMovement.ToString();
            _pieceCarryText.text = pieceCarryRemainder.ToString();
            
        }



        private IEnumerator ResetGameManager(float waitBefore)
        {

            yield return new WaitForSeconds(waitBefore);
            Destroy(_timeController);
            foreach (Slot slot in slots)
            {
                slot.gameObject.SetActive(true);
            }
            foreach(Piece piece in pieces)
            {
                piece.transform.SetParent(piecesGameObject.transform);
            }

            pieces[pieces.Count - 1].gameObject.SetActive(false);

            pieceCarryRemainder = pieceCarryMax;
            hintRemainder = hintMax;
            currentMovement = 0;

            RandomizePieces();

            yield return new WaitUntil( ()=>!randomRoutineRunning);

            
            _timeController = gameObject.AddComponent<TimeController>();
            _timeController.ResetTimeController(240);
            _timeController.State = TimeController.TimeState.Start;

            _scoreController.ResetScoreController();

            replayRoutineRunning = false;

            foreach (Piece piece in pieces)
            {
                piece.OpenTailEffect();
            }

            UpdateInfoTexts();
            hintButton.enabled = true;

        }
        
        private void RandomizePieces()
        {
            if(!randomRoutineRunning)
                StartCoroutine(RandomRoutine());

        }

        private bool randomRoutineRunning;
        private IEnumerator RandomRoutine()
        {
            randomRoutineRunning = true;
            foreach (Slot slot in slots)
            {
                slot.GetComponent<Button>().enabled = false;
            }
            for (int i = 0; i < 25; i++)
            {
                int randomIndex;
                while (true)
                {
                    randomIndex = Random.Range(0, slots.Count);
                    if (slots[randomIndex] != EmptySlot)
                    {
                        break;
                    }
                }
                Transform emptySlotTransform = EmptySlot.transform;
                EmptySlot.CurrentPiece = slots[randomIndex].CurrentPiece;
                slots[randomIndex].CurrentPiece.transform.DOMove(emptySlotTransform.position, 0.07f);
                slots[randomIndex].CurrentPiece.transform.SetAsLastSibling();
                slots[randomIndex].CurrentPiece = null;
                
                yield return new WaitForSeconds(0.09f);

            }
            randomRoutineRunning = false;
            foreach (Slot slot in slots)
            {
                slot.GetComponent<Button>().enabled = true;
                slot.CinkoUsed = false;
            }

        }


        public void LoseTheGame()
        {
            SoundManager.Instance.PlaySoundClip("Lose");
            ScreenManager.Instance.OpenPopUp("LosePopUp");
            foreach (Slot slot in slots)
            {
                slot.GetComponent<Button>().enabled = false;
            }

        }
        public void WinTheGame()
        {
            
            pieces[pieces.Count-1].gameObject.SetActive(true);
            if (EmptySlot.LeftNeighbor.CurrentPiece.IsShinning())
            {
                pieces[pieces.Count - 1].StartShinningEffect(1.6f);
            }

            SoundManager.Instance.PlaySoundClip("Win");
            ScreenManager.Instance.OpenPopUp("VictoryPopUp");
            _scoreController.OnGameEnd();
            _timeController.State = TimeController.TimeState.Pause;
            foreach (Slot slot in slots)
            {
                slot.GetComponent<Button>().enabled = false;
            }

        }



        #region GetterSetter
        public static GameManager Instance{get => instance;}

        public Slot EmptySlot { get
            {

                foreach (Slot slot in slots)
                {
                    if (slot.CurrentPiece == null)
                    {
                        return slot;
                    }
                }
                return null;
            }
        }

        public int PieceCarryRemainder { get => pieceCarryRemainder; set
            {
                if (value <= 0)
                {
                    ShowPopUpMessageText("tasima hakki bitti", 1.2f);
                }

                pieceCarryRemainder = value;
            }
        }
        public TimeController TimeController { get => _timeController; set => _timeController = value; }
        public int CurrentMovement { get => currentMovement; set => currentMovement = value; }
        public int HintRemainder { 
            get => hintRemainder; 
            set {
                if (value <= 0)
                {
                    GameObject.Find("ShowHintButton").GetComponent<Button>().interactable = false;
                    ShowPopUpMessageText("ipucu hakki bitti",1.2f);
                }
                    
                hintRemainder = value;
            }
        }
        public List<Slot> Slots { get => slots; set => slots = value; }
        public bool IsHintActive { get => _isHintActive;
            set
            {
                _isHintActive = value;
                if (value) {
                    foreach (Slot s in slots)
                    {
                        s.CurrentPiece?.StartSelectableEffect();
                    }
                }
                else
                {
                    foreach (Slot s in slots)
                    {
                        s.CurrentPiece?.StopSelectableEffect();
                    }
                }
            }
        }

        #endregion



        public void StartGame3x3()
        {
            pieces = pieces3x3;
            slots = slots3x3;
            
            foreach(Piece piece in pieces)
            {
                piece.CloseTailEffect();
            }
            hintButton.enabled = false;
            IsHintActive = false;
            StartCoroutine(ResetGameManager(8f));

        }

        public void StartGame4x4()
        {
            pieces = pieces4x4;
            slots = slots4x4;

            foreach (Piece piece in pieces)
            {
                piece.CloseTailEffect();
            }
            hintButton.enabled = false;
            IsHintActive = false;
            StartCoroutine(ResetGameManager(10f));

        }


        public void ReloadScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        public void RePlay()
        {
            foreach (Piece piece in pieces)
            {
                piece.CloseTailEffect();
            }
            if (!replayRoutineRunning)
            {
                hintButton.enabled = false;
                IsHintActive = false;
                StartCoroutine(RePlayRoutine());
            }


        }
        private bool replayRoutineRunning = true;
        private IEnumerator RePlayRoutine()
        {
            replayRoutineRunning=true;
            _timeController.State = TimeController.TimeState.Pause;
            foreach (Slot slot in slots)
            {
                slot.GetComponent<Button>().enabled = false;
            }
            foreach (Piece piece in pieces)
            {
                piece.CloseSelectedEffect();
                piece.Image.DOFade(0, 0.1f);
            }
            yield return new WaitForSeconds(1f);
            foreach (Slot s in slots)
            {
                if (s.CorrectPiece != null)
                {
                    s.CurrentPiece = s.CorrectPiece;
                    s.CurrentPiece.transform.position = s.transform.position;
                }
                else
                {
                    pieces[pieces.Count - 1].gameObject.SetActive(true);
                    s.CurrentPiece = null;
                }
            }

            foreach (Piece piece in pieces)
            {
                piece.Image.DOFade(1, 0.5f);
            }

            yield return new WaitForSeconds(1f);
            StartCoroutine(ResetGameManager(0.5f));

        }


        public void OpenTutorial()
        {
            mainMenu.SetActive(false);
            tutorial.SetActive(true);
        }

        public void OnHintButtonPressed()
        {

            if(!onHintButtonPressedRoutineActive)
                StartCoroutine(OnHintButtonPressedRoutine());


        }


        private bool onHintButtonPressedRoutineActive;
        private IEnumerator OnHintButtonPressedRoutine()
        {

            onHintButtonPressedRoutineActive = true;
            
            if (_isHintActive == true)
            {
                IsHintActive = false;
                yield return new WaitForSeconds(0.5f);
            }
            else if (_isHintActive == false)
            {
                SoundManager.Instance.PlaySoundClip("Hint");
                IsHintActive = true;
            }
            
            
            onHintButtonPressedRoutineActive = false;

        }



        [SerializeField]
        private TMP_FontAsset _popUpMessageFont;
        private void ShowPopUpMessageText(string text,float lifeTime)
        {
            GameObject textGameObject = new GameObject();
            GameObject currentScreen = ScreenManager.Instance.CurrentScreen;
            int x = Screen.width / 2;
            int y = Screen.height / 2;
            //var textGameObject = Instantiate(emptyGameObject, new Vector3(x, y, 0), Quaternion.identity);
            textGameObject.transform.position = new Vector3(x, y, 0);
            textGameObject.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
            TextMeshProUGUI textMeshPro = textGameObject.AddComponent<TextMeshProUGUI>();
            textMeshPro.fontSize = 50;
            if(_popUpMessageFont != null)
                textMeshPro.font = _popUpMessageFont;
            textMeshPro.transform.localScale = textMeshPro.transform.localScale * 3;
            textMeshPro.margin = new Vector4(-50, -50, -50, -50);
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.text = text;
            textMeshPro.color = new Color(0, 0, 0, 0);
            textMeshPro.DOFade(1, 1f).OnComplete(() =>
            {
                Destroy(textGameObject,lifeTime);

            }
            );

        }

    }


}