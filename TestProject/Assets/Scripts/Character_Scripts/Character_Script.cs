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
		level = 1;
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
	public string profession { get; set; }
	public Equipment[] equipment { get; set; }
	public Game_Controller controller { get; set; }
	public Transform curr_tile { get; set; }
	public List<Transform> reachable_tiles { get; set; }
	
	//public SpriteRenderer renderer;
	public void FindReachable(GameObject grid){
		reachable_tiles = new List<Transform> ();
		int x_index = curr_tile.GetComponent<Tile_Data> ().x_index;
		int y_index = curr_tile.GetComponent<Tile_Data> ().y_index;
		int i = -dexterity;
		int j = -dexterity;
		while ( i <= dexterity) {
			while (j <= dexterity) {
				//print ("i " + i);
				//print ("j " + j);
				if (x_index + i  >= 0 && x_index + i < 20){
					if( y_index + j >= 0 && y_index + j < 20)
					{
						int h = grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTopTile(x_index + i, y_index + j ).GetComponent<Tile_Data>().tile_height-1;
						//print ("distance " + (Mathf.Abs(i)+ Mathf.Abs(j)));
						if (Mathf.Abs (i) + Mathf.Abs (j) <= dexterity){
							//print ("tile " + (x_index +i) + ","+ (y_index+ j) + " is reachable");
							if (grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j, h).GetComponent<Tile_Data>().traversible){
								reachable_tiles.Add(grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j, h));
							}
						}
					}
				}
				j += 1;
			}
			j = -dexterity;
			i += 1;
		}
	}

	public Character_Script(){
	}
	
	// Update is called once per frame
	void Update () {
	}
}
