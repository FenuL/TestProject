using UnityEngine;
using System.Collections;

public class Camera_Controller : MonoBehaviour {

	public int resolution_x;
	public int resolution_y;

	// Use this for initialization
	void Start () {
		Screen.SetResolution(resolution_x, resolution_y, true);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
