using UnityEngine;
using System.Collections;

public class Camera_Controller : MonoBehaviour {

	public int resolutionx;
	public int resolutiony;

	// Use this for initialization
	void Start () {
		Screen.SetResolution(resolutionx, resolutiony, true);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
