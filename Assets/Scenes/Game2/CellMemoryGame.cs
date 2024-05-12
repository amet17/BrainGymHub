using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellMemoryGame : MonoBehaviour
{
    public Color defaultColor = new Color(Color.white.r, Color.white.g, Color.white.b, 0.7f);
    public Color activeColor = new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.7f);
    public Color errorColor = new Color(Color.red.r, Color.red.g, Color.red.b, 0.7f);
    public Color currentCellColor = new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.5f);
    public Color nextStepColor = new Color(Color.green.r, Color.green.g, Color.green.b, 0.7f);
    public bool isActive = false;
    public TextMeshPro textInfo;
    public int idInArray = 0;
    MemoryGame gameMemory;
    void Start()
    {
        setColor(defaultColor);
        gameMemory = FindObjectOfType<MemoryGame>();
    }


    void Update()
    {

    }

    public void setColor(Color currentColor)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = currentColor;
    }
    public void setDefaultColor()
    {
        setColor(defaultColor);
    }
    public void setText(string text)
    {
        textInfo.text = text;
    }
    public void setNextStepColor()
    {
        setColor(nextStepColor);
    }
    public void setActiveColor()
    {
        setColor(activeColor);
    }
    public void setErrorColor()
    {
        setColor(errorColor);
    }

    public void activateCell()
    {
        isActive = true;
        //if (gameMemory != null)
        //{
        //    gameMemory.checkCell();
        //}
        //if (gameHopscotchManager.nextStepCell.textInfo.text.Equals(this.textInfo.text)){
        setActiveColor();
        //}
        //else
        //{
        //    setErrorColor();
        //}

    }
    public void activateCell(float duration)
    {
        Invoke("activateCell", duration);
    }

    public void activateCellAndCheckFinish(float duration)
    {
        //Invoke("activateCell", duration);
        Invoke("checkFinish", duration);

    }

    public void cancelInvokeaAtivateCellAndCheckFinish()
    {
        CancelInvoke("checkFinish");
        CancelInvoke("activateCell");
    }

    public void checkFinish() {
        if (gameMemory.isGameOver)
        {
            return;
        }
        if(gameMemory.arrCellNumberSequence[gameMemory.currentStep] == idInArray - 1){
            gameMemory.currentStep++;
            if (gameMemory.currentStep >= gameMemory.arrCellNumberSequence.Count)
            {
                gameMemory.YouWin();
            }
            else
            {
                gameMemory.trueCellAudio.Play();
            }
            activateCell();
        }
        else
        {
            //gameMemory.errorCellAudio.Play();
            setColor(errorColor);
            gameMemory.YouLoose();
        }
    }

    public void deActivateCell(float duration)
    {
        Invoke("deActivateCell", duration);
    }

    public void deActivateCell()
    {
        CancelInvoke();
        isActive = false;
        setDefaultColor();

    }

    public void deActivateCellWithoutCancelInvoke(float duration)
    {
        Invoke("deActivateCellWithoutCancelInvoke", duration);
    }

    public void deActivateCellWithoutCancelInvoke()
    {
        isActive = false;
        setDefaultColor();

    }
}

