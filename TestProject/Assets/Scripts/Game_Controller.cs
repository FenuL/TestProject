using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Controller : MonoBehaviour {

	public string curr_map;
	public int curr_player_num;
	public GameObject curr_player;
	public GameObject cursor;
	public GameObject tile_grid;
	public Tile_Data data;
	public Transform[,,] tiles;
	public Transform clicked_tile;
	public Transform selected_tile;
	public bool initialized = false;
	public int curr_turn;
	public Transform reachable_tile_prefab;


	public void NextPlayer(){
		curr_player_num = curr_player_num + 1;
		if(curr_player_num >= 4){
			curr_player_num = 0;
		}
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject game_object in objects) {
			if(game_object.GetComponent<Player_Script>().player_num == curr_player_num){
				curr_player.GetComponent<Animator>().SetBool("Selected", false);
				curr_player = game_object; 
				curr_player.GetComponent<Animator>().SetBool("Selected", true);

			}
		}
		curr_player.GetComponent<Player_Script>().FindReachable(tile_grid);
		CleanReachable ();
		MarkReachable ();

	}

	// Use this for initialization
	void Start () {
		curr_map = "Assets/Maps/tile_map.txt";
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject game_object in objects) {
			if(game_object.GetComponent<Player_Script>().player_num == curr_player_num){
				curr_player = game_object; 
				curr_player.GetComponent<Animator>().SetBool("Selected", true);
			}
		}
		cursor = GameObject.FindGameObjectWithTag ("Cursor");
	}
	
	// Update is called once per frame
	void Update () {
		//player selection
		if(Input.GetKeyDown("1")){
			curr_player_num = 0;
		}else if(Input.GetKeyDown("2")){
			curr_player_num = 1;
		}else if(Input.GetKeyDown("3")){
			curr_player_num = 2;
		}else if(Input.GetKeyDown("4")){
			curr_player_num = 3;
		}
		//map selection
		if (Input.GetKeyDown ("space")) {
			if (curr_map == "Assets/Maps/falls_map.txt") {
				curr_map = "Assets/Maps/tile_map.txt";
			} else if (curr_map == "Assets/Maps/tile_map.txt"){
				curr_map = "Assets/Maps/falls_map.txt";
			}
		} 

		if (selected_tile == null || clicked_tile == null) {
			initialized = false;
		}
		if (! initialized) {
			Initialize ();
			initialized = true;
		}
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit2D hit;
		
		/*if (Physics.Raycast (ray, out hit)) {
			// if the ray hit a collider, we'll get the world-coordinate here.
			Vector3 worldPos = hit.point;
			print("x="+ worldPos.x + ", y=" + worldPos.y);
		}*/
		hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		
		//hit: RaycastHit2D = Physics2D.Raycast(transform.position, -Vector2.up);
		//hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity, 0);
		if (hit){
			data = hit.transform.GetComponent<Tile_Data>();
			//print ("x: " + data.x_index + ", y: " + data.y_index + ", z: " + data.z_index);
			//if (curr_tile != selected_tile){
			//selected_tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
			selected_tile = hit.transform;
			cursor.transform.position = new Vector3 ((float)(selected_tile.position.x+selected_tile.GetComponent<SpriteRenderer>().sprite.rect.width/200)-.85f, 
			                                         (float)(selected_tile.position.y + (selected_tile.GetComponent<SpriteRenderer>().sprite.rect.height)/200),
			                                            selected_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
			//renderer = (SpriteRenderer)curr_tile.GetComponent<SpriteRenderer> ();
			cursor.GetComponent<SpriteRenderer> ().sortingOrder = selected_tile.GetComponent<SpriteRenderer> ().sortingOrder + 2;

			
			//selected_tile.GetComponent<SpriteRenderer> ().color = new Color(0f, 0f, 0f, 1f); // Set to opaque black
			//print("test");
			//}
			//Vector3 worldPos = hit.point;
			//print("x="+ worldPos.x + ", y=" + worldPos.y);
		}
		
		
		
		if (Input.GetMouseButton (0)) {
			cursor.GetComponent<Animator>().SetBool("Clicked", true);
			//print("test2");
			//clicked_tile.GetComponent<SpriteRenderer> ().color = new Color (255f, 255f, 255f, 1f); // Set to white
			clicked_tile = selected_tile;
			//clicked_tile.GetComponent<SpriteRenderer> ().color = new Color (255f, 0f, 0f, 1f); // Set to blue
			if (data.traversible) {
				foreach (Transform tile in curr_player.GetComponent<Player_Script>().reachable_tiles){
					if (tile.Equals(clicked_tile)){
						curr_player.GetComponent<Player_Script>().curr_tile.GetComponent<Tile_Data> ().traversible = true;
						curr_player.GetComponent<Player_Script>().curr_tile = clicked_tile;
						curr_player.GetComponent<Player_Script>().curr_tile.GetComponent<Tile_Data> ().traversible = false;
						curr_player.transform.position = new Vector3 (curr_player.GetComponent<Player_Script>().curr_tile.position.x, 
						                                              curr_player.GetComponent<Player_Script>().curr_tile.position.y + (float)(curr_player.GetComponent<SpriteRenderer> ().sprite.rect.height / curr_player.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit + 0.15f),
						                                              curr_player.GetComponent<Player_Script>().curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
						//renderer = (SpriteRenderer)curr_tile.GetComponent<SpriteRenderer> ();
						curr_player.GetComponent<SpriteRenderer> ().sortingOrder = curr_player.GetComponent<Player_Script>().curr_tile.GetComponent<SpriteRenderer> ().sortingOrder + 5;

						//display possible movement tiles.
						//if (player_num == controller.GetComponent<Game_Controller> ().curr_player) {
						
						NextPlayer ();
					}
				}

			}
		}

		if (Input.GetMouseButtonUp (0)) {
			cursor.GetComponent<Animator>().SetBool("Clicked", false);
		}
	}

	void Initialize(){
		tiles = tile_grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTiles ();
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject game_object in objects) {
			game_object.GetComponent<Player_Script>().curr_tile = tiles[game_object.GetComponent<Player_Script>().player_num,0,2];
			game_object.transform.position = new Vector3 (game_object.GetComponent<Player_Script>().curr_tile.position.x, 
		         game_object.GetComponent<Player_Script>().curr_tile.position.y + (float)(game_object.GetComponent<SpriteRenderer> ().sprite.rect.height / game_object.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit + 0.15f),
		         game_object.GetComponent<Player_Script>().curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
			game_object.GetComponent<Player_Script>().curr_tile.GetComponent<Tile_Data>().traversible = false;
			game_object.GetComponent<Player_Script>().FindReachable(tile_grid);
		}
		selected_tile = tiles [0, 0, 2];
		clicked_tile = tiles[0, 0, 2];
		MarkReachable ();
		//transform.position = new Vector3(curr_tile.position.x, curr_tile.position.y+(float)(transform.GetComponent<SpriteRenderer> ().sprite.rect.height/transform.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit + 0.15f), curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
			
	}

	void MarkReachable(){
		foreach (Transform tile in curr_player.GetComponent<Player_Script> ().reachable_tiles){
			reachable_tile_prefab.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
			Instantiate(reachable_tile_prefab, new Vector3(tile.position.x,
			                                               tile.position.y+0.08f,
			                                               tile.position.z),
			            Quaternion.identity);
			//+ tile.GetComponent<SpriteRenderer>().sprite.rect.width/200
			// + selected_tile.GetComponent<SpriteRenderer>().sprite.rect.height/200
		}

	}

	void CleanReachable(){
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Reachable");
		foreach (GameObject game_object in objects) {
			Destroy(game_object);
		}
	}

	void OnApplicationQuit() {
		reachable_tile_prefab.GetComponent<SpriteRenderer> ().sortingOrder = 0;
	}

}
