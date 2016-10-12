using UnityEngine;
using System.Collections;
using UnityEditor;

public class Tile_Grid : MonoBehaviour{
	public float TILE_LENGTH = 43;
	public float TILE_WIDTH = 85;
	public float TILE_HEIGHT = 20;
	public static int MAX_TILES = 10;
	public static int tile_grid_width=40;
	public static int tile_grid_height=40;
	public int map_height;
	public int map_width;
	Transform tile;
	Sprite[] tile_sprite_sheet;
	Transform item;
	Sprite[] item_sprite_sheet;
	SpriteRenderer sprite;
	int[,] item_sprites = new int[tile_grid_width, tile_grid_height];
	Transform[,] tiles = new Transform[tile_grid_width, tile_grid_height];
	int[,] tile_heights = new int[tile_grid_width, tile_grid_height];
	int[,] tile_sprites = new int[tile_grid_width, tile_grid_height];
    public Tile_Data.Graph navmesh { get; set; }

	public Tile_Grid(string[] lines, Transform tile_prefab, Sprite[] newTileSprites, Transform item_prefab, Sprite[] newItemSpriteSheet){
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
				    /*if (z != tile_heights[x,y]-1){
						Destroy (instance.gameObject.GetComponent<PolygonCollider2D>());
					}*/
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

	}

	public Transform[,] getTiles(){
		return tiles;
	}

	public Transform getTile(int x, int y){
		return tiles [x, y];
	}

/*	public Transform getTopTile(int x, int y){
		if (tiles [x, y].GetComponent<Tile_Data>().tile_height == 0)
			return tiles [x, y];
		else
			return tiles [x, y, tiles [x, y].GetComponent<Tile_Data>().tile_height - 1];
	}
*/
	public double get_TILE_HEIGHT(){
		return TILE_HEIGHT;
	}
}

public class Draw_Tile_Grid : MonoBehaviour {

	public Transform tile;
	public Sprite[] tile_sprite_sheet;
	public Transform item;
	public Sprite[] item_sprite_sheet;
	public Tile_Grid tile_grid;
	public GameObject controller;
	public string curr_map;

	// Use this for initialization
	void Start () {
		//string[] lines = System.IO.File.ReadAllLines(@"Assets/Maps/falls_map.txt");
		curr_map = controller.GetComponent<Game_Controller>().curr_map;
		string[] lines = System.IO.File.ReadAllLines(curr_map);
		tile_grid = new Tile_Grid(lines, tile, tile_sprite_sheet, item, item_sprite_sheet);
		tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
        //tile_grid.navmesh.printGraph();
	}
	
	// Update is called once per frame
	void Update () {
		//print ("curr_map  " + curr_map);
		//print ("currMap " + controller.GetComponent<Game_Controller> ().curr_map);
		if (curr_map != controller.GetComponent<Game_Controller> ().curr_map) {
			curr_map = controller.GetComponent<Game_Controller> ().curr_map;
			//destroy the old map
			GameObject[] objects = GameObject.FindGameObjectsWithTag ("Tile");
			foreach (GameObject game_object in objects) {
				Destroy (game_object);
			}
			objects = GameObject.FindGameObjectsWithTag ("Object");
			foreach (GameObject game_object in objects) {
				Destroy (game_object);
			}
			string[] lines = System.IO.File.ReadAllLines(curr_map);
			tile_grid = new Tile_Grid(lines, tile, tile_sprite_sheet, item, item_sprite_sheet);
			tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
		}

	}
	
	void OnApplicationQuit() {
		tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
	}
}
