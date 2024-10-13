using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swingGrapple : MonoBehaviour
{
    //componenets	
	private Rigidbody2D rb;
    private Collider2D collider;
    private DistanceJoint2D joint;
	public Camera cam;
    private LineRenderer lr;

	public float grappleDelay = 300;

	//internals
	private bool mouse0Pressed;
	private bool grappleToggle = true;
	private bool grappleTimerActive = false;

	private RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
		//Get Internal Components
		rb = GetComponent<Rigidbody2D>();
		collider = GetComponent<CapsuleCollider2D>();
		joint = GetComponent<DistanceJoint2D>();
		lr = GetComponent<LineRenderer>();

        joint.autoConfigureConnectedAnchor = false;
        joint.enabled = false;
		lr.enabled = false;
        /*
         //Convert Layer Name to Layer Number
int cubeLayerIndex = LayerMask.NameToLayer("cube");

//Calculate layermask to Raycast to. (Ignore "cube" layer)
int layerMask = (1 << cubeLayerIndex);
//Invert to ignore it
layerMask = ~layerMask;
         */
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouse0Pressed = true;
        }			
		lr.SetPosition(0, transform.position);
    }

    void FixedUpdate()
    {
        if (mouse0Pressed == true && grappleToggle == true)
        {	
			print("grapple cast");
			//raycast
            hit = Physics2D.Raycast(transform.position, (cam.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized);
			
			joint.connectedAnchor = hit.point;            
			joint.enabled = true;
			lr.SetPosition(1, hit.point);
			lr.enabled = true;
			//StartCoroutine(ExecuteAfterTime(grappleDelay));			
			mouse0Pressed = false;
			grappleToggle = false;

			
        } else if (mouse0Pressed == true && grappleToggle == false){
			StopCoroutine(ExecuteAfterTime(grappleDelay));
			joint.enabled = false;
			grappleToggle = true;
			mouse0Pressed = false;
			lr.enabled = false;
		}
			
    }

	 IEnumerator ExecuteAfterTime(float time)
 	{

		if (grappleTimerActive)
         yield break;
		
    	grappleTimerActive = true;
     	yield return new WaitForSeconds(time);
 		Debug.Log("timer expired");
     	// Code to execute after the delay
		print("grapple executed");
		joint.connectedAnchor = hit.point;            
		joint.enabled = true;
		lr.SetPosition(1, hit.point);
		lr.enabled = true;

		grappleTimerActive=false;
 	}
    
}
