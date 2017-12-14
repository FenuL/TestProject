using UnityEngine;
using System.Collections;

/// <summary>
/// Script for handling interaction with Non Character Objects on the Tile Grid. 
/// </summary>
public class Object_Script : MonoBehaviour {

    /// <summary>
    /// Game_Controller controller - The game controller object. 
    /// </summary>
    public Game_Controller controller { get; set; }

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start () {
        controller = Game_Controller.controller;
    }
	
	/// <summary>
    /// Called once per Frame to update the Object. 
    /// Makes the Object face the Camera.
    /// </summary>
	void Update () {
        //Change sprite facing to match current camera angle
        transform.eulerAngles = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);
    }
}
