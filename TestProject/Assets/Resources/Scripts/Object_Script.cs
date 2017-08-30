using UnityEngine;
using System.Collections;

public class Object_Script : MonoBehaviour {

    public Game_Controller controller { get; set; }

    // Use this for initialization
    void Start () {
        controller = Game_Controller.controller;
    }
	
	// Update is called once per frame
	void Update () {
        //Change sprite facing to match current camera angle
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y = rot.y + controller.main_camera.GetComponent<Camera_Controller>().rotationAmount * Time.deltaTime * 2;
        if (rot.y > 360)
            rot.y -= 360;
        else if (rot.y < 360)
            rot.y += 360;
        transform.eulerAngles = rot;
    }
}
