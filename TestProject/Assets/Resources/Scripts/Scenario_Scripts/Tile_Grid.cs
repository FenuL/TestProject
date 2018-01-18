using UnityEngine;
using System.Collections;

/// <summary>
/// Class for the Grid of Tiles that make up a Scenario's base. 
/// </summary>
public class Tile_Grid : ScriptableObject
{
    /// <summary>
    /// Constants:
    ///     File Locations:
    ///     static string OBJECT_SPRITESHEET_FILE - The spritesheet used to draw objects.
    ///     static string OBJECT_PREFAB_SRC - The source for the prefab used to create Objects.
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
    ///     Graph navmesh - The Nav Mesh used to navigate the Tile Grid.
    /// </summary>
    //Constants
    //File Locations
    private static string OBJECT_SPRITESHEET_FILE = "Sprites/Object Sprites/object_spritesheet_transparent";
    private static string OBJECT_PREFAB_SRC = "Prefabs/Object Prefabs/object_prefab";
    private static string CHARACTER_PREFAB_SRC = "Prefabs/Character_Prefab/Character_Prefab";
    private static string REACHABLE_PREFAB_SRC = "Prefabs/Tile Prefabs/Reachable";
    //Grid size restriction in number of tiles
    private static int MAX_WIDTH = 40;
    private static int MAX_LENGTH = 40;
    private static int MAX_HEIGHT = 30;
    private static int MIN_WIDTH = 5;
    private static int MIN_LENGTH = 5;
    private static int MIN_HEIGHT = 1;
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
    public GameObject object_prefab { get; private set; }
    public GameObject reachable_prefab { get; private set; }
    public GameObject character_prefab { get; private set; }

    //Spritesheets
    Sprite[] object_sprite_sheet;

    public int grid_width { get; private set; }
    public int grid_length { get; private set; }
    private Material[] materials;
    private double[] modifiers;
    public int[,] tile_mat_ids { get; private set; }
    public int[,] object_sprite_ids { get; private set; }
    public int[,] character_ids { get; private set; }
    public int[,] tile_heights { get; private set; }
    public Transform[,] tiles { get; private set; }
    public Graph navmesh { get; private set; }

    /// <summary>
    /// Constructor for the class.
    /// </summary>
    /// <param name="width">The width of the new Tile Grid</param>
    /// <param name="length">The length of the new Tile Grid</param>
    /// <param name="new_materials">The list of Materials used to make the Tiles.</param>
    /// <param name="new_modifiers">The modifiers for the tile types.</param>
    /// <param name="new_tile_mat_ids">The IDs for the tile types.</param>
    /// <param name="new_object_sprite_ids">The ids for objects to put on the Tile Grid.</param>
    /// <param name="new_character_sprite_ids">The ids for Character sprites to put on the Tile Grid.</param>
    public Tile_Grid(int width, int length, Material[] new_materials, double[] new_modifiers, int[,] new_tile_mat_ids, int[,] new_object_sprite_ids, int[,] new_character_ids)
    {
        //Load the prefabs
        object_prefab = Resources.Load(OBJECT_PREFAB_SRC, typeof(GameObject)) as GameObject;
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
        tiles = new Transform[width, length];
        navmesh = new Graph();
    }

    /// <summary>
    /// Deprecated Coroutine for animating the transition for the Elevate process.
    /// </summary>
    /*public IEnumerator Raise()
    {
        float elapsedTime = 0;
        float duration = .3f;
        Vector3 start = transform.position;
        Vector3 target = curr_tile.position + camera_position_offset + new Vector3(0, height_offset + Tile_Grid.tile_scale * (curr_tile.GetComponent<Tile>().height), 0);
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start,
                target,
                elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }*/


    /// <summary>
    /// Function for Raising/Lowering tiles.
    /// </summary>
    /// <param name="target">The target tile to Raise/Lower</param>
    /// <param name="height">The height to change the tile.</param>
    public void Elevate(Transform target, int height)
    {
        //Modify tile height
        Tile tile = target.GetComponent<Tile>();
        tile.height += height;
        tile_heights[tile.index[0], tile.index[1]] += height;

        if (tile.height < 1)
        {
            tile.height = 1;
        }
        if (tile.height > 30)
        {
            tile.height = 30;
        }

        //Modify object
        string file = "Objects/Tiles/" + tile.height + "TCanvas";
        GameObject tile3d = Resources.Load(file, typeof(GameObject)) as GameObject;
        float NEWTILEWIDTH = 1.5f;
        float NEWTILELENGTH = 1.5f;
        float NEWSCALE = 2f;
        GameObject instance;
        //If the shift is positive, we create the tile lower and raise it up.
        if (height > 0)
        {
            instance = ((GameObject)Instantiate(tile3d, target.position - new Vector3(), Quaternion.identity));
            instance.transform.localScale = new Vector3(tile_scale, tile_scale, tile_scale);
            if (tile.obj != null)
            {
                if (tile.obj.GetComponent<Character_Script>() != null)
                {
                    tile.obj.transform.position = instance.transform.position + tile.obj.GetComponent<Character_Script>().camera_position_offset + new Vector3(0, tile_scale * tile.height + tile.obj.GetComponent<Character_Script>().height_offset, 0);
                    tile.obj.GetComponent<Character_Script>().curr_tile = instance.transform;
                }
            }
        }
        else
        {
            //if the shift is negative, we create the tile higher and lower it. 
            instance = ((GameObject)Instantiate(tile3d, target.position + new Vector3(), Quaternion.identity));
            instance.transform.localScale = new Vector3(tile_scale, tile_scale, tile_scale);
            if (tile.obj != null)
            {

                tile.obj.transform.position = instance.transform.position + tile.obj.GetComponent<Character_Script>().camera_position_offset + new Vector3(0, tile_scale * tile.height + tile.obj.GetComponent<Character_Script>().height_offset, 0);
                tile.obj.GetComponent<Character_Script>().curr_tile = instance.transform;
                //instance.transform.GetComponent<Tile>().obj = tile.obj;
                //Debug.Log(instance.transform.GetComponent<Tile>().obj.GetComponent<Character_Script>().name);
            }
        }

        //Add a collider to the tile (TEMPORARY)
        BoxCollider collider = instance.AddComponent<BoxCollider>();
        collider.size = new Vector3(NEWTILELENGTH * NEWSCALE, 0, NEWTILEWIDTH * NEWSCALE);
        collider.center = new Vector3(0, tile.height, 0);

        //Change material
        instance.GetComponentInChildren<Renderer>().material = materials[tile_mat_ids[tile.index[0], tile.index[1]] % 10];

        //Generate the tile data for the tile
        instance.AddComponent<Tile>();
        instance.GetComponent<Tile>().Instantiate(tile);

        //Store the instantiated tile in our Tile Tranform Grid;
        tiles[tile.index[0], tile.index[1]] = instance.transform;

        //Modify navmesh
        foreach (Edge e in instance.GetComponent<Tile>().edges)
        {
            //Modify local edges
            if (e != null)
            {
                e.Update_Cost(e.tile2);

                //Debug.Log("Edge between (" + e.tile1.index[0] + "," + e.tile1.index[1] + ") and (" + e.tile2.index[0] + "," + e.tile2.index[1] + ") has cost " + e.cost);
                //modify edges of adjacent nodes
                foreach (Edge edge in e.tile2.edges)
                {
                    if (edge != null)
                    {
                        if (edge.tile2.Equals(e.tile1))
                        {
                            edge.Update_Cost(e.tile1);
                            //edge.tile2 = e.tile1;
                            //Debug.Log("Edge between (" + edge.node1.index[0] + ", " + edge.node1.index[1] + ") and(" + edge.node2.index[0] + ", " + edge.node2.index[1] + ") has cost " + edge.cost);
                        }
                    }
                }
            }
        }

        //Destroy old object 
        Destroy(tile.gameObject);
    }

    /// <summary>
    /// Function used to Start the Tile_Grid since Tild_Grid is not a Monobehavior and is not attached to any specific GameObject.
    /// </summary>
    public void Instantiate()
    {
        int tile_number = 0;
        for (int x = 0; x < grid_width; x++)
        {
            for (int y = 0; y < grid_length; y++)
            {
                tile_number++;

                //Instantiate the tile object.
                string file = "Objects/Tiles/" + tile_heights[x, y] + "TCanvas";
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

                //Add a collider to the tile (TEMPORARY)
                BoxCollider collider = instance.AddComponent<BoxCollider>();
                //collider.size = new Vector3(NEWTILELENGTH*NEWSCALE,tile_heights[x,y]*NEWTILEHEIGHT*(NEWSCALE/2),NEWTILEWIDTH*NEWSCALE);
                collider.size = new Vector3(NEWTILELENGTH * NEWSCALE, 0, NEWTILEWIDTH * NEWSCALE);
                //collider.center = new Vector3(0, 10, 0);
                collider.center = new Vector3(0, tile_heights[x, y], 0);

                //Set the material
                instance.GetComponentInChildren<Renderer>().material = materials[tile_mat_ids[x, y] % 10];

                //Generate the tile data for the tile
                instance.AddComponent<Tile>();
                instance.GetComponent<Tile>().Instantiate(x, y, tile_heights[x, y], tile_mat_ids[x, y], modifiers);

                //Store the instantiated tile in our Tile Tranform Grid;
                tiles[x, y] = instance.transform;

                //Add a node to the navmesh
                navmesh.addTile(tiles[x, y].GetComponent<Tile>());

                //Connect the new node to previous nodes in the mesh
                if (x > 0)
                {
                    tiles[x, y].GetComponent<Tile>().addEdge(tiles[x - 1, y].GetComponent<Tile>(), 3);
                }
                if (y > 0)
                {
                    tiles[x, y].GetComponent<Tile>().addEdge(tiles[x, y - 1].GetComponent<Tile>(), 0);
                }

                //Create Characters on top of tiles
                if (character_ids[x, y] != 0)
                {

                    //Calculate offsets
                    float height_offset = (character_prefab.GetComponent<SpriteRenderer>().sprite.rect.height *
                        character_prefab.transform.localScale.y /
                        character_prefab.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit /
                        2);
                    float offset = (height_offset) / 3.5f;

                    //Instantiate the prefab
                    GameObject character = ((GameObject)Instantiate(character_prefab,
                        new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y),
                            (float)(NEWSTARTY + tile_scale * tile_heights[x, y]),
                            (float)(NEWSTARTZ - NEWTILELENGTH * x)),
                        Quaternion.identity));

                    //Set the character tile to the current tile
                    character.GetComponent<Character_Script>().curr_tile = getTile(x, y);
                    character.GetComponent<Character_Script>().character_id = character_ids[x, y];

                    //Set the tile object to that character and set traversible to false;
                    tiles[x, y].GetComponent<Tile>().obj = character;
                    //tiles[x, y].GetComponent<Tile>().traversible = false;
                }

                //create OBJECTS on top of tiles
                SpriteRenderer sprite;
                if (object_sprite_ids[x, y] != 0)
                {
                    //pull up the object prefab
                    sprite = object_prefab.GetComponent<SpriteRenderer>();

                    //set the right sprite;
                    sprite.sprite = (Sprite)object_sprite_sheet[object_sprite_ids[x, y] - 1];
                    //sprite.sortingOrder = tile_number;
                    sprite.sortingOrder = 5000;

                    //instantiate the object
                    //Instantiate(object_prefab, new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y + XOFFSET), (float)(NEWSTARTY + YOFFSET), (float)(NEWSTARTZ - NEWTILELENGTH * x + ZOFFSET)), Quaternion.identity);
                    tiles[x, y].GetComponent<Tile>().obj = (GameObject)Instantiate(object_prefab, 
                        new Vector3((float)(NEWSTARTX - NEWTILEWIDTH * y),
                            (float)(NEWSTARTY + 0.66f + .5f * tile_heights[x, y]),
                            (float)(NEWSTARTZ - NEWTILELENGTH * x)), 
                        Quaternion.identity);
                    //Instantiate(object_prefab, new Vector3((float)(START_X - (x) * (TILE_WIDTH / 200) + (y) * (TILE_WIDTH / 200)), (float)(START_Y - (x) * (TILE_LENGTH / 200) - (y) * (TILE_LENGTH / 200) + tile_heights[x, y] * TILE_HEIGHT / 100.0 + .35f), 0), Quaternion.identity);
                }

                //If there is an object on the tile, mark it non traversible
                if (object_sprite_ids[x, y] == 0)
                {
                    tiles[x, y].GetComponent<Tile>().traversible = true;
                }
                else
                {
                    tiles[x, y].GetComponent<Tile>().traversible = false;
                }
            }
        }
    }

    /// <summary>
    /// Function to return a specific tile in the array.
    /// </summary>
    /// <param name="x">X index for the lookup.</param>
    /// <param name="y">Y index for the lookup.</param>
    /// <returns>Returns the Tile at the specified Index, or null if that index does not exist.</returns>
    public Transform getTile(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < grid_width && y < grid_length)
        {
            return tiles[x, y];
        }
        return null;
    }
}
