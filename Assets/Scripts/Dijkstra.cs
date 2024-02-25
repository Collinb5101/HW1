using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.MemoryProfiler;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;

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
        Node endNode = end.GetComponent<Node>();

        //creates a new node record for start node and end node and current node
        NodeRecord startRecord = new NodeRecord();
        NodeRecord endNodeRecord = new NodeRecord();
        NodeRecord currentNode = new NodeRecord();

        //initializes start node
        startRecord.Tile = start;
        startRecord.Node = startNode;
        startRecord.Connection = null;
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
            if(currentNode.Node = endNode)
            {
                //break the while early
                break;
            }

            //for every connection in the connection list
            foreach(GameObject connection in currentNode.Node.Connections.Values)
            {
                //get the cost estimate for the end node
                endNode = connection.GetComponent<NodeRecord>().Node;
                float endNodeCost = currentNode.CostSoFar + connection.GetComponent<NodeRecord>().CostSoFar;

                //if the end node is in the closed list
                if(closed.Contains(endNode.GetComponent<NodeRecord>())) 
                {
                    //skip
                    continue;
                }
                //if the end node is in the open list
                else if(open.Contains(endNode.GetComponent<NodeRecord>()))
                {
                    //loop through the open list
                    foreach(NodeRecord node in open)
                    {
                        //when found set the end node and exit the loop
                        if(node.Node == endNode)
                        {
                            endNodeRecord = node;
                            break;
                        }
                    }
                    //if the cost of the end node record cost is less than the end node cost keep going
                    if (endNodeRecord.CostSoFar <= endNodeCost)
                    {
                        continue;
                    }

                }
                else
                {
                    //endNodeRecord = new NodeRecord();
                    endNodeRecord.Node = endNode;
                }

                //update the cost and the connection
                endNodeRecord.CostSoFar = endNodeCost;
                endNodeRecord.Connection = connection;

                //display the costs
                if(displayCosts)
                {
                    endNodeRecord.Display(endNodeCost);
                }

                //if the end node is not in the open list
                if(!open.Contains(endNode.GetComponent<NodeRecord>()))
                {
                    //add it
                    open.Add(endNode.GetComponent<NodeRecord>());
                }

                //color the open tiles
                if(colorTiles)
                {
                    endNodeRecord.ColorTile(openColor);
                }
                yield return new WaitForSeconds(waitTime);
            }

            //remove the current node from the open list and put it 
            //into the closed list
            open.Remove(currentNode);
            closed.Add(currentNode);

            //color the closed tiles
            if (colorTiles)
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

        // Determine whether Dijkstra found a path and print it here.
        if (currentNode.Node != endNode)
        {
            UnityEngine.Debug.Log("you fucked up");
        }
        else
        {
            while(currentNode.Node != startNode)
            {
                path.Push(currentNode);
                currentNode = currentNode.Connection.GetComponent<NodeRecord>();

                if (colorTiles)
                {
                    endNodeRecord.ColorTile(pathColor);
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
    public GameObject Connection { get; set; } = null;
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
