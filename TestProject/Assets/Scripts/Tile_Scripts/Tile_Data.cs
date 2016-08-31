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
	}
	
	public Tile_Data(int x, int y, int height){
		x_index = x;
		y_index = y;
		node.height = height;
		tile_sprite_index = 1;
		node.traversible = true;
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
        public double weight;
        public bool traversible;
        public Node parent;
        public int height;
        public int modifier;
        public int[] id;
        public List<Edge> edges;

        public Node(int newHeight, int newModifier, int[] newID)
        {
            height = newHeight;
            modifier = newModifier;
            id = newID;
            edges = new List<Edge>();
            weight = -1;
            traversible = true;
        }

        public void addEdge(Node n)
        {
            Edge e = new Edge(this, n);
            edges.Add(e);
            Edge e1 = new Edge(n, this);
            n.edges.Add(e1);
        }

        public void printEdges()
        {
            foreach (Edge e in edges)
            {
                Debug.Log("Node (" + id[0] + "," + id[1] + ") Connects to Node: (" + e.node2.id[0] + "," + e.node2.id[1] + ") with a cost of " + e.cost);
            }
        }
    }

    public class Edge
    {
        public double cost;
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
    }

    public class Graph
    {
        public List<Node> nodes;

        public Graph()
        {
            nodes = new List<Node>();
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

        public void bfs(Node start, int limit)
        {
            foreach (Node n in nodes)
            {
                n.weight = -1;
                n.parent = null;
            }
            Queue<Node> queue = new Queue<Node>();
            Node current;
            start.weight = 0;
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                current = queue.Dequeue();
                Debug.Log("Current Node: " + current.id[0] + "," + current.id[1] + "; Cost: " + current.weight);
                foreach (Edge e in current.edges){
                    if (e.node2.weight == -1 && current.weight + e.cost <= limit && e.node2.traversible) {
                        //Debug.Log("Cost:" + e.cost);
                        e.node2.weight = current.weight + e.cost;
                        e.node2.parent = current;
                        queue.Enqueue(e.node2);
                    }
                    //If the node has been visited before and it is not the starting node, but we found a shorter path to the node we still update it.
                    if (e.node2.weight > 0 && e.node2.weight > current.weight + e.cost && e.node2.traversible)
                    {
                        e.node2.weight = current.weight + e.cost;
                        e.node2.parent = current;
                        queue.Enqueue(e.node2);
                    }
                }
            }
        }
    }
}
