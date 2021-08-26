using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for storing Data on the indivindexual Tile Objects. 
/// </summary>
[Serializable]
public class Tile : MonoBehaviour{

    private static int MAX_HEIGHT = 15;
    private static int MIN_HEIGHT = 1;
    public static string MAT_FOLDER = "Materials/";
    public static string TILE_MATS = MAT_FOLDER + "Tile_Materials/";
    public static string DEFAULT_MAT = TILE_MATS + "TileMat_0";
    public static string TILE_MESHES = "Objects/Tiles/";
    public static string TILE_OBJECT_PREFIX = "TileSet_";

    /// <summary>
    /// bool reachable - True if the tile is reachable by the current Character ability.
    /// double weight - The cost of traversing to this Tile. USed for navigation purposes.
    /// int distance - The distance in number of tiles for this Tile. Used for navigation purposes.
    /// bool visited - If the Tile has been visited by a search. USed for navigation purposes. 
    /// bool traversible - If the Tile can be walked through. Used for navigation purposes.
    /// GameObject obj - The Object sitting on top of this Tile.
    /// Tile_Effect effect - The effect currently on this Tile. 
    /// Tile parent - The parent tile for navitation purposes. TODO GET RID OF THIS.
    /// int height - The height of the Tile.
    /// double modifier - The modifier for the tile. How much it takes to move through the Tile.
    /// int[] index - The index of the Tile in the Tile_Grid and navmesh Dictionary.
    /// Edge[] edges - the Edges connecting the Tile to other Tiles.
    /// </summary>
    [JsonProperty] public int[] index { get; private set; }
    [JsonProperty] public int tile_type { get; private set; }
    [JsonProperty] public float rotation { get; private set; }
    private int _height;
    [JsonProperty] public int height
    {
        get
        {
            if (Has_Object())
            {
                Object_Script data = obj.GetComponent<Object_Script>();
                if (data != null)
                {
                    if (data.solid)
                    {
                        return _height + data.height;
                    }
                }
            }
            return _height;
        } private set
        {
            _height = value;
        }
    }
    [JsonProperty] public double modifier { get; private set; }
    [JsonProperty] public int material { get; private set; }
    public Edge[] edges { get; private set; }
    [JsonProperty] public GameObject character { get; private set; }
    [JsonProperty] public GameObject obj { get; private set; }
    [JsonProperty] public GameObject hazard { get; private set; }
    public bool reachable { get; private set; }
    public double weight { get; private set; }
    public int distance { get; private set; }
    public bool visited { get; private set; }
    private bool _traversible;
    [JsonProperty]
    public bool traversible
    {
        get
        {
            if (Has_Object())
            {
                Object_Script data = obj.GetComponent<Object_Script>();
                if (data != null)
                {
                    if (data.solid)
                    {
                        return data.traversible;
                    }
                }
            }
            return _traversible;
        }
        private set
        {
            _traversible = value;
        }
    }
    public Tile parent { get; private set; }

    /// <summary>
    /// Sets the Tile's Character to the specified object.
    /// </summary>
    /// <param name="new_chara">The Character to put on the tile</param>
    public void Set_Character(GameObject new_chara)
    {
        if (new_chara.GetComponent<Character_Script>())
        {
            character = new_chara;
        }
    }
    /// <summary>
    /// Sets the Tile's Object to the specified object.
    /// </summary>
    /// <param name="new_obj">The Object to put on the tile</param>
    public void Set_Obj(GameObject new_obj)
    {
        if (new_obj.GetComponent<Object_Script>())
        {
            obj = new_obj;
        }
    }
    /// <summary>
    /// Sets the Tile's traversible value to the specified value.
    /// </summary>
    /// <param name="trav">True or False.</param>
    public void Set_Traversible(bool trav)
    {
        traversible = trav;
    }
    /// <summary>
    /// Sets the Tile's visited value to the specified value.
    /// </summary>
    /// <param name="trav">True or False.</param>
    public void Set_Visited(bool visit)
    {
        visited = visit;
    }
    /// <summary>
    /// Sets the Tile's weight value to the specified value.
    /// </summary>
    /// <param name="trav">The amount to set the new weight.</param>
    public void Set_Weight(double new_weight)
    {
        weight = new_weight;
    }
    /// <summary>
    /// Sets the Tile's distance value to the specified value.
    /// </summary>
    /// <param name="trav">The amount to set the new distance.</param>
    public void Set_Distance(int dist)
    {
        distance = dist;
    }
    /// <summary>
    /// Sets the Tile's parent value to the specified value.
    /// </summary>
    /// <param name="trav">The Tile to which to set the new parent.</param>
    public void Set_Parent(Tile new_parent)
    {
        parent = new_parent;
    }
    /// <summary>
    /// Sets the Tile's effect value to the specified value.
    /// </summary>
    /// <param name="new_hazard">The Tile_Effect add to the Tile.</param>
    public void Set_Hazard(GameObject new_hazard)
    {
        if (new_hazard.GetComponent<Hazard>())
        {
            hazard = new_hazard;
        }
    }
    public void Set_Height(int new_height)
    {
        height = new_height;
        if (height > MAX_HEIGHT)
        {
            height = MAX_HEIGHT;
        }
        if (SceneManager.GetActiveScene().name == "Editor")
        {
            if (height < 0)
            {
                height = 0;
            }
        }else
        {
            if (height < MIN_HEIGHT)
            {
                height = MIN_HEIGHT;
            }
        }

        if (height != 0)
        {
            //Modify object
            //string file = TILE_MESHES + TILE_OBJECT_PREFIX + tile_type + TILE_OBJECT_SUFFIX;
            gameObject.GetComponentsInChildren<MeshRenderer>()[0].enabled = true;
            //gameObject.GetComponentsInChildren<MeshFilter>()[0].mesh = Resources.Load(file, typeof(Mesh)) as Mesh;

            //Modify the collider
            //BoxCollider collider = gameObject.GetComponent<BoxCollider>();
            //collider.enabled = true;
            //collider.size = new Vector3(Scenario.TILE_WIDTH * Scenario.COLL_SCALE, 0, Scenario.TILE_LENGTH * Scenario.COLL_SCALE);
            //collider.center = new Vector3(0, height, 0);

            //Modify the position
            transform.position = new Vector3(
                transform.position.x,
                Scenario.START_Y + Scenario.TILE_HEIGHT* height,
                transform.position.z
                );

            //Modify navmesh
            foreach (Edge e in edges)
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
        }
        else
        {
            traversible = false;
            gameObject.GetComponentsInChildren<MeshRenderer>()[0].enabled = false;

            //Modify the collider
            BoxCollider collider = gameObject.GetComponent<BoxCollider>();
            if (SceneManager.GetActiveScene().name == "Editor")
            {
                collider.enabled = true;
                //collider.size = new Vector3(Scenario.TILE_WIDTH * Scenario.COLL_SCALE, 0, Scenario.TILE_LENGTH * Scenario.COLL_SCALE);
                //collider.center = new Vector3(0, 1, 0);
            }
            else
            {
                collider.enabled = false;
            }
        }
    }
    public void Set_Rotation(int angle)
    {
        int bound_angle = angle % 360;
        if (bound_angle < 0)
        {
            bound_angle = 360 + bound_angle;
        }
        if ((bound_angle >= 0 && bound_angle < 45) || (bound_angle >= 315))
        {
            bound_angle = 0;
        }else if (bound_angle >= 45 && bound_angle < 135 )
        {
            bound_angle = 90;
        }
        else if (bound_angle >= 135 && bound_angle < 225)
        {
            bound_angle = 180;
        }
        else if (bound_angle >= 225 && bound_angle < 315)
        {
            bound_angle = 270;
        }
        rotation = bound_angle;
        transform.rotation = Quaternion.Euler(-90,bound_angle,0);
    }
    public void Set_Tile_Type(int type)
    {
        Mesh mesh = Resources.Load(TILE_MESHES + TILE_OBJECT_PREFIX + type, typeof(Mesh)) as Mesh;
        if(mesh != null)
        {
            gameObject.GetComponentsInChildren<MeshFilter>()[0].mesh = mesh;
            tile_type = type;
        }
        else
        {
            mesh = Resources.Load(TILE_MESHES + TILE_OBJECT_PREFIX + "0", typeof(Mesh)) as Mesh;
            gameObject.GetComponentsInChildren<MeshFilter>()[0].mesh = mesh;
            tile_type = 0;
        }
    }
    public void Set_Material(int material_num)
    {
        Material mat = (Material)Resources.Load(TILE_MATS + "TileMat_" + material_num);
        if (mat != null)
        {
            GetComponentInChildren<Renderer>().material = mat;
            material = material_num;
        }
        else
        {
            mat = (Material)Resources.Load(DEFAULT_MAT);
            GetComponentInChildren<Renderer>().material = mat;
            material = 0;
        }
    }
    public void Set_Modifier(double mod)
    {
        modifier = mod;
    }

    /// <summary>
    /// Returns a data object containing the information of this tile.
    /// </summary>
    /// <returns>A Tile_Data object with the relevant fields of this tile.</returns>
    public Tile_Data Export_Data()
    {
        //Debug.Log("Turning tile" + index[0] +  "," + index[1] + " to data");
        return new Tile_Data(this);
    }

    /// <summary>
    /// Function to add an Edge to the array of Edges for the Tile. 
    /// </summary>
    /// <param name="tile">The end Tile for the new Edge.</param>
    /// <param name="dir">The direction in which to add the Edge. The index into the Edge array.</param>
    public void addEdge(Tile tile, int dir)
    {
        Edge e = new Edge(this, tile);
        edges[dir] = e;
        Edge e1 = new Edge(tile, this);
        if (dir == 0)
        {
            tile.edges[2] = e1;
        }
        else if (dir == 1)
        {
            tile.edges[3] = e1;
        }
        else if (dir == 2)
        {
            tile.edges[0] = e1;
        }
        else if (dir == 3)
        {
            tile.edges[1] = e1;
        }

    }

    /// <summary>
    /// Returns true if the Tile has a Character on it, False otherwise
    /// </summary>
    /// <returns>True if the Tile has an Object, False otherwise.</returns>
    public bool Has_Character()
    {
        if (character != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns True if the Tile has an Effect on it, False otherwise.
    /// </summary>
    /// <returns>True if the Tile has an Object, False otherwise.</returns>
    public bool Has_Hazard()
    {
        if(hazard != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns True if the Tile has an Object on it, False otherwise. 
    /// </summary>
    /// <returns>True if the Tile has an Object, False otherwise.</returns>
    public bool Has_Object()
    {
        if (obj != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Print all the Edges for this Tile. For debugging purposes.
    /// </summary>
    public void printEdges()
    {
        foreach (Edge e in edges)
        {
            if (e != null)
            {
                Debug.Log("Tile (" + index[0] + "," + index[1] + ") Connects to Tile: (" + e.tile2.index[0] + "," + e.tile2.index[1] + ") with a cost of " + e.cost);
                //Debug.Log("Distance: " + distance);
            }
        }
    }

    /// <summary>
    /// Resets the reachable values and the weight of the tiles in preparation for pathfinding.
    /// </summary>
    public void Reset_Reachable()
    {
        weight = -1;
        parent = null;
        visited = false;
    }

    /// <summary>
    /// Function that returns the Edges array sorted by the the cheapest Edge.
    /// </summary>
    /// <returns>Edge array sorted by cheapest.</returns>
    public Edge[] cheapestEdges()
    {
        double mostWeight = 300;
        Edge[] new_edges = new Edge[4];
        for (int x = 0; x < 4; x++)
        {
            if (edges[x].cost <= mostWeight)
            {
                mostWeight = edges[x].cost;
                new_edges[x] = edges[x];
                x = 0;
            }
            else
            {
                new_edges[x] = null;
            }
        }
        return new_edges;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="newHeight"></param>
    /// <param name="sprite"></param>
    /// <param name="modifiers"></param>
	public void Instantiate(int x, int y, int type, float rot, int newHeight, int sprite, double[] modifiers){
        double mod = modifiers[sprite %10];
        int[] newIndex = { x, y };
        tile_type = type;
        rotation = rot;
        gameObject.transform.rotation = new Quaternion(0,rotation,0,0);
        height = newHeight;
        modifier = mod;
        index = newIndex;
        edges = new Edge[4];
        weight = -1;
        distance = -1;
        visited = false;
        traversible = true;
        reachable = true;
        parent = null;
        obj = null;
    }

    /// <summary>
    /// Function used to copy tile data over to another game object. USed for Elevate Actions. 
    /// </summary>
    /// <param name="tile"></param>
    public void Instantiate(Tile tile)
    {
        reachable = tile.reachable;
        weight = tile.weight;
        distance = tile.distance;
        rotation = tile.rotation;
        tile.tile_type = tile_type;
        visited = false;
        traversible = tile.traversible;
        obj = tile.obj;
        parent = tile.parent;
        height = tile.height;
        modifier = tile.modifier;
        index = tile.index;
        edges = new Edge[4];
        for(int x = 0; x < tile.edges.Length; x++)
        {
            if (tile.edges[x] != null)
            {
                edges[x] = new Edge(this, tile.edges[x].tile2);
            }
        }
    }

    /// <summary>
    /// Default instantiation for a Tile
    /// </summary>
    public void Instantiate(int x, int y)
    {
        index = new int[2] { x, y };
        weight = -1;
        distance = -1;
        visited = false;
        reachable = true;
        parent = null;

        rotation = 0;
        tile_type = 0;
        height = 1;
        traversible = true;
        modifier = 0;
        material = 0;
        Set_Material(material);

        //Set up edges
        edges = new Edge[4];
        Scenario scenario = Game_Controller.Get_Curr_Scenario();
        //Only need to worry about adding 2 edges since we will be adding new tiles to the edge of the grid.
        if (index[0] > 0)
        {
            addEdge(scenario.Get_Tile(index[0] - 1, index[1]), 3);
        }
        if (index[1] > 0)
        {
            addEdge(scenario.Get_Tile(index[0], index[1] - 1), 0);
        }

        hazard = null;
        obj = null;
        character = null;
    }

    /// <summary>
    /// Function used to copy tile data over to another game object. Used when instantiating the Scenario.
    /// </summary>
    /// <param name="data">Data to be transferred over to the object.</param>
    public void Instantiate(Tile_Data data)
    {
        index = data.index;
        weight = -1;
        distance = -1;
        visited = false;
        reachable = true;
        parent = null;

        rotation = data.rotation;
        tile_type = data.tile_type;
        traversible = data.traversible;
        height = data.height;
        modifier = data.modifier;
        material = data.material;
        index = data.index;
        Set_Material(material);

        //Set up edges
        edges = new Edge[4];
        Scenario scenario = Game_Controller.Get_Curr_Scenario();
        //Only need to worry about adding 2 edges since we will be adding new tiles to the edge of the grid.
        if (index[0] > 0)
        {
            addEdge(scenario.Get_Tile(index[0] - 1, index[1]), 3);
        }
        if (index[1] > 0)
        {
            addEdge(scenario.Get_Tile(index[0], index[1]-1), 0);
        }

        if(data.hazard != null)
        {
            scenario.Add_Hazard(this, data.hazard);
        }

        if (data.obj != null)
        {
            scenario.Add_Object(this, data.obj);
        }

        if(data.character != null)
        {
            scenario.Add_Character(this, data.character);
        }
    }

    /// <summary>
    /// Function to compare if two Tiles are equal.
    /// </summary>
    /// <param name="tile">The Tile to compare to the current Tile.</param>
    /// <returns></returns>
    public bool Equals(Tile tile)
    {
        if (tile == null || tile.index == null || index == null)
        {
            return false;
        }
        if (index[0] == tile.index[0] &&
            index[1] == tile.index[1])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Function to print data on this tile. For Debugging Purposes.
    /// </summary>
    public void printTileData()
    {
        Debug.Log("Tile (" + index[0] + "," + index[1] + ") reachable: " + reachable);
        Debug.Log("Tile (" + index[0] + "," + index[1] + ") height: " + height);
        Debug.Log("Tile (" + index[0] + "," + index[1] + ") traversible: " + traversible);
        Debug.Log("Tile (" + index[0] + "," + index[1] + ") weight: " + weight);
        foreach (Edge e in edges)
        {
            if (e != null && e.tile1 != null && e.tile2!= null)
            {
                Debug.Log("Tile (" + e.tile1.index[0] + "," + e.tile1.index[1] + ") Edge connects to Tile (" + e.tile2.index[0] + "," + e.tile2.index[1] + ") and has cost " + e.cost);
            }else if (e == null)
            {
                Debug.Log("Edge is null");
            }else if (e.tile2 == null)
            {
                Debug.Log("Edge has no destination");
            }
            else if (e.tile1 == null)
            {
                Debug.Log("Edge has no start");
            }

        }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
		reachable = true;
	}
}
