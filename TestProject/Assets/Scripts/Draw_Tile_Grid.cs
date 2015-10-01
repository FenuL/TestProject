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

public class Tile_Grid : ScriptableObject{
	public float TILE_LENGTH = 43;
	public float TILE_WIDTH = 85;
	public float TILE_HEIGHT = 12;
	public static int tile_grid_width=20;
	public static int tile_grid_height=20;
	Transform tile;
	Sprite[] tileSpriteSheet;
	SpriteRenderer sprite;
	Tile[,,] tiles = new Tile[tile_grid_width, tile_grid_height,10];
	int[,] tile_heights = new int[tile_grid_width, tile_grid_height];
	int[,] tile_sprites = new int[tile_grid_width, tile_grid_height];

	public Tile_Grid(string[] lines, Transform tile_prefab, Sprite[] newTileSprites){
		tile = tile_prefab;
		tileSpriteSheet = newTileSprites;
		//read the height map
		int tile_height;
		int tile_sprite;
		int tile_number = 0;
		int row_num = 0;
		int col_num = 0;
		double start_x_pos = 0;
		double start_y_pos = 4;
		foreach (string line in lines){
			string[] elements = line.Split(' ');
			foreach (string s in elements){
				if (row_num == 1) { 
					if (col_num==0){
						if (int.TryParse(s, out tile_grid_width)){
						}
					}
					else if (col_num == 1) {
						if (int.TryParse(s, out tile_grid_height)){
						}
					}
				}
				else if (row_num >= 3 && row_num <= tile_grid_height+3){
					if (int.TryParse(s, out tile_height)){
						tile_heights[row_num-3,col_num] = tile_height;
						//double tile_x_pos = start_x_pos - (row_num-3) * (TILE_WIDTH/200) + (col_num) * (TILE_WIDTH/200);
						//double tile_y_pos = start_y_pos - (row_num-3) * (TILE_LENGTH/200) - (col_num) * (TILE_LENGTH/200);
						//print("line_num:" + line_num);
						//print ("num_num:" + num_num);
						//print ("x_pos:" + tile_x_pos);
						//print ("y_pos:" + tile_y_pos);
						//tiles[row_num-3,col_num] = new Tile(row_num-3, col_num, tile_x_pos, tile_y_pos, tile_height);
					}
				}
				else if(row_num >= tile_grid_height+4){
					if (int.TryParse(s, out tile_sprite)){
						tile_sprites[row_num-tile_grid_height-4,col_num] = tile_sprite;
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
					sprite.sprite = tileSpriteSheet[tile_sprites[x,y]-1];
					
					//Instantiate the tile object. Destroy the collider on the prefab if the tile is not on top of the stack. 
					if (z != tile_heights[x,y]-1){
						Destroy (((Transform)Instantiate(tile, new Vector3((float)(start_x_pos - (x) * (TILE_WIDTH/200) + (y) * (TILE_WIDTH/200)), (float)(start_y_pos - (x) * (TILE_LENGTH/200) - (y) * (TILE_LENGTH/200)+z*TILE_HEIGHT/100.0), 0), Quaternion.identity)).gameObject.GetComponent<PolygonCollider2D>());
					}
					else {
						Instantiate(tile, new Vector3((float)(start_x_pos - (x) * (TILE_WIDTH/200) + (y) * (TILE_WIDTH/200)), (float)(start_y_pos - (x) * (TILE_LENGTH/200) - (y) * (TILE_LENGTH/200)+z*TILE_HEIGHT/100.0), 0), Quaternion.identity);
					}
					//Increase the tile draw order so other tiles are drawn correctly.
					if (sprite)
					{
						tile_number++;
						sprite.sortingOrder = tile_number;
					}
					tiles[x,y,z] = new Tile(tile,x,y,tile_heights[x,y],tile_sprites[x,y]);
					script = tile.GetComponent<TileData>();
					script.x_index = x;
					script.y_index = y;
					script.z_index = z;
					script.tile_height = tile_heights[x,y];
					script.tile_sprite_index = tile_sprites[x,y];
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
	public Tile_Grid tileGrid;

	// Use this for initialization
	void Start () {
		string[] lines = System.IO.File.ReadAllLines(@"Assets/height_map.txt");
		tileGrid = new Tile_Grid(lines, tile, tileSpriteSheet);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
