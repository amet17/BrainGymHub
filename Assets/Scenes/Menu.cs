using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuPanel, hopscotchGameLevelMenuPanel, memoryGameLevelMenuPanel, hopscotchGameDesk, memoryGameDesk, hopscotchGameMenu, finishHopscotchGameButtons, finishMemoryGameButtons, stepTimer;
    public Text textInfo, gameName;
    public AudioSource choseGameAudio; 
    // Start is called before the first frame update
    void Start()
    {
        openMainMenu(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openMainMenu()
    {
        textInfo.text = "";
        gameName.text = "Brain Gym Hub";
        mainMenuPanel.SetActive(true);
        hopscotchGameLevelMenuPanel.SetActive(false);
        memoryGameLevelMenuPanel.SetActive(false);
        hopscotchGameDesk.SetActive(false);
        memoryGameDesk.SetActive(false);
        finishHopscotchGameButtons.SetActive(false);
        finishMemoryGameButtons.SetActive(false);
        stepTimer.SetActive(false);

        Invoke("playChoseGameAudio", 1f);
    }

    public void openHopscotchGameLevelMenu() //Hopscotch Game
    {
        textInfo.text = "";
        gameName.text= "Hopscotch game";
        mainMenuPanel.SetActive(false);
        hopscotchGameLevelMenuPanel.SetActive(true);
        memoryGameLevelMenuPanel.SetActive(false);
        hopscotchGameDesk.SetActive(true);
        memoryGameDesk.SetActive(false);
        hopscotchGameMenu.SetActive(false);
        finishHopscotchGameButtons.SetActive(false);
        finishMemoryGameButtons.SetActive(false);
        stepTimer.SetActive(false);
    }
    public void openHopscotchGameMenu()
    {
        textInfo.text = "";
        mainMenuPanel.SetActive(false);
        hopscotchGameLevelMenuPanel.SetActive(false);
        memoryGameLevelMenuPanel.SetActive(false);
        memoryGameDesk.SetActive(false);
        hopscotchGameMenu.SetActive(true);
        //hopscotchGameDesk.SetActive(true);
        finishHopscotchGameButtons.SetActive(false);
        finishMemoryGameButtons.SetActive(false);
        stepTimer.SetActive(false);
    }
    public void openMemoryGameLevelMenu() //Memory Game
    {
        textInfo.text = "";
        gameName.text = "Copycat Challenge";
        mainMenuPanel.SetActive(false);
        hopscotchGameLevelMenuPanel.SetActive(false);
        memoryGameLevelMenuPanel.SetActive(true);
        hopscotchGameDesk.SetActive(false);
        memoryGameDesk.SetActive(true);
        hopscotchGameMenu.SetActive(false);
        finishHopscotchGameButtons.SetActive(false);
        finishMemoryGameButtons.SetActive(false);
        stepTimer.SetActive(false);

    }

    public void hideAllButtons() {
        mainMenuPanel.SetActive(false);
        hopscotchGameLevelMenuPanel.SetActive(false);
        memoryGameLevelMenuPanel.SetActive(false);
        hopscotchGameMenu.SetActive(false);
        finishHopscotchGameButtons.SetActive(false);
        finishMemoryGameButtons.SetActive(false);
        stepTimer.SetActive(false);
    }

    public void openFinishHopscotchGameButtons()
    {
        mainMenuPanel.SetActive(false);
        hopscotchGameLevelMenuPanel.SetActive(false);
        memoryGameLevelMenuPanel.SetActive(false);
        memoryGameDesk.SetActive(false);
        hopscotchGameMenu.SetActive(false);
        finishHopscotchGameButtons.SetActive(true);
        finishMemoryGameButtons.SetActive(false);
        stepTimer.SetActive(false);
    }
    public void openFinishMemoryGameButtons()
    {
        mainMenuPanel.SetActive(false);
        hopscotchGameLevelMenuPanel.SetActive(false);
        memoryGameLevelMenuPanel.SetActive(false);
        hopscotchGameDesk.SetActive(false);
        hopscotchGameMenu.SetActive(false);
        finishHopscotchGameButtons.SetActive(false);
        finishMemoryGameButtons.SetActive(true);
        stepTimer.SetActive(false);
    }

    void playChoseGameAudio() {
        choseGameAudio.Play();
    }


}
