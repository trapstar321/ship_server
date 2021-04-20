using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    private float xRot = 0;

    public List<float> buffer = new List<float>();    

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = buffer.Count - 1; i >= 0; i--)
        {
            /*float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;*/

            /*xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, -45f, 45f);*/

            //transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
            playerBody.Rotate(Vector3.up * buffer[i]);

            buffer.RemoveAt(i);            
        }
    }
}
