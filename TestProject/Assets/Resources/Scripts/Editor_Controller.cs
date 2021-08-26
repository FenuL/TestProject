using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Editor_Controller : MonoBehaviour {

    private static string EDITING_PREFAB_SRC = "Prefabs/Scenario Prefabs/Tile Prefabs/Reachable";
    private static string SCENARIO_FILEPATH = "Assets/Resources/Scenarios/";

    private Scenario scenario;
    private List<Tile> tiles;
    private List<GameObject> editing_tile_objects;

    private GameObject editing_prefab;

    public Scenario Get_Scenario()
    {
        return scenario;
    }
    public Tile Get_Tile()
    {
        return tiles[0];
    }
    public List<Tile> Get_Tiles()
    {
        return tiles;
    }

    public void Add_Tile(Tile t)
    {
        if (!tiles.Contains(t))
        {
            tiles.Add(t);
            GameObject obj = (GameObject)Instantiate(editing_prefab, new Vector3(t.transform.position.x,
                                                                   //tile.position.y+0.08f,
                                                                   //(float)(tile.position.y + (tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) - .24f,
                                                                   t.transform.position.y + (-Scenario.START_Y)+ .015f,
                                                                   t.transform.position.z),
                                                                   Quaternion.identity);
            editing_tile_objects.Add(obj);

        }
    }

    public void Mark_Tiles()
    {
        foreach (Tile t in tiles)
        {
            GameObject obj = (GameObject)Instantiate(editing_prefab, new Vector3(t.transform.position.x,
                                                                   //tile.position.y+0.08f,
                                                                   //(float)(tile.position.y + (tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) - .24f,
                                                                   t.transform.position.y + (-Scenario.START_Y) +.015f,
                                                                   t.transform.position.z),
                                                                   Quaternion.identity);
            editing_tile_objects.Add(obj);
        }
    }

    public void Clear_Tiles()
    {
        foreach (GameObject game_object in editing_tile_objects)
        {
            Destroy(game_object);
        }
        editing_tile_objects = new List<GameObject>();
    }

    public void Reset_Tiles()
    {
        foreach (GameObject game_object in editing_tile_objects)
        {
            Destroy(game_object);
        }
        editing_tile_objects = new List<GameObject>();
        tiles = new List<Tile>();
    }

	// Use this for initialization
	void Start () {
        //UI_Controller.Get_Controller().Start(Scenes.Editor);
        editing_prefab = Resources.Load(EDITING_PREFAB_SRC, typeof(GameObject)) as GameObject;
        editing_tile_objects = new List<GameObject>();
        scenario = (GameObject.FindGameObjectWithTag("Scenario") as GameObject).GetComponent<Scenario>();
        Game_Controller.controller.Set_Editor_Controller(this);
        tiles = new List<Tile>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Update_Fields(int value, GameObject inspector)
    {
        //Scenario Editor
        if (value == 0)
        {
            InputField[] fields = inspector.GetComponentsInChildren<InputField>();
            Dropdown[] dropdowns = inspector.GetComponentsInChildren<Dropdown>();
            /*int x = 0;
            foreach (InputField i in fields)
            {
                Debug.Log("index " + x + " " + fields[x].text);
                x++;
            }*/
            scenario.Set_Id(fields[0].text);
            scenario.Set_Name(fields[1].text);
            scenario.Set_Sector(fields[2].text);
            scenario.Set_Description(fields[3].text);
            scenario.Set_Objective(dropdowns[0].value);
            //TODO import the rest of these things
        }

    }

    public void Export(int value)
    {
        //Scenario Editor
        if(value == 0)
        {
            Save_Scenario_Data();
        }
    }

    public void Save_Scenario_Data()
    {
        Scenario_Data data = scenario.Export_Data();
        if (data != null)
        {
             StartCoroutine(Game_Controller.Serialize_To_File<Scenario_Data>(data, SCENARIO_FILEPATH + data.scenario_id + "-" + data.scenario_name));
        }
    }
}
