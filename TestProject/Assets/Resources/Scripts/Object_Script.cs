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
        transform.eulerAngles = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);
    }
}
