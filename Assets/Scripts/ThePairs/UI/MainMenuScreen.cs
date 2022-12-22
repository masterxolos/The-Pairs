using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ThePairs.UI
{

    public class MainMenuScreen : MonoBehaviour
    {

        [SerializeField]
        private GameObject _loading;

        [SerializeField]
        private float _loadingTime = 3f;

        [SerializeField]
        private TextMeshProUGUI _loadingText;

        private string[] _loadingTexts =
        {
        "LOADING",
        "LOADING .",
        "LOADING . .",
        "LOADING . . .",
    };

        private void Start()
        {
            _loadingText.DOFade(0.3f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        private void OnEnable()
        {
            SoundManager.Instance.PlayMusic();
            StartCoroutine(LoadingRoutine());
        }

        private IEnumerator LoadingRoutine()
        {
            OpenLoading();

            int parts = 50;
            float deltaFill = 1f / parts;
            float deltaTime = _loadingTime / parts;

            Slider loadingBar = _loading.transform.GetChild(1).gameObject.GetComponent<Slider>();
            loadingBar.value = 0;

            int textIndex = 0;

            for (int i = 0; i < parts; i++)
            {
                if (i % (parts / 10) == 0)
                {
                    _loadingText.text = _loadingTexts[textIndex % _loadingTexts.Length];
                    textIndex++;
                }

                loadingBar.value += deltaFill;
                yield return new WaitForSeconds(deltaTime);

            }


            //yield return new WaitForSeconds(_loadingTime);

            CloseLoading();
        }


        private void OpenLoading()
        {

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            _loading.SetActive(true);

        }


        private void CloseLoading()
        {

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            _loading.SetActive(false);

        }



    }


}