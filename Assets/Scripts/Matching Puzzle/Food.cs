using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MatchingPuzzle
{
    public class Food : MonoBehaviour, ICanvasRaycastFilter
    {

        public Sprite sprite;

        private Image image;
        [SerializeField]
        private Slot slot;

        private bool waitingToDestroy;

        private bool falling;
        private void Awake()
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
            }

        }

        private void Start()
        {
            image = GetComponent<Image>();
            image.sprite = sprite;
        }


        private const float DelayLimit = 0.075f;
        private float delay;

        private void Update()
        {


            delay += Time.deltaTime;
            if (delay <= DelayLimit)
            {
                return;
            }
            delay = 0;

            if (slot.CurrentPiece == null)
            {
                if (slot.DownNeighbor != null && !falling)
                {
                    slot = slot.DownNeighbor;
                    Vector3 newPos = slot.transform.position + new Vector3(0, slot.RectTransform.sizeDelta.y, 0);
                    falling = true;
                    transform.DOMove(newPos, 0.15f).OnComplete(delegate
                    {
                        falling = false;
                    });
                }
                else if(slot.DownNeighbor == null && !falling)
                {
                    // kazana yolla
                    //Debug.Log("kazana");
                    Vector3 newPos = GameManager.Instance.Boiler.transform.position;
                    falling = true;
                    Destroy(gameObject, 2f);
                    transform.DOMove(newPos, 0.5f).OnComplete(delegate
                    {
                        SoundManager.Instance.PlaySoundClip("DropBoiler");
                    });

                }
            }

        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return false;
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

        public bool Falling { get => falling; set => falling = value; }
        public bool WaitingToDestroy { get => waitingToDestroy; set => waitingToDestroy = value; }
        public Slot Slot { get => slot; set => slot = value; }
    }

}
