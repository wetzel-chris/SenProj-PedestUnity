using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public float speed;

	void Update()
     {
         if(Input.GetKey(KeyCode.RightArrow))
             transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
         if(Input.GetKey(KeyCode.LeftArrow))
             transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
         if(Input.GetKey(KeyCode.DownArrow))
             transform.Translate(new Vector3(0,-speed * Time.deltaTime,0));
         if(Input.GetKey(KeyCode.UpArrow))
             transform.Translate(new Vector3(0,speed * Time.deltaTime,0));
         if(Input.GetKey(KeyCode.W))
             transform.Translate(new Vector3(0,0,speed * Time.deltaTime));
         if(Input.GetKey(KeyCode.S))
             transform.Translate(new Vector3(0,0,-speed * Time.deltaTime));

         if(Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.down * speed * Time.deltaTime);
         if(Input.GetKey(KeyCode.D))
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
     }
}
