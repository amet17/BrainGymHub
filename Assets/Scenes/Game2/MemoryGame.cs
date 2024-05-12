using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemoryGame : MonoBehaviour
{
    public Text textInfo;
    public Menu menu;
    public GameObject prefab;
    private List<CellMemoryGame> arrObjects;
    public List<int> arrCellNumberSequence;
    public GameObject stepTimer, stepTimerLine;
    public CellMemoryGame nextStepCell;
    GameStartTimer gameTimer;
    public float distance = 0.01f;
    public float gameDeskScale = 0.45f;
    public AudioSource trueCellAudio, errorCellAudio, finishAudio, nextCellAudio, youFailAudio, youWinAudio, help1Audio, help2Audio;
    int rowCount = 3;
    int columnCount = 4;
    int stepCount = 4;
    public int currentStep = 0;
    int lastActiveCellNumber = 0;
    float stepTime = 3f;
    float activationCellDuration=1f;
    float startGameDuration = 4;
    public bool isGameOver = false;
    System.Random rnd;

    // Start is called before the first frame update

    void Start()
    {
        rnd = new System.Random();
        gameTimer = FindObjectOfType<GameStartTimer>();
        arrObjects = new List<CellMemoryGame>();
        arrCellNumberSequence = new List<int>();
        menu = GameObject.FindObjectOfType<Menu>();


        //RestartGame();
    }

    public void playNextCellAudio() {
        nextCellAudio.Play();
    }
    public void playNextCellAudio(float duration) {
        Invoke("playNextCellAudio", duration);
    }

    public void GenerateGameDesk()
    {       
        textInfo.text = "";
        transform.localScale = new Vector3(1, 1, 1);
        int currentNumber = 0;
        transform.position = new Vector3(0, 0, 0);
        for (int j = 0; j < rowCount; j++)
        {
            for (int i = 0; i < columnCount; i++)
            {
                currentNumber++;
                GameObject newObject = Instantiate(prefab); 
                newObject.transform.parent = transform;
                newObject.transform.position = new Vector3(i * gameObject.transform.lossyScale.x + i * distance, 0, j * gameObject.transform.lossyScale.z + j * distance);
                arrObjects.Add(newObject.GetComponent<CellMemoryGame>());
                newObject.GetComponent<CellMemoryGame>().setText(currentNumber.ToString());
                newObject.GetComponent<CellMemoryGame>().idInArray = currentNumber;
            }
        }
        transform.localScale = new Vector3(gameDeskScale, gameDeskScale, gameDeskScale);
        UpdateBoundsAndPosition();

        for (int i = 0; i < stepCount; i++) {
            //int nextStep = generateNextCellNumber();
            arrCellNumberSequence.Add(generateNextCellNumber());
        }
    }

    public void RestartGame()
    {
        DestroyGame();
        GenerateGameDesk();
        startGame();
    }


    // Update is called once per frame
    void Update()
    {

    }

    int generateNextCellNumber()
    {
        int currentCellNum = rnd.Next(0, (rowCount * columnCount) - 1);
        if (currentCellNum != lastActiveCellNumber)
        {
            lastActiveCellNumber = currentCellNum;
            return lastActiveCellNumber;
        }
        else
        {
            return generateNextCellNumber();
        }
    }

    void startGame()
    {
        menu.hideAllButtons();
        showMessageRememberSequence();
        Invoke("startGameAndHelp", 4);
    }

    void startGameAndHelp()
    {
        textInfo.text = "";
        gameTimer.gameObject.SetActive(true);
        gameTimer.StartTimer(3);
        Invoke("showMessageRememberSequence", 4);



        for (int i = 0; i < arrCellNumberSequence.Count; i++)
        {
            arrObjects[arrCellNumberSequence[i]].activateCell(i * activationCellDuration + startGameDuration);
            playNextCellAudio(i * activationCellDuration + startGameDuration);
            arrObjects[arrCellNumberSequence[i]].deActivateCellWithoutCancelInvoke(i * activationCellDuration + startGameDuration + activationCellDuration);
        }
        Invoke("showMessageRepeatSequence", arrCellNumberSequence.Count * activationCellDuration + startGameDuration + activationCellDuration);

    }

    void showMessageRememberSequence() {
        textInfo.text = "Remember the sequence!";
        help1Audio.Play();
    }
    void showMessageRepeatSequence()
    {
        textInfo.text = "Repeat the sequence!";
        help2Audio.Play();
    }

    void nextStep()
    {

    }
    public void checkCell(int currentCell)
    {
        if (arrCellNumberSequence[currentStep] == currentCell-1)
        {
            trueCellAudio.Play();
            currentStep++;
        }
        else
        {
            errorCellAudio.Play();
        }

        if (currentStep >= stepCount)
        {
            finishAudio.Play();
        }

    }
    
   public void YouWin() {
        isGameOver = true;
        textInfo.text = "You Win!!!";
        finishAudio.Play();
        menu.openFinishMemoryGameButtons();
        Invoke("youWinAudioPlay", 1f);
    }

    public void YouLoose()
    {
        StopAllAudio();
        CancelInvoke();
        StopAllCoroutines();
        isGameOver = true;
        errorCellAudio.Play();
    textInfo.text = "You loose!!!";
            menu.openFinishMemoryGameButtons();

        Invoke("youFailAudioPlay", 1f);        
    }

    void youFailAudioPlay() {
        youFailAudio.Play();
    }
    void youWinAudioPlay()
    {
        youWinAudio.Play();
    }

    void StopAllAudio() {
        trueCellAudio.Stop();
        errorCellAudio.Stop();
        finishAudio.Stop();
        nextCellAudio.Stop();
        youFailAudio.Stop();
        youWinAudio.Stop();
        help1Audio.Stop();
        help2Audio.Stop();
    }

    void errorAnimation()
    {
        for (int i = 0; i < arrObjects.Count; i++)
        {

        }
    }
    void UpdateBoundsAndPosition()
    {
        float sizeX = columnCount * gameDeskScale + ((columnCount - 1) * distance * gameDeskScale);
        float sizeZ = rowCount * gameDeskScale + rowCount * distance * gameDeskScale;
        transform.position = new Vector3(0, 0, 0);
        transform.position = new Vector3(-(sizeX / 2 - gameDeskScale/2), transform.position.y, gameDeskScale);
    }

    private Bounds CalculateObjectBounds(Transform objectTransform)
    {
        var bounds = new Bounds(objectTransform.position, Vector3.zero);
        foreach (var renderer in objectTransform.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    IEnumerator ScaleOverTime(GameObject objectToScale, Vector3 toScale, float duration)
    {
        float currentTime = 0f;
        Vector3 initialScale = objectToScale.transform.localScale;

        while (currentTime <= duration)
        {
            float t = currentTime / duration;
            objectToScale.transform.localScale = Vector3.Lerp(initialScale, toScale, t);
            currentTime += Time.deltaTime;
            yield return null;
        }

        objectToScale.transform.localScale = toScale;
    }


    public void GameMode1()
    {
        gameObject.SetActive(true);
        DestroyGame();
        rowCount = 2;
        columnCount = 2;
        stepTime = 3f;
        stepCount = 4;
        //Invoke("GenerateGameDesk", 1);
        GenerateGameDesk();
        startGame();
    }
    public void GameMode2()
    {
        gameObject.SetActive(true);
        DestroyGame();
        rowCount = 3;
        columnCount = 3;
        stepTime = 3f;
        stepCount = 5;
        //Invoke("GenerateGameDesk", 1);


        GenerateGameDesk();
        startGame();
    }
    public void GameMode3()
    {
        gameObject.SetActive(true);
        DestroyGame();
        rowCount = 4;
        columnCount = 3;
        stepTime = 3f;
        stepCount = 6;
        //Invoke("GenerateGameDesk", 1);
        GenerateGameDesk();
        startGame();
    }
    public void GameMode4()
    {
        gameObject.SetActive(true);
        DestroyGame();
        rowCount = 4;
        columnCount = 4;
        stepTime = 3f;
        //Invoke("GenerateGameDesk", 1);
        stepCount = 6;
        GenerateGameDesk();
        startGame();
    }
    public void GameMode5()
    {

        DestroyGame();
        rowCount = 5;
        columnCount = 4;
        stepTime = 3f;
        stepCount = 8;
        //Invoke("GenerateGameDesk", 1);
        GenerateGameDesk();
        startGame();
    }
    public void DestroyGame()
    {
        currentStep = 0;
        lastActiveCellNumber = 0;
        isGameOver = false;
        CancelInvoke();
        StopAllCoroutines();
        arrCellNumberSequence.Clear();
        for (int i = 0; i < arrObjects.Count; i++)
        {
            Destroy(arrObjects[i].gameObject);
        }
        arrObjects.Clear();

    }

}
