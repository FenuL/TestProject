using UnityEngine;
using System.Collections;

public class Camera_Controller : MonoBehaviour {

	float horizontal_speed = 2.0f;
	float vertical_speed = 2.0f;

	Game_Controller controller;// = Game_Controller.controller;

	void OnGUI(){
		if (GUI.Button (new Rect (10, 100, 100, 30), "Save")) {
			Game_Controller.controller.Save();
		}
		if (GUI.Button (new Rect (10, 140, 100, 30), "Load")) {
			Game_Controller.controller.Save();
		}
		if (GUI.Button (new Rect (10, Screen.height-160, 50, 30), "Prev")) {
			Game_Controller.controller.PrevPlayer();
		}
		if (GUI.Button (new Rect (70, Screen.height-160, 50, 30), "Next")) {
			Game_Controller.controller.NextPlayer();
		}

		//print (controller.curr_player.GetComponent<Character_Script> ().character_name);
		 GUI.TextArea (new Rect (10, Screen.height - 120, 200, 110), controller.curr_player.GetComponent<Character_Script> ().character_name + "\n" + 
		              "Aura: " + controller.curr_player.GetComponent<Character_Script> ().aura_curr + " / " + controller.curr_player.GetComponent<Character_Script> ().aura_max + "\n" + 
		              "Canisters:" + controller.curr_player.GetComponent<Character_Script> ().canister_curr + " / " + controller.curr_player.GetComponent<Character_Script> ().canister_max + "\n" + 
		              "Lvl: " + controller.curr_player.GetComponent<Character_Script> ().level + "\n" +
		              "Str: " + controller.curr_player.GetComponent<Character_Script> ().strength + "   Crd: " + controller.curr_player.GetComponent<Character_Script> ().coordination + "\n" + 
		              "Spt: " + controller.curr_player.GetComponent<Character_Script> ().spirit + "   Dex: " + controller.curr_player.GetComponent<Character_Script> ().dexterity + "\n" + 
		              "Vit: " + controller.curr_player.GetComponent<Character_Script> ().vitality);

		for (int x=0; x < controller.curr_player.GetComponent<Character_Script>().actions.Length; x++) {
			//GUI.Button button = 
			string text = controller.curr_player.GetComponent<Character_Script>().actions[x];
			if (GUI.Button (new Rect (Screen.width-110, Screen.height-120+40*x, 100, 30), text)) {
				if (text == "Move"){
					controller.curr_player.GetComponent<Character_Script>().Action("Move");
				}
				if (text == "Attack"){
					controller.curr_player.GetComponent<Character_Script>().Action("Attack");
				}
				if (text == "Wait"){
					controller.NextPlayer();
				}
			}
		}

	}

	// Use this for initialization
	void Start () {
		//Screen.SetResolution(resolution_x, resolution_y, true);
		controller = Game_Controller.controller;
	}

	// Update is called once per frame
	void Update () {
		//check mouse position and scroll camera if necessary
		//transform.Translate(horizontal_speed* Input.GetAxis("Mouse Y"),vertical_speed* Input.GetAxis("Mouse X"),0);
		if (transform.position.x >= -2) {
			if (Input.mousePosition.x <= 200) {
				transform.Translate (-.2f, 0, 0);// (transform.position.x-1,transform.position.y, transform.position.z);
			}
		}
		if (transform.position.x <= 2) {
			if (Input.mousePosition.x >= Screen.width - 200) {
				transform.Translate (.2f, 0, 0);
			}
		}
		if (transform.position.y >= -4) {
			if (Input.mousePosition.y <= 100) {
				transform.Translate (0, -.2f, 0);// (transform.position.x-1,transform.position.y, transform.position.z);
			}
		}
		if (transform.position.y <= 0) {
			if (Input.mousePosition.y >= Screen.height - 100) {
				transform.Translate (0, .2f, 0);
			}
		}
	}
}
