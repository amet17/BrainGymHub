using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHopscotch : MonoBehaviour
{
    public Text textInfo;
    public Menu menu;
    public GameObject prefab;
    private List<Cell> arrObjects;
    public GameObject stepTimer, stepTimerLine;
    public Cell nextStepCell;
    GameStartTimer gameTimer;
    public float distance = 0.01f;
    public float gameDeskScale = 0.45f;
    public AudioSource trueCellAudio, errorCellAudio, finishAudio, nextCellAudio, youFailAudio, youWinAudio, gameHelpAudio;
    int rowCount = 3;
    int columnCount = 4;
    int stepCount = 10;
    int currentStep = 0;
    int lastActiveCellNumber = 0;
    float stepTime = 3f;
    bool isGameOver = false;
    System.Random rnd;


    // Start is called before the first frame update

    void Start()
    {
        rnd = new System.Random();
        gameTimer = FindObjectOfType<GameStartTimer>();
        arrObjects = new List<Cell>();
        menu = GameObject.FindObjectOfType<Menu>();
    }

    public void GenerateGameDesk()
    {
        textInfo.text = "";
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = new Vector3(0, 0, 0);
        int currentNumber = 0;
        for (int j = 0; j < rowCount; j++)
        {
            for (int i = 0; i < columnCount; i++)
            {
                currentNumber++;
                GameObject newObject = Instantiate(prefab);
                newObject.transform.parent = transform;
                newObject.transform.position = new Vector3(i * gameObject.transform.lossyScale.x + i * distance, 0, j * gameObject.transform.lossyScale.z + j * distance);
                arrObjects.Add(newObject.GetComponent<Cell>());
                newObject.GetComponent<Cell>().setText(currentNumber.ToString());
            }
        }
        transform.localScale = new Vector3(gameDeskScale, gameDeskScale, gameDeskScale);
        UpdateBoundsAndPosition();     
    }

    public void RestartGame() {
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
        //menu.openHopscotchGameMenu();
        menu.hideAllButtons();
        gameTimer.gameObject.SetActive(true);
        gameHelpAudio.Play();

        Invoke("startTimerAndPlayGame", 4);


        //nextStep();

    }
    void startTimerAndPlayGame() {
        gameTimer.StartTimer(3);
        Invoke("nextStep", 4f);
        Invoke("showMessageInStartGame", 4);
    }
    void showMessageInStartGame() {
        textInfo.text = "Step on the squares when they light up green.";
    }

    void nextStep() {

        nextCellAudio.Play();
        arrObjects[lastActiveCellNumber].deActivateCell();
        StopAllCoroutines();
        stepTimerLine.transform.localScale = new Vector3(1, 1, 1);
        stepTimer.SetActive(true);
        StartCoroutine(ScaleOverTime(stepTimerLine, new Vector3(0, 1, 1), stepTime));

        if (currentStep <= stepCount)
        {
            nextStepCell= arrObjects[generateNextCellNumber()];
            nextStepCell.setNextStepColor();

        }
        currentStep++;
        Invoke("checkCellWhenTimeExit", stepTime);
    }

    public void checkCell() {
        if (!isGameOver && arrObjects[lastActiveCellNumber].isActive)
        {
            checkCellWhenTimeExit();
        }

    }

    public void checkCellWhenTimeExit() {
        CancelInvoke();
        if (arrObjects[lastActiveCellNumber].isActive)
        {
            trueCellAudio.Play();
            if (currentStep >= stepCount)
            {
                isGameOver = true;
                textInfo.text = "You Win!!!";
                print("FINISH");
                finishAudio.Play();
                menu.openFinishHopscotchGameButtons();
                Invoke("youWinAudioPlay", 2f);
            }
            else
            {
                nextStep();
            }

        }
        else
        {
            isGameOver = true;
            errorCellAudio.Play();
            print("You loose");
            textInfo.text = "You loose!!!";
            menu.openFinishHopscotchGameButtons();

            Invoke("youFailAudioPlay", 2f);
        }


    }


   


    void youFailAudioPlay()
    {
        youFailAudio.Play();
    }
    void youWinAudioPlay()
    {
        youWinAudio.Play();
    }

    void errorAnimation() {
        for (int i = 0; i < arrObjects.Count; i++) {
            
        }
    }
    void UpdateBoundsAndPosition()
    {
        float sizeX = columnCount * gameDeskScale + ((columnCount - 1) * distance * gameDeskScale);
        float sizeZ = rowCount * gameDeskScale + rowCount * distance * gameDeskScale;
        transform.position = new Vector3(0, 0, 0);
        transform.position = new Vector3(-(sizeX / 2 - gameDeskScale / 2), transform.position.y, gameDeskScale);
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
        stepCount = 10;
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
        stepTime = 4f;
        stepCount = 15;
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
        stepTime = 4f;
        stepCount = 15;
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
        stepTime = 5f;
        stepCount = 20;
        //Invoke("GenerateGameDesk", 1);
        GenerateGameDesk();
        startGame();
    }
    public void GameMode5()
    {
        gameObject.SetActive(true);
        DestroyGame();
        rowCount = 5;
        columnCount = 4;
        stepTime = 5f;
        stepCount = 25;
        //Invoke("GenerateGameDesk", 1);
        GenerateGameDesk();
        startGame();
    }
    public void DestroyGame() {
        currentStep = 0;
        lastActiveCellNumber = 0;
        isGameOver = false;
        CancelInvoke();
        StopAllCoroutines();
        for(int i=0; i < arrObjects.Count; i++){
            Destroy(arrObjects[i].gameObject);
        }
        arrObjects.Clear();

    }

}
