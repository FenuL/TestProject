using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Game_Controller : MonoBehaviour {
    public static string STARTING_SCENARIO = "Assets/Resources/Maps/tile_map.txt";

	public static Game_Controller controller;
    public Scenario curr_scenario;
    public ArrayList avail_scenarios;
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
		curr_scenario = new Scenario(STARTING_SCENARIO);
        avail_scenarios = new ArrayList();
        avail_scenarios.Add(curr_scenario);
        curr_scenario.LoadScenario();
        action_menu.GetComponent<Action_Menu_Script>().resetActions();
        //MarkReachable ();
    }
	
	// Update is called once per frame
	void Update () {
        curr_scenario.Update();
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
