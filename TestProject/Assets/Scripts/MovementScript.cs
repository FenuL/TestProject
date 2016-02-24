using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour{

	// Use this for initialization
	void Start () {
	}
	public int PlayerNum;
	public string PlayerName;
	public int PlayerHealth;
	public int PlayerMana;
	public int Strength;
	public int Coordination;
	public int Spirit;
	public int Dexterity;
	public int Vitality;
	public int level;
	public string profession;
	public Equipment[] Equipment;
	public GameObject controller;
	public GameObject Tile_Grid;
	public Draw_Tile_Grid script;
	private TileData data;
	private Tile[,,] tiles;
	private Transform clickedTile;
	private Transform currentTile;
	private Transform selectedTile;
	private bool initialized = false;
	//public SpriteRenderer renderer;

	// Update is called once per frame
	void Update () {
		if (currentTile == null || selectedTile == null || clickedTile == null) {
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
			data = hit.transform.GetComponent<TileData>();
			print ("x: " + data.x_index + ", y: " + data.y_index + ", z: " + data.z_index);
			//if (currentTile != selectedTile){
			selectedTile.GetComponent<SpriteRenderer> ().color = new Color(255f, 255f, 255f, 1f);
			selectedTile = hit.transform;

			selectedTile.GetComponent<SpriteRenderer> ().color = new Color(0f, 0f, 0f, 1f); // Set to opaque black
				//print("test");
			//}
			//Vector3 worldPos = hit.point;
			//print("x="+ worldPos.x + ", y=" + worldPos.y);
		}

		if(Input.GetMouseButton(0)){
			//print("test2");
			clickedTile.GetComponent<SpriteRenderer> ().color=new Color(255f, 255f, 255f, 1f); // Set to white
			clickedTile = selectedTile;
			clickedTile.GetComponent<SpriteRenderer> ().color=new Color(255f, 0f, 0f, 1f); // Set to blue
			if(data.traversible){
				if(PlayerNum == controller.GetComponent<Game_Controller>().currentPlayer){
					currentTile.GetComponent<TileData>().traversible = true;
					currentTile = clickedTile;
					currentTile.GetComponent<TileData>().traversible = false;
					transform.position = new Vector3(currentTile.position.x, currentTile.position.y+(float)(transform.GetComponent<SpriteRenderer> ().sprite.rect.height/transform.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit + 0.15f), currentTile.position.z); //script.tileGrid.TILE_LENGTH+script.tileGrid.TILE_HEIGHT)/200.0), currentTile.position.z);
					//renderer = (SpriteRenderer)currentTile.GetComponent<SpriteRenderer> ();
					transform.GetComponent<SpriteRenderer> ().sortingOrder = currentTile.GetComponent<SpriteRenderer> ().sortingOrder + 1;
				}
			}

		}



	}

	void Initialize(){
		script = Tile_Grid.GetComponent<Draw_Tile_Grid>();
		tiles = script.tileGrid.getTiles ();
		currentTile = tiles[0, 0, 2].getObj ();
		selectedTile = tiles [0, 0, 2].getObj ();
		clickedTile = tiles[0, 0, 2].getObj ();


	}
}
