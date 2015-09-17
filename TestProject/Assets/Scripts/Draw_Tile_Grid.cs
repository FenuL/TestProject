using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	//variables
	int tile_height;
	int tile_sprite;
	int tile_x_index;
	int tile_y_index;
	double tile_x_pos;
	double tile_y_pos;

	//methods
	public Tile(int x_index, int y_index, double x_pos, double y_pos, int height, int sprite){
		tile_x_index = x_index;
		tile_y_index = y_index;
		tile_x_pos = x_pos;
		tile_y_pos = y_pos;
		tile_height = height;
		tile_sprite = sprite;
	}

	public Tile(int x_index, int y_index, double x_pos, double y_pos, int height){
		tile_x_index = x_index;
		tile_y_index = y_index;
		tile_x_pos = x_pos;
		tile_y_pos = y_pos;
		tile_height = height;
		tile_sprite = 1;
	}

	public int get_x_index(){
		return tile_x_index;
	}
	
	public int get_y_index(){
		return tile_y_index;
	}

	public double get_x_pos(){
		return tile_x_pos;
	}

	public double get_y_pos(){
		return tile_y_pos;
	}

	public int get_height(){
		return tile_height;
	}

	public int get_tile_sprite(){
		return tile_sprite;
	}

	public void set_tile_sprite(int i){
		tile_sprite = i;
	}
}

public class Tile_Grid : MonoBehaviour {
	double TILE_LENGTH = 43;
	double TILE_WIDTH = 85;
	double TILE_HEIGHT = 12;
	static int tile_grid_width=20;
	static int tile_grid_height=20;
	Tile[,] tiles = new Tile[tile_grid_width, tile_grid_height];

	public Tile_Grid(string[] lines){
		//read the height map
		int tile_height;
		int tile_sprite;
		int line_num = 0;
		int num_num = 0;
		double start_x_pos = 0;
		double start_y_pos = 4;
		foreach (string line in lines){
			string[] elements = line.Split(' ');
			foreach (string s in elements){
				if (line_num == 1) { 
					if (num_num==0){
						if (int.TryParse(s, out tile_grid_width)){
						}
					}
					else if (num_num == 1) {
						if (int.TryParse(s, out tile_grid_height)){
						}
					}
				}
				else if (line_num >= 3 && line_num <= tile_grid_height+3){
					if (int.TryParse(s, out tile_height)){
						double tile_x_pos = start_x_pos - (line_num-3) * (TILE_WIDTH/200) + (num_num) * (TILE_WIDTH/200);
						double tile_y_pos = start_y_pos - (line_num-3) * (TILE_LENGTH/200) - (num_num) * (TILE_LENGTH/200);
						//print("line_num:" + line_num);
						//print ("num_num:" + num_num);
						//print ("x_pos:" + tile_x_pos);
						//print ("y_pos:" + tile_y_pos);
						tiles[line_num-3,num_num] = new Tile(line_num-3, num_num, tile_x_pos, tile_y_pos, tile_height);
					}
				}
				else if(line_num >= tile_grid_height+4){
					if (int.TryParse(s, out tile_sprite)){
						//print("line_num:" + line_num);
						//print ("line_num - tile_grid_height - 4 = " + (line_num-tile_grid_height-4));
						//print ("num_num:" + num_num);
						tiles[line_num-tile_grid_height-4,num_num].set_tile_sprite(tile_sprite);
					}
				}
				num_num++;
			}
			num_num = 0;
			line_num++;

		}

		//create the tiles
		foreach (Tile t in tiles){

			//var tile_object = new GameObject("tile");
			//tile_object.AddComponent();
			//GameObject tile_object = new GameObject("tile");
			//cube.AddComponent<Rigidbody>();
			//cube.transform.position = new Vector3(x, y, 0);
			//GameObject tile = (GameObject) Instantiate (tile_object, new Vector3(t.get_x_pos(), t.get_y_pos(), 0));
		}
	}

	public Tile[,] get_tiles(){
		return tiles;
	}

	public double get_TILE_HEIGHT(){
		return TILE_HEIGHT;
	}
}

public class Draw_Tile_Grid : MonoBehaviour {

	public Transform Tile1;
	public Transform Tile2;
	public Transform Tile3;
	public Transform Tile4;
	private Tile_Grid tile_grid;

	// Use this for initialization
	void Start () {
		int tile_number = 0;
		string[] lines = System.IO.File.ReadAllLines(@"Assets/height_map.txt");
		tile_grid = new Tile_Grid(lines);
		SpriteRenderer sprite1 = Tile1.GetComponent<SpriteRenderer>();
		SpriteRenderer sprite2 = Tile2.GetComponent<SpriteRenderer>();
		SpriteRenderer sprite3 = Tile3.GetComponent<SpriteRenderer>();
		SpriteRenderer sprite4 = Tile4.GetComponent<SpriteRenderer>();
		Transform tran;
		GameObject obj;
		foreach (Tile t in tile_grid.get_tiles()) {

			for (int i=0; i < t.get_height(); i++){

				if (t.get_tile_sprite() == 1){

					if (i != t.get_height()-1){
						Destroy (((Transform)Instantiate(Tile1, new Vector3((float)t.get_x_pos(), (float)(t.get_y_pos()+i*tile_grid.get_TILE_HEIGHT()/100.0), 0), Quaternion.identity)).gameObject.GetComponent<PolygonCollider2D>());
						sprite1 = Tile1.GetComponent<SpriteRenderer>();
					}
					else {
						Instantiate(Tile1, new Vector3((float)t.get_x_pos(), (float)(t.get_y_pos()+i*tile_grid.get_TILE_HEIGHT()/100.0), 0), Quaternion.identity);
						sprite1 = Tile1.GetComponent<SpriteRenderer>();
					}
				}
				else if (t.get_tile_sprite() == 2){
					if (i != t.get_height()-1){
						Destroy (((Transform)Instantiate(Tile2, new Vector3((float)t.get_x_pos(), (float)(t.get_y_pos()+i*tile_grid.get_TILE_HEIGHT()/100.0), 0), Quaternion.identity)).gameObject.GetComponent<PolygonCollider2D>());
						sprite1 = Tile2.GetComponent<SpriteRenderer>();
					}
					else {
						Instantiate(Tile2, new Vector3((float)t.get_x_pos(), (float)(t.get_y_pos()+i*tile_grid.get_TILE_HEIGHT()/100.0), 0), Quaternion.identity);
						sprite1 = Tile2.GetComponent<SpriteRenderer>();
					}
				}
				else if (t.get_tile_sprite() == 3){
					if (i != t.get_height()-1){
						Destroy (((Transform)Instantiate(Tile3, new Vector3((float)t.get_x_pos(), (float)(t.get_y_pos()+i*tile_grid.get_TILE_HEIGHT()/100.0), 0), Quaternion.identity)).gameObject.GetComponent<PolygonCollider2D>());
						sprite1 = Tile3.GetComponent<SpriteRenderer>();
					}
					else {
						Instantiate(Tile3, new Vector3((float)t.get_x_pos(), (float)(t.get_y_pos()+i*tile_grid.get_TILE_HEIGHT()/100.0), 0), Quaternion.identity);
						sprite1 = Tile3.GetComponent<SpriteRenderer>();
					}
				}
				else if (t.get_tile_sprite() == 4){
					if (i != t.get_height()-1){
						Destroy (((Transform)Instantiate(Tile4, new Vector3((float)t.get_x_pos(), (float)(t.get_y_pos()+i*tile_grid.get_TILE_HEIGHT()/100.0), 0), Quaternion.identity)).gameObject.GetComponent<PolygonCollider2D>());
						sprite1 = Tile4.GetComponent<SpriteRenderer>();
					}
					else {
						Instantiate(Tile4, new Vector3((float)t.get_x_pos(), (float)(t.get_y_pos()+i*tile_grid.get_TILE_HEIGHT()/100.0), 0), Quaternion.identity);
						sprite1 = Tile4.GetComponent<SpriteRenderer>();
					}
				}
				if (sprite1)
				{
					tile_number++;
					sprite1.sortingOrder = tile_number;

					//sprite.sortingOrder = tile_number;
					//Renderer.sortingOrder
				}
				if (sprite2)
				{
					tile_number++;
					sprite2.sortingOrder = tile_number;
					
					//sprite.sortingOrder = tile_number;
					//Renderer.sortingOrder
				}
				if (sprite3)
				{
					tile_number++;
					sprite3.sortingOrder = tile_number;
					
					//sprite.sortingOrder = tile_number;
					//Renderer.sortingOrder
				}
				if (sprite4)
				{
					tile_number++;
					sprite4.sortingOrder = tile_number;
					
					//sprite.sortingOrder = tile_number;
					//Renderer.sortingOrder
				}

			}

		}
		sprite1.sortingOrder = 0;
		sprite2.sortingOrder = 0;
		sprite3.sortingOrder = 0;
		sprite4.sortingOrder = 0;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
