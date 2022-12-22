using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace MatchingPuzzle
{
    public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        private Image image;
        private Button button;


        [SerializeField]
        private Piece currentPiece;

        [SerializeField]
        private Slot upNeighbor, downNeighbor, leftNeighbor, rightNeighbor;

        private List<Slot> neighbors;

        private bool pointerStillHere;


        private void Awake()
        {
            //Time.timeScale = 0.25f;

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
            if(upNeighbor != null)
                neighbors.Add(upNeighbor);
            if(downNeighbor != null)
                neighbors.Add(downNeighbor);
            if(leftNeighbor!=null)
                neighbors.Add(leftNeighbor);
            if(rightNeighbor!=null)
                neighbors.Add(rightNeighbor);


        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerStillHere = true;

            Invoke("PointerEnterEffects", 0.2f);
                
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            pointerStillHere = false;

        }


        private void PointerEnterEffects()
        {
            if (!pointerStillHere)
                return;
            if (CurrentPiece != null)
            {

                // tüm eþleþen parçalar listeye alýndý
                List<Piece> allMatcingPieces = GetAllMatcingPieces(CurrentPiece.PieceColor);

                // dikey eþleþen parçalar listeye alýndý
                List<Piece> perpendicularMatchingPieces = GetPerpendicularMatcingPieces(CurrentPiece.PieceColor);

                // dikey eþleþen parçalar tüm eþleþen parçalardan çýkartýldý
                List<Piece> allMatcingPiecesWithoutPerpendicular = new List<Piece>(allMatcingPieces);
                foreach (Piece piece in perpendicularMatchingPieces)
                {
                    if (allMatcingPiecesWithoutPerpendicular.Contains(piece))
                    {
                        allMatcingPiecesWithoutPerpendicular.Remove(piece);
                    }
                }

                // dikey olanlar 
                if (perpendicularMatchingPieces.Count <= 1)
                {
                    foreach (Piece piece in perpendicularMatchingPieces)
                    {
                        bottommostPiece.ShowLine("right");
                        bottommostPiece.ShowLine("left");
                        bottommostPiece.ShowLine("down");
                        bottommostPiece.ShowLine("up");
                    }
                    
                }
                if(perpendicularMatchingPieces.Count > 1)//sadece kendisi varsa effecte gerek yok
                {
                    foreach (Piece piece in perpendicularMatchingPieces)
                    {
                        // sag ve sol tarafta çizgi
                        if (piece != topmostPiece || piece != bottommostPiece)
                        {
                            piece.ShowLine("right");
                            piece.ShowLine("left");
                        }
                         
                    }
                    // sag sol ve ust tarafta çizgi
                    topmostPiece.ShowLine("left");
                    topmostPiece.ShowLine("right");
                    topmostPiece.ShowLine("up");
                    // sag sol ve alt tarafta çizgi
                    bottommostPiece.ShowLine("right");
                    bottommostPiece.ShowLine("left");
                    bottommostPiece.ShowLine("down");
                }


                // dikey olmayanlar parlatýldý
                foreach (Piece piece in allMatcingPiecesWithoutPerpendicular)
                {
                    piece.ShinningEffect();
                }

            }
        }


        private void SlotSelected()
        {

            //Debug.Log(name + " selected ");
            // slot bos
            if (CurrentPiece!=null && CurrentPiece.WaitingToDestroy && GameManager.Instance.joker == false)
            {
                return;
            }

            // slotun parcasi henüz düsmemis
            if(CurrentPiece!=null && CurrentPiece.Falling)
            {
                return;
            }



            bool destroy =false;
            // komþularda ayný renk var mý kontrolü
            if(currentPiece != null)
            {
                int x = 0;
                foreach (Slot s in neighbors)
                {

                    if (s != null && s.CurrentPiece != null && s.CurrentPiece.PieceColor == CurrentPiece.PieceColor)
                    {
                        x++;
                        destroy = true;
                        break;
                    }
                }
                if (GameManager.Instance.joker && x==0)
                {
                    MatchingPuzzle.GameManager.Instance.joker = false;
                    GameManager.Instance.JokerUsed();
                    destroy = true; 
                }
            }
            
            if (destroy)
            {
                DestroyPiece();

            }
            else
            {
                // seçilemez taþ effekti 
                CurrentPiece?.Shake();

                SoundManager.Instance.PlaySoundClip("NoMatch");
            }

                
            
            EventSystem.current.SetSelectedGameObject(null);
            

        }
       

        // coroutine ile gorsel olarak daha iyi bir sey yapilabilir
        // birden tum taslarin patlamasi yerine sýra ile taslar patlayabilir
        // tum taslarin patlamasi bitince usteki taslar dusebilir
        private void DestroyPiece()
        {

            SoundManager.Instance.PlaySoundClip("Match");

            // tüm eþleþen parçalar listeye alýndý
            List<Piece> allMatcingPieces = GetAllMatcingPieces(CurrentPiece.PieceColor);

            // dikey eþleþen parçalar listeye alýndý
            List<Piece> perpendicularMatchingPieces = GetPerpendicularMatcingPieces(CurrentPiece.PieceColor);
            
            // dikey eþleþen parçalar tüm eþleþen parçalardan çýkartýldý
            List<Piece> allMatcingPiecesWithoutPerpendicular = new List<Piece>(allMatcingPieces);
            foreach (Piece piece in perpendicularMatchingPieces)
            {
                if (allMatcingPiecesWithoutPerpendicular.Contains(piece))
                {
                    allMatcingPiecesWithoutPerpendicular.Remove(piece);
                }
            }

            PieceColor color = CurrentPiece.PieceColor;
            // dikey olmayanlarýn rengi degistirildi
            foreach (Piece piece in allMatcingPiecesWithoutPerpendicular)
            {
                ScoreManager.Instance.IncreaseScore();
                do
                {
                    piece.PieceColor = GameManager.Instance.GetRandomColor();

                } while (piece.PieceColor.Equals(color));
            }

            // dikey olanlar yok edildi
            foreach (Piece piece in perpendicularMatchingPieces)
            {
                Destroy(piece.gameObject, 5f);
                ScoreManager.Instance.IncreaseScore();
                piece.PieceColor = PieceColor.None;
                piece.WaitingToDestroy = true;

            }


        }

        private List<Piece> GetAllMatcingPieces(PieceColor color)
        {

            List<Piece> result = new List<Piece>();
            
            HashSet<Slot> visited = new HashSet<Slot>();
            Queue<Slot> queue = new Queue<Slot>();

            
            queue.Enqueue(this);
            
            while (queue.Count!=0)
            {
                Slot slot = queue.Dequeue();
                
                if (visited.Contains(slot))
                    continue;

                visited.Add(slot);
                if (slot.CurrentPiece != null && slot.CurrentPiece.PieceColor == color)
                {
                    result.Add(slot.CurrentPiece);
                }
                else
                {
                    continue;
                }

                foreach (Slot n in slot.Neighbors)
                {
                    if (!visited.Contains(n))
                    {
                        queue.Enqueue(n);
                    }
                }


            }

            return result;

        }
        
        private Piece topmostPiece;
        private Piece bottommostPiece;
        private List<Piece> GetPerpendicularMatcingPieces(PieceColor color)
        {
            topmostPiece= CurrentPiece;
            bottommostPiece= CurrentPiece;
            List<Piece> perpendicularPieces = new List<Piece>();
            perpendicularPieces.Add(CurrentPiece);
            Slot up, down;
            up = upNeighbor;
            down = downNeighbor;
            while (true)
            {

                if (up != null && up.CurrentPiece != null && up.CurrentPiece.PieceColor == CurrentPiece.PieceColor)
                {
                    perpendicularPieces.Add(up.CurrentPiece);
                    topmostPiece = up.CurrentPiece;
                    up = up.upNeighbor;
                    
                }
                else
                {
                    up = null;
                }
                if (down != null && down.CurrentPiece != null && down.CurrentPiece.PieceColor == CurrentPiece.PieceColor)
                {
                    perpendicularPieces.Add(down.CurrentPiece);
                    bottommostPiece = down.CurrentPiece;
                    down = down.downNeighbor;
                }
                else
                {
                    down = null;
                }

                if (up == null && down == null)
                {
                    break;
                }

            }
            return perpendicularPieces;

        }



        private const float DelayLimit = 0.075f;
        private float delay;

        private static bool dropSound;

        private void Update()
        {
            delay+=Time.deltaTime;
            if(delay <= DelayLimit)
            {
                return;
            }
            delay = 0;

            // parca silinme asamasinda 
            if(CurrentPiece!=null && CurrentPiece.WaitingToDestroy)
            {
                CurrentPiece = null;
                return ;
            }
            // parca dusuruluyor
            if(CurrentPiece != null && !CurrentPiece.Falling && downNeighbor != null && DownNeighbor.CurrentPiece==null)
            {
                CurrentPiece.Falling = true;
                DownNeighbor.CurrentPiece = CurrentPiece;
                CurrentPiece = null;

                
                if(!dropSound)
                    SoundManager.Instance.PlaySoundClip("Drop",0.2f);

                dropSound = true;

                Piece refDownPiece = DownNeighbor.CurrentPiece;
                refDownPiece.transform.DOMove(DownNeighbor.transform.position, 0.15f).OnComplete(delegate
                {
                    refDownPiece.Falling = false;
                    dropSound = false;
                });

            }

        }







        #region GetterSetter

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
        public Image Image { get => image; set => image = value; }
        public Piece CurrentPiece { get => currentPiece; set => currentPiece = value; }

        #endregion


    }

}

