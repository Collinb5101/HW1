using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs search using A*.
/// </summary>
public class AStar : MonoBehaviour
{
    // Colors for the different search categories.
    public static Color openColor = Color.cyan;
    public static Color closedColor = Color.blue;
    public static Color activeColor = Color.yellow;
    public static Color pathColor = Color.yellow;

    // The stopwatch for timing search.
    private static Stopwatch watch = new Stopwatch();

    public static IEnumerator search(GameObject start, GameObject end, Heuristic heuristic, float waitTime, bool colorTiles = false, bool displayCosts = false, Stack<NodeRecord> path = null)
    {
        // Starts the stopwatch.
        watch.Start();

        // Add your A* code here.
        NodeRecord startRecord = new NodeRecord();
        NodeRecord currentRecord = new NodeRecord();

        startRecord.Tile = start;
        startRecord.Node = startNode;
        startRecord.connection = null;
        startRecord.CostSoFar = 0;
        startRecord.EstimatedCostSoFar = heuristic(start);

        float scale = startRecord.Tile.transform.localScale.x;

        //creates an open list of node records and adds the start node to it
        List<NodeRecord> open = new List<NodeRecord>();
        open.Add(startRecord);

        //creates a closed list of node records
        List<NodeRecord> closed = new List<NodeRecord>();

        while (open.Count > 0)
        {
            //initialize a variable for the current node and lowest cost
            //currentNode = new NodeRecord();
            float lowestCost = float.MaxValue;

            //for every element inside the open list
            for (int i = 0; i < open.Count; i++)
            {
                //if the current cost of this node is the lowest cost
                if (open[i].CostSoFar < lowestCost)
                {
                    //this cost becomes the lowest cost and this node becomes the current node
                    lowestCost = open[i].CostSoFar;
                    currentRecord = open[i];
                }
            }

            //if colorTiles is true
            if (colorTiles)
            {
                //color the current node to be the active node color
                currentRecord.ColorTile(activeColor);
            }

            //wait for specified amount of time
            yield return new WaitForSeconds(waitTime);

            //if the current node is the goal node
            if (currentRecord.Tile == end)
            {
                //break the while early
                break;
            }


            //for every connection in the connection list
            foreach (KeyValuePair<Direction, GameObject> connection in currentRecord.Node.Connections)
            {
                //get the cost estimate for the end node
                //endNode = connection.GetComponent<NodeRecord>().Node;
                Node endNode = connection.Value.GetComponent<Node>();
                float endNodeCost = currentRecord.CostSoFar + 1;

                bool exitEarly = false;
                NodeRecord endNodeRecord = new NodeRecord();

                foreach (NodeRecord nodeRecord in closed)
                {
                    if (nodeRecord.Node == endNode)
                    {
                        exitEarly = true;
                        break;
                    }
                }

                //if the end node is in the closed list
                if (exitEarly)
                {
                    continue;
                }
                else
                {
                    //if the end node is in the open list
                    foreach (NodeRecord nodeRecord in open)
                    {
                        if (nodeRecord.Node == endNode)
                        {
                            exitEarly = true;
                            endNodeRecord = nodeRecord;
                            break;
                        }

                    }

                    if (exitEarly)
                    {
                        if (endNodeRecord.CostSoFar <= endNodeCost)
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
                    endNodeRecord.connection = currentRecord.Node.Connections;
                    endNodeRecord.Tile = connection.Value;

                    if (displayCosts)
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
            if (colorTiles && currentRecord.Node != startRecord.Node && currentRecord.Node != end.GetComponent<Node>())
            {
                currentRecord.ColorTile(closedColor);
            }
        }
        // Stops the stopwatch.
        watch.Stop();

        UnityEngine.Debug.Log("Seconds Elapsed: " + (watch.ElapsedMilliseconds / 1000f).ToString());
        UnityEngine.Debug.Log("Nodes Expanded: " + "print the number of nodes expanded here.");

        // Reset the stopwatch.
        watch.Reset();

        // Determine whether A* found a path and print it here.

        yield return null;
    }

    public delegate float Heuristic(GameObject start, GameObject tile, GameObject goal);

    public static float Uniform (GameObject start, GameObject tile, GameObject goal)
    {
        return 0f;
    }

    public static float Manhattan (GameObject start, GameObject tile, GameObject goal)
    {
        return 0f;
    }

    public static float CrossProduct (GameObject start, GameObject tile, GameObject goal)
    {
        return 0f;
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
    public Dictionary<Direction, GameObject> Connection;
    public float CostSoFar { get; set; } = 0;
    public float EstimatedCostSoFar { get; set; } = 0;

    // Sets the tile's color.
    public void ColorTile(Color newColor)
    {
        SpriteRenderer renderer = Tile.GetComponentInChildren<SpriteRenderer>();
        renderer.material.color = newColor;
    }

    // Displays a string on the tile.
    public void Display(float value)
    {
        TextMesh text = Tile.GetComponent<TextMesh>();
        text.text = value.ToString();
    }
}
