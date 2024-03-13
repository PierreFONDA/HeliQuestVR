using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBehaviour : MonoBehaviour {

    [SerializeField] private float jumpHeight;
    [SerializeField] private float speed;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /*if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            transform.position += new Vector3(0f, jumpHeight, 0f);
        }*/

        Vector2 touchPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        transform.position += Vector3.forward * speed * -touchPosition.x;
        transform.position += Vector3.right * speed * touchPosition.y;
    }
}
