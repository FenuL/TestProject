using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Tile_Data : MonoBehaviour{
	
	public int x_index;
	public int y_index;
	public int z_index;
	public int tile_sprite_index;
	public int object_sprite;
	public bool reachable;
    public Node node;

	//methods
	public Tile_Data(int x, int y, int height, int sprite){
		x_index = x;
		y_index = y;
		node.height = height;
		tile_sprite_index = sprite;
		node.traversible = true;
        node.weight = -1;
	}
	
	public Tile_Data(int x, int y, int height){
		x_index = x;
		y_index = y;
		node.height = height;
		tile_sprite_index = 1;
		node.traversible = true;
        node.weight = -1;
    }

	public void instantiate(int x, int y, int height, int sprite){
		x_index = x;
		y_index = y;
		tile_sprite_index = sprite;
        int mod = 0;
        switch (tile_sprite_index)
        {
            case 13:
                mod = 1;
                break;
            case 6:
                mod = 2;
                break;
        }
        int[] index = { x_index, y_index };
        node = new Node(height, mod, index);
	}

	// Use this for initialization
	void Start () {
		reachable = true;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public class Node
    {
        public int weight;
        public int distance;
        public bool visited;
        public bool traversible;
        public GameObject obj;
        public Node parent;
        public int height;
        public int modifier;
        public int[] id;
        public Edge[] edges;

        public Node(int newHeight, int newModifier, int[] newID)
        {
            height = newHeight;
            modifier = newModifier;
            id = newID;
            edges = new Edge[4];
            weight = -1;
            distance = -1;
            visited = false;
            traversible = true;
            parent = null;
            obj = null;
        }

        public void setObj(GameObject new_obj)
        {
            obj = new_obj;
        }

        public void addEdge(Node n, int dir)
        {
            Edge e = new Edge(this, n);
            edges[dir] = e;
            Edge e1 = new Edge(n, this);
            if (dir == 0)
            {
                n.edges[2] = e1;
            }
            else if (dir == 1)
            {
                n.edges[3] = e1;
            }
            else if (dir == 2)
            {
                n.edges[0] = e1;
            }
            else if (dir == 3)
            {
                n.edges[1] = e1;
            }

        }

        public void printEdges()
        {
            foreach (Edge e in edges)
            {
                if (e != null)
                {
                    Debug.Log("Node (" + id[0] + "," + id[1] + ") Connects to Node: (" + e.node2.id[0] + "," + e.node2.id[1] + ") with a cost of " + e.cost);
                    //Debug.Log("Distance: " + distance);
                }
            }
        }

        public Edge[] cheapestEdges()
        {
            double mostWeight = 300;
            Edge[] new_edges = new Edge[4];
            for (int x =0; x<4; x++)
            {
                if (edges[x].cost <= mostWeight)
                {
                    mostWeight = edges[x].cost;
                    new_edges[x] = edges[x];
                    x = 0;
                }else
                {
                    new_edges[x] = null;
                }
            }
            return new_edges;
        }

    }

    public class Edge
    {
        public int cost;
        public Node node1;
        public Node node2;

        public Edge(Node newsource, Node newdestination)
        {
            node1 = newsource;
            node2 = newdestination;
            int height_diff = node1.height - node2.height;
            if (height_diff >= -1)
            {
                cost = 1;
            }
            else if (height_diff == -2)
            {
                cost = 5;
            }
            else if (height_diff == -3)
            {
                cost = 15;
            }
            else if (height_diff < -3)
            {
                cost = 125;
            }
            else
            {
                cost = 1;
            }
            cost = cost + node2.modifier;
        }

        public Edge compare(Edge e1, Edge e2)
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

        public bool equals(Edge e1, Edge e2)
        {
            if (e1.node1.id[0] == e2.node1.id[0] && e1.node1.id[1] == e2.node1.id[1] && e1.node2.id[0] == e2.node2.id[0] && e1.node2.id[1] == e2.node2.id[1] && e1.cost == e2.cost)
            {
                return true;
            }
            return false;
        }
    }

    public class Graph
    {
        public List<Node> nodes;
        public Stack<Node> visitedNodes;

        public Graph()
        {
            nodes = new List<Node>();
            visitedNodes = new Stack<Node>();
        }

        public void addNode(Node n)
        {
            nodes.Add(n);
        }

        public void printGraph()
        {
            foreach (Node n in nodes)
            {
                n.printEdges();
            }
        }

        public Stack<Node> shortestPath(Node start, Node end, int cost_limit, int distance_limit)
        {
            start.traversible = true;
            visitedNodes = new Stack<Node>();
            int cost = 0;
            int distance = 0;
            int branch_cost = 0;
            Node current = start;
            Stack<Node> curr_path = new Stack<Node>();
            Stack<Node> visited_nodes = new Stack<Node>();
            Stack<Node> theoretical_branch = new Stack<Node>();
            curr_path.Push(current);
            current.visited = true;
            visitedNodes.Push(current);
            int tot_diff_x = end.id[0] - start.id[0];
            int tot_diff_y = end.id[1] - start.id[1];
            //If the total distance of the shortest path is longer than the limit, there is no shortest path
            if(Mathf.Abs(tot_diff_x) + Mathf.Abs(tot_diff_y) > distance_limit)
            {
                return null;
            }
            int curr_diff_x = tot_diff_x;
            int curr_diff_y = tot_diff_y;
            
            while (current.id[0] != end.id[0] && current.id[1] != end.id[1])
            {
                //Debug.Log("Current Node Id: (" + current.id[0] + "," + current.id[1] + ")");
                //Check which distance is greater
                curr_diff_x = current.id[0] - start.id[0];
                curr_diff_y = current.id[1] - start.id[1];
                if (Mathf.Abs(curr_diff_x) >= Mathf.Abs(curr_diff_y) && curr_diff_x != 0)
                {
                    //If the x difference is greater, we want to start looking in the x direction
                    //check what direction we want to start looking in
                    //if the current x id is less than the x end id we need to move south east.
                    if (current.id[0] < end.id[0])
                    {
                        //Simplest scenario. If the cost of the direct path is less than
                        //or equal to the cost of alternate paths, take the direct route
                        if (current.edges[1] != null &&
                            current.edges[1].node2.traversible &&
                            current.edges[1].cost <= current.edges[2].cost &&
                            current.edges[1].cost <= current.edges[0].cost &&
                            current.edges[1].cost <= current.edges[3].cost)
                        {
                            //if we have not tried the direct path, we take it
                            if (!current.edges[1].node2.visited)
                            {
                                //update the cost of the path
                                //update the distance of the path
                                //mark the current tile as visited
                                //set the current to the new tile.
                                //push the current to the stack;
                                //add the node to the visited node stack;
                                cost += (int)current.edges[1].cost;
                                distance += 1;
                                current.visited = true;
                                current = current.edges[1].node2;
                                curr_path.Push(current);
                                visited_nodes.Push(current);
                            }
                            //if we have tried this path, we may need to backtrack.
                            else
                            {
                                cost -= (int)current.edges[1].cost;
                                distance -= 1;
                                current = current.edges[1].node2;
                                curr_path.Pop();
                            }

                        }
                        //More complex scenario. If the cost of the direct path is not 
                        //less than the cost of the alternate path, we need to investigate 
                        //alternative paths. Prioritize looking south west;
                        else if (current.edges[2] != null &&
                            current.edges[2].node2.traversible &&
                            current.edges[2].cost <= current.edges[1].cost &&
                            current.edges[2].cost <= current.edges[0].cost &&
                            current.edges[2].cost <= current.edges[3].cost)
                        {
                            //if we have not tried the path, we take it
                            if (!current.edges[2].node2.visited)
                            {
                                //update the cost of the path
                                //set the current to the new tile.
                                //push the current to the stack;
                                //add the node to the visited node stack;
                                cost += (int)current.edges[2].cost;
                                distance += 1;
                                current.visited = true;
                                current = current.edges[2].node2;
                                curr_path.Push(current);
                                visited_nodes.Push(current);
                            }

                        }
                        //More complex scenario. If the cost of the direct path is not 
                        //less than the cost of the alternate path, we need to investigate 
                        //alternative paths. Next look North East;
                        else if (current.edges[0] != null &&
                            current.edges[0].node2.traversible &&
                            current.edges[0].cost <= current.edges[1].cost &&
                            current.edges[0].cost <= current.edges[2].cost &&
                            current.edges[0].cost <= current.edges[3].cost)
                        {
                            //if we have not tried the path, we take it
                            if (!current.edges[0].node2.visited)
                            {
                                //update the cost of the path
                                //set the current to the new tile.
                                //push the current to the stack;
                                //add the node to the visited node stack;
                                cost += (int)current.edges[0].cost;
                                distance += 1;
                                current.visited = true;
                                current = current.edges[0].node2;
                                curr_path.Push(current);
                                visited_nodes.Push(current);
                            }

                        }
                        //More complex scenario. If the cost of the direct path is not 
                        //less than the cost of the alternate path, we need to investigate 
                        //alternative paths. Next look North West;
                        else if (current.edges[3] != null &&
                            current.edges[3].node2.traversible &&
                            current.edges[3].cost <= current.edges[2].cost &&
                            current.edges[3].cost <= current.edges[0].cost &&
                            current.edges[3].cost <= current.edges[1].cost)
                        {
                            if (!current.edges[3].node2.visited)
                            {
                                //update the cost of the path
                                //set the current to the new tile.
                                //push the current to the stack;
                                //add the node to the visited node stack;
                                cost += (int)current.edges[3].cost;
                                distance += 1;
                                current.visited = true;
                                current = current.edges[3].node2;
                                curr_path.Push(current);
                                visited_nodes.Push(current);
                            }

                        }
                        //If None of the above are true, none of the adjacent tiles are traversible there is no path
                        else
                        {
                            return null;
                        }
                    }
                    //if the current x id is more than the x end id we need to move north west.
                    else
                    {
                        //Simplest scenario. If the cost of the direct path is less than
                        //or equal to the cost of alternate paths, take the direct route
                        if (current.edges[3] != null &&
                            current.edges[3].node2.traversible &&
                            current.edges[3].cost <= current.edges[2].cost &&
                            current.edges[3].cost <= current.edges[0].cost &&
                            current.edges[3].cost <= current.edges[1].cost)
                        {
                            //if we have not tried the direct path, we take it
                            if (!current.edges[3].node2.visited)
                            {
                                //update the cost of the path
                                //update the distance of the path
                                //mark the current tile as visited
                                //set the current to the new tile.
                                //push the current to the stack;
                                //add the node to the visited node stack;
                                cost += (int)current.edges[3].cost;
                                distance += 1;
                                current.visited = true;
                                current = current.edges[3].node2;
                                curr_path.Push(current);
                                visited_nodes.Push(current);
                            }
                            //if we have tried this path, we may need to backtrack.
                            else
                            {
                                cost -= (int)current.edges[1].cost;
                                distance -= 1;
                                current = current.edges[1].node2;
                                curr_path.Pop();
                            }

                        }
                        //More complex scenario. If the cost of the direct path is not 
                        //less than the cost of the alternate path, we need to investigate 
                        //alternative paths. Prioritize looking north east;
                        else if (current.edges[0] != null &&
                            current.edges[0].node2.traversible &&
                            current.edges[0].cost <= current.edges[1].cost &&
                            current.edges[0].cost <= current.edges[2].cost &&
                            current.edges[0].cost <= current.edges[3].cost)
                        {
                            //if we have not tried the path, we take it
                            if (!current.edges[0].node2.visited)
                            {
                                //update the cost of the path
                                //set the current to the new tile.
                                //push the current to the stack;
                                //add the node to the visited node stack;
                                cost += (int)current.edges[0].cost;
                                distance += 1;
                                current.visited = true;
                                current = current.edges[0].node2;
                                curr_path.Push(current);
                                visited_nodes.Push(current);
                            }

                        }
                        //More complex scenario. If the cost of the direct path is not 
                        //less than the cost of the alternate path, we need to investigate 
                        //alternative paths. Next look South West;
                        else if (current.edges[2] != null &&
                            current.edges[2].node2.traversible &&
                            current.edges[2].cost <= current.edges[1].cost &&
                            current.edges[2].cost <= current.edges[0].cost &&
                            current.edges[2].cost <= current.edges[3].cost)
                        {
                            //if we have not tried the path, we take it
                            if (!current.edges[2].node2.visited)
                            {
                                //update the cost of the path
                                //set the current to the new tile.
                                //push the current to the stack;
                                //add the node to the visited node stack;
                                cost += (int)current.edges[2].cost;
                                distance += 1;
                                current.visited = true;
                                current = current.edges[2].node2;
                                curr_path.Push(current);
                                visited_nodes.Push(current);
                            }

                        }
                        //More complex scenario. If the cost of the direct path is not 
                        //less than the cost of the alternate path, we need to investigate 
                        //alternative paths. Next look South East;
                        else if (current.edges[1] != null &&
                            current.edges[1].node2.traversible &&
                            current.edges[1].cost <= current.edges[2].cost &&
                            current.edges[1].cost <= current.edges[0].cost &&
                            current.edges[1].cost <= current.edges[3].cost)
                        {
                            if (!current.edges[1].node2.visited)
                            {
                                //update the cost of the path
                                //set the current to the new tile.
                                //push the current to the stack;
                                //add the node to the visited node stack;
                                cost += (int)current.edges[1].cost;
                                distance += 1;
                                current.visited = true;
                                current = current.edges[1].node2;
                                curr_path.Push(current);
                                visited_nodes.Push(current);
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
                        //if the current y id is less than the y end id we need to move South West.
                        if (current.id[1] < end.id[1])
                        {
                            //Simplest scenario. If the cost of the direct path is less than
                            //or equal to the cost of alternate paths, take the direct route
                            if (current.edges[2] != null &&
                                current.edges[2].node2.traversible &&
                                current.edges[2].cost <= current.edges[3].cost &&
                                current.edges[2].cost <= current.edges[0].cost &&
                                current.edges[2].cost <= current.edges[1].cost)
                            {
                                //if we have not tried the direct path, we take it
                                if (!current.edges[2].node2.visited)
                                {
                                    //update the cost of the path
                                    //update the distance of the path
                                    //mark the current tile as visited
                                    //set the current to the new tile.
                                    //push the current to the stack;
                                    //add the node to the visited node stack;
                                    cost += (int)current.edges[2].cost;
                                    distance += 1;
                                    current.visited = true;
                                    current = current.edges[2].node2;
                                    curr_path.Push(current);
                                    visited_nodes.Push(current);
                                }
                                //if we have tried this path, we may need to backtrack.
                                else
                                {
                                    cost -= (int)current.edges[1].cost;
                                    distance -= 1;
                                    current = current.edges[1].node2;
                                    curr_path.Pop();
                                }

                            }
                            //More complex scenario. If the cost of the direct path is not 
                            //less than the cost of the alternate path, we need to investigate 
                            //alternative paths. Prioritize looking South East;
                            else if (current.edges[1] != null &&
                                current.edges[1].node2.traversible &&
                                current.edges[1].cost <= current.edges[2].cost &&
                                current.edges[1].cost <= current.edges[0].cost &&
                                current.edges[1].cost <= current.edges[3].cost)
                            {
                                //if we have not tried the path, we take it
                                if (!current.edges[1].node2.visited)
                                {
                                    //update the cost of the path
                                    //set the current to the new tile.
                                    //push the current to the stack;
                                    //add the node to the visited node stack;
                                    cost += (int)current.edges[1].cost;
                                    distance += 1;
                                    current.visited = true;
                                    current = current.edges[1].node2;
                                    curr_path.Push(current);
                                    visited_nodes.Push(current);
                                }

                            }
                            //More complex scenario. If the cost of the direct path is not 
                            //less than the cost of the alternate path, we need to investigate 
                            //alternative paths. Next look North West;
                            else if (current.edges[3] != null &&
                                current.edges[3].node2.traversible &&
                                current.edges[3].cost <= current.edges[1].cost &&
                                current.edges[3].cost <= current.edges[2].cost &&
                                current.edges[3].cost <= current.edges[0].cost)
                            {
                                //if we have not tried the path, we take it
                                if (!current.edges[3].node2.visited)
                                {
                                    //update the cost of the path
                                    //set the current to the new tile.
                                    //push the current to the stack;
                                    //add the node to the visited node stack;
                                    cost += (int)current.edges[3].cost;
                                    distance += 1;
                                    current.visited = true;
                                    current = current.edges[3].node2;
                                    curr_path.Push(current);
                                    visited_nodes.Push(current);
                                }

                            }
                            //More complex scenario. If the cost of the direct path is not 
                            //less than the cost of the alternate path, we need to investigate 
                            //alternative paths. Next look North East;
                            else if (current.edges[0] != null &&
                                current.edges[0].node2.traversible &&
                                current.edges[0].cost <= current.edges[2].cost &&
                                current.edges[0].cost <= current.edges[3].cost &&
                                current.edges[0].cost <= current.edges[1].cost)
                            {
                                if (!current.edges[0].node2.visited)
                                {
                                    //update the cost of the path
                                    //set the current to the new tile.
                                    //push the current to the stack;
                                    //add the node to the visited node stack;
                                    cost += (int)current.edges[0].cost;
                                    distance += 1;
                                    current.visited = true;
                                    current = current.edges[0].node2;
                                    curr_path.Push(current);
                                    visited_nodes.Push(current);
                                }

                            }
                            //If None of the above are true, none of the adjacent tiles are traversible there is no path
                            else
                            {
                                return null;
                            }
                        }
                        //if the current y id is more than the y end id we need to move North East.
                        else
                        {
                            //Simplest scenario. If the cost of the direct path is less than
                            //or equal to the cost of alternate paths, take the direct route
                            if (current.edges[0] != null &&
                                current.edges[0].node2.traversible &&
                                current.edges[0].cost <= current.edges[2].cost &&
                                current.edges[0].cost <= current.edges[3].cost &&
                                current.edges[0].cost <= current.edges[1].cost)
                            {
                                //if we have not tried the direct path, we take it
                                if (!current.edges[3].node2.visited)
                                {
                                    //update the cost of the path
                                    //update the distance of the path
                                    //mark the current tile as visited
                                    //set the current to the new tile.
                                    //push the current to the stack;
                                    //add the node to the visited node stack;
                                    cost += (int)current.edges[3].cost;
                                    distance += 1;
                                    current.visited = true;
                                    current = current.edges[3].node2;
                                    curr_path.Push(current);
                                    visited_nodes.Push(current);
                                }
                                //if we have tried this path, we may need to backtrack.
                                else
                                {
                                    cost -= (int)current.edges[1].cost;
                                    distance -= 1;
                                    current = current.edges[1].node2;
                                    curr_path.Pop();
                                }

                            }
                            //More complex scenario. If the cost of the direct path is not 
                            //less than the cost of the alternate path, we need to investigate 
                            //alternative paths. Prioritize looking North West;
                            else if (current.edges[3] != null &&
                                current.edges[3].node2.traversible &&
                                current.edges[3].cost <= current.edges[1].cost &&
                                current.edges[3].cost <= current.edges[2].cost &&
                                current.edges[3].cost <= current.edges[0].cost)
                            {
                                //if we have not tried the path, we take it
                                if (!current.edges[3].node2.visited)
                                {
                                    //update the cost of the path
                                    //set the current to the new tile.
                                    //push the current to the stack;
                                    //add the node to the visited node stack;
                                    cost += (int)current.edges[3].cost;
                                    distance += 1;
                                    current.visited = true;
                                    current = current.edges[3].node2;
                                    curr_path.Push(current);
                                    visited_nodes.Push(current);
                                }

                            }
                            //More complex scenario. If the cost of the direct path is not 
                            //less than the cost of the alternate path, we need to investigate 
                            //alternative paths. Next look South East;
                            else if (current.edges[1] != null &&
                                current.edges[1].node2.traversible &&
                                current.edges[1].cost <= current.edges[2].cost &&
                                current.edges[1].cost <= current.edges[0].cost &&
                                current.edges[1].cost <= current.edges[3].cost)
                            {
                                //if we have not tried the path, we take it
                                if (!current.edges[1].node2.visited)
                                {
                                    //update the cost of the path
                                    //set the current to the new tile.
                                    //push the current to the stack;
                                    //add the node to the visited node stack;
                                    cost += (int)current.edges[1].cost;
                                    distance += 1;
                                    current.visited = true;
                                    current = current.edges[1].node2;
                                    curr_path.Push(current);
                                    visited_nodes.Push(current);
                                }

                            }
                            //More complex scenario. If the cost of the direct path is not 
                            //less than the cost of the alternate path, we need to investigate 
                            //alternative paths. Next look South West;
                            else if (current.edges[2] != null &&
                                current.edges[2].node2.traversible &&
                                current.edges[2].cost <= current.edges[1].cost &&
                                current.edges[2].cost <= current.edges[0].cost &&
                                current.edges[2].cost <= current.edges[3].cost)
                            {
                                if (!current.edges[2].node2.visited)
                                {
                                    //update the cost of the path
                                    //set the current to the new tile.
                                    //push the current to the stack;
                                    //add the node to the visited node stack;
                                    cost += (int)current.edges[2].cost;
                                    distance += 1;
                                    current.visited = true;
                                    current = current.edges[2].node2;
                                    curr_path.Push(current);
                                    visited_nodes.Push(current);
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
                    //if the current x index is greater than the end x id then we need to move north west.
                    //else if (current.id[0] > end.id[0])
                    //{
                        //if (current.edges[1].cost <= current.edges[2].cost && current.edges[1].cost <= current.edges[0].cost)
                        //{
                            //cost += current.edges[1].cost
                        //}
                    //}
                //}else
                //{

                //}
                /*
                if (curr_diff_x != 0)
                {

                }
                foreach (Edge e in current.edges)
                {
                    if (e.node2.traversible && Mathf.Abs(end.id[0]-current.id[0]) < diffX)
                    {
                        current.weight = cost;
                        cost = cost + (int)e.cost;
                        current = e.node2;
                        curr_path.Push(current);
                        visitedNodes.Add(current);
                    }
                    else if (e.node2.traversible && Mathf.Abs(end.id[1] - current.id[1]) < diffY)
                    {
                        current.weight = cost;
                        cost = cost + (int)e.cost;
                        current = e.node2;
                        curr_path.Push(current);
                        visitedNodes.Add(current);
                    }
                }
            }
            return curr_path;
            /*if ()
            {
                return curr_path;
            }

            foreach (Edge e in start.edges)
            {
                
            }
            else if ()
            {

            }
            Node current = start;
            Stack <Node> path  = new Stack<Node>();

            //Debug.Log("Current Node: " + current.id[0] + "," + current.id[1] + "; Cost: " + current.weight);
            while (current.id[0] != end.id[0] && current.id[1] != end.id[1])
            {
                foreach
            }
            return curr_path;
        }*/

        public int shortestPathCost()
        {
            return 2;
        }

        public void bfs(Node start, int cost_limit, int distace_limit)
        {
            //reset the previously visited nodes
            Node n;
            while (visitedNodes.Count != 0)
            {
               n = visitedNodes.Pop();
                n.weight = -1;
                n.parent = null;
                n.visited = false;
            }
            visitedNodes = new Stack<Node>();
            Queue<Node> queue = new Queue<Node>();
            Node current;
            start.weight = 0;
            start.distance = 0;
            queue.Enqueue(start);
            visitedNodes.Push(start);
            while (queue.Count > 0)
            {
                current = queue.Dequeue();
                //Debug.Log("Current Node: " + current.id[0] + "," + current.id[1] + "; Cost: " + current.weight);
                foreach (Edge e in current.edges){
                    
                    if(e != null)
                    {
                        if (!e.node2.visited && 
                            current.weight + e.cost <= cost_limit && 
                            (! e.node2.obj || e.node2.obj.tag == start.obj.tag) && 
                            current.distance < distace_limit)
                        {
                            //Debug.Log("Cost:" + e.cost);
                            e.node2.weight = current.weight + e.cost;
                            e.node2.distance = current.distance + 1;
                            e.node2.visited = true;
                            e.node2.parent = current;
                            queue.Enqueue(e.node2);
                            visitedNodes.Push(e.node2);
                        }
                        //If the node has been visited before and 
                        //it is not the starting node, but 
                        //we found a shorter path to the node we still update it.
                        if (e.node2.visited && 
                            e.node2.id[0] != start.id[0] && 
                            e.node2.id[1] != start.id[1] && 
                            e.node2.weight > current.weight + e.cost && 
                            e.node2.distance < distace_limit)
                        {
                            e.node2.weight = current.weight + e.cost;
                            e.node2.distance = current.distance + 1;
                            e.node2.parent = current;
                            queue.Enqueue(e.node2);
                        }
                    }
                    
                }
            }
        }
    }
}
