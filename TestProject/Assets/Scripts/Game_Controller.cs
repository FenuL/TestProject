using UnityEngine;
using System.Collections;

public class Game_Controller : MonoBehaviour {

	public string currentMap;
	public int currentPlayer;
	public int currentTurn;


	// Use this for initialization
	void Start () {
		currentMap = "Assets/Maps/tile_map.txt";
	}
	
	// Update is called once per frame
	void Update () {
		//player selection
		if(Input.GetKeyDown("1")){
			currentPlayer = 0;
		}else if(Input.GetKeyDown("2")){
			currentPlayer = 1;
		}else if(Input.GetKeyDown("3")){
			currentPlayer = 2;
		}else if(Input.GetKeyDown("4")){
			currentPlayer = 3;
		}
		//map selection
		if (Input.GetKeyDown ("space")) {
			if (currentMap == "Assets/Maps/falls_map.txt") {
				currentMap = "Assets/Maps/tile_map.txt";
			} else if (currentMap == "Assets/Maps/tile_map.txt"){
				currentMap = "Assets/Maps/falls_map.txt";
			}
		} 
	}
}
