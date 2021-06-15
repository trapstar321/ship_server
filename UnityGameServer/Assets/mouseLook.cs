using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;      

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void Rotate(float mouseX) {        
        playerBody.Rotate(Vector3.up * mouseX);        
    }    
}
