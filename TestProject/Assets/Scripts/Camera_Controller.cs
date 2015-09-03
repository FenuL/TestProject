using UnityEngine;
using System.Collections;

public class Camera_Controller : MonoBehaviour {

	SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit2D hit;
		/*if (Physics.Raycast (ray, out hit)) {
			// if the ray hit a collider, we'll get the world-coordinate here.
			Vector3 worldPos = hit.point;
			print("x="+ worldPos.x + ", y=" + worldPos.y);
		}*/
		hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		//hit: RaycastHit2D = Physics2D.Raycast(transform.position, -Vector2.up);
		//hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity, 0);
		if (hit){
		    if(renderer){
				renderer.color = new Color(255f, 255f, 255f, 1f);
			}
			renderer = (SpriteRenderer)hit.transform.GetComponent<SpriteRenderer> ();
			renderer.color = new Color(255f, 0f, 0f, 1f); // Set to opaque black
			Vector3 worldPos = hit.point;
			print("x="+ worldPos.x + ", y=" + worldPos.y);
		}
	}
}
