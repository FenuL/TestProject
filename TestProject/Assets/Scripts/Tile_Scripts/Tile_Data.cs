using UnityEngine;
using System.Collections;



public class Tile_Data : MonoBehaviour{
	
	public int x_index;
	public int y_index;
	public int z_index;
	public int tile_height;
	public int tile_sprite_index;
	public bool traversible;
	public int object_sprite;
	public bool reachable;

	//methods
	public Tile_Data(int x, int y, int height, int sprite){
		x_index = x;
		y_index = y;
		tile_height = height;
		tile_sprite_index = sprite;
		traversible = true;
	}
	
	public Tile_Data(int x, int y, int height){
		x_index = x;
		y_index = y;
		tile_height = height;
		tile_sprite_index = 1;
		traversible = true;
	}

	public void instantiate(int x, int y, int height, int sprite){
		x_index = x;
		y_index = y;
		tile_height = height;
		tile_sprite_index = sprite;
		traversible = true;
	}

	// Use this for initialization
	void Start () {
		reachable = true;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
