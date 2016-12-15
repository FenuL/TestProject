using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class Tile_Grid : ScriptableObject{
    //Constants
    public static string TILE_SPRITESHEET_FILE = "Sprites/Tile Sprites/tile_spritesheetv3_transparent";
    public static string OBJECT_SPRITESHEET_FILE = "Sprites/Object Sprites/object_spritesheet_transparent";
    public static string TILE_PREFAB_SRC = "Prefabs/Tile Prefabs/tile prefab";
    public static string OBJECT_PREFAB_SRC = "Prefabs/Object Prefabs/object_prefab";
    public static string REACHABLE_PREFAB_SRC = "Prefabs/Tile Prefabs/Reachable_Tile";

    //Tile size in pixels
    public static float TILE_LENGTH = 43;
	public static float TILE_WIDTH = 85;
	public static float TILE_HEIGHT = 20;
    //Grid size restriction in number of tiles
	public static int MAX_WIDTH = 40;
	public static int MAX_HEIGHT = 40;
    public static int MIN_WIDTH = 5;
    public static int MIN_HEIGHT = 5;
    //Starting coordinates for the grid;
    public static float START_X = 0;
    public static float START_Y = 3.5f;

    //Prefabs
	public GameObject tile_prefab;
    public GameObject object_prefab;
    public GameObject reachable_prefab;

    //Spritesheets
    Sprite[] tile_sprite_sheet;
	Sprite[] object_sprite_sheet;

    SpriteRenderer sprite;

    public int grid_width;
    public int grid_length;
    int[,] tile_sprite_ids;
    int[,] object_sprite_ids;
    int[,] character_sprite_ids;
    int[,] tile_heights;
    Transform[,] tiles;
    public Tile_Data.Graph navmesh { get; set; }

    public Tile_Grid(int width, int length, int[,] new_tile_sprite_ids, int[,] new_object_sprite_ids, int[,] new_character_sprite_ids)
    {
        //Load the prefabs
        tile_prefab = Resources.Load(TILE_PREFAB_SRC, typeof(GameObject)) as GameObject;
        object_prefab = Resources.Load(OBJECT_PREFAB_SRC, typeof(GameObject)) as GameObject;
        reachable_prefab = Resources.Load(REACHABLE_PREFAB_SRC, typeof(GameObject)) as GameObject;

        //Load the spritesheets
        tile_sprite_sheet = Resources.LoadAll<Sprite>(TILE_SPRITESHEET_FILE);
        object_sprite_sheet = Resources.LoadAll<Sprite>(OBJECT_SPRITESHEET_FILE);

        //Set the variables
        grid_width = width;
        grid_length = length; 
        tile_sprite_ids = new_tile_sprite_ids;
        object_sprite_ids = new_object_sprite_ids;
        character_sprite_ids = new_character_sprite_ids;
        tile_heights = new int[width, length];
        for(int x =0; x<width; x++)
        {
            for (int y =0; y<length; y++)
            {
                tile_heights[x, y] = tile_sprite_ids[x, y] /10 +1;
            }
        }

        //Generate the Tile transforms and navmesh
        tiles = new Transform[width, length];
        navmesh = new Tile_Data.Graph();
    }

    public void Instantiate()
    {
        int tile_number = 0;
        for (int x = 0; x < grid_width; x++)
        {
            for (int y = 0; y < grid_length; y++)
            {
                //Set the correct sprite for the tile
                sprite = tile_prefab.GetComponent<SpriteRenderer>();
                sprite.sprite = (Sprite)tile_sprite_sheet[tile_sprite_ids[x, y]];
                sprite.sortingOrder = tile_number;
                tile_number++;

                //Instantiate the tile object.
                if (SceneManager.GetActiveScene().name == "isometric grid")
                {
                    Transform instance = ((GameObject)Instantiate(tile_prefab, new Vector3((float)(START_X - (x) * (TILE_WIDTH / 200) + (y) * (TILE_WIDTH / 200)), (float)(START_Y - (x) * (TILE_LENGTH / 200) - (y) * (TILE_LENGTH / 200)), 0), Quaternion.identity)).transform;
                    instance.gameObject.GetComponent<PolygonCollider2D>().offset.Set(0, .20f * tile_heights[x, y] + 3);

                    //Destroy the animator on the tile if necessary
                    if (tile_sprite_ids[x, y] - 1 != 5)
                    {
                        Destroy(instance.gameObject.GetComponent<Animator>());
                    }

                    //Generate the tile data for the tile
                    instance.GetComponent<Tile_Data>().instantiate(x, y, tile_heights[x, y], tile_sprite_ids[x, y]);

                    //Store the instantiated tile in our Tile Tranform Grid;
                    tiles[x, y] = instance;

                    //Add a node to the navmesh
                    navmesh.addNode(tiles[x, y].GetComponent<Tile_Data>().node);

                    //Connect the new node to previous nodes in the mesh
                    if (x > 0)
                    {
                        tiles[x, y].GetComponent<Tile_Data>().node.addEdge(tiles[x - 1, y].GetComponent<Tile_Data>().node, 3);
                    }
                    if (y > 0)
                    {
                        tiles[x, y].GetComponent<Tile_Data>().node.addEdge(tiles[x, y - 1].GetComponent<Tile_Data>().node, 0);
                    }

                    //If there is an object on the tile, mark it non traversible
                    if (object_sprite_ids[x, y] == 0)
                    {
                        tiles[x, y].GetComponent<Tile_Data>().node.traversible = true;
                    }
                    else
                    {
                        tiles[x, y].GetComponent<Tile_Data>().node.traversible = false;
                    }

                    //create OBJECTS on top of tiles
                    if (object_sprite_ids[x, y] != 0)
                    {
                        //pull up the object prefab
                        sprite = object_prefab.GetComponent<SpriteRenderer>();

                        //set the right sprite;
                        sprite.sprite = (Sprite)object_sprite_sheet[object_sprite_ids[x, y] - 1];
                        sprite.sortingOrder = tile_number;

                        //instantiate the object
                        Instantiate(object_prefab, new Vector3((float)(START_X - (x) * (TILE_WIDTH / 200) + (y) * (TILE_WIDTH / 200)), (float)(START_Y - (x) * (TILE_LENGTH / 200) - (y) * (TILE_LENGTH / 200) + tile_heights[x, y] * TILE_HEIGHT / 100.0 + .35f), 0), Quaternion.identity);
                    }
                }
                //3d logic
                else
                {
         
                    string file = "Objects/" + tile_heights[x,y] + "_tall";
                    int XOFFSET=0;
                    int YOFFSET=0;
                    int ZOFFSET=0;
                    if (tile_heights[x,y] == 1)
                    {
                        XOFFSET = 0;
                        YOFFSET = 0;
                        ZOFFSET = 0;
                    }
                    else if (tile_heights[x, y] == 2)
                    {
                        XOFFSET = 0;
                        YOFFSET = 1;
                        ZOFFSET = -30;
                    }
                    else if (tile_heights[x, y] == 3)
                    {
                        XOFFSET = 2;
                        YOFFSET = 5;
                        ZOFFSET = -3;
                    }
                    else if (tile_heights[x, y] == 4)
                    {
                        XOFFSET = 2;
                        YOFFSET = 2;
                        ZOFFSET = -3;
                    }
                    else if (tile_heights[x, y] == 5)
                    {
                        XOFFSET = 0;
                        YOFFSET = 2;
                        ZOFFSET = -3;
                    }
                    else if (tile_heights[x, y] == 6)
                    {
                        XOFFSET = 2;
                        YOFFSET = 2;
                        ZOFFSET = -1;
                    }
                    else if (tile_heights[x, y] == 7)
                    {
                        XOFFSET = 0;
                        YOFFSET = 2;
                        ZOFFSET = -1;
                    }
                    else if (tile_heights[x, y] == 8)
                    {
                        XOFFSET = 2;
                        YOFFSET = 10;
                        ZOFFSET = -2;
                    }
                    GameObject tile3d = Resources.Load(file, typeof(GameObject)) as GameObject;
                    int NEWSTARTX = 0;
                    int NEWSTARTY = 0;
                    int NEWSTARTZ = 0;
                    int NEWTILEWIDTH = 2;
                    int NEWTILELENGTH = 2;
                    int NEWTILEHEIGHT = 1; 
                    GameObject instance = ((GameObject)Instantiate(tile3d, new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y + XOFFSET), (float)(NEWSTARTY + YOFFSET), (float)(NEWSTARTZ - NEWTILELENGTH * x + ZOFFSET)), Quaternion.identity));

                    //Generate the tile data for the tile
                    instance.AddComponent<Tile_Data>();
                    instance.GetComponent<Tile_Data>().instantiate(x, y, tile_heights[x, y], tile_sprite_ids[x, y]);

                    //Store the instantiated tile in our Tile Tranform Grid;
                    tiles[x, y] = instance.transform;

                    //Add a node to the navmesh
                    navmesh.addNode(tiles[x, y].GetComponent<Tile_Data>().node);

                    //Connect the new node to previous nodes in the mesh
                    if (x > 0)
                    {
                        tiles[x, y].GetComponent<Tile_Data>().node.addEdge(tiles[x - 1, y].GetComponent<Tile_Data>().node, 3);
                    }
                    if (y > 0)
                    {
                        tiles[x, y].GetComponent<Tile_Data>().node.addEdge(tiles[x, y - 1].GetComponent<Tile_Data>().node, 0);
                    }

                    //create OBJECTS on top of tiles
                    if (object_sprite_ids[x, y] != 0)
                    {
                        //pull up the object prefab
                        sprite = object_prefab.GetComponent<SpriteRenderer>();

                        //set the right sprite;
                        sprite.sprite = (Sprite)object_sprite_sheet[object_sprite_ids[x, y] - 1];
                        sprite.sortingOrder = tile_number;

                        //instantiate the object
                        Instantiate(object_prefab, new Vector3((float)(START_X - (x) * (TILE_WIDTH / 200) + (y) * (TILE_WIDTH / 200)), (float)(START_Y - (x) * (TILE_LENGTH / 200) - (y) * (TILE_LENGTH / 200) + tile_heights[x, y] * TILE_HEIGHT / 100.0 + .35f), 0), Quaternion.identity);
                    }

                    //If there is an object on the tile, mark it non traversible
                    if (object_sprite_ids[x, y] == 0)
                    {
                        tiles[x, y].GetComponent<Tile_Data>().node.traversible = true;
                    }
                    else
                    {
                        tiles[x, y].GetComponent<Tile_Data>().node.traversible = false;
                    }
                }
            }
        }
        sprite.sortingOrder = 0;
    }

	/*public Tile_Grid(string[] lines, Transform tile_prefab, Sprite[] newTileSprites, Transform item_prefab, Sprite[] newItemSpriteSheet){
		tile = tile_prefab;
		tile_sprite_sheet = newTileSprites;
		item = item_prefab;
		item_sprite_sheet = newItemSpriteSheet;
		//read the height map
		int tile_sprite;
		int item_sprite;
		int tile_number = 0;
		int row_num = 0;
		int col_num = 0;
		int i = 0;
		double start_x_pos = 0;
		double start_y_pos = 3.5;
        navmesh = new Tile_Data.Graph();
		foreach (string line in lines){
			string[] elements = line.Split(';');
			foreach (string e in elements){
				if (row_num == 1) { 
					if (col_num == 0){
						if (int.TryParse(e, out map_width)){

						}
					}
					else if (col_num == 1) {
						if (int.TryParse(e, out map_height)){

						}
					}
				}
				else if (row_num >= 3 && row_num < map_height+3){
					//print ("string " + e);
					//string[] str = e.Split (',');
					//i = 0;
					//foreach (string s in str) {
					    if (int.TryParse(e, out tile_sprite)){
							//print ("Tile " + (row_num-3) + "," + col_num + "," + i + " sprite = " + s);
							tile_sprites[row_num-3,col_num] = tile_sprite;
							//tile_sprites[row_num-tile_grid_height-4,col_num,i] = tile_sprite;
					//	    i++;
					//		if (i >= MAX_TILES){
					//			break;
					//		}
					//	}
					}
					//print ("i = " + i);
					tile_heights[row_num-3,col_num] = tile_sprite/10+1;
				}
				else if(row_num >= map_height+4){
					if (int.TryParse(e, out item_sprite)){
						//TODO FIX TILE OBJECTS
						item_sprites[row_num-map_height-4,col_num] = item_sprite;
						//tile_sprites[row_num-tile_grid_height-4,col_num] = tile_sprite;
						//print("line_num:" + line_num);
						//print ("line_num - tile_grid_height - 4 = " + (line_num-tile_grid_height-4));
						//print ("num_num:" + num_num);
						//tiles[row_num-tile_grid_height-4,col_num].setTileSpriteIndex(tile_sprite);
					}
				}
				col_num++;
			}
			col_num = 0;
			row_num++;
		}

		for (int x = 0; x < map_width; x++){
			for (int y = 0; y < map_height; y++){
				//for (int z=0; z < tile_heights[x,y]; z++){
					//Set the correct sprite for the tile
					sprite = tile.GetComponent<SpriteRenderer>();
					sprite.sprite = tile_sprite_sheet[tile_sprites[x,y]-1];
 
					//Instantiate the tile object. Destroy the collider on the prefab if the tile is not on top of the stack.
					Transform instance= (Transform)Instantiate(tile, new Vector3((float)(start_x_pos - (x) * (TILE_WIDTH/200) + (y) * (TILE_WIDTH/200)), (float)(start_y_pos - (x) * (TILE_LENGTH/200) - (y) * (TILE_LENGTH/200)), 0), Quaternion.identity);
				    instance.gameObject.GetComponent<PolygonCollider2D>().offset.Set(0,.20f*tile_heights[x,y]+3);
				    //if (z != tile_heights[x,y]-1){
					//	Destroy (instance.gameObject.GetComponent<PolygonCollider2D>());
					//}
					if (tile_sprites[x,y]-1 != 5){
						Destroy (instance.gameObject.GetComponent<Animator>());
					}
					instance.GetComponent<Tile_Data>().instantiate(x,y,tile_heights[x,y],tile_sprites[x,y]);
                    

					//else {
					//	instance = Instantiate(tile, new Vector3((float)(start_x_pos - (x) * (TILE_WIDTH/200) + (y) * (TILE_WIDTH/200)), (float)(start_y_pos - (x) * (TILE_LENGTH/200) - (y) * (TILE_LENGTH/200)+z*TILE_HEIGHT/100.0), 0), Quaternion.identity);
					//}
					//Increase the tile draw order so other tiles are drawn correctly.
					//if (sprite)
					//{
					tile_number++;
					sprite.sortingOrder = tile_number;
					//}
					tiles[x,y] = instance;
                    navmesh.addNode(tiles[x,y].GetComponent<Tile_Data>().node);
                    if (x > 0)
                    {
                        tiles[x,y].GetComponent<Tile_Data>().node.addEdge(tiles[x - 1, y].GetComponent<Tile_Data>().node, 3);
                    }
                    if (y > 0)
                    {
                        tiles[x,y].GetComponent<Tile_Data>().node.addEdge(tiles[x, y - 1].GetComponent<Tile_Data>().node, 0);
                    }

                if (item_sprites[x,y] == 0){
						tiles[x,y].GetComponent<Tile_Data>().node.traversible = true;
					}
					else{
						tiles[x,y].GetComponent<Tile_Data>().node.traversible = false;
					}
				//}
				//create OBJECTS on top of tiles
				if (item_sprites[x,y] != 0) {
					sprite = item.GetComponent<SpriteRenderer>();
					sprite.sprite = item_sprite_sheet[item_sprites[x,y]-1];
					sprite.sortingOrder = tile_number;
					Instantiate(item, new Vector3((float)(start_x_pos - (x) * (TILE_WIDTH/200) + (y) * (TILE_WIDTH/200)), (float)(start_y_pos - (x) * (TILE_LENGTH/200) - (y) * (TILE_LENGTH/200)+tile_heights[x,y]*TILE_HEIGHT/100.0+.35f), 0), Quaternion.identity);

					//print (sprite.sortingOrder);
					//Instantiate(new Transform())
				}
			}
		}
		sprite.sortingOrder = 0;

	}*/

	public Transform[,] getTiles(){
		return tiles;
	}

	public Transform getTile(int x, int y){
		return tiles [x, y];
	}

	public double get_TILE_HEIGHT(){
		return TILE_HEIGHT;
	}

    void OnApplicationQuit()
    {
        reachable_prefab.GetComponent<SpriteRenderer>().sortingOrder = 0;
        tile_prefab.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);
    }
}

public class Scenario : MonoBehaviour {
    //Constants
    public static string PLAYER_STATS_FILE = "Assets/Resources/Characters/Player_Characters/Player_Character_Data.txt";
    public static string MONSTER_STATS_FILE = "Assets/Resources/Characters/Monster_Characters/Monster_Character_Data.txt";

    public enum Objectives { Vanquish, Resupply, Boss, Escort }
    public Tile_Grid tile_grid;
	public Game_Controller controller;
	public string scenario_file;
    public int scenario_id;
    public string scenario_sector;
    public string scenario_name;
    public string description;
    public Objectives objective;
    public List<string> rewards;
    public string bonus_objective;
    public List<string> bonus_rewards;
    public List<int> unlocks_scenarios;
    public List<int> unlocks_scenarios_on_loss;
    public List<int> unlocks_scenarios_on_win;
    public List<int> unlocks_scenarios_on_bonus;
    public GameObject curr_player;
    public GameObject highlighted_player;
    public GameObject cursor;
    public int turn_index;
    public int curr_character_num;
    public ArrayList characters;
    public Character_Script[] player_character_data;
    public Character_Script[] monster_character_data;
    public List<GameObject> turn_order;
    public Transform clicked_tile;
    public Transform selected_tile;
    List<Transform> reachable_tiles;
    List<GameObject> reachable_tile_objects;
    public int curr_round;

    public Scenario(string filename)
    {
        controller = Game_Controller.controller;
        scenario_file = filename;
        //Initialize Lists
        rewards = new List<string>();
        bonus_rewards = new List<string>();
        List<int> unlocks_scenarios = new List<int>();
        List<int> unlocks_scenarios_on_loss = new List<int>();
        List<int> unlocks_scenarios_on_win = new List<int>();
        List<int> unlocks_scenarios_on_bonus = new List<int>();
        //Read in the file
        string[] lines = System.IO.File.ReadAllLines(scenario_file);
        string line = "";
        string[] elements = new string[2];
        //30 is the default grid size;
        int grid_width=30;
        int grid_length=30;
        int[,] tile_sprite_ids = new int[grid_width, grid_length];
        int[,] object_sprite_ids = new int[grid_width, grid_length];
        int[,] character_sprite_ids = new int[grid_width, grid_length];
        //Read through the file line by line, looking for specific headings
        for (int i = 0; i < lines.Length; i++)
        {
            line = lines[i];
            switch (line)
            {
                //ID of the scenario
                case "[ID]":
                    scenario_name = lines[i + 1];
                    i += 2;
                    break;
                //What Sector the scenario can be found in
                case "[Sector]":
                    scenario_sector = lines[i + 1];
                    i += 2;
                    break;
                //Name of the Scenario
                case "[Name]":
                    scenario_name = lines[i + 1];
                    i+=2;
                    break;
                //Description for the Scenario
                case "[Description]":
                    description = lines[i + 1];
                    i += 2;
                    break;
                //Objective for the scenario (see Objectives enum for list of possible objectives.)
                case "[Objective]":
                    foreach (Objectives obj in System.Enum.GetValues(typeof(Objectives)))
                    {
                        if(obj.ToString() == lines[i + 1])
                        {
                            objective = obj;
                        }
                    }
                    i += 2;
                    break;
                //Reward for winning the scenario
                case "[Reward]":
                    for(int j = i+1; j<lines.Length; j++)
                    {
                        if (lines[j] != "")
                        {
                            rewards.Add(lines[j]);
                        }
                        else
                        {
                            i = j;
                            j = lines.Length;
                        }
                    }
                    break;
                //Bonus objective for the scenario
                case "[Bonus Objective]":
                    bonus_objective = lines[i + 1];
                    i += 2;
                    break;
                //Reward for completing the Bonus Objective
                case "[Bonus Reward]":
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j] != "")
                        {
                            bonus_rewards.Add(lines[j]);
                        }
                        else
                        {
                            i = j;
                            j = lines.Length;
                        }
                    }
                    break;
                //Scenario IDs that will be unlocked regardless of wether or not the Objective is met
                case "[Unlocks]":
                    elements = lines[i + 1].Split(';');
                    int id = scenario_id;
                    foreach (string s in elements)
                    {
                        if (int.TryParse(s, out id))
                        {
                            unlocks_scenarios.Add(id);
                        }
                    }
                    i += 2;
                    break;
                //Scenario IDs that will be unlocked if the Objective is not met
                case "[Unlocks on Loss]":
                    elements = lines[i + 1].Split(';');
                    id = scenario_id;
                    foreach (string s in elements)
                    {
                        if (int.TryParse(s, out id))
                        {
                            unlocks_scenarios_on_loss.Add(id);
                        }
                    }
                    i += 2;
                    break;
                //Scenario IDs that will be unlocked if the Objective is met
                case "[Unlocks on Win]":
                    elements = lines[i + 1].Split(';');
                    id = scenario_id;
                    foreach (string s in elements)
                    {
                        if (int.TryParse(s, out id))
                        {
                            unlocks_scenarios_on_win.Add(id);
                        }
                    }
                    i += 2;
                    break;
                //Scenario IDs that will be unlocked if the Bonus Objective is met
                case "[Unlocks on Bonus]":
                    elements = lines[i + 1].Split(';');
                    id = scenario_id;
                    foreach (string s in elements)
                    {
                        if (int.TryParse(s, out id))
                        {
                            unlocks_scenarios_on_bonus.Add(id);
                        }
                    }
                    i += 2;
                    break;
                //Size of the tile grid map
                case "[Grid Size]":
                    int width = 30;
                    int length = 30;
                    if (int.TryParse(lines[i + 1].Split(';')[0], out width))
                    {
                        grid_width = width;
                    }
                    if (int.TryParse(lines[i + 1].Split(';')[1], out length))
                    {
                        grid_length = length;
                    }
                    i += 2;
                    break;
                case "[Tile Map]":
                    tile_sprite_ids = new int[grid_width, grid_length];
                    for( int k = i+1; k<i+grid_length+1; k++)
                    {
                        string[] entries = lines[k].Split(';');
                        for (int l = 0; l<grid_width; l++)
                        {
                            int sprite;
                            if (int.TryParse(entries[l], out sprite))
                            {
                                tile_sprite_ids[k-i-1, l] = sprite;
                            }
                        }
                    }
                    break;
                case "[Object Map]":
                    object_sprite_ids = new int[grid_width, grid_length];
                    for (int k = i + 1; k < i + grid_length+1; k++)
                    {
                        string[] entries = lines[k].Split(';');
                        for (int l = 0; l < grid_width; l++)
                        {
                            int sprite;
                            if (int.TryParse(entries[l], out sprite))
                            {
                                object_sprite_ids[k-i-1, l] = sprite;
                            }
                        }
                    }
                    break;
                case "[Character Map]":
                    character_sprite_ids = new int[grid_width, grid_length];
                    for (int k = i + 1; k < i + grid_length + 1; k++)
                    {
                        string[] entries = lines[k].Split(';');
                        for (int l = 0; l < grid_width; l++)
                        {
                            int sprite;
                            if (int.TryParse(entries[l], out sprite))
                            {
                                character_sprite_ids[k - i - 1, l] = sprite;
                            }
                        }
                    }
                    break;
                //Default in case none of the above cases are met
                default:
                    break;
            }
            //Resume new scenario creation
            tile_grid = new Tile_Grid(grid_width, grid_length, tile_sprite_ids, object_sprite_ids, character_sprite_ids);
            tile_grid.tile_prefab.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);

            curr_round = 0;
            curr_character_num = 0;
            turn_index = 0;
            turn_order = new List<GameObject>();
            reachable_tiles = new List<Transform>();
            reachable_tile_objects = new List<GameObject>();
        }
    }

    Character_Script[] ReadCharacterData(string file)
    {
        string[] lines = System.IO.File.ReadAllLines(file);
        Character_Script[] objects = new Character_Script[lines.Length / 13];

        int count = 0;
        string name = "";
        int level = 1;
        int strength = 1;
        int coordination = 1;
        int spirit = 1;
        int dexterity = 1;
        int vitality = 1;
        int speed = 5;
        int canister_max = 1;
        string weapon = "Sword";
        string armor = "Light";
        foreach (string line in lines)
        {
            string[] elements = line.Split(':');
            if (!elements[0].Contains("#") && elements.Length > 1)
            {
                if (elements[0] == "name")
                {
                    name = elements[1];
                }
                else if (elements[0] == "level")
                {
                    if (int.TryParse(elements[1], out level))
                    { }
                }
                else if (elements[0] == "strength")
                {
                    if (int.TryParse(elements[1], out strength))
                    { }
                }
                else if (elements[0] == "coordination")
                {
                    if (int.TryParse(elements[1], out coordination))
                    { }
                }
                else if (elements[0] == "spirit")
                {
                    if (int.TryParse(elements[1], out spirit))
                    { }
                }
                else if (elements[0] == "dexterity")
                {
                    if (int.TryParse(elements[1], out dexterity))
                    { }
                }
                else if (elements[0] == "vitality")
                {
                    if (int.TryParse(elements[1], out vitality))
                    { }
                }
                else if (elements[0] == "speed")
                {
                    if (int.TryParse(elements[1], out speed))
                    { }
                }
                else if (elements[0] == "canister_max")
                {
                    if (int.TryParse(elements[1], out canister_max))
                    { }
                }
                else if (elements[0] == "weapon")
                {
                    weapon = elements[1];
                }
                else if (elements[0] == "armor")
                {
                    armor = elements[1];
                }
                else if (elements[0] == "accessories")
                {

                }
            }
            if (elements[0] == "")
            {
                objects[count] = new Character_Script(name, level, strength, coordination, spirit, dexterity, vitality, speed, canister_max, weapon, armor);
                count++;
                name = "";
                level = 1;
                strength = 1;
                coordination = 1;
                spirit = 1;
                dexterity = 1;
                vitality = 1;
                speed = 5;
                canister_max = 1;
                weapon = "Sword";
                armor = "Light";
            }
        }
        return objects;
    }

    public static int SortByDex(GameObject o1, GameObject o2)
    {
        if (o1.GetComponent<Character_Script>().dexterity > o2.GetComponent<Character_Script>().dexterity)
        {
            return 1;
        }
        else if (o1.GetComponent<Character_Script>().dexterity < o2.GetComponent<Character_Script>().dexterity)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public void LoadScenario()
    {
        //Instantiate the actual tile objects
        tile_grid.Instantiate();

        //Load player and monster stats and come up with turn order
        player_character_data = ReadCharacterData(PLAYER_STATS_FILE);
        monster_character_data = ReadCharacterData(MONSTER_STATS_FILE);
        Character_Script[] character_data;
        int char_num = 0;
        characters = new ArrayList();

        cursor = GameObject.FindGameObjectWithTag("Cursor");
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");

        //set starting positions for players
        //TODO change this 
        foreach (GameObject game_object in objects)
        {
            characters.Add(game_object);
            game_object.GetComponent<Character_Script>().character_num = char_num;
            game_object.GetComponent<Character_Script>().curr_tile = tile_grid.getTile(char_num, 0);
            game_object.transform.position = new Vector3(game_object.GetComponent<Character_Script>().curr_tile.position.x,
                game_object.GetComponent<Character_Script>().curr_tile.position.y,
                game_object.GetComponent<Character_Script>().curr_tile.position.z);
                 //game_object.GetComponent<Character_Script>().curr_tile.position.y + (float)(game_object.GetComponent<SpriteRenderer> ().sprite.rect.height / game_object.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit + 0.15f),
                 //(float)(game_object.GetComponent<Character_Script>().curr_tile.position.y + (game_object.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f,
                 //game_object.GetComponent<Character_Script>().curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
            game_object.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().node.traversible = false;
            //game_object.GetComponent<Character_Script>().FindReachable(tile_grid, game_object.GetComponent<Character_Script>().action_max, game_object.GetComponent<Character_Script>().dexterity);
            //game_object.GetComponent<SpriteRenderer>().sortingOrder = game_object.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
            char_num++;
        }
        //set starting position for monsters
        //TODO change this
        objects = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject game_object in objects)
        {
            characters.Add(game_object);
            game_object.GetComponent<Character_Script>().character_num = char_num;
            game_object.GetComponent<Character_Script>().curr_tile = tile_grid.getTile(19 - game_object.GetComponent<Character_Script>().character_num, 19);// [19-game_object.GetComponent<Character_Script>().character_num,19,0];
            game_object.transform.position = new Vector3(game_object.GetComponent<Character_Script>().curr_tile.position.x,
                game_object.GetComponent<Character_Script>().curr_tile.position.y,
                game_object.GetComponent<Character_Script>().curr_tile.position.z);
                                                          //game_object.GetComponent<Character_Script>().curr_tile.position.y + 0.5f,
                                                          //(float)(game_object.GetComponent<Character_Script>().curr_tile.position.y + (game_object.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f,
                                                          //game_object.GetComponent<Character_Script>().curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
            game_object.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().node.traversible = false;
            //game_object.GetComponent<Character_Script>().FindReachable(tile_grid, game_object.GetComponent<Character_Script>().action_max, game_object.GetComponent<Character_Script>().dexterity);
            //game_object.GetComponent<SpriteRenderer>().sortingOrder = game_object.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
            char_num++;
        }

        foreach (GameObject game_object in characters)
        {

            if (game_object.tag == "Player")
            {
                character_data = player_character_data;
            }
            else
            {
                character_data = monster_character_data;
            }
            
            game_object.GetComponent<Character_Script>().character_name = character_data[game_object.GetComponent<Character_Script>().character_id].character_name;
            game_object.GetComponent<Character_Script>().level = character_data[game_object.GetComponent<Character_Script>().character_id].level;
            game_object.GetComponent<Character_Script>().strength = character_data[game_object.GetComponent<Character_Script>().character_id].strength;
            game_object.GetComponent<Character_Script>().coordination = character_data[game_object.GetComponent<Character_Script>().character_id].coordination;
            game_object.GetComponent<Character_Script>().spirit = character_data[game_object.GetComponent<Character_Script>().character_id].spirit;
            game_object.GetComponent<Character_Script>().dexterity = character_data[game_object.GetComponent<Character_Script>().character_id].dexterity;
            game_object.GetComponent<Character_Script>().vitality = character_data[game_object.GetComponent<Character_Script>().character_id].vitality;
            game_object.GetComponent<Character_Script>().speed = character_data[game_object.GetComponent<Character_Script>().character_id].speed;
            game_object.GetComponent<Character_Script>().canister_max = character_data[game_object.GetComponent<Character_Script>().character_id].canister_max;

            //game_object.GetComponent<Character_Script>().Equip(character_data[game_object.GetComponent<Character_Script>().character_id].weapon);
            //game_object.GetComponent<Character_Script>().Equip(character_data[game_object.GetComponent<Character_Script>().character_id].armor);
            game_object.GetComponent<Character_Script>().weapon = character_data[game_object.GetComponent<Character_Script>().character_id].weapon;
            game_object.GetComponent<Character_Script>().armor = character_data[game_object.GetComponent<Character_Script>().character_id].armor;
            game_object.GetComponent<Character_Script>().aura_max = character_data[game_object.GetComponent<Character_Script>().character_id].aura_max;
            game_object.GetComponent<Character_Script>().aura_curr = character_data[game_object.GetComponent<Character_Script>().character_id].aura_curr;
            game_object.GetComponent<Character_Script>().action_max = 20;// character_data[game_object.GetComponent<Character_Script>().character_id].action_max;
            game_object.GetComponent<Character_Script>().action_curr = 20;// character_data[game_object.GetComponent<Character_Script>().character_id].action_curr;
            game_object.GetComponent<Character_Script>().action_cost = 0;
            game_object.GetComponent<Character_Script>().actions = character_data[game_object.GetComponent<Character_Script>().character_id].actions;
            game_object.GetComponent<Character_Script>().canister_curr = character_data[game_object.GetComponent<Character_Script>().character_id].canister_curr;
            game_object.GetComponent<Character_Script>().state = character_data[game_object.GetComponent<Character_Script>().character_id].state;
            game_object.GetComponent<Character_Script>().controller = character_data[game_object.GetComponent<Character_Script>().character_id].controller;
            //game_object.GetComponent<Character_Script>().Randomize();
            turn_order.Add(game_object);
        }
        turn_order.Sort(SortByDex);
        curr_character_num = characters.Count - 1;
        curr_player = turn_order[curr_character_num];
        curr_player.GetComponent<Animator>().SetBool("Selected", true);
        char_num = 0;

        

        //set initial values for selected and clicked tiles.
        selected_tile = tile_grid.getTiles()[0, 0];
        clicked_tile = tile_grid.getTiles()[0, 0];
    }

    public void unloadScenario()
    {
        //destroy the old map
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject game_object in objects)
        {
            Destroy(game_object);
        }
        objects = GameObject.FindGameObjectsWithTag("Object");
        foreach (GameObject game_object in objects)
        {
            Destroy(game_object);
        }

        tile_grid.tile_prefab.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);

    }

	// Use this for initialization
	void Start () {
        controller = Game_Controller.controller;
	}

    void playerInput()
    {
        //player selection
        if (Input.GetKeyDown("1"))
        {
            curr_character_num = 0;
        }
        else if (Input.GetKeyDown("2"))
        {
            curr_character_num = 1;
        }
        else if (Input.GetKeyDown("3"))
        {
            curr_character_num = 2;
        }
        else if (Input.GetKeyDown("4"))
        {
            curr_character_num = 3;
        }

        //check for mouse clicks
        if (Input.GetMouseButtonDown(0))
        {
            cursor.GetComponent<Animator>().SetBool("Clicked", true);
            clicked_tile = selected_tile;
            if (curr_player.GetComponent<Character_Script>().state != Character_Script.States.Idle ||
                curr_player.GetComponent<Character_Script>().state != Character_Script.States.Dead)
            {
                foreach (Transform tile in reachable_tiles)
                {
                    if (tile.Equals(clicked_tile))
                    {
                        if (clicked_tile.GetComponent<Tile_Data>().node.traversible && curr_player.GetComponent<Character_Script>().state == Character_Script.States.Moving)
                        {
                            StartCoroutine(curr_player.GetComponent<Character_Script>().Move(clicked_tile));
                            CleanReachable();
                            //NextPlayer();
                        }
                        if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Attacking)
                        {
                            foreach (GameObject character in characters)
                            {
                                if (character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().x_index == clicked_tile.GetComponent<Tile_Data>().x_index &&
                                    character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().y_index == clicked_tile.GetComponent<Tile_Data>().y_index)
                                {
                                    curr_player.GetComponent<Character_Script>().Attack(character);
                                    //NextPlayer();
                                    break;
                                }
                            }
                        }
                        if (clicked_tile.GetComponent<Tile_Data>().node.traversible && curr_player.GetComponent<Character_Script>().state == Character_Script.States.Blinking)
                        {
                            curr_player.GetComponent<Character_Script>().Blink(clicked_tile);
                            //NextPlayer();
                        }
                    }
                }
            }

        }

        //check for mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            cursor.GetComponent<Animator>().SetBool("Clicked", false);
        }

        //Next player button
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextPlayer();
        }

        //Prev player button
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PrevPlayer();
        }
    }

    public void checkMousePos()
    {
        //create a ray at the mouse position, point it towards the grid. See if it hits a collider.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit)
        {
            Tile_Data tile_data = hit.transform.GetComponent<Tile_Data>();
            //set the player's action cost to be the weight of the selected tile
            if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Moving)
            {
                curr_player.GetComponent<Character_Script>().action_cost = (int)tile_data.node.weight;//Math.Abs(curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().x_index - tile_data.x_index) * (int)(curr_player.GetComponent<Character_Script>().armor.weight + curr_player.GetComponent<Character_Script>().weapon.weight) + Math.Abs(curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().y_index - tile_data.y_index) * (int)(curr_player.GetComponent<Character_Script>().armor.weight + curr_player.GetComponent<Character_Script>().weapon.weight) + (tile_data.node.height - curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().node.height) * 2;
                if (curr_player.GetComponent<Character_Script>().action_cost < 1)
                {
                    curr_player.GetComponent<Character_Script>().action_cost = 0;
                }
                if (curr_player.GetComponent<Character_Script>().action_cost > curr_player.GetComponent<Character_Script>().action_curr)
                {
                    curr_player.GetComponent<Character_Script>().action_cost = curr_player.GetComponent<Character_Script>().action_curr;
                }
            }
            if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Blinking)
            {
                curr_player.GetComponent<Character_Script>().action_cost = Math.Abs(curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().x_index - tile_data.x_index) + Math.Abs(curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().y_index - tile_data.y_index);
                if (curr_player.GetComponent<Character_Script>().action_cost < 1)
                {
                    curr_player.GetComponent<Character_Script>().action_cost = 1;
                }
                if (curr_player.GetComponent<Character_Script>().action_cost > curr_player.GetComponent<Character_Script>().action_curr)
                {
                    curr_player.GetComponent<Character_Script>().action_cost = curr_player.GetComponent<Character_Script>().action_curr;
                }
            }
            //If the tile is not traversible we know it is occupied, check for a character there
            //this is done to print stats of the current highlighted character
            if (!tile_data.node.traversible)
            {
                foreach (GameObject character in characters)
                {
                    if (character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().x_index == tile_data.x_index && character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().y_index == tile_data.y_index)
                    {
                        highlighted_player = character;
                    }
                }
            }
            else
            {
                highlighted_player = null;
            }

            //update cursor location
            selected_tile = hit.transform;
            cursor.transform.position = new Vector3((float)(selected_tile.position.x + selected_tile.GetComponent<SpriteRenderer>().sprite.rect.width / 200) - .85f,
                                                     (float)(selected_tile.position.y + (selected_tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100),
                                                        selected_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
                                                                                   //renderer = (SpriteRenderer)curr_tile.GetComponent<SpriteRenderer> ();
            cursor.GetComponent<SpriteRenderer>().sortingOrder = selected_tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }

    }

	// Update is called once per frame
	public void Update () {
        if (scenario_id == controller.curr_scenario.scenario_id)
        {
            //if we are on this scenario and the current player has been assigned (scenario is active)
            curr_player = turn_order[curr_character_num];
           
            if (curr_player != null)
            {
                playerInput();
                checkMousePos();
            }
        }         
    }

    public void FindReachable(int cost_limit, int distance_limit)
    {
        tile_grid.navmesh.bfs(curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().node, cost_limit, distance_limit);

        reachable_tiles = new List<Transform>();
        int x_index = curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().x_index;
        int y_index = curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().y_index;
        int i = -distance_limit;
        int j = -distance_limit;
        while (i <= distance_limit)
        {
            while (j <= distance_limit)
            {
                if (x_index + i >= 0 && x_index + i < tile_grid.grid_width)
                {
                    if (y_index + j >= 0 && y_index + j < tile_grid.grid_length)
                    {
                        int h = tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().node.height - 1;
                        if (Mathf.Abs(i) + Mathf.Abs(j) <= curr_player.GetComponent<Character_Script>().SPEED)
                        {
                            if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Moving)
                            {
                                //if (grid.GetComponent<Scenario>().tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().traversible){
                                //    if ((Math.Abs(x_index - (x_index + i)) * (int)(armor.weight + weapon.weight) + Math.Abs(y_index - (y_index + j)) * (int)(armor.weight + weapon.weight) + (grid.GetComponent<Scenario>().tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().tile_height - curr_tile.GetComponent<Tile_Data>().tile_height) * 2) < action_curr)
                                //    {
                                //        reachable_tiles.Add(grid.GetComponent<Scenario>().tile_grid.getTile(x_index + i, y_index + j));
                                //    }
                                //}
                                if (tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().node.weight <= curr_player.GetComponent<Character_Script>().action_curr &&
                                    tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().node.weight > 0 &&
                                    tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().node.distance <= distance_limit)
                                {
                                    reachable_tiles.Add(tile_grid.getTile(x_index + i, y_index + j));
                                }
                            }
                            if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Blinking)
                            {
                                if (tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().node.traversible)
                                {
                                    if ((Math.Abs(x_index - (x_index + i)) + Math.Abs(y_index - (y_index + j))) < curr_player.GetComponent<Character_Script>().action_curr)
                                    {
                                        reachable_tiles.Add(tile_grid.getTile(x_index + i, y_index + j));
                                    }
                                }
                            }
                            if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Attacking)
                            {
                                //print ("scanned x index: " + x_index + i )
                                //Prevent Self-Harm
                                if (i != 0 || j != 0)
                                {
                                    reachable_tiles.Add(tile_grid.getTile(x_index + i, y_index + j));
                                }
                            }

                        }
                    }
                }
                j += 1;
            }
            j = -distance_limit;
            i += 1;
        }
    }

    public void ResetReachable()
    {
        foreach (Transform t in reachable_tiles)
        {
            t.GetComponent<Tile_Data>().node.weight = -1;
            t.GetComponent<Tile_Data>().node.parent = null;
        }
        reachable_tiles = new List<Transform>();
        CleanReachable();
    }

    public void MarkReachable()
    {
        reachable_tile_objects = new List<GameObject>();
        foreach (Transform tile in reachable_tiles)
        {
            //tile_grid.reachable_prefab.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Moving || curr_player.GetComponent<Character_Script>().state == Character_Script.States.Blinking)
            {
                tile_grid.reachable_prefab.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
            }
            if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Attacking)
            {
                tile_grid.reachable_prefab.GetComponent<SpriteRenderer>().color = new Color(255, 255, 0);
            }
            reachable_tile_objects.Add((GameObject)Instantiate(tile_grid.reachable_prefab, new Vector3(tile.position.x,
                                                           //tile.position.y+0.08f,
                                                           //(float)(tile.position.y + (tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) - .24f,
                                                           tile.position.y,
                                                           tile.position.z),
                                                           Quaternion.identity));

            //+ tile.GetComponent<SpriteRenderer>().sprite.rect.width/200
            // + selected_tile.GetComponent<SpriteRenderer>().sprite.rect.height/200
        }

    }

    public void CleanReachable()
    {
        //GameObject[] objects = GameObject.FindGameObjectsWithTag("Reachable");
        foreach (GameObject game_object in reachable_tile_objects)
        {
            Destroy(game_object);
            reachable_tile_objects.Remove(game_object);
        }
    }

    public void NextPlayer()
    {
        curr_character_num = curr_character_num - 1;
        if (curr_character_num < 0)
        {
            curr_character_num = characters.Count - 1;
        }
        curr_player.GetComponent<Animator>().SetBool("Selected", false);
        curr_player = turn_order[curr_character_num];
        if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Dead)
        {
            NextPlayer();
        }
        else
        {
            curr_player.GetComponent<Animator>().SetBool("Selected", true);
        }
        //curr_player.GetComponent<Character_Script>().FindReachable(tile_grid);
        CleanReachable();
        controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
        //MarkReachable ();

        //Center camera on player
        Camera.main.transform.position = curr_player.transform.position - Camera.main.transform.forward * 20;

    }

    public void PrevPlayer()
    {
        curr_character_num = curr_character_num + 1;
        if (curr_character_num >= characters.Count)
        {
            curr_character_num = 0;
        }

        curr_player.GetComponent<Animator>().SetBool("Selected", false);
        curr_player = turn_order[curr_character_num];
        if (curr_player.GetComponent<Character_Script>().state == Character_Script.States.Dead)
        {
            PrevPlayer();
        }
        else
        {
            curr_player.GetComponent<Animator>().SetBool("Selected", true);
        }
        CleanReachable();
        controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
        //MarkReachable ();

        //Center camera on player
        Camera.main.transform.position = curr_player.transform.position - Camera.main.transform.forward * 20;
    }

    public void NextRound()
    {
        curr_round += 1;
        foreach (GameObject game_object in characters)
        {
            /*if (turn_order.Count > 0)
            {
                foreach (GameObject obj in turn_order)
                {
                    if (game_object.GetComponent<Character_Script>().dexterity > obj.GetComponent<Character_Script>().dexterity)
                    {
                        turn_order.Add(gameObject);
                    }
                    if (game_object.GetComponent<Character_Script>().dexterity == obj.GetComponent<Character_Script>().dexterity)
                    {
                        if (game_object.GetComponent<Character_Script>().character_num > obj.GetComponent<Character_Script>().character_num)
                        {
                            turn_order.Add(gameObject);
                        }
                        else
                        {
                            int index = turn_order.IndexOf(obj);
                            turn_order.Insert(index, gameObject);
                            turn_order.Insert(index + 1, obj);
                        }
                    }
                }
            }
            else turn_order.Add(gameObject);*/
        }
    }
}
