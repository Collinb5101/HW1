using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.MemoryProfiler;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;
using System.Linq;

/// <summary>
/// Performs search using Dijkstra's algorithm.
/// </summary>
public class Dijkstra : MonoBehaviour
{
    // Colors for the different search categories.
    public static Color openColor = Color.cyan;
    public static Color closedColor = Color.blue;
    public static Color activeColor = Color.yellow;
    public static Color pathColor = Color.yellow;

    // The stopwatch for timing search.
    private static Stopwatch watch = new Stopwatch();


    public static IEnumerator search(GameObject start, GameObject end, float waitTime, bool colorTiles = false, bool displayCosts = false, Stack<NodeRecord> path = null)
    {
        // Starts the stopwatch.
        watch.Start();

        // Add your Dijkstra code here.
        Node startNode = start.GetComponent<Node>();

        //creates a new node record for start node and end node and current node
        NodeRecord startRecord = new NodeRecord();
        NodeRecord currentNode = new NodeRecord();

        //initializes start node
        startRecord.Tile = start;
        startRecord.Node = startNode;
        startRecord.connection = null;
        startRecord.CostSoFar = 0;

        //startRecord.Node.printConnections();

        //creates an open list of node records and adds the start node to it
        List<NodeRecord> open = new List<NodeRecord>();
        open.Add(startRecord);

        //creates a closed list of node records
        List<NodeRecord> closed = new List<NodeRecord>();

        while(open.Count > 0)
        {
            //initialize a variable for the current node and lowest cost
            //currentNode = new NodeRecord();
            float lowestCost = float.MaxValue;

            //for every element inside the open list
            for(int i = 0; i < open.Count; i++)
            {
                //if the current cost of this node is the lowest cost
                if (open[i].CostSoFar < lowestCost)
                {
                    //this cost becomes the lowest cost and this node becomes the current node
                    lowestCost = open[i].CostSoFar;
                    currentNode = open[i];
                }
            }

            //if colorTiles is true
            if(colorTiles)
            {
                //color the current node to be the active node color
                currentNode.ColorTile(activeColor);
            }

            //wait for specified amount of time
            yield return new WaitForSeconds(waitTime);

            //if the current node is the goal node
            if(currentNode.Tile == end)
            {
                //break the while early
                break;
            }


            //for every connection in the connection list
            foreach (KeyValuePair<Direction, GameObject> connection in currentNode.Node.Connections)
            {
                //get the cost estimate for the end node
                //endNode = connection.GetComponent<NodeRecord>().Node;
                Node endNode = connection.Value.GetComponent<Node>();
                float endNodeCost = currentNode.CostSoFar + 1;

                bool exitEarly = false;
                NodeRecord endNodeRecord = new NodeRecord();

                foreach(NodeRecord nodeRecord in closed)
                {
                    if(nodeRecord.Node == endNode)
                    {
                        exitEarly = true;
                        break;
                    }
                }

                //if the end node is in the closed list
                if(exitEarly)
                {
                    continue;
                }
                else
                {
                    //if the end node is in the open list
                    foreach (NodeRecord nodeRecord in open)
                    {
                        if(nodeRecord.Node == endNode)
                        {
                            exitEarly = true;
                            endNodeRecord = nodeRecord;
                            break;
                        }

                    }

                    if(exitEarly)
                    {
                        if(endNodeRecord.CostSoFar <= endNodeCost)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        endNodeRecord = new NodeRecord();
                        endNodeRecord.Node = endNode;
                    }

                    endNodeRecord.CostSoFar = endNodeCost;
                    endNodeRecord.connection = currentNode.Node.Connections;
                    endNodeRecord.Tile = connection.Value;

                    if(displayCosts)
                    {
                        endNodeRecord.Display(endNodeCost);
                    }

                    // Open Records did not contain the end node 
                    if (!exitEarly)
                    {
                        open.Add(endNodeRecord);
                    }

                    // Tile Color
                    if (colorTiles && endNodeRecord.Node != startRecord.Node && endNodeRecord.Node != end.GetComponent<Node>())
                    {
                        endNodeRecord.ColorTile(openColor);
                    }

                    yield return new WaitForSeconds(waitTime);
                }
            }

            //remove the current node from the open list and put it 
            //into the closed list
            open.Remove(currentNode);
            closed.Add(currentNode);

            //color the closed tiles
            if (colorTiles && currentNode.Node != startRecord.Node && currentNode.Node != end.GetComponent<Node>())
            {
                currentNode.ColorTile(closedColor);
            }
        }

        // Stops the stopwatch.
        watch.Stop();

        UnityEngine.Debug.Log("Seconds Elapsed: " + (watch.ElapsedMilliseconds / 1000f).ToString());
        UnityEngine.Debug.Log("Nodes Expanded: " + closed.Count.ToString());

        // Reset the stopwatch.
        watch.Reset();


        if(path==null)
        {
            path = new Stack<NodeRecord>();
        }

        // Determine whether Dijkstra found a path and print it here.
        if (currentNode == null || currentNode.Node != end.GetComponent<Node>())
        {
            UnityEngine.Debug.Log("you fucked up");
        }
        else
        {
            while(currentNode != startRecord)
            {
                path.Push(currentNode);
                foreach(NodeRecord nodeRecord in closed)
                {
                    if(nodeRecord.Node.Connections == currentNode.connection)
                    {
                        currentNode = nodeRecord;
                        break;
                    }
                }


                if (colorTiles && currentNode.Node != startRecord.Node && currentNode.Node != end.GetComponent<Node>())
                {
                    currentNode.ColorTile(pathColor);
                }

                yield return new WaitForSeconds(waitTime);
            }
            UnityEngine.Debug.Log("Path Length: " + path.Count.ToString());
        }

        yield return null;
    }
}

/// <summary>
/// A class for recording search statistics.
/// </summary>
public class NodeRecord
{
    // The tile game object.
    public GameObject Tile { get; set; } = null;

    // Set the other class properties here.
    public Node Node { get; set; } = null;
    public Dictionary<Direction, GameObject> connection;
    public float CostSoFar { get; set; } = 0;

    // Sets the tile's color.
    public void ColorTile (Color newColor)
    {
        SpriteRenderer renderer = Tile.GetComponentInChildren<SpriteRenderer>();
        renderer.material.color = newColor;
    }

    // Displays a string on the tile.
    public void Display (float value)
    {
        TextMesh text = Tile.GetComponent<TextMesh>();
        text.text = value.ToString();
    }
}
