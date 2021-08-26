using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Class for the Grid of Tiles that make up a Scenario's base. 
/// </summary>

[Serializable]
public class Tile_Grid : MonoBehaviour
{
    /// <summary>
    /// Constants:
    ///     File Locations:
    ///     static string OBJECT_SPRITESHEET_FILE - The spritesheet used to draw objects.
    ///     static string OBJECT_PREFAB_SRC - The source for the prefab used to create Objects.
    ///     static string EFFECT_PREFAB_SRC - The source for the prefab used to create Effects.
    ///     static string EMPTY_PREFAB_SRC - The source for the prefab used to separate rows of Tiles.
    ///     static string REACHABLE_PREFAB_SRC - The source for the prefab used to dentote what tiles are reachable. 
    ///     static string CHARACTER_PREFAB_SRC - The source for the prefab used to create Characters.
    /// 
    ///     Grid size restriction in number of tiles:
    ///     static int MAX_WIDTH - The maximum number of columns in the tile grid.
    ///     static int MAX_LENGTH - The maximum number of rows in the tile grid.
    ///     static int MAX_HEIGHT - The maximum height a Tile can be.
    ///     static int MIN_WIDTH - The minimum number of columns in the tile grid. 
    ///     static int MIN_LENGTH - The minimum number of Rows in the tile grid. 
    ///     static int MIN_HEIGHT - The lowest height a Tile can be.
    /// 
    ///     Starting coordinates for the grid:
    ///     static float START_X - Global X coordinate to start drawing the Tile Grid.
    ///     static float START_Y - Global Y coordinate to start drawing the Tile Grid.
    /// 
    ///     Scale for the tiles:
    ///     static float tile_scale - What to set the Scale of the Tile Prefab object. 
    /// 
    /// Variables:
    ///     Prefabs:
    ///     object_prefab - Used to generate the Objects on top of the Tiles.
    ///     hazard_prefab - Used to generate the Effects on top of the Tiles.
    ///     reachable_prefab - Used to generate the Reachable Tile Indicators.
    ///     character_prefab - Used to generate Characters on top of the Tiles. 
    ///     
    ///     Spritesheets:
    ///     Sprite[] object_sprite_sheet - used to draw Objects on the tile grid. 
    ///     
    ///     int grid_width - The width of the current tile grid
    ///     int grid_length { The length of the current tile grid
    ///     Material[] materials - the materials array used to replace the tile prefab materials. 
    ///     double[] modifiers - modifiers for the tiles.
    ///     int[,] tile_mat_ids - The materials of each individual tile in the grid.
    ///     int[,] object_sprite_ids - The ids for the sprites of Objects on top of the Tile Grid.
    ///     int[,] character_sprite_ids - The Ids of Characters on the Tile Grid.
    ///     int[,] tile_heights - The heights of tiles on the Tile Grid.
    ///     Transform[,] tiles - The actual Tile Objects that make up the Tile Grid.
    /// </summary>
    //Constants
    //File Locations
    private static string OBJECT_SPRITESHEET_FILE = "Sprites/Object Sprites/object_spritesheet_transparent";
    private static string OBJECT_PREFAB_SRC = "Prefabs/Scenario Prefabs/Object Prefabs/object_prefab";
    private static string HAZARD_PREFAB_SRC = "Prefabs/Scenario Prefabs/Hazard Prefabs/hazard_prefab";
    private static string EMPTY_PREFAB_SRC = "Prefabs/Scenario Prefabs/empty_prefab";
    private static string CHARACTER_PREFAB_SRC = "Prefabs/Character_Prefab/Character_Prefab";
    //Grid size restriction in number of tiles
    public static int MAX_WIDTH = 40;
    public static int MAX_LENGTH = 40;
    public static int MAX_HEIGHT = 30;
    public static int MIN_WIDTH = 5;
    public static int MIN_LENGTH = 5;
    public static int MIN_HEIGHT = 1;
    //Starting coordinates for the grid;
    private static float START_X = 0;
    private static float START_Y = 3.5f;
    //Scale for the tiles
    private static float tile_scale = 0.5f;
    public static float TILE_SCALE {
        get
        {
            return tile_scale;
        }
        private set
        {
            tile_scale = value;
        }
    }

    //Prefabs
    GameObject object_prefab;
    GameObject hazard_prefab;
    GameObject empty_prefab;
    GameObject reachable_prefab;
    GameObject character_prefab;

    //Spritesheets
    Sprite[] object_sprite_sheet;

    Material[] materials;
    double[] modifiers;
    [SerializeField] Tile[,] tiles;
    Stack<Tile> visited_tiles;

    public int Get_Width()
    {
        return tiles.GetLength(0);
    }
    public int Get_Length()
    {
        return tiles.GetLength(1);
    }
    public Tile Get_Tile(int x, int y)
    {
        Tile tile = null;
        try
        {
            tile = tiles[x,y];
        } catch (IndexOutOfRangeException e)
        {
            Debug.Log("Tile [" + x + "," + y + "] could not be found");
        }
        return tile;
    }

    /// <summary>
    /// Exports the current Tile_Grid as a Tile_Grid_Data object that can be serialized.
    /// </summary>
    /// <returns></returns>
    public Tile_Grid_Data Export_Data()
    {
        return new Tile_Grid_Data(this);
    }

    /// <summary>
    /// Prints the entire graph, edge by edge.
    /// </summary>
    public void Print_Graph()
    {
        foreach (Tile t in tiles)
        {
            t.printEdges();
        }
    }

    /// <summary>
    /// Add a Tile to the Tile_Grid
    /// </summary>
    /// <param name="tile">The Tile to add to the Tile_Grid</param>
    public void Add_Tile(Tile tile)
    {
        int x = tile.index[0];
        int y = tile.index[1];
        tiles[x, y] = tile;
        // Connect the new node to previous nodes in the mesh
        if (x > 0)
        {
            tile.addEdge(tiles[x-1,y], 3);
        }
        if (y > 0)
        {
            tile.addEdge(tiles[x,y-1], 0);
        }
    }

    /// <summary>
    /// Empty start method for the class.
    /// </summary>
    public void Start()
    {

    }

    /// <summary>
    /// Start Method for class. Loads prefabs and creates a generic grid that is x by y.
    /// </summary>
    /// <param name="width">The width of the new grid.</param>
    /// <param name="length">The height of the new grid.</param>
    public void Start(int width, int length)
    {
        //Load the prefabs
        object_prefab = Resources.Load(OBJECT_PREFAB_SRC, typeof(GameObject)) as GameObject;
        hazard_prefab = Resources.Load(HAZARD_PREFAB_SRC, typeof(GameObject)) as GameObject;
        empty_prefab = Resources.Load(EMPTY_PREFAB_SRC, typeof(GameObject)) as GameObject;
        character_prefab = Resources.Load(CHARACTER_PREFAB_SRC, typeof(GameObject)) as GameObject;

        //Load the spritesheets
        object_sprite_sheet = Resources.LoadAll<Sprite>(OBJECT_SPRITESHEET_FILE);

        tiles = new Tile[width,length];
        visited_tiles = new Stack<Tile>();

        Material material = (Material)Resources.Load("Objects/Materials/Tile_Materials/TileMat00");
        if (width > MAX_WIDTH)
        {
            width = MAX_WIDTH;
        }
        if(width < MIN_WIDTH)
        {
            width = MIN_WIDTH;
        }
        if (length > MAX_LENGTH)
        {
            length = MAX_LENGTH;
        }
        if (length < MIN_LENGTH)
        {
            length = MIN_LENGTH;
        }

        for (int x =0; x< width; x++)
        {
            GameObject empty = ((GameObject)Instantiate(empty_prefab,
                    new Vector3(0,
                        0,
                        0),
                    Quaternion.identity));

            //Set empty object as father for a row.
            empty.transform.parent = GameObject.FindGameObjectWithTag("Scenario").transform.GetChild(0);

            //Change the object name to be the tile index
            empty.name = "Row " + x;
            for (int y =0; y< length; y++)
            {
                //Instantiate the tile object.
                string file = "Objects/Tiles/" + 1 + "TCanvas";
                //string file = "Objects/Tiles/Object-3x3x" + tile_heights[x, y];

                GameObject tile3d = Resources.Load(file, typeof(GameObject)) as GameObject;
                int NEWSTARTX = 0;
                int NEWSTARTY = 0;
                int NEWSTARTZ = 0;
                float NEWTILEWIDTH = 1.5f;
                float NEWTILELENGTH = 1.5f;
                float NEWSCALE = 2f;
                //GameObject instance = ((GameObject)Instantiate(tile3d, new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y + XOFFSET), (float)(NEWSTARTY + YOFFSET), (float)(NEWSTARTZ - NEWTILELENGTH * x + ZOFFSET)), Quaternion.identity));
                GameObject instance = ((GameObject)Instantiate(tile3d,
                    new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y),
                        (float)(NEWSTARTY),
                        (float)(NEWSTARTZ - NEWTILELENGTH * x)),
                    Quaternion.identity));
                instance.transform.localScale = new Vector3(tile_scale, tile_scale, tile_scale);

                //Set the parent to be the Scenario object Tile_Grid
                instance.transform.parent = GameObject.FindGameObjectWithTag("Scenario").transform.GetChild(0).GetChild(x);

                //Change the object name to be the tile index
                instance.name = "[" + x + "," + y + "]";

                //Change the Tag to be a Tile
                instance.tag = "Tile";

                //Add a collider to the tile (TEMPORARY)
                BoxCollider collider = instance.AddComponent<BoxCollider>();
                //collider.size = new Vector3(NEWTILELENGTH*NEWSCALE,tile_heights[x,y]*NEWTILEHEIGHT*(NEWSCALE/2),NEWTILEWIDTH*NEWSCALE);
                collider.size = new Vector3(NEWTILELENGTH * NEWSCALE, 0, NEWTILEWIDTH * NEWSCALE);
                //collider.center = new Vector3(0, 10, 0);
                collider.center = new Vector3(0, 1, 0);

                //Set the material
                //instance.GetComponentInChildren<Renderer>().material = materials[tile_mat_ids[x, y] % 10];

                instance.GetComponentInChildren<Renderer>().material = material;

                //Generate the tile data for the tile
                instance.AddComponent<Tile>();
                instance.GetComponent<Tile>().Instantiate(x, y, 0, 0, 1, 0, new double[10]);

                //Store the instantiated tile in our Tile Tranform Grid;
                Add_Tile(instance.GetComponent<Tile>());
            }
        }
    }

    /// <summary>
    /// Reads a JSON string and returns a Character_Action class.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The Character_Action the JSON represents, or null if the json couldn't be parsed.</returns>
    public static Tile_Grid Parse_Json(string json)
    {
        Tile_Grid grid = JsonUtility.FromJson<Tile_Grid>(json);
        //Debug.Log("act name " + act.name);
        if (grid.tiles != null && grid.tiles.Length > 0)
        {
            return grid;
        }
        else
        {
            Debug.Log("Invalid JSON: " + json);
        }
        return null;

    }

    public void Start(string map_file)
    {
        object_prefab = Resources.Load(OBJECT_PREFAB_SRC, typeof(GameObject)) as GameObject;
        hazard_prefab = Resources.Load(HAZARD_PREFAB_SRC, typeof(GameObject)) as GameObject;
        empty_prefab = Resources.Load(EMPTY_PREFAB_SRC, typeof(GameObject)) as GameObject;
        character_prefab = Resources.Load(CHARACTER_PREFAB_SRC, typeof(GameObject)) as GameObject;

        //Load the spritesheets
        object_sprite_sheet = Resources.LoadAll<Sprite>(OBJECT_SPRITESHEET_FILE);

        Tile_Grid grid = Parse_Json(System.IO.File.ReadAllText(map_file));
        string json = JsonUtility.ToJson(grid);
        Debug.Log("Grid: " + json);
        //Debug.Log("Fields:" + action2.name);

        //tiles = new List<TileList>();
        tiles = new Tile[30, 30];
        visited_tiles = new Stack<Tile>();

        for( int x =0; x< grid.tiles.GetLength(0); x++)
        {
            //tiles.Add(new TileList());
            GameObject empty = ((GameObject)Instantiate(empty_prefab,
                    new Vector3(0,
                        0,
                        0),
                    Quaternion.identity));

            //Set empty object as father for a row.
            empty.transform.parent = Game_Controller.Get_Curr_Scenario().transform.GetChild(0);

            //Change the object name to be the tile index
            empty.name = "Row " + x;

            for( int y = 0; y< tiles.GetLength(1); y++)
            {
                Tile tile = grid.tiles[x, y];
                //Instantiate the tile object.
                string file = "Objects/Tiles/" + tile.height + "TCanvas";
                //string file = "Objects/Tiles/Object-3x3x" + tile_heights[x, y];

                GameObject tile3d = Resources.Load(file, typeof(GameObject)) as GameObject;
                int NEWSTARTX = 0;
                int NEWSTARTY = 0;
                int NEWSTARTZ = 0;
                float NEWTILEWIDTH = 1.5f;
                float NEWTILELENGTH = 1.5f;
                float NEWSCALE = 2f;
                //GameObject instance = ((GameObject)Instantiate(tile3d, new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y + XOFFSET), (float)(NEWSTARTY + YOFFSET), (float)(NEWSTARTZ - NEWTILELENGTH * x + ZOFFSET)), Quaternion.identity));
                GameObject instance = ((GameObject)Instantiate(tile3d,
                    new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y),
                        (float)(NEWSTARTY),
                        (float)(NEWSTARTZ - NEWTILELENGTH * x)),
                    Quaternion.identity));
                instance.transform.localScale = new Vector3(tile_scale, tile_scale, tile_scale);

                //Set the parent to be the Scenario object Tile_Grid
                instance.transform.parent = Game_Controller.Get_Curr_Scenario().transform.GetChild(0).GetChild(x);

                //Change the object name to be the tile index
                instance.name = "[" + x + "," + y + "]";

                //Change the Tag to be a Tile
                instance.tag = "Tile";

                //Add a collider to the tile (TEMPORARY)
                BoxCollider collider = instance.AddComponent<BoxCollider>();
                //collider.size = new Vector3(NEWTILELENGTH*NEWSCALE,tile_heights[x,y]*NEWTILEHEIGHT*(NEWSCALE/2),NEWTILEWIDTH*NEWSCALE);
                collider.size = new Vector3(NEWTILELENGTH * NEWSCALE, 0, NEWTILEWIDTH * NEWSCALE);
                //collider.center = new Vector3(0, 10, 0);
                collider.center = new Vector3(0, tile.height, 0);

                //Set the material
                //instance.GetComponentInChildren<Renderer>().material = materials[tile_mat_ids[x, y] % 10];
                //instance.GetComponentInChildren<Renderer>().material = materials[tile_mat_ids[x, y] % 10];

                //Generate the tile data for the tile
                instance.AddComponent<Tile>();
                //instance.GetComponent<Tile>().Instantiate(x, y, tile.height, tile_mat_ids[x, y], modifiers);

                //Store the instantiated tile in our Tile Tranform Grid;
                Add_Tile(instance.GetComponent<Tile>());

                //Create Objects on top of tiles
                if (tile.obj != null)
                {
                    GameObject prefab = object_prefab;
                    if (tile.obj.GetComponent<Character_Script>())
                    {
                        prefab = character_prefab;
                    }
                    //Calculate offsets
                    float height_offset = (prefab.GetComponent<SpriteRenderer>().sprite.rect.height *
                        prefab.transform.localScale.y /
                        prefab.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit /
                        2);
                    float offset = (height_offset) / 3.5f;

                    //Instantiate the prefab
                    GameObject obj = ((GameObject)Instantiate(prefab,
                        new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y),
                            (float)(NEWSTARTY + tile_scale * tile.height),
                            (float)(NEWSTARTZ - NEWTILELENGTH * x)),
                        Quaternion.identity));

                    if (tile.obj.GetComponent<Character_Script>())
                    {
                        //Set the character tile to the current tile
                        obj.GetComponent<Character_Script>().Set_Curr_Tile(tiles[x,y]);

                        //Set the parent to be the Scenario object Characters
                        obj.transform.parent = Game_Controller.Get_Curr_Scenario().transform.GetChild(3);
                    }
                    else
                    {
                        //pull up the object prefab
                        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();

                        //set the right sprite;
                        sprite.sprite = (Sprite)object_sprite_sheet[tile.obj.GetComponent<Object_Script>().sprite - 1];
                        //sprite.sortingOrder = tile_number;
                        sprite.sortingOrder = 5000;
                        //Set the character tile to the current tile
                        obj.GetComponent<Object_Script>().Set_Curr_Tile(tiles[x,y]);

                        //Set the parent to be the Scenario object Tile_Objects
                        obj.transform.parent = Game_Controller.Get_Curr_Scenario().transform.GetChild(1);

                        //Set the name to be the name of the object.
                        obj.name = obj.GetComponent<Object_Script>().obj_name;
                    }
                    //character.GetComponent<Character_Script>().character_id = character_ids[x, y];

                    //Set the tile object to that character and set traversible to false;
                    tiles[x,y].Set_Obj(obj);
                    tiles[x,y].Set_Traversible(false);
                }
                //Instantiate Tile Effects
                if (tile.hazard != null)
                {
                    //Calculate offsets
                    float height_offset = (hazard_prefab.GetComponent<SpriteRenderer>().sprite.rect.height *
                        hazard_prefab.transform.localScale.y /
                        hazard_prefab.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit /
                        2);
                    float offset = (height_offset) / 3.5f;

                    //Instantiate the prefab
                    GameObject hazard = ((GameObject)Instantiate(hazard_prefab,
                        new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y),
                            (float)(NEWSTARTY + tile_scale * tile.height),
                            (float)(NEWSTARTZ - NEWTILELENGTH * x)),
                        Quaternion.identity));
                }
                y++;
            }
            x++;
        }
    }

    /// <summary>
    /// Start method for class
    /// </summary>
    /// <param name="width">The width of the new Tile Grid</param>
    /// <param name="length">The length of the new Tile Grid</param>
    /// <param name="new_materials">The list of Materials used to make the Tiles.</param>
    /// <param name="new_modifiers">The modifiers for the tile types.</param>
    /// <param name="new_tile_mat_ids">The IDs for the tile types.</param>
    /// <param name="new_object_sprite_ids">The ids for objects to put on the Tile Grid.</param>
    /// <param name="new_character_sprite_ids">The ids for Character sprites to put on the Tile Grid.</param>
    /*public void Start(int width, int length, Material[] new_materials, double[] new_modifiers, int[,] new_tile_mat_ids, int[,] new_object_sprite_ids, int[,] new_character_ids)
    {
        //Load the prefabs
        object_prefab = Resources.Load(OBJECT_PREFAB_SRC, typeof(GameObject)) as GameObject;
        hazard_prefab = Resources.Load(EFFECT_PREFAB_SRC, typeof(GameObject)) as GameObject;
        empty_prefab = Resources.Load(EMPTY_PREFAB_SRC, typeof(GameObject)) as GameObject;
        reachable_prefab = Resources.Load(REACHABLE_PREFAB_SRC, typeof(GameObject)) as GameObject;
        character_prefab = Resources.Load(CHARACTER_PREFAB_SRC, typeof(GameObject)) as GameObject;

        //Load the spritesheets
        object_sprite_sheet = Resources.LoadAll<Sprite>(OBJECT_SPRITESHEET_FILE);

        //Set the variables
        grid_width = width;
        grid_length = length;
        modifiers = new_modifiers;
        materials = new_materials;
        tile_mat_ids = new_tile_mat_ids;
        object_sprite_ids = new_object_sprite_ids;
        character_ids = new_character_ids;
        tile_heights = new int[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                tile_heights[x, y] = tile_mat_ids[x, y] / 10 + 1;
            }
        }

        //Generate the Tile transforms and navmesh
        tiles = new Tile[width, length];
        navmesh = new Graph();
        idle = true;
    }*/



    /// <summary>
    /// Create a Tile Effect on a specific Tile.
    /// </summary>
    /// <param name="tile_obj">The GameObject on which to create the Hazard.</param>
    /// <param name="data">The data used to create the Hazard.</param>
    public void Create_Hazard(GameObject tile_obj, Hazard_Data data)
    {
        Tile tile = tile_obj.GetComponent<Tile>();
        if (tile != null) {
            //create Effects on top of tiles
            GameObject hazard_obj = (GameObject)Instantiate(hazard_prefab,
                    new Vector3((float)(tile_obj.transform.position.x),
                        (float)(tile_obj.transform.position.y + 0.66f),
                        (float)(tile_obj.transform.position.z)),
                    Quaternion.identity);
            Hazard hazard = hazard_obj.GetComponent<Hazard>();
            hazard.Instantiate(data);
            tile.Set_Hazard(hazard_obj);

        }
    }

    

    

}
