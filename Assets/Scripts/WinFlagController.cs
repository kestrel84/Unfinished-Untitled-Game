using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinFlagController : MonoBehaviour
{
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    public TimeTrialController _timetrial;
    
    private SpriteRenderer _renderer;
    private bool _active;


    //Leaderboard stuff
    

    // Start is called before the first frame update
    void Awake()
    {
        //_leaderboard = new Leaderboard();
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = inactiveSprite;
        _active = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activateFlag (){
        _renderer.sprite = activeSprite;
        _active = true;
    }

    private void OnTriggerEnter2D(Collider2D other){
        if (_active){
            
            _active = false;
            _renderer.sprite = inactiveSprite;
            _timetrial.onWin();
        }
    }

    public bool getActive() {return _active;} 
}
