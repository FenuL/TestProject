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
		              "AU: " + controller.curr_player.GetComponent<Character_Script> ().aura_curr + " / " + controller.curr_player.GetComponent<Character_Script> ().aura_max + "\n" +
                      "AP: " + controller.curr_player.GetComponent<Character_Script>().action_curr + " / " + controller.curr_player.GetComponent<Character_Script>().action_max + "\n" +
                      "Can: " + controller.curr_player.GetComponent<Character_Script> ().canister_curr + " / " + controller.curr_player.GetComponent<Character_Script> ().canister_max + "\n" +
		              "Str: " + controller.curr_player.GetComponent<Character_Script> ().strength + "   Crd: " + controller.curr_player.GetComponent<Character_Script> ().coordination + "    Spt: " + controller.curr_player.GetComponent<Character_Script>().spirit + "\n" + 
		              "Dex: " + controller.curr_player.GetComponent<Character_Script> ().dexterity + "   Vit: " + controller.curr_player.GetComponent<Character_Script>().vitality + "\n" + 
		              "Wep: " + controller.curr_player.GetComponent<Character_Script>().weapon.name + "   Armor: " + controller.curr_player.GetComponent<Character_Script>().armor.name);



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
		if (transform.position.x >= -4) {
			if (Input.mousePosition.x <= 20) {
				transform.Translate (-.2f, 0, 0);// (transform.position.x-1,transform.position.y, transform.position.z);
			}
		}
		if (transform.position.x <= 4) {
			if (Input.mousePosition.x >= Screen.width - 20) {
				transform.Translate (.2f, 0, 0);
			}
		}
		if (transform.position.y >= -5) {
			if (Input.mousePosition.y <= 10) {
				transform.Translate (0, -.2f, 0);// (transform.position.x-1,transform.position.y, transform.position.z);
			}
		}
		if (transform.position.y <= 1) {
			if (Input.mousePosition.y >= Screen.height - 10) {
				transform.Translate (0, .2f, 0);
			}
		}
	}
}
