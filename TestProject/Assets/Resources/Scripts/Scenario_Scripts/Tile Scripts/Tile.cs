using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for storing Data on the indivindexual Tile Objects. 
/// </summary>
public class Tile : MonoBehaviour{
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
    public bool reachable { get; private set; }
    public double weight { get; set; }
    public int distance { get; set; }
    public bool visited { get; set; }
    public bool traversible { get; set; }
    public GameObject obj { get; set; }
    public GameObject effect { get; set; }
    public Tile parent { get; set; }
    public int height { get; set; }
    public double modifier { get; private set; }
    public int[] index { get; private set; }
    public Edge[] edges { get; private set; }

    /// <summary>
    /// Constructor for the Class.
    /// </summary>
    /// <param name="newHeight">The height of the new Tile</param>
    /// <param name="newModifier">The modifier for the new Tile</param>
    /// <param name="newID">The id for the new Tile.</param>
    public Tile(int newHeight, double newModifier, int[] newID)
    {
        height = newHeight;
        modifier = newModifier;
        index = newID;
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

    public bool Has_Effect()
    {
        if(effect != null)
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
	public void Instantiate(int x, int y, int newHeight, int sprite, double[] modifiers){
        double mod = modifiers[sprite %10];
        int[] newIndex = { x, y };
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
    /// Function to compare if two Tiles are equal.
    /// </summary>
    /// <param name="tile">The Tile to compare to the current Tile.</param>
    /// <returns></returns>
    public bool Equals(Tile tile)
    {
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
