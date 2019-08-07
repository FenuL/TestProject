using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    /// List<Character_Actions> all_actions - The list of all possible Character_Actions created by parsing the Character_Action List File. 
    /// Transform action_menu - the Action Menu used to select different Character Actions. 
    /// </summary>
    public static string STARTING_SCENARIO = "Assets/Resources/Maps/tile_map.txt";
    //public static string STARTING_SCENARIO = "Assets/Resources/Maps/falls_map.txt";

    public static Game_Controller controller;
    public static FloatingText popup;
    public static GameObject canvas;
    public static Scenario curr_scenario { get; private set; }
    public ArrayList avail_scenarios;
    public GameObject main_camera;
    public Dictionary<string, Character_Action> all_actions { get; private set; }
    public Dictionary<string, Weapon> all_weapons { get; private set; }
    public Dictionary<string, Armor> all_armors { get; private set; }
    public Transform action_menu;
    public Dictionary<Controlls, KeyCode[]> controlls { get; private set; }

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
    /// Sets up the Character_Action Menu
    /// Loads the first Scenario.
    /// </summary>
	void Start () {
        controlls = createControlls();
        canvas = GameObject.Find("Canvas");
        popup = Resources.Load<FloatingText>("Prefabs/Scenario Prefabs/Object Prefabs/PopupTextParent");
        main_camera = GameObject.FindGameObjectWithTag("MainCamera");
        all_actions = Character_Action.Load_Actions();
        all_weapons = Weapon.Load_Weapons();
        all_armors = Armor.Load_Armors();
        curr_scenario = GameObject.Find("Scenario").GetComponent<Scenario>();
        curr_scenario.Start(STARTING_SCENARIO);
        avail_scenarios = new ArrayList();
        avail_scenarios.Add(curr_scenario);
        curr_scenario.Load_Scenario();
        action_menu.GetComponent<Action_Menu_Script>().Initialize();
        //action_menu.GetComponent<Action_Menu_Script>().resetActions();
        //MarkReachable ();
    }

    /// <summary>
    /// Method to create default Controlls.
    /// </summary>
    /// <returns>A dictionary with the default controlls mapped to the default keys.</returns>
    public Dictionary<Controlls, KeyCode[]> createControlls()
    {
        Dictionary<Controlls, KeyCode[]> controlls = new Dictionary<Controlls, KeyCode[]>();
        foreach (Controlls ctrl in Controlls.GetValues(typeof(Controlls)))
        {
            KeyCode[] keys = new KeyCode[2];
            if (ctrl == Controlls.Ability_Hotkey_0)
            {
                keys[0] = KeyCode.Alpha1;
                keys[1] = KeyCode.Keypad1;
            }
            else if (ctrl == Controlls.Ability_Hotkey_1)
            {
                keys[0] = KeyCode.Alpha2;
                keys[1] = KeyCode.Keypad2;
            }
            else if (ctrl == Controlls.Ability_Hotkey_2)
            {
                keys[0] = KeyCode.Alpha3;
                keys[1] = KeyCode.Keypad3;
            }
            else if (ctrl == Controlls.Ability_Hotkey_3)
            {
                keys[0] = KeyCode.Alpha4;
                keys[1] = KeyCode.Keypad4;
            }
            else if (ctrl == Controlls.Ability_Hotkey_4)
            {
                keys[0] = KeyCode.Alpha5;
                keys[1] = KeyCode.Keypad5;
            }
            else if (ctrl == Controlls.Ability_Hotkey_5)
            {
                keys[0] = KeyCode.Alpha6;
                keys[1] = KeyCode.Keypad6;
            }
            else if (ctrl == Controlls.Ability_Hotkey_6)
            {
                keys[0] = KeyCode.Alpha7;
                keys[1] = KeyCode.Keypad7;
            }
            else if (ctrl == Controlls.Ability_Hotkey_7)
            {
                keys[0] = KeyCode.Alpha8;
                keys[1] = KeyCode.Keypad8;
            }
            else if (ctrl == Controlls.Ability_Hotkey_8)
            {
                keys[0] = KeyCode.Alpha9;
                keys[1] = KeyCode.Keypad9;
            }
            else if (ctrl == Controlls.Ability_Hotkey_9)
            {
                keys[0] = KeyCode.Alpha0;
                keys[1] = KeyCode.Keypad0;
            }
            else if (ctrl == Controlls.Camera_Scroll_Up)
            {
                keys[0] = KeyCode.W;
                keys[1] = KeyCode.UpArrow;
            }
            else if (ctrl == Controlls.Camera_Scroll_Down)
            {
                keys[0] = KeyCode.S;
                keys[1] = KeyCode.DownArrow;
            }
            else if (ctrl == Controlls.Camera_Scroll_Left)
            {
                keys[0] = KeyCode.A;
                keys[1] = KeyCode.LeftArrow;
            }
            else if (ctrl == Controlls.Camera_Scroll_Right)
            {
                keys[0] = KeyCode.D;
                keys[1] = KeyCode.RightArrow;
            }
            else if (ctrl == Controlls.Camera_Turn_Left)
            {
                keys[0] = KeyCode.Q;
                keys[1] = KeyCode.LeftBracket;
            }
            else if (ctrl == Controlls.Camera_Turn_Right)
            {
                keys[0] = KeyCode.E;
                keys[1] = KeyCode.RightBracket;
            }
            else if (ctrl == Controlls.Next_Player)
            {
                keys[0] = KeyCode.Equals;
                keys[1] = KeyCode.KeypadPlus;
            }
            else if (ctrl == Controlls.Previous_Player)
            {
                keys[0] = KeyCode.Minus;
                keys[1] = KeyCode.KeypadMinus;
            }
            else if (ctrl == Controlls.Pause)
            {
                keys[0] = KeyCode.P;
                keys[1] = KeyCode.P;
            }
            controlls.Add(ctrl, keys);
        }

        return controlls;
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
        Character_Script character = curr_scenario.curr_player.Peek().GetComponent<Character_Script>();
        if (!character.ending_turn)
        {
            if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_0][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_0][1]))
            {
                if (character.actions.Count >= 1)
                {
                    character.actions[0].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_1][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_1][1]))
            {
                if (character.actions.Count >= 2)
                {
                    character.actions[1].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_2][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_2][1]))
            {
                if (character.actions.Count >= 3)
                {
                    character.actions[2].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_3][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_3][1]))
            {
                if (character.actions.Count >= 4)
                {
                    character.actions[3].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_4][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_4][1]))
            {
                if (character.actions.Count >= 5)
                {
                    character.actions[4].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_5][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_5][1]))
            {
                if (character.actions.Count >= 6)
                {
                    character.actions[5].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_6][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_6][1]))
            {
                if (character.actions.Count >= 7)
                {
                    character.actions[6].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_7][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_7][1]))
            {
                if (character.actions.Count >= 8)
                {
                    character.actions[7].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_8][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_8][1]))
            {
                if (character.actions.Count >= 9)
                {
                    character.actions[8].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_9][0]) ||
                Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_9][1]))
            {
                if (character.actions.Count >= 10)
                {
                    character.actions[9].Select();
                }
            }
            else if (Input.GetKeyDown(controlls[Controlls.Pause][0]) ||
                Input.GetKeyDown(controlls[Controlls.Pause][1]))
            {
                if (character.curr_action != null)
                {
                    if (character.curr_action.Peek().paused)
                    {
                        character.curr_action.Peek().Resume();
                    }
                    else
                    {
                        character.curr_action.Peek().Pause();
                    }
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
                GraphicRaycaster caster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
                //Create the PointerEventData
                PointerEventData data = new PointerEventData(null);
                //Look at mouse position
                data.position = Input.mousePosition;
                //Create list to receive results
                List<RaycastResult> results = new List<RaycastResult>();
                //Raycast
                caster.Raycast(data, results);
                if (results.Count == 0)
                {
                    curr_scenario.clicked_tile = curr_scenario.selected_tile;
                }
                if ((character.state != Character_States.Idle ||
                    character.state != Character_States.Dead))
                {
                    foreach (Transform tile in curr_scenario.reachable_tiles)
                    {
                        if (tile.Equals(curr_scenario.clicked_tile))
                        {
                            //Debug.Log(character.name + " num of curr_action " + character.curr_action.Count);
                            //Add selected tile to list of targets. If the right number of targets is met, trigger the Action.
                            if (character.curr_action.Peek().action_type == Action_Types.Path && Input.GetKey(KeyCode.LeftShift))
                            {
                                //Get the current path to the clicked tile and save it under the current path for the Action.
                                if (character.curr_action.Peek().Add_Waypoint(curr_scenario.clicked_tile.gameObject))
                                {
                                    //character.curr_action.Peek().Find_Path(curr_scenario.clicked_tile.gameObject);
                                    Game_Controller.curr_scenario.Reset_Reachable();
                                    character.curr_action.Peek().Find_Reachable_Tiles(true);
                                    Game_Controller.curr_scenario.Mark_Reachable();
                                }
                                
                            }
                            else
                            {
                                character.curr_action.Peek().Add_Target(curr_scenario.clicked_tile.gameObject);
                                if (character.curr_action.Peek().Has_Valid_Targets() && character.state == Character_States.Idle)
                                {
                                    //Debug.Log("Acting");

                                    character.StartCoroutine(character.Act(character.curr_action.Peek(), curr_scenario.clicked_tile));
                                }
                            }
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
        if (Input.GetKeyDown(controlls[Controlls.Next_Player][0]) ||
            Input.GetKeyDown(controlls[Controlls.Next_Player][1]))
        {
            curr_scenario.Next_Player();
        }

        //Prev player button
        if (Input.GetKeyDown(controlls[Controlls.Previous_Player][0]) ||
            Input.GetKeyDown(controlls[Controlls.Previous_Player][1]))
        {
            curr_scenario.Prev_Player();
        }

        //Camera Turning
        //Debug.Log(curr_scenario.curr_player.GetComponent<Character_Script>().state);
        if ((Input.GetKeyDown(controlls[Controlls.Camera_Turn_Right][0]) || Input.GetKeyDown(controlls[Controlls.Camera_Turn_Right][1])) 
            && curr_scenario.curr_player.Peek().GetComponent<Character_Script>().state != Character_States.Walking)
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
        if ((Input.GetKeyDown(controlls[Controlls.Camera_Turn_Left][0]) || Input.GetKeyDown(controlls[Controlls.Camera_Turn_Left][1])) &&
            curr_scenario.curr_player.Peek().GetComponent<Character_Script>().state != Character_States.Walking)
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
