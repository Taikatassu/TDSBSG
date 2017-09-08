using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform mainMenuHolder;
    Button startButton;
    Button quitButton;

    private void Start()
    {
        GameObject newStartButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
        startButton = newStartButton.GetComponent<Button>();
        startButton.GetComponentInChildren<Text>().text = "PLAY";
        startButton.onClick.AddListener(OnStartButtonPressed);

        GameObject newQuitButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
        quitButton = newQuitButton.GetComponent<Button>();
        quitButton.GetComponentInChildren<Text>().text = "QUIT";
        quitButton.onClick.AddListener(OnQuitButtonPressed);
    }

    private void OnStartButtonPressed()
    {
        Debug.Log("Start button pressed");
        //Start the game (load first level, close main menu)
    }

    private void OnQuitButtonPressed()
    {
        Debug.Log("Quit button pressed");
        //Stop everything, close application
    }

}
