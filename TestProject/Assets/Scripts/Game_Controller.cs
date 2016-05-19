using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Game_Controller : MonoBehaviour {

	public static Game_Controller controller;
	
	public string curr_map;
	public int curr_character_num;
	public GameObject[] characters;
	public GameObject curr_player;
	public GameObject cursor;
	public GameObject tile_grid;
	public Tile_Data tile_data;
	public Transform[,] tiles;
	public Transform clicked_tile;
	public Transform selected_tile;
	public bool initialized = false;
	public int curr_turn;
	public Transform reachable_tile_prefab;

	public void Save(){
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/game_data.dat");

		GameData data = new GameData();
		data.curr_map = curr_map;
		data.curr_character_num = curr_character_num;
		data.characters = characters;
		data.curr_player = curr_player;
		data.cursor = cursor;
		data.tile_grid = tile_grid;
		data.tile_data = tile_data;
		data.tiles = tiles;
		data.clicked_tile = clicked_tile;
		data.selected_tile = selected_tile;
		data.initialized = initialized;
		data.curr_turn = curr_turn;
		formatter.Serialize (file, data);
		file.Close ();

	}

	public void Load(){
		if (File.Exists (Application.persistentDataPath + "/game_data.dat")) {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/game_data.dat", FileMode.Open);
			GameData data = (GameData) formatter.Deserialize(file);
			curr_map = data.curr_map;
			curr_character_num = data.curr_character_num;
			characters = data.characters;
			curr_player = data.curr_player;
			cursor = data.cursor;
			tile_grid = data.tile_grid;
			tile_data = data.tile_data;
			tiles = data.tiles;
			clicked_tile = data.clicked_tile;
			selected_tile = data.selected_tile;
			initialized = data.initialized;
			curr_turn = data.curr_turn;
			formatter.Serialize (file, data);

			file.Close();
		}
	}

	public void NextPlayer(){
		curr_character_num = curr_character_num + 1;
		if(curr_character_num >= 5){
			curr_character_num = 0;
		}

		foreach (GameObject game_object in characters) {
			if(game_object.GetComponent<Character_Script>().character_num == curr_character_num){
				curr_player.GetComponent<Animator>().SetBool("Selected", false);
				curr_player = game_object; 
				curr_player.GetComponent<Animator>().SetBool("Selected", true);

			}
		}
		//curr_player.GetComponent<Character_Script>().FindReachable(tile_grid);
		CleanReachable ();
		//MarkReachable ();

	}

	public void PrevPlayer(){
		curr_character_num = curr_character_num - 1;
		if(curr_character_num < 0) {
			curr_character_num = 4;
		}

		foreach (GameObject game_object in characters) {
			if(game_object.GetComponent<Character_Script>().character_num == curr_character_num){
				curr_player.GetComponent<Animator>().SetBool("Selected", false);
				curr_player = game_object; 
				curr_player.GetComponent<Animator>().SetBool("Selected", true);
				
			}
		}
		//curr_player.GetComponent<Character_Script>().FindReachable(tile_grid);
		CleanReachable ();
		//MarkReachable ();
		
	}

	void Awake (){
		if (controller == null) {
			DontDestroyOnLoad (gameObject);
			controller = this;
		} else if (controller != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		int x = 0;
		curr_map = "Assets/Maps/tile_map.txt";
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject game_object in objects) {
			game_object.GetComponent<Character_Script>().character_num = x;
			game_object.GetComponent<Character_Script>().Randomize();
			if(game_object.GetComponent<Character_Script>().character_num == curr_character_num){
				curr_player = game_object; 
				curr_player.GetComponent<Animator>().SetBool("Selected", true);
			}
			x++;
		}
		objects = GameObject.FindGameObjectsWithTag ("Monster");
		//print (objects.Length);
		foreach (GameObject game_object in objects) {
			game_object.GetComponent<Character_Script>().character_num = x; 
			game_object.GetComponent<Character_Script>().Randomize();
			if(game_object.GetComponent<Character_Script>().character_num == curr_character_num){
				curr_player = game_object; 
				curr_player.GetComponent<Animator>().SetBool("Selected", true);
			}
			x++;
		}
		cursor = GameObject.FindGameObjectWithTag ("Cursor");
		x = 0;
		//MarkReachable ();
	}
	
	// Update is called once per frame
	void Update () {
		//player selection
		if(Input.GetKeyDown("1")){
			curr_character_num = 0;
		}else if(Input.GetKeyDown("2")){
			curr_character_num = 1;
		}else if(Input.GetKeyDown("3")){
			curr_character_num = 2;
		}else if(Input.GetKeyDown("4")){
			curr_character_num = 3;
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
			tile_data = hit.transform.GetComponent<Tile_Data>();
			//print ("x: " + data.x_index + ", y: " + data.y_index + ", z: " + data.z_index);
			//if (curr_tile != selected_tile){
			//selected_tile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
			selected_tile = hit.transform;
			cursor.transform.position = new Vector3 ((float)(selected_tile.position.x+selected_tile.GetComponent<SpriteRenderer>().sprite.rect.width/200)-.85f, 
			                                         (float)(selected_tile.position.y + (selected_tile.GetComponent<SpriteRenderer>().sprite.rect.height)/100),
			                                            selected_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
			//renderer = (SpriteRenderer)curr_tile.GetComponent<SpriteRenderer> ();
			cursor.GetComponent<SpriteRenderer> ().sortingOrder = selected_tile.GetComponent<SpriteRenderer> ().sortingOrder + 1;

			
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
			//if (tile_data.traversible) {
				foreach (Transform tile in curr_player.GetComponent<Character_Script>().reachable_tiles){
					if (tile.Equals(clicked_tile)){
						if (curr_player.GetComponent<Character_Script>().state == "Moving"){
							curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data> ().traversible = true;
							curr_player.GetComponent<Character_Script>().curr_tile = clicked_tile;
							curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data> ().traversible = false;
							if (curr_player.CompareTag("Player")) { 
								curr_player.transform.position = new Vector3 (curr_player.GetComponent<Character_Script>().curr_tile.position.x, 
						            	                                  //curr_player.GetComponent<Character_Script>().curr_tile.position.y + (float)(curr_player.GetComponent<SpriteRenderer> ().sprite.rect.height / curr_player.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit + 0.15f),
							    	                                      (float)(curr_player.GetComponent<Character_Script>().curr_tile.position.y + (curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height)/100)+ 0.15f,
						    	                                          curr_player.GetComponent<Character_Script>().curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
							}
							else {
								curr_player.transform.position = new Vector3 (curr_player.GetComponent<Character_Script>().curr_tile.position.x, 
								                                              (float)(curr_player.GetComponent<Character_Script>().curr_tile.position.y + (curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height)/100)+ 0.15f,
							    	                                          //curr_player.GetComponent<Character_Script>().curr_tile.position.y+0.5f,
								                                              curr_player.GetComponent<Character_Script>().curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
							}
							//renderer = (SpriteRenderer)curr_tile.GetComponent<SpriteRenderer> ();
							curr_player.GetComponent<SpriteRenderer> ().sortingOrder = curr_player.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer> ().sortingOrder+1;

							//display possible movement tiles.
							//if (character_num == controller.GetComponent<Game_Controller> ().curr_player) {
						}
						if (curr_player.GetComponent<Character_Script>().state == "Attacking"){
							foreach (GameObject character in characters) {
								if (character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data> ().x_index  == clicked_tile.GetComponent<Tile_Data>().x_index &&
								    character.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data> ().y_index  == clicked_tile.GetComponent<Tile_Data>().y_index){
									character.GetComponent<Character_Script>().aura_curr -= curr_player.GetComponent<Character_Script>().strength;
								}
							}
						}
						NextPlayer ();
					}
				}

			//}
		}

		if (Input.GetMouseButtonUp (0)) {
			cursor.GetComponent<Animator>().SetBool("Clicked", false);
		}

		//Next player button
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			NextPlayer();
		}

		//Prev player button
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			PrevPlayer();
		}

	}

	void Initialize(){
		tiles = tile_grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTiles ();
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject game_object in objects) {
			game_object.GetComponent<Character_Script>().curr_tile = tile_grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(game_object.GetComponent<Character_Script>().character_num,0);
			game_object.transform.position = new Vector3 (game_object.GetComponent<Character_Script>().curr_tile.position.x, 
		         //game_object.GetComponent<Character_Script>().curr_tile.position.y + (float)(game_object.GetComponent<SpriteRenderer> ().sprite.rect.height / game_object.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit + 0.15f),
			     (float)(game_object.GetComponent<Character_Script>().curr_tile.position.y + (game_object.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height)/100)+ 0.15f,
			     game_object.GetComponent<Character_Script>().curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
			game_object.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().traversible = false;
			//game_object.GetComponent<Character_Script>().FindReachable(tile_grid);
		}
		objects = GameObject.FindGameObjectsWithTag ("Monster");
		foreach (GameObject game_object in objects) {
			game_object.GetComponent<Character_Script>().curr_tile = tile_grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(19-game_object.GetComponent<Character_Script>().character_num,19);// [19-game_object.GetComponent<Character_Script>().character_num,19,0];
			game_object.transform.position = new Vector3 (game_object.GetComponent<Character_Script>().curr_tile.position.x, 
			                                              //game_object.GetComponent<Character_Script>().curr_tile.position.y + 0.5f,
			                                              (float)(game_object.GetComponent<Character_Script>().curr_tile.position.y + (game_object.GetComponent<Character_Script>().curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height)/100)+ 0.15f,
			                                              game_object.GetComponent<Character_Script>().curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
			game_object.GetComponent<Character_Script>().curr_tile.GetComponent<Tile_Data>().traversible = false;
			//game_object.GetComponent<Character_Script>().FindReachable(tile_grid);
		}
		selected_tile = tiles [0, 0];
		clicked_tile = tiles[0, 0];
		//CleanReachable ();
		MarkReachable ();
		//transform.position = new Vector3(curr_tile.position.x, curr_tile.position.y+(float)(transform.GetComponent<SpriteRenderer> ().sprite.rect.height/transform.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit + 0.15f), curr_tile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), curr_tile.position.z);
			
	}

	public void MarkReachable(){
		foreach (Transform tile in curr_player.GetComponent<Character_Script> ().reachable_tiles){
			reachable_tile_prefab.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
			if (curr_player.GetComponent<Character_Script>().state == "Moving"){
				reachable_tile_prefab.GetComponent<SpriteRenderer>().color=new Color(0,0,255);
			}
			if (curr_player.GetComponent<Character_Script>().state == "Attacking"){
				reachable_tile_prefab.GetComponent<SpriteRenderer>().color=new Color(255,255,0);
			}
			Instantiate(reachable_tile_prefab, new Vector3(tile.position.x,
			                                               //tile.position.y+0.08f,
			                                               (float)(tile.position.y + (tile.GetComponent<SpriteRenderer>().sprite.rect.height)/100)-.24f,
			                                               tile.position.z),
			                                               Quaternion.identity);

			//+ tile.GetComponent<SpriteRenderer>().sprite.rect.width/200
			// + selected_tile.GetComponent<SpriteRenderer>().sprite.rect.height/200
		}

	}

	public void CleanReachable(){
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Reachable");
		foreach (GameObject game_object in objects) {
			Destroy(game_object);
		}
	}

	void OnApplicationQuit() {
		reachable_tile_prefab.GetComponent<SpriteRenderer> ().sortingOrder = 0;
	}

}

[Serializable]
class GameData
{
	public string curr_map;
	public int curr_character_num;
	public GameObject[] characters;
	public GameObject curr_player;
	public GameObject cursor;
	public GameObject tile_grid;
	public Tile_Data tile_data;
	public Transform[,] tiles;
	public Transform clicked_tile;
	public Transform selected_tile;
	public bool initialized;
	public int curr_turn;
}
