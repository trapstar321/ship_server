using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject L_Cannon_1;
    public GameObject L_Cannon_2;
    public GameObject R_Cannon_1;
    public GameObject R_Cannon_2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CannonRotate(string direction, string side)
    {
        if (direction == "CannonUp" && side == "Right")
        {
            R_Cannon_1.transform.Rotate(new Vector3(0, 0, 0.2f));
            R_Cannon_2.transform.Rotate(new Vector3(0, 0, 0.2f));
            ServerSend.CannonRotate(GetComponent<Player>().id, "CannonUp", "Right");
        }

        if (direction == "CannonUp" && side == "Left")
        {
            L_Cannon_1.transform.Rotate(new Vector3(0, 0, 0.2f));
            L_Cannon_2.transform.Rotate(new Vector3(0, 0, 0.2f));
            ServerSend.CannonRotate(GetComponent<Player>().id, "CannonUp", "Left");
        }

        if (direction == "CannonDown" && side == "Right")
        {
            R_Cannon_1.transform.Rotate(new Vector3(0, 0, -0.2f));
            R_Cannon_2.transform.Rotate(new Vector3(0, 0, -0.2f));
            ServerSend.CannonRotate(GetComponent<Player>().id, "CannonDown", "Right");
        }

        if (direction == "CannonDown" && side == "Left")
        {
            L_Cannon_1.transform.Rotate(new Vector3(0, 0, -0.2f));
            L_Cannon_2.transform.Rotate(new Vector3(0, 0, -0.2f));
            ServerSend.CannonRotate(GetComponent<Player>().id, "CannonDown", "Left");
        }
    }
}
         

