using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialController : MonoBehaviour
{
    
    private bool allButtonsHit;
    private bool winFlagActivated;
    private bool startFlagActivated;
    private ButtonTrigger[] _triggers;
    private WinFlagController _winFlagController;
    private StartFlagController _startFlagController;

    public TimerController _timer;
    public LeaderboardController _lbc;
    public class Leaderboard{
        public float[] nums = {100000.0f, 100000.0f, 100000.0f};
    }
    public Leaderboard _leaderboard;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        string result = "{}";
        FileManager.LoadFromFile("leaderboard", out result);
        print(result);
        _leaderboard = JsonUtility.FromJson<Leaderboard>(result);

        _triggers = GetComponentsInChildren<ButtonTrigger>();
        _winFlagController = GetComponentInChildren<WinFlagController>();
        _startFlagController = GetComponentInChildren<StartFlagController>();

        winFlagActivated = false;
        _startFlagController.activateFlag();
        startFlagActivated = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!allButtonsHit){
            allButtonsHit = true;
            foreach (ButtonTrigger t in _triggers) {
                if (t.isOn == false) allButtonsHit = false;
            }
        }
        if (allButtonsHit && !winFlagActivated){
            winFlagActivated = true;
            _winFlagController.activateFlag();
        }
    }

    public void onStart(){
        print("started");
        winFlagActivated = false;
        allButtonsHit = false;
        _timer.resetTimer();
        _timer.startTimer();
    }

    public void onWin(){
        if (_timer.timerRunning()) _timer.stopTimer();
        updateLeaderboard(_timer.getCurrentTime());
    }

    private void updateLeaderboard(float newTime){
        //0 index is the shortest time (the fastest)
        //2 index is the longest time (the slowest)



        for (int i = 0; i < _leaderboard.nums.Length; i++){
            if (newTime < _leaderboard.nums[i]) {
                float temp = newTime;
                for (int j = 0; j < _leaderboard.nums.Length; j++){
                    float temp2 = _leaderboard.nums[j];
                    _leaderboard.nums[j] = temp;
                    temp = temp2;
                }
                break;
            }
        }


        foreach (float num in _leaderboard.nums){
            print(num.ToString());
        }
        FileManager.WriteToFile("leaderboard", JsonUtility.ToJson(_leaderboard));
        _lbc.showLeaderboard(_leaderboard.nums);

    }
}
