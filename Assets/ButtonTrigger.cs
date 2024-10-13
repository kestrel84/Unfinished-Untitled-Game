using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{

    public Sprite onSprite;
    public Sprite offSprite;
    private SpriteRenderer _renderer;

    
    public bool isOn;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        isOn = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other){
        if (!isOn){
            isOn = true;
            _renderer.sprite = onSprite;
        }
    }
}
