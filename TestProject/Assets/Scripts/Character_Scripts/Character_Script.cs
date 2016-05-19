using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character_Script : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	public void Randomize(){
		aura_max = Random.Range (10, 30);
		aura_curr = aura_max;
		canister_max = Random.Range (0,3);
		canister_curr = canister_max;
		armor = Random.Range (0, 5);
		strength = Random.Range (1,7);
		coordination = Random.Range (1, 7);
		spirit = Random.Range (1, 7);
		dexterity = Random.Range (1, 7);
		vitality = Random.Range (1, 7);
		actions = new string[3];
		actions [0] = "Move";
		actions [1] = "Attack";
		actions [2] = "Wait";
		state = "Moving";
		range = 1;
		level = 1;
		character_name = "Character " + character_num;
		controller = Game_Controller.controller;
	}

	public int character_id { get; set; } 
	public int character_num { get; set; }
	public string character_name { get; set; }
	public int aura_max { get; set; }
	public int aura_curr { get; set; }
	public int canister_max { get; set; }
	public int canister_curr { get; set; }
	public int armor { get; set; }
	public int strength { get; set; }
	public int coordination { get; set; }
	public int spirit { get; set; }
	public int dexterity { get; set; }
	public int vitality { get; set; }
	public int level { get; set; }
	public int range { get; set; }
	public string[] actions{ get; set;}
	public string profession { get; set; }
	public string state { get; set; }
	public Equipment[] equipment { get; set; }
	public Game_Controller controller { get; set; }
	public Transform curr_tile { get; set; }
	public List<Transform> reachable_tiles { get; set; }
	
	//public SpriteRenderer renderer;
	public void FindReachable(GameObject grid, int limit){
		reachable_tiles = new List<Transform> ();
		int x_index = curr_tile.GetComponent<Tile_Data> ().x_index;
		int y_index = curr_tile.GetComponent<Tile_Data> ().y_index;
		int i = -limit;
		int j = -limit;
		while ( i <= limit) {
			while (j <= limit) {
				//print ("i " + i);
				//print ("j " + j);
				if (x_index + i  >= 0 && x_index + i < grid.GetComponent<Draw_Tile_Grid>().tile_grid.map_width){
					if( y_index + j >= 0 && y_index + j < grid.GetComponent<Draw_Tile_Grid>().tile_grid.map_height)
					{
						int h = grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j ).GetComponent<Tile_Data>().tile_height-1;
						//print ("distance " + (Mathf.Abs(i)+ Mathf.Abs(j)));
						if (Mathf.Abs (i) + Mathf.Abs (j) <= dexterity){
							//print ("tile " + (x_index +i) + ","+ (y_index+ j) + " is reachable");
							if (state == "Moving"){
								if (grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().traversible){
									reachable_tiles.Add(grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j));
								}
							}
							if (state == "Attacking"){
								reachable_tiles.Add(grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j));
							}

						}
					}
				}
				j += 1;
			}
			j = -limit;
			i += 1;
		}
	}

	public void Action(string s){
		if (s == "Move") {
			state = "Moving";
			FindReachable(controller.tile_grid, dexterity);
			controller.CleanReachable ();
			controller.MarkReachable ();
		}
		if (s == "Attack") {
			state = "Attacking";
			FindReachable(controller.tile_grid, range);
			controller.CleanReachable ();
			controller.MarkReachable ();
		}

	}

	public Character_Script(){
	}
	
	// Update is called once per frame
	void Update () {
		if (aura_curr < 0) {
			aura_curr = 0;
		}
	}
}
