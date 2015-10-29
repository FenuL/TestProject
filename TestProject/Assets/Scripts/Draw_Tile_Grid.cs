using UnityEngine;
using System.Collections;
using UnityEditor;

public class Tile {
	//variables
	Transform tileObj;
	int tileXIndex;
	int tileYIndex;
	int tileHeight;
	int tileSpriteIndex;

	//methods
	public Tile(Transform tile, int x_index, int y_index, int height, int sprite){
		tileObj = tile;
		tileXIndex = x_index;
		tileYIndex = y_index;
		tileHeight = height;
		tileSpriteIndex = sprite;
	}

	public Tile(Transform tile, int x_index, int y_index, int height){
		tileObj = tile;
		tileXIndex = x_index;
		tileYIndex = y_index;
		tileHeight = height;
		tileSpriteIndex = 1;
	}

	public Tile(int x_index, int y_index, int height){
		tileXIndex = x_index;
		tileYIndex = y_index;
		tileHeight = height;
		tileSpriteIndex = 1;
	}

	public int getXIndex(){
		return tileXIndex;
	}
	
	public int getYIndex(){
		return tileYIndex;
	}

	public Transform getObj (){
		return tileObj;
	}

	public void setObj (Transform tile){
		tileObj = tile;
	}

	public int getHeight(){
		return tileHeight;
	}

	public int getTileSpriteIndex(){
		return tileSpriteIndex;
	}

	public void setTileSpriteIndex(int i){
		tileSpriteIndex = i;
	}
}

public class Tile_Grid : MonoBehaviour{
	public float TILE_LENGTH = 43;
	public float TILE_WIDTH = 85;
	public float TILE_HEIGHT = 25;
	public static int MAX_TILES = 10;
	public static int tile_grid_width=20;
	public static int tile_grid_height=20;
	Transform tile;
	Sprite[] tileSpriteSheet;
	Transform item;
	Sprite[] itemSpriteSheet;
	SpriteRenderer sprite;
	int[,] item_sprites = new int[tile_grid_width, tile_grid_height];
	Tile[,,] tiles = new Tile[tile_grid_width, tile_grid_height,MAX_TILES];
	int[,] tile_heights = new int[tile_grid_width, tile_grid_height];
	int[,,] tile_sprites = new int[tile_grid_width, tile_grid_height,MAX_TILES];

	public Tile_Grid(string[] lines, Transform tile_prefab, Sprite[] newTileSprites, Transform item_prefab, Sprite[] newItemSpriteSheet){
		tile = tile_prefab;
		tileSpriteSheet = newTileSprites;
		item = item_prefab;
		itemSpriteSheet = newItemSpriteSheet;
		//read the height map
		int tile_height;
		int tile_sprite;
		int item_sprite;
		int tile_number = 0;
		int row_num = 0;
		int col_num = 0;
		int i = 0;
		double start_x_pos = 0;
		double start_y_pos = 3.5;
		foreach (string line in lines){
			string[] elements = line.Split(';');
			foreach (string e in elements){
				if (row_num == 1) { 
					if (col_num==0){
						//if (int.TryParse(e, out tile_grid_width)){
						//}
					}
					else if (col_num == 1) {
						//if (int.TryParse(e, out tile_grid_height)){
						//}
					}
				}
				else if (row_num >= 3 && row_num < tile_grid_height+3){
					//print ("string " + e);
					string[] str = e.Split (',');
					i = 0;
					foreach (string s in str) {
					    if (int.TryParse(s, out tile_sprite)){
							//print ("Tile " + (row_num-3) + "," + col_num + "," + i + " sprite = " + s);
							tile_sprites[row_num-3,col_num,i] = tile_sprite;
							//tile_sprites[row_num-tile_grid_height-4,col_num,i] = tile_sprite;
						    i++;
							if (i >= MAX_TILES){
								break;
							}
						}
					}
					//print ("i = " + i);
					tile_heights[row_num-3,col_num] = i;
				}
				else if(row_num >= tile_grid_height+4){
					if (int.TryParse(e, out item_sprite)){
						//TODO FIX TILE OBJECTS
						item_sprites[row_num-tile_grid_height-4,col_num] = item_sprite;
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

		GameObject Tile_Grid;
		TileData script;
		for (int x = 0; x < tile_grid_width; x++){
			for (int y = 0; y < tile_grid_height; y++){
				for (int z=0; z < tile_heights[x,y]; z++){
					//Set the correct sprite for the tile
					sprite = tile.GetComponent<SpriteRenderer>();
					sprite.sprite = tileSpriteSheet[tile_sprites[x,y,z]-1];
 
					//Instantiate the tile object. Destroy the collider on the prefab if the tile is not on top of the stack.
					Transform instance= (Transform)Instantiate(tile, new Vector3((float)(start_x_pos - (x) * (TILE_WIDTH/200) + (y) * (TILE_WIDTH/200)), (float)(start_y_pos - (x) * (TILE_LENGTH/200) - (y) * (TILE_LENGTH/200)+z*TILE_HEIGHT/100.0), 0), Quaternion.identity);
					if (z != tile_heights[x,y]-1){
						Destroy (instance.gameObject.GetComponent<PolygonCollider2D>());
					}
					if (tile_sprites[x,y,z]-1 != 2){
						Destroy (instance.gameObject.GetComponent<Animator>());
					}
					//else {
					//	instance = Instantiate(tile, new Vector3((float)(start_x_pos - (x) * (TILE_WIDTH/200) + (y) * (TILE_WIDTH/200)), (float)(start_y_pos - (x) * (TILE_LENGTH/200) - (y) * (TILE_LENGTH/200)+z*TILE_HEIGHT/100.0), 0), Quaternion.identity);
					//}
					//Increase the tile draw order so other tiles are drawn correctly.
					//if (sprite)
					//{
					tile_number++;
					sprite.sortingOrder = tile_number;
					//}
					tiles[x,y,z] = new Tile(tile,x,y,tile_heights[x,y],tile_sprites[x,y,z]);
					script = tile.GetComponent<TileData>();
					script.x_index = x;
					script.y_index = y;
					script.z_index = z;
					script.tile_height = tile_heights[x,y];
					script.tile_sprite_index = tile_sprites[x,y,z];
					if(item_sprites[x,y] == 0){
						script.traversible = true;
					}
					else{
						script.traversible = false;
					}
					script.object_sprite = item_sprites[x,y];
				}
				if (item_sprites[x,y] != 0) {
					sprite = item.GetComponent<SpriteRenderer>();
					sprite.sprite = itemSpriteSheet[item_sprites[x,y]-1];
					sprite.sortingOrder = tile_number + 1;
					Instantiate(item, new Vector3((float)(start_x_pos - (x) * (TILE_WIDTH/200) + (y) * (TILE_WIDTH/200)), (float)(start_y_pos - (x) * (TILE_LENGTH/200) - (y) * (TILE_LENGTH/200)+tile_heights[x,y]*TILE_HEIGHT/100.0+ 0.075f), 0), Quaternion.identity);

					print (sprite.sortingOrder);
					//Instantiate(new Transform())
				}
			}
		}
		sprite.sortingOrder = 0;

	}

	public Tile[,,] getTiles(){
		return tiles;
	}

	public Tile getTopTile(int x, int y){
		return tiles [x, y, tiles [x, y, 0].getHeight () - 1];
	}

	public double get_TILE_HEIGHT(){
		return TILE_HEIGHT;
	}
}

public class Draw_Tile_Grid : MonoBehaviour {

	public Transform tile;
	public Sprite[] tileSpriteSheet;
	public Transform item;
	public Sprite[] itemSpriteSheet;
	public Tile_Grid tileGrid;
	public string currMap;

	// Use this for initialization
	void Start () {
		//string[] lines = System.IO.File.ReadAllLines(@"Assets/Maps/falls_map.txt");
		currMap = "Assets/Maps/tile_map.txt";
		string[] lines = System.IO.File.ReadAllLines(currMap);
		tileGrid = new Tile_Grid(lines, tile, tileSpriteSheet, item, itemSpriteSheet);
		tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			if(currMap == "Assets/Maps/falls_map.txt"){
				//destroy the old map
			    GameObject[] objects = GameObject.FindGameObjectsWithTag ("Tile");
			    foreach (GameObject game_object in objects) {
				    Destroy (game_object);
			    }
				objects = GameObject.FindGameObjectsWithTag ("Object");
				foreach (GameObject game_object in objects) {
					Destroy (game_object);
				}
				//create new map
				currMap = "Assets/Maps/tile_map.txt";
				string[] lines = System.IO.File.ReadAllLines(currMap);
			    tileGrid = new Tile_Grid(lines, tile, tileSpriteSheet, item, itemSpriteSheet);
			    tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
			}else{
				//destroy the old map
				GameObject[] objects = GameObject.FindGameObjectsWithTag ("Tile");
				foreach (GameObject game_object in objects) {
					Destroy (game_object);
				}
				objects = GameObject.FindGameObjectsWithTag ("Object");
				foreach (GameObject game_object in objects) {
					Destroy (game_object);
				}
				//create new map
				currMap = "Assets/Maps/falls_map.txt";
				string[] lines = System.IO.File.ReadAllLines(currMap);
				tileGrid = new Tile_Grid(lines, tile, tileSpriteSheet, item, itemSpriteSheet);
				tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
			}
		}
	}
	
	void OnApplicationQuit() {
		tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
	}
}
