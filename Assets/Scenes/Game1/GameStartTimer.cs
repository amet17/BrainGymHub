using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartTimer : MonoBehaviour
{
    public Text textInfo;
    int currentTime;
    AudioSource timerAudio;
    // Start is called before the first frame update
    

    void Start()
    {
        timerAudio = GetComponent<AudioSource>();
        textInfo.text ="";
        //StartTimer(8);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartTimer(int timerTime) {
        gameObject.SetActive(true);
        textInfo.text = "";
        currentTime = timerTime;
        CancelInvoke("UpdateTime");
        Invoke("UpdateTime", 1f);
    }

    void UpdateTime(){
        textInfo.text = currentTime.ToString();
        timerAudio.Play();
        if (currentTime > 0)
        {
            Invoke("UpdateTime", 1f);
        }
        else
        {
            //gameObject.SetActive(false);
            textInfo.text = "";

        }
        currentTime--;

    }
}
