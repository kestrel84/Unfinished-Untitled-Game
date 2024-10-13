using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardController : MonoBehaviour
{
    public TMP_Text _text;
    

    // Start is called before the first frame update
    void Start()
    {
        hideLeaderboard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showLeaderboard(float[] nums){
        string temp = "";
        foreach (float num in nums){
            temp += num.ToString();
            temp += "\n";
        }

        _text.text = temp;
    }

    public void hideLeaderboard(){
        _text.text = "";
    }
}
