using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{   
    public TMP_Text _timer;

    private float _currentTime;
    private bool _timerRunning;

    // Start is called before the first frame update
    void Start()
    {
        _currentTime = 0;
        _timerRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timerRunning){
            _currentTime += Time.deltaTime;
            _timer.text = _currentTime.ToString();
        }
    }

    public void startTimer(){
        _timerRunning = true;
    }

    public void stopTimer(){
        _timerRunning = false;
    }

    public void resetTimer(){
        _currentTime = 0;
    }

    public bool timerRunning(){return _timerRunning;}
    public float getCurrentTime(){return _currentTime;}
}
