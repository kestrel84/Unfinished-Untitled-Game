using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinArrowController : MonoBehaviour
{
    public Camera cam;
    public Transform player;
    public Transform target;
    public WinFlagController winFlagController;
    private Image _image;

    void Awake(){
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!winFlagController.getActive()) {
            _image.enabled = false;
        } else {
            _image.enabled = true;
        }



        Vector3 diff = target.position - player.position;
        transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) - 90);
        transform.position = player.position + (5 * diff.normalized);   
    }
}
