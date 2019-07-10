using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for navigating the collection of Tiles connected by Edges.
/// </summary>
public class Graph
{
    /// <summary>
    /// Dictionary<int[], Tile> tiles - The List of Tiles to navigate. The key to the Dictionary is the Tile index.
    /// Stack<Tile> visitedTiles - The Stack of tiles visited while traversing the Graph.
    /// </summary>
    private Dictionary<int[], Tile> tiles;
    private Stack<Tile> visitedTiles;

    /// <summary>
    /// Generic Constructor. Creates a blank Graph.
    /// </summary>
    public Graph()
    {
        tiles = new Dictionary<int[], Tile>();
        visitedTiles = new Stack<Tile>();
    }

    /// <summary>
    /// Adds a Tile to the Dictionary.
    /// </summary>
    /// <param name="n"></param>
    public void addTile(Tile n)
    {
        tiles.Add(n.index, n);
    }

    /// <summary>
    /// Remove a time from the Dictionary.
    /// </summary>
    /// <param name="n"></param>
    public void Remove_Tile(Tile n)
    {
        tiles.Remove(n.index);
    }

    /// <summary>
    /// Prints the entire graph, edge by edge.
    /// </summary>
    public void printGraph()
    {
        foreach (Tile n in tiles.Values)
        {
            n.printEdges();
        }
    }

    /// <summary>
    /// Function to compute the shortest path between two points in the graph.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="cost_limit"></param>
    /// <param name="distance_limit"></param>
    /// <returns></returns>
    public Stack<Tile> shortestPath(Tile start, Tile end, int cost_limit, int distance_limit)
    {
        start.traversible = true;
        visitedTiles = new Stack<Tile>();
        int cost = 0;
        int distance = 0;
        int branch_cost = 0;
        Tile current = start;
        Stack<Tile> curr_path = new Stack<Tile>();
        Stack<Tile> visited_tiles = new Stack<Tile>();
        Stack<Tile> theoretical_branch = new Stack<Tile>();
        curr_path.Push(current);
        current.visited = true;
        visitedTiles.Push(current);
        int tot_diff_x = end.index[0] - start.index[0];
        int tot_diff_y = end.index[1] - start.index[1];
        //If the total distance of the shortest path is longer than the limit, there is no shortest path
        if (Mathf.Abs(tot_diff_x) + Mathf.Abs(tot_diff_y) > distance_limit)
        {
            return null;
        }
        int curr_diff_x = tot_diff_x;
        int curr_diff_y = tot_diff_y;

        while (current.index[0] != end.index[0] && current.index[1] != end.index[1])
        {
            //Debug.Log("Current Tile Id: (" + current.index[0] + "," + current.index[1] + ")");
            //Check which distance is greater
            curr_diff_x = current.index[0] - start.index[0];
            curr_diff_y = current.index[1] - start.index[1];
            if (Mathf.Abs(curr_diff_x) >= Mathf.Abs(curr_diff_y) && curr_diff_x != 0)
            {
                //If the x difference is greater, we want to start looking in the x direction
                //check what direction we want to start looking in
                //if the current x index is less than the x end index we need to move south east.
                if (current.index[0] < end.index[0])
                {
                    //Simplest scenario. If the cost of the direct path is less than
                    //or equal to the cost of alternate paths, take the direct route
                    if (current.edges[1] != null &&
                        current.edges[1].tile2.traversible &&
                        current.edges[1].cost <= current.edges[2].cost &&
                        current.edges[1].cost <= current.edges[0].cost &&
                        current.edges[1].cost <= current.edges[3].cost)
                    {
                        //if we have not tried the direct path, we take it
                        if (!current.edges[1].tile2.visited)
                        {
                            //update the cost of the path
                            //update the distance of the path
                            //mark the current tile as visited
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[1].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[1].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }
                        //if we have tried this path, we may need to backtrack.
                        else
                        {
                            cost -= (int)current.edges[1].cost;
                            distance -= 1;
                            current = current.edges[1].tile2;
                            curr_path.Pop();
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Prioritize looking south west;
                    else if (current.edges[2] != null &&
                        current.edges[2].tile2.traversible &&
                        current.edges[2].cost <= current.edges[1].cost &&
                        current.edges[2].cost <= current.edges[0].cost &&
                        current.edges[2].cost <= current.edges[3].cost)
                    {
                        //if we have not tried the path, we take it
                        if (!current.edges[2].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[2].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[2].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Next look North East;
                    else if (current.edges[0] != null &&
                        current.edges[0].tile2.traversible &&
                        current.edges[0].cost <= current.edges[1].cost &&
                        current.edges[0].cost <= current.edges[2].cost &&
                        current.edges[0].cost <= current.edges[3].cost)
                    {
                        //if we have not tried the path, we take it
                        if (!current.edges[0].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[0].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[0].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Next look North West;
                    else if (current.edges[3] != null &&
                        current.edges[3].tile2.traversible &&
                        current.edges[3].cost <= current.edges[2].cost &&
                        current.edges[3].cost <= current.edges[0].cost &&
                        current.edges[3].cost <= current.edges[1].cost)
                    {
                        if (!current.edges[3].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[3].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[3].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //If None of the above are true, none of the adjacent tiles are traversible there is no path
                    else
                    {
                        return null;
                    }
                }
                //if the current x index is more than the x end index we need to move north west.
                else
                {
                    //Simplest scenario. If the cost of the direct path is less than
                    //or equal to the cost of alternate paths, take the direct route
                    if (current.edges[3] != null &&
                        current.edges[3].tile2.traversible &&
                        current.edges[3].cost <= current.edges[2].cost &&
                        current.edges[3].cost <= current.edges[0].cost &&
                        current.edges[3].cost <= current.edges[1].cost)
                    {
                        //if we have not tried the direct path, we take it
                        if (!current.edges[3].tile2.visited)
                        {
                            //update the cost of the path
                            //update the distance of the path
                            //mark the current tile as visited
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[3].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[3].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }
                        //if we have tried this path, we may need to backtrack.
                        else
                        {
                            cost -= (int)current.edges[1].cost;
                            distance -= 1;
                            current = current.edges[1].tile2;
                            curr_path.Pop();
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Prioritize looking north east;
                    else if (current.edges[0] != null &&
                        current.edges[0].tile2.traversible &&
                        current.edges[0].cost <= current.edges[1].cost &&
                        current.edges[0].cost <= current.edges[2].cost &&
                        current.edges[0].cost <= current.edges[3].cost)
                    {
                        //if we have not tried the path, we take it
                        if (!current.edges[0].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[0].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[0].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Next look South West;
                    else if (current.edges[2] != null &&
                        current.edges[2].tile2.traversible &&
                        current.edges[2].cost <= current.edges[1].cost &&
                        current.edges[2].cost <= current.edges[0].cost &&
                        current.edges[2].cost <= current.edges[3].cost)
                    {
                        //if we have not tried the path, we take it
                        if (!current.edges[2].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[2].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[2].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Next look South East;
                    else if (current.edges[1] != null &&
                        current.edges[1].tile2.traversible &&
                        current.edges[1].cost <= current.edges[2].cost &&
                        current.edges[1].cost <= current.edges[0].cost &&
                        current.edges[1].cost <= current.edges[3].cost)
                    {
                        if (!current.edges[1].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[1].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[1].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //If None of the above are true, none of the adjacent tiles are traversible there is no path
                    else
                    {
                        return null;
                    }
                }
            }
            //If the 
            else if (Mathf.Abs(curr_diff_y) >= Mathf.Abs(curr_diff_x) && curr_diff_y != 0)
            {
                //If the y difference is greater, we want to start looking in the y direction
                //check what direction we want to start looking in
                //if the current y index is less than the y end index we need to move South West.
                if (current.index[1] < end.index[1])
                {
                    //Simplest scenario. If the cost of the direct path is less than
                    //or equal to the cost of alternate paths, take the direct route
                    if (current.edges[2] != null &&
                        current.edges[2].tile2.traversible &&
                        current.edges[2].cost <= current.edges[3].cost &&
                        current.edges[2].cost <= current.edges[0].cost &&
                        current.edges[2].cost <= current.edges[1].cost)
                    {
                        //if we have not tried the direct path, we take it
                        if (!current.edges[2].tile2.visited)
                        {
                            //update the cost of the path
                            //update the distance of the path
                            //mark the current tile as visited
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[2].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[2].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }
                        //if we have tried this path, we may need to backtrack.
                        else
                        {
                            cost -= (int)current.edges[1].cost;
                            distance -= 1;
                            current = current.edges[1].tile2;
                            curr_path.Pop();
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Prioritize looking South East;
                    else if (current.edges[1] != null &&
                        current.edges[1].tile2.traversible &&
                        current.edges[1].cost <= current.edges[2].cost &&
                        current.edges[1].cost <= current.edges[0].cost &&
                        current.edges[1].cost <= current.edges[3].cost)
                    {
                        //if we have not tried the path, we take it
                        if (!current.edges[1].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[1].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[1].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Next look North West;
                    else if (current.edges[3] != null &&
                        current.edges[3].tile2.traversible &&
                        current.edges[3].cost <= current.edges[1].cost &&
                        current.edges[3].cost <= current.edges[2].cost &&
                        current.edges[3].cost <= current.edges[0].cost)
                    {
                        //if we have not tried the path, we take it
                        if (!current.edges[3].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[3].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[3].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Next look North East;
                    else if (current.edges[0] != null &&
                        current.edges[0].tile2.traversible &&
                        current.edges[0].cost <= current.edges[2].cost &&
                        current.edges[0].cost <= current.edges[3].cost &&
                        current.edges[0].cost <= current.edges[1].cost)
                    {
                        if (!current.edges[0].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[0].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[0].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //If None of the above are true, none of the adjacent tiles are traversible there is no path
                    else
                    {
                        return null;
                    }
                }
                //if the current y index is more than the y end index we need to move North East.
                else
                {
                    //Simplest scenario. If the cost of the direct path is less than
                    //or equal to the cost of alternate paths, take the direct route
                    if (current.edges[0] != null &&
                        current.edges[0].tile2.traversible &&
                        current.edges[0].cost <= current.edges[2].cost &&
                        current.edges[0].cost <= current.edges[3].cost &&
                        current.edges[0].cost <= current.edges[1].cost)
                    {
                        //if we have not tried the direct path, we take it
                        if (!current.edges[3].tile2.visited)
                        {
                            //update the cost of the path
                            //update the distance of the path
                            //mark the current tile as visited
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[3].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[3].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }
                        //if we have tried this path, we may need to backtrack.
                        else
                        {
                            cost -= (int)current.edges[1].cost;
                            distance -= 1;
                            current = current.edges[1].tile2;
                            curr_path.Pop();
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Prioritize looking North West;
                    else if (current.edges[3] != null &&
                        current.edges[3].tile2.traversible &&
                        current.edges[3].cost <= current.edges[1].cost &&
                        current.edges[3].cost <= current.edges[2].cost &&
                        current.edges[3].cost <= current.edges[0].cost)
                    {
                        //if we have not tried the path, we take it
                        if (!current.edges[3].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[3].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[3].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Next look South East;
                    else if (current.edges[1] != null &&
                        current.edges[1].tile2.traversible &&
                        current.edges[1].cost <= current.edges[2].cost &&
                        current.edges[1].cost <= current.edges[0].cost &&
                        current.edges[1].cost <= current.edges[3].cost)
                    {
                        //if we have not tried the path, we take it
                        if (!current.edges[1].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[1].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[1].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //More complex scenario. If the cost of the direct path is not 
                    //less than the cost of the alternate path, we need to investigate 
                    //alternative paths. Next look South West;
                    else if (current.edges[2] != null &&
                        current.edges[2].tile2.traversible &&
                        current.edges[2].cost <= current.edges[1].cost &&
                        current.edges[2].cost <= current.edges[0].cost &&
                        current.edges[2].cost <= current.edges[3].cost)
                    {
                        if (!current.edges[2].tile2.visited)
                        {
                            //update the cost of the path
                            //set the current to the new tile.
                            //push the current to the stack;
                            //add the tile to the visited tile stack;
                            cost += (int)current.edges[2].cost;
                            distance += 1;
                            current.visited = true;
                            current = current.edges[2].tile2;
                            curr_path.Push(current);
                            visited_tiles.Push(current);
                        }

                    }
                    //If None of the above are true, none of the adjacent tiles are traversible there is no path
                    else
                    {
                        return null;
                    }
                }
            }
        }
        current = curr_path.Pop();
        current.weight = cost;
        curr_path.Push(current);
        return curr_path;
    }

    /// <summary>
    /// Does a Breadth First Search of the Graph and marks the shortest path to all tiles within a given range.
    /// </summary>
    /// <param name="start">The start Tile for the search.</param>
    /// <param name="tag">The the tag to ignore for collision detection.</param>
    /// <param name="cost_limit">The range limit in cost of Edges for how far to look. </param>
    /// <param name="distace_limit">The range limit in distance for how far to look.</param>
    public void bfs(Tile start, string tag, int cost_limit, int distace_limit)
    {
        //reset the previously visited tiles
        Tile n;
        //Debug.Log("visited tiles " + visitedTiles.Count);
        while (visitedTiles.Count != 0)
        {
            n = visitedTiles.Pop();
            n.weight = -1;
            n.parent = null;
            n.visited = false;
        }
        visitedTiles = new Stack<Tile>();
        Queue<Tile> queue = new Queue<Tile>();
        Tile current;
        start.weight = 0;
        start.distance = 0;
        queue.Enqueue(start);
        visitedTiles.Push(start);

        while (queue.Count > 0)
        {
            current = queue.Dequeue();
            //Debug.Log("Current Tile: " + current.index[0] + "," + current.index[1] + "; Cost: " + current.weight);
            foreach (Edge e in current.edges)
            {

                if (e != null)
                {

                    if (e.tile2 != null &&
                        !e.tile2.visited &&
                        current.weight + e.cost <= cost_limit &&
                        //start.obj != null &&
                        (e.tile2.obj == null || e.tile2.obj.tag == tag) &&
                        current.distance < distace_limit)
                    {
                        //Debug.Log("Cost:" + e.cost);
                        e.tile2.weight = current.weight + e.cost;
                        e.tile2.distance = current.distance + 1;
                        e.tile2.visited = true;
                        e.tile2.parent = current;
                        queue.Enqueue(e.tile2);
                        visitedTiles.Push(e.tile2);
                    }
                    //If the tile has been visited before and 
                    //it is not the starting tile, but 
                    //we found a shorter path to the tile we still update it.
                    if (e.tile2.visited &&
                        e.tile2.index[0] != start.index[0] &&
                        e.tile2.index[1] != start.index[1] &&
                        e.tile2.weight > current.weight + e.cost &&
                        e.tile2.distance < distace_limit)
                    {
                        e.tile2.weight = current.weight + e.cost;
                        e.tile2.distance = current.distance + 1;
                        e.tile2.parent = current;
                        queue.Enqueue(e.tile2);
                    }
                }
            }
        }
    }

    public Stack<Tile> FindPath(Tile start, Tile finish)
    {
        Stack<Tile> path = new Stack<Tile>();
        //If we already have a path from a bfs call.
        //Debug.Log("Start exists " + start.index[0] + "," + start.index[1]);
        //Debug.Log("Finish Exists " + finish.index[0] + "," + finish.index[1]);
        if (visitedTiles.Count != 0)
        {
            Tile temp_tile = finish;
            Tile prev_tile = start;

            //Construct a stack that is a path from the clicked tile to the source.
            while (!(temp_tile.index[0] == start.index[0] && temp_tile.index[1] == start.index[1]))
            {
                path.Push(temp_tile);
                //Look at the parent tile.
                //Debug.Log("parent tile: " + temp_tile.parent.index[0] + "," + temp_tile.parent.index[1]);
                temp_tile = temp_tile.parent;
                
            }

            //reset the previously visited tiles
            Tile n;
            while (visitedTiles.Count != 0)
            {
                n = visitedTiles.Pop();
                n.weight = -1;
                n.parent = null;
                n.visited = false;
            }

        }
        return path;
    }
}
