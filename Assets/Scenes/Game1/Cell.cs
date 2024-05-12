using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Color defaultColor = new Color(Color.white.r, Color.white.g, Color.white.b, 0.7f);
    public Color activeColor = new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.7f);
    public Color errorColor = new Color(Color.red.r, Color.red.g, Color.red.b, 0.7f);
    public Color nextStepColor = new Color(Color.green.r, Color.green.g, Color.green.b, 0.7f);
    public bool isActive = false;
    public TextMeshPro textInfo;
    GameHopscotch gameHopscotchManager;
    void Start()
    {
        setColor(defaultColor);
        gameHopscotchManager = FindObjectOfType<GameHopscotch>();
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
        if (gameHopscotchManager != null) { 
        gameHopscotchManager.checkCell();
        }
        //if (gameHopscotchManager.nextStepCell.textInfo.text.Equals(this.textInfo.text)){
            setActiveColor();
        //}
        //else
        //{
        //    setErrorColor();
        //}
        
    }
    public void deActivateCell()
    {
        isActive = false;
        setDefaultColor();

    }
}

