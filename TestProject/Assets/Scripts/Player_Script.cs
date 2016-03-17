using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Script : MonoBehaviour{

	// Use this for initialization
	void Start () {

	}

	public int player_num;
	public string player_name;
	public int player_health;
	public int player_mana;
	public int strength;
	public int coordination;
	public int spirit;
	public int dexterity;
	public int vitality;
	public int level;
	public string profession;
	public Equipment[] equipment;
	public GameObject controller;
	public Transform curr_tile;
	public List<Transform> reachable_tiles;

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

	// Update is called once per frame
	void Update () {
	}
}
