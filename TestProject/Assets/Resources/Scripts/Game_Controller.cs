using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Game Controller class. Handles overarching Game processes. 
/// </summary>
public class Game_Controller : MonoBehaviour {
    /// <summary>
    /// string STARTING_SCENARIO - the Scenario to load at the start of the game.
    /// Game_Controller controller - Controller. Makes sure there is only one copy of this.
    /// GameObject canvas - The Canvas to draw the UI on.
    /// Scenario curr_scenario - The current Scenario that is Loaded.
    /// ArrayList avail_Scenarios - The Array of currently available Scenarios.
    /// GameObject main_camera - The Main camera object.
    /// List<Actions> all_actions - The list of all possible Actions created by parsing the Action List File. 
    /// Transform action_menu - the Action Menu used to select different Character Actions. 
    /// </summary>
    public static string STARTING_SCENARIO = "Assets/Resources/Maps/tile_map.txt";
    //public static string STARTING_SCENARIO = "Assets/Resources/Maps/falls_map.txt";

    public static Game_Controller controller;
    public static FloatingText popup;
    public static GameObject canvas;
    public Scenario curr_scenario { get; private set; }
    public ArrayList avail_scenarios;
    public GameObject main_camera;
    public List<Action> all_actions { get; private set; }
    public Transform action_menu;

    /// <summary>
    /// Save the Current Game Status.
    /// TODO: UPDATE THIS TO CURRENT STANDARDS
    /// </summary>
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

    /// <summary>
    /// Load an existing game Save.
    /// TODO: UPDATE THIS TO CURRENT STANDARDS
    /// </summary>
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

    /// <summary>
    /// Ran before a Start Method. Ensures there is only ever one Game_Controller.
    /// </summary>
	void Awake (){
		if (controller == null) {
			DontDestroyOnLoad (gameObject);
			controller = this;
		} else if (controller != this) {
			Destroy(gameObject);
		}
	}

    /// <summary>
    /// Used for initialization.
    /// Finds the Canvas to draw the UI text on.
    /// Finds the Main Camera.
    /// Sets up the Action Menu
    /// Loads the first Scenario.
    /// </summary>
	void Start () {
        canvas = GameObject.Find("Canvas");
        popup = Resources.Load<FloatingText>("Prefabs/Object Prefabs/PopupTextParent");
        main_camera = GameObject.FindGameObjectWithTag("MainCamera");
        all_actions = Action.Load_Actions();
        curr_scenario = new Scenario(STARTING_SCENARIO);
        avail_scenarios = new ArrayList();
        avail_scenarios.Add(curr_scenario);
        curr_scenario.Load_Scenario();
        action_menu.GetComponent<Action_Menu_Script>().Initialize();
        action_menu.GetComponent<Action_Menu_Script>().resetActions();
        //MarkReachable ();
    }

    /// <summary>
    /// Creates Floating Text from the FloatingText prefab at a specified location.
    /// </summary>
    /// <param name="text">The text to display. </param>
    /// <param name="location">The location to display the text. </param>
    /// <param name="color">The color of the text. </param>
    public static void Create_Floating_Text(string text, Transform location, Color color)
    {
        FloatingText instance = Instantiate(popup);
        Vector2 screen_position = Camera.main.WorldToScreenPoint(new Vector3(location.position.x + UnityEngine.Random.Range(-.5f, .5f),
            location.position.y + UnityEngine.Random.Range(1.5f,1.7f), location.position.z));
        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screen_position;
        instance.Set_Text(text);
        instance.Set_Color(color);
    }

    /// <summary>
    /// Called once per frame to update the Game.
    /// Updates the Scenario.
    /// Checks for Input
    /// Checks Mouse Poistion
    /// </summary>
    void Update () {
        curr_scenario.Update();
        Read_Input();
        checkMousePos();
    }

    /// <summary>
    /// Reads Player Input.
    /// </summary>
    void Read_Input()
    {
        //map selection
        if (Input.GetKeyDown("space"))
        {
            if (curr_scenario.scenario_file == "Assets/Resources/Maps/falls_map.txt")
            {
                curr_scenario.Unload_Scenario();
                curr_scenario = new Scenario("Assets/Maps/tile_map.txt");
                curr_scenario.Load_Scenario();
            }
            else if (curr_scenario.scenario_file == "Assets/Resources/Maps/tile_map.txt")
            {
                curr_scenario.Unload_Scenario();
                curr_scenario = new Scenario("Assets/Maps/falls_map.txt");
                curr_scenario.Load_Scenario();
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
                curr_scenario.Next_Player();
            }

            //check for mouse clicks
            if (Input.GetMouseButtonDown(0))
            {
                //cursor.GetComponent<Animator>().SetBool("Clicked", true);
                curr_scenario.clicked_tile = curr_scenario.selected_tile;
                if ((character.state != Character_States.Idle ||
                    character.state != Character_States.Dead) &&
                    !action_menu.GetComponent<Action_Menu_Script>().isOpen)
                {
                    foreach (Transform tile in curr_scenario.reachable_tiles)
                    {
                        if (tile.Equals(curr_scenario.clicked_tile))
                        {
                            character.StartCoroutine(character.Act(character.curr_action, curr_scenario.clicked_tile));
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
            curr_scenario.Next_Player();
        }

        //Prev player button
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            curr_scenario.Prev_Player();
        }

        //Camera Turning
        //Debug.Log(curr_scenario.curr_player.GetComponent<Character_Script>().state);
        if (Input.GetKeyDown(KeyCode.Q) && curr_scenario.curr_player.GetComponent<Character_Script>().state != Character_States.Walking)
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
        if (Input.GetKeyDown(KeyCode.E) && curr_scenario.curr_player.GetComponent<Character_Script>().state != Character_States.Walking)
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

    /// <summary>
    /// Checks the Mouse position and gives a Tile if the mouse is hovering over it.
    /// </summary>
    public void checkMousePos()
    {
        //create a ray at the mouse position, point it towards the grid. See if it hits a collider.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //deprecated 2d raycast physics
        //hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (Physics.Raycast(ray, out hit, 100))
        {
            Tile tile_data = hit.transform.GetComponent<Tile>();
            if (tile_data != null)
            {
                //tile_data.printTileData();
                //If the tile is not traversible we know it is occupied, check for a character there
                //this is done to print stats of the current highlighted character
                if (!tile_data.traversible)
                {
                    foreach (GameObject character in curr_scenario.characters)
                    {
                        if (character != null && 
                            character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile>().index[0] == tile_data.index[0] && 
                            character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile>().index[1] == tile_data.index[1])
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

}

/// <summary>
/// Used for Saving and Loading the Game. 
/// </summary>
[Serializable]
class GameData
{
	public string curr_map;
	public int curr_character_num;
	public GameObject[] characters;
	public GameObject curr_player;
	public GameObject cursor;
	public GameObject tile_grid;
	public Tile tile_data;
	public Transform[,] tiles;
	public Transform clicked_tile;
	public Transform selected_tile;
	public bool initialized;
	public int curr_round;
}
