using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SlidingPuzzle
{
    public class Piece : MonoBehaviour , ICanvasRaycastFilter
    {
        private Image image;


        [SerializeField]
        private GameObject _shinningEffect, _trailEffect, _selectableEffect, _selectedEffect;


        [SerializeField]
        private GameObject AlreadyCorrectAnimation;

        private void Awake()
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
            }
            _shinningEffect = transform.GetChild(0).gameObject;

            _trailEffect = transform.GetChild(1).gameObject;
            Image trailMaskImage = _trailEffect.transform.GetChild(0).gameObject.GetComponent<Image>();
            trailMaskImage.sprite = image.sprite;

            _selectableEffect = transform.GetChild(2).gameObject;
            



            _selectedEffect = transform.GetChild(3).gameObject;
            StopShinningEffect();
            StopSelectableEffect();
            CloseTailEffect();
            CloseSelectedEffect();
        }

        private void Start()
        {
            image = GetComponent<Image>();

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

        private bool isShinning;
        public void StartShinningEffect()
        {
            isShinning = true;
            _shinningEffect.SetActive(true);
        }

        public void StartShinningEffect(float lifeTime)
        {
            isShinning = true;
            _shinningEffect.SetActive(true);
            Invoke("StopShinningEffect", lifeTime);
        }
        public void StopShinningEffect()
        {
            isShinning = false;
            _shinningEffect.SetActive(false);
        }

        public bool IsShinning()
        {
            return isShinning;
        }

        public void StartSelectableEffect()
        {
            _selectableEffect.SetActive(true);
        }

        public void StartSelectableEffect(float lifeTime)
        {
            _selectableEffect.SetActive(true);
            Invoke("StopSelectableEffect", lifeTime);
        }
        public void StopSelectableEffect()
        {
            _selectableEffect.SetActive(false);
        }

        public void OpenTailEffect()
        {
            _trailEffect.SetActive(true);
        }
        public void CloseTailEffect()
        {
            _trailEffect.SetActive(false);
        }

        public void OpenSelectedEffect()
        {
            _selectedEffect.SetActive(true);
        }
        public void CloseSelectedEffect()
        {
            _selectedEffect.SetActive(false);
        }



        public void SpawnAlreadyCorrectAnimation(float lifeTime)
        {

            StartCoroutine(SpawnAlreadyCorrectAnimationRoutine(lifeTime));

        }

        private IEnumerator SpawnAlreadyCorrectAnimationRoutine(float lifeTime)
        {
            transform.SetAsLastSibling();
            GameObject alreadyCorrectAnimation = Instantiate(AlreadyCorrectAnimation);
            alreadyCorrectAnimation.transform.SetParent(transform);
            alreadyCorrectAnimation.GetComponent<RectTransform>().sizeDelta = RectTransform.sizeDelta;
            alreadyCorrectAnimation.transform.position = transform.position;
            yield return new WaitForSeconds(lifeTime);
            Destroy(alreadyCorrectAnimation);
        }

    }

}
