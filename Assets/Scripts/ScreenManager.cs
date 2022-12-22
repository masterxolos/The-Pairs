using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{

    private static ScreenManager instance;
    public static ScreenManager Instance { get => instance; set => instance = value; }
    public GameObject CurrentScreen { get => currentScreen; }
    public bool Bloked { get => _bloked; set => _bloked = value; }
    public GameObject CurrentPopUp { get => currentPopUp; }

    [SerializeField]
    private GameObject currentScreen;

    private GameObject currentPopUp;

    [SerializeField]
    private GameObject[] screens;

    [SerializeField]
    private GameObject[] popUps;

    private bool _bloked;
    private void Awake()
    {

        screenDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject s in screens)
        {
            screenDictionary.Add(s.name, s);
        }

        popUpDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject s in popUps)
        {
            popUpDictionary.Add(s.name, s);
        }

        MakeSingleton();

    }

    public Dictionary<string, GameObject> screenDictionary;
    private Dictionary<string, GameObject> popUpDictionary;

    public void LoadScreen(string screenName)
    {
        if (_bloked)
            return;

        //SoundManager.Instance.PlayMusic();

        currentScreen?.SetActive(false);
        currentScreen = screenDictionary[screenName];
        currentScreen.SetActive(true);

    }


    public void OpenPopUp(string popUpName)
    {
        if (_bloked)
            return;

        currentPopUp?.SetActive(false);
        currentPopUp = popUpDictionary[popUpName];
        currentPopUp.SetActive(true);
    }

    public bool IsPopUpOpened(string popUpName)
    {
        return currentPopUp == popUpDictionary[popUpName];
    }

    public void ClosePopUp()
    {
        currentPopUp?.SetActive(false);
        currentPopUp = null;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }



}
