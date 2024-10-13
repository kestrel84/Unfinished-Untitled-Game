using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowController : MonoBehaviour
{
    public Camera cam;
    public Transform player;
    public Transform target;
    public ButtonTrigger ButtonTrigger;
   

    // Update is called once per frame
    void Update()
    {
        if (!ButtonTrigger.isOn){
            Vector3 diff = target.position - player.position;
            transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) - 90);
            transform.position = player.position + (5 * diff.normalized);
        } else {
            gameObject.SetActive(false);
        }
    }
}
