using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    [SerializeField]
    private float moveSpeed, rotateSpeed;
    private Vector3 direction, rotation;
    
	// Update is called once per frame
	void Update () {

        direction = Vector3.zero;
        rotation = new Vector3(0/*-1 * Input.GetAxis("Mouse Y")*/, Input.GetAxis("Mouse X"), 0);

        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            direction += Vector3.up;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            direction += Vector3.down;
        }

        direction.Normalize();
        rotation.Normalize();

        transform.Translate(direction * moveSpeed);
        transform.Rotate(rotation * rotateSpeed);
    }
}
