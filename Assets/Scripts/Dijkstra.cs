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

        //creates a new node record for start node
        NodeRecord startRecord = new NodeRecord();
        startRecord.node = start.GetComponent<Node>();
        startRecord.connection = null;
        startRecord.CostSoFar = 0;

        //creates an open list of node records and adds the start node to it
        List<NodeRecord> open = new List<NodeRecord>();
        open.Add(startRecord);

        //creates a closed list of node records
        List<NodeRecord> closed = new List<NodeRecord>();

        while(open.Count > 0)
        {
            //initialize a variable for the current node and lowest cost
            NodeRecord currentNode= new NodeRecord();
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
            if(currentNode.node = end.GetComponent<Node>())
            {
                //break the while early
                break;
            }
        }

        // Stops the stopwatch.
        watch.Stop();

        UnityEngine.Debug.Log("Seconds Elapsed: " + (watch.ElapsedMilliseconds / 1000f).ToString());
        UnityEngine.Debug.Log("Nodes Expanded: " + "print the number of nodes expanded here.");

        // Reset the stopwatch.
        watch.Reset();

        // Determine whether Dijkstra found a path and print it here.

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
    public Node node = null;
    public GameObject connection = null;
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
