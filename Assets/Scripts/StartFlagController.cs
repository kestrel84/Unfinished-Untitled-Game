using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFlagController : MonoBehaviour
{

    public Sprite inactiveSprite;
    public Sprite activeSprite;
    private SpriteRenderer _renderer;
    private bool _active;

    public TimeTrialController _timetrial;
    // Start is called before the first frame update
    void Start(){
        print("getting renderers");
        _renderer = GetComponent<SpriteRenderer>();
    }
    
    
    void Awake()
    {
        _renderer.sprite = inactiveSprite;
        _active = false;
    }

    public void activateFlag(){
        print("start activated");
        _renderer.sprite = activeSprite;
        _active = true;
    }

    private void OnTriggerEnter2D(Collider2D other){
        if (_active){
            print("start flag triggered");
            _active = false;    
            _renderer.sprite = inactiveSprite;
            _timetrial.onStart();
        }
    }
    // Update is called once per frame
    
}
