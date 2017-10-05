using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Game_Controller : MonoBehaviour {
    public static string STARTING_SCENARIO = "Assets/Resources/Maps/tile_map.txt";
    //public static string STARTING_SCENARIO = "Assets/Resources/Maps/falls_map.txt";

    public static Game_Controller controller;
    public Scenario curr_scenario;
    public ArrayList avail_scenarios;
    public GameObject main_camera;
    public List<Character_Script.Action> all_actions { get; set; }
    public IList<int> keys;
    public Transform action_menu;
	public bool initialized = false;

	public void Save(){
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/game_data.dat");

		GameData data = new GameData();
		/*data.curr_map = scenariocurr_map;
		data.curr_character_num = curr_character_num;
		data.characters = characters;
		data.curr_player = curr_player;
		data.cursor = cursor;
		data.tile_grid = tile_grid;
		data.tile_data = tile_data;
		data.tiles = tiles;
		data.clicked_tile = clicked_tile;
		data.selected_tile = selected_tile;
		data.initialized = initialized;
		data.curr_round = curr_round;*/
		formatter.Serialize (file, data);
		file.Close ();

	}

	public void Load(){
		if (File.Exists (Application.persistentDataPath + "/game_data.dat")) {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/game_data.dat", FileMode.Open);
			GameData data = (GameData) formatter.Deserialize(file);
			/*curr_map = data.curr_map;
			curr_character_num = data.curr_character_num;
			characters = data.characters;
			curr_player = data.curr_player;
			cursor = data.cursor;
			tile_grid = data.tile_grid;
			tile_data = data.tile_data;
			tiles = data.tiles;
			clicked_tile = data.clicked_tile;
			selected_tile = data.selected_tile;
			initialized = data.initialized;
			curr_round = data.curr_round;*/
			formatter.Serialize (file, data);

			file.Close();
		}
	}

	void Awake (){
		if (controller == null) {
			DontDestroyOnLoad (gameObject);
			controller = this;
		} else if (controller != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
        main_camera = GameObject.FindGameObjectWithTag("MainCamera");
        all_actions = Character_Script.Action.Load_Actions();
        curr_scenario = new Scenario(STARTING_SCENARIO);
        avail_scenarios = new ArrayList();
        avail_scenarios.Add(curr_scenario);
        curr_scenario.LoadScenario();
        action_menu.GetComponent<Action_Menu_Script>().controller = this;
        action_menu.GetComponent<Action_Menu_Script>().isOpen = false;
        action_menu.GetComponent<Action_Menu_Script>().resetActions();
        //MarkReachable ();
    }
	
	// Update is called once per frame
	void Update () {
        curr_scenario.Update();
        Read_Input();
        checkMousePos();
    }

    void Read_Input()
    {
        //map selection
        if (Input.GetKeyDown("space"))
        {
            if (curr_scenario.scenario_file == "Assets/Resources/Maps/falls_map.txt")
            {
                curr_scenario.unloadScenario();
                curr_scenario = new Scenario("Assets/Maps/tile_map.txt");
                curr_scenario.LoadScenario();
            }
            else if (curr_scenario.scenario_file == "Assets/Resources/Maps/tile_map.txt")
            {
                curr_scenario.unloadScenario();
                curr_scenario = new Scenario("Assets/Maps/falls_map.txt");
                curr_scenario.LoadScenario();
            }
        }

        //Action menu hotkeys
        Character_Script character = curr_scenario.curr_player.GetComponent<Character_Script>();
        if (!character.ending_turn)
        {
            if (Input.GetKeyDown("1"))
            {
                if (character.actions.Count >= 1)
                {
                    character.actions[0].Select(character);
                }
            }
            else if (Input.GetKeyDown("2"))
            {
                if (character.actions.Count >= 2)
                {
                    character.actions[1].Select(character);
                }
            }
            else if (Input.GetKeyDown("3"))
            {
                if (character.actions.Count >= 3)
                {
                    character.actions[2].Select(character);
                }
            }
            else if (Input.GetKeyDown("4"))
            {
                if (character.actions.Count >= 4)
                {
                    character.actions[3].Select(character);
                }
            }
            else if (Input.GetKeyDown("5"))
            {
                if (character.actions.Count >= 5)
                {
                    character.actions[4].Select(character);
                }
            }
            else if (Input.GetKeyDown("6"))
            {
                if (character.actions.Count >= 6)
                {
                    character.actions[5].Select(character);
                }
            }
            else if (Input.GetKeyDown("7"))
            {
                if (character.actions.Count >= 7)
                {
                    character.actions[6].Select(character);
                }
            }
            else if (Input.GetKeyDown("8"))
            {
                if (character.actions.Count >= 8)
                {
                    character.actions[7].Select(character);
                }
            }
            else if (Input.GetKeyDown("9"))
            {
                if (character.actions.Count >= 9)
                {
                    character.actions[8].Select(character);
                }
            }
            else if (Input.GetKeyDown("0"))
            {
                if (character.actions.Count >= 10)
                {
                    character.actions[9].Select(character);
                }
            }
            if (Input.GetKeyDown("k"))
            {
                character.Die();
                curr_scenario.NextPlayer();
            }



            //check for mouse clicks
            if (Input.GetMouseButtonDown(0))
            {
                //cursor.GetComponent<Animator>().SetBool("Clicked", true);
                curr_scenario.clicked_tile = curr_scenario.selected_tile;
                if ((character.state != Character_Script.States.Idle ||
                    character.state != Character_Script.States.Dead) &&
                    !action_menu.GetComponent<Action_Menu_Script>().isOpen)
                {
                    foreach (Transform tile in curr_scenario.reachable_tiles)
                    {
                        if (tile.Equals(curr_scenario.clicked_tile))
                        {
                            character.StartCoroutine(character.Act(curr_scenario.clicked_tile));
                        }
                    }
                }

            }
        }

        //check for mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            //cursor.GetComponent<Animator>().SetBool("Clicked", false);
        }

        //Next player button
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            curr_scenario.NextPlayer();
        }

        //Prev player button
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            curr_scenario.PrevPlayer();
        }

        //Camera Turning
        //Debug.Log(curr_scenario.curr_player.GetComponent<Character_Script>().state);
        if (Input.GetKeyDown(KeyCode.Q) && curr_scenario.curr_player.GetComponent<Character_Script>().state != Character_Script.States.Walking)
        {
            //Debug.Log("x:" + main_camera.transform.rotation.x + ", y:" + main_camera.transform.rotation.y + "z:" + main_camera.transform.rotation.z + ", w:" + main_camera.transform.rotation.w);

            //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            //transform.RotateAround(transform.position, Vector3.up, -90f);
            //main_camera.transform.RotateAround(curr_scenario.selected_tile.transform.position, Vector3.up, 90* Time.deltaTime);
            //main_camera.GetComponent<Camera_Controller>().targetRotation *= Quaternion.AngleAxis(90, main_camera.transform.forward);
            if (! main_camera.GetComponent<Camera_Controller>().rotating)
            {
                main_camera.GetComponent<Camera_Controller>().rotating = true;
                main_camera.GetComponent<Camera_Controller>().rotationAmount -= 90;
                foreach (GameObject chara in curr_scenario.characters)
                {
                    chara.GetComponent<Character_Script>().rotate = true;
                    //chara.GetComponent<Character_Script>().camera_offset += 1;
                    //chara.GetComponent<Character_Script>().orientation += 1;
                    //chara.GetComponent<Character_Script>().Orient();
                }
            }

            //main_camera.GetComponent<Camera_Controller>().rotationAmount -= 90;
            //update orientation based on camera
            //Debug.Log(curr_scenario.curr_player.GetComponent<Character_Script>().orientation);
            
        }
        if (Input.GetKeyDown(KeyCode.E) && curr_scenario.curr_player.GetComponent<Character_Script>().state != Character_Script.States.Walking)
        {
            //Debug.Log("x:" + main_camera.transform.rotation.x + ", y:" + main_camera.transform.rotation.y + "z:" + main_camera.transform.rotation.z + ", w:" + main_camera.transform.rotation.w);
            //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            //transform.RotateAround(transform.position, Vector3.up, 90f);
            if (!main_camera.GetComponent<Camera_Controller>().rotating)
            {
                main_camera.GetComponent<Camera_Controller>().rotating = true;
                main_camera.GetComponent<Camera_Controller>().rotationAmount += 90;
                foreach (GameObject chara in curr_scenario.characters)
                {
                    chara.GetComponent<Character_Script>().rotate = true;
                    //chara.GetComponent<Character_Script>().camera_offset -= 1;
                    //chara.GetComponent<Character_Script>().orientation -= 1;
                    //chara.GetComponent<Character_Script>().Orient();
                }
            }

            //main_camera.GetComponent<Camera_Controller>().rotationAmount += 90;
            //Debug.Log(curr_scenario.curr_player.GetComponent<Character_Script>().orientation);
            
        }
        

    }

    public void checkMousePos()
    {
        //create a ray at the mouse position, point it towards the grid. See if it hits a collider.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //deprecated 2d raycast physics
        //hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (Physics.Raycast(ray, out hit, 100))
        {
            Tile_Data tile_data = hit.transform.GetComponent<Tile_Data>();
            if (tile_data != null)
            {
                //If the tile is not traversible we know it is occupied, check for a character there
                //this is done to print stats of the current highlighted character
                if (!tile_data.node.traversible)
                {
                    foreach (GameObject character in curr_scenario.characters)
                    {
                        if (character != null && character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().x_index == tile_data.x_index && character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().y_index == tile_data.y_index)
                        {
                            curr_scenario.highlighted_player = character;
                        }
                    }
                }
                else
                {
                    curr_scenario.highlighted_player = null;
                }

                //update cursor location
                curr_scenario.selected_tile = hit.transform;
                curr_scenario.Update_Cursor(curr_scenario.selected_tile);
            }
        }
    }

    void Initialize(){

    }





}



[Serializable]
class GameData
{
	public string curr_map;
	public int curr_character_num;
	public GameObject[] characters;
	public GameObject curr_player;
	public GameObject cursor;
	public GameObject tile_grid;
	public Tile_Data tile_data;
	public Transform[,] tiles;
	public Transform clicked_tile;
	public Transform selected_tile;
	public bool initialized;
	public int curr_round;
}
