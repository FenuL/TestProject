using UnityEngine;
using System.Collections;

public class Object_Editor_UI : MonoBehaviour {
    
    public void Enable()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
    }

    //Disables the Editor
    public void Disable()
    {
        gameObject.SetActive(false);
    }


	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
