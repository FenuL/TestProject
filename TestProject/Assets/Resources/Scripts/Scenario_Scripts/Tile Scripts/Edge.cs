using UnityEngine;
using System.Collections;

/// <summary>
/// Class for an Edge connecting two Tiles.
/// </summary>
public class Edge
{
    /// <summary>
    /// double cost - The cost in speed for a Character to travel on this Edge.
    /// Tile tile1 - The Tile at which the Edge starts.
    /// Tile tile2 - The Tile at which the Edge ends.
    /// </summary>
    public double cost { get; private set; }
    public Tile tile1 { get; private set; }
    public Tile tile2 { get; private set; }

    /// <summary>
    /// Constructor for the Class.
    /// </summary>
    /// <param name="newsource">The source Tile for the new Edge.</param>
    /// <param name="newdestination">The end Tile for the new Edge.</param>
    public Edge(Tile newsource, Tile newdestination)
    {
        tile1 = newsource;
        tile2 = newdestination;
        int height_diff = tile1.height - tile2.height;
        if (height_diff >= -1)
        {
            cost = 1;
        }
        else if (height_diff == -2)
        {
            cost = 3;
        }
        else if (height_diff == -3)
        {
            cost = 7;
        }
        else if (height_diff < -3)
        {
            cost = 25;
        }
        else
        {
            cost = 1;
        }
        cost = cost + tile2.modifier;
    }

    /// <summary>
    /// Function to update the Edge to a new endTile and recalculate the Cost.
    /// </summary>
    /// <param name="newTile2">The new End Tile for the Edge.</param>
    public void Update_Cost(Tile newTile2)
    {
        tile2 = newTile2;
        int height_diff = tile1.height - tile2.height;
        if (height_diff >= -1)
        {
            cost = 1;
        }
        else if (height_diff == -2)
        {
            cost = 3;
        }
        else if (height_diff == -3)
        {
            cost = 7;
        }
        else if (height_diff < -3)
        {
            cost = 25;
        }
        else
        {
            cost = 1;
        }
        cost = cost + tile2.modifier;

    }

    /// <summary>
    /// Function to Compare between two Edges. Returns the Edge with the highest Cost. 
    /// </summary>
    /// <param name="e1">The first Edge to compare.</param>
    /// <param name="e2">The second Edge to compare.</param>
    /// <returns>The Edge with the highest Cost. Ties return e1.</returns>
    public Edge Compare(Edge e1, Edge e2)
    {
        if (e1.cost <= e2.cost)
        {
            return e1;
        }
        else if (e1.cost > e2.cost)
        {
            return e2;
        }
        else return null;
    }

    /// <summary>
    /// Function to check if two Edges are equal.
    /// </summary>
    /// <param name="e1">The first Edge to compare.</param>
    /// <param name="e2">The second Edge to compare.</param>
    /// <returns>True if the Edges are the same. False if otherwise. </returns>
    public bool Equals(Edge e1, Edge e2)
    {
        if (e1.tile1.index[0] == e2.tile1.index[0] && 
            e1.tile1.index[1] == e2.tile1.index[1] && 
            e1.tile2.index[0] == e2.tile2.index[0] && 
            e1.tile2.index[1] == e2.tile2.index[1] && 
            e1.cost == e2.cost)
        {
            return true;
        }
        return false;
    }
}
