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

        //create and initialize the start record
        startRecord.Tile = start;
        startRecord.Node = start.GetComponent<Node>();
        startRecord.Connection = null;
        startRecord.CostSoFar = 0;
        startRecord.EstimatedCostSoFar = heuristic(start, startRecord.Tile, end);

        //set the scale
        float scale = startRecord.Tile.transform.localScale.x;

        //creates an open list of node records and adds the start node to it
        List<NodeRecord> open = new List<NodeRecord>();
        open.Add(startRecord);

        //creates a closed list of node records
        List<NodeRecord> closed = new List<NodeRecord>();

        //while the open list isn't empty
        while (open.Count > 0)
        {
            //initialize a variable for the lowest cost
            float lowestCost = float.MaxValue;

            //for every element inside the open list
            for (int i = 0; i < open.Count; i++)
            {
                //if the current estimated cost of this node is the lowest cost
                if (open[i].EstimatedCostSoFar < lowestCost)
                {
                    //this cost becomes the lowest cost and this node becomes the current node
                    lowestCost = open[i].EstimatedCostSoFar;
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
                //get the cost estimate for the end node and set the connection
                Node endNode = connection.Value.GetComponent<Node>();
                float endNodeCost = currentRecord.CostSoFar + scale;

                //initialize the node record and the exit condition
                bool exitEarly = false;
                NodeRecord endNodeRecord = new NodeRecord();

                //check every node in the closed list
                foreach (NodeRecord nodeRecord in closed)
                {
                    //if the node is the end node
                    if (nodeRecord.Node == endNode)
                    {
                        //set it
                        endNodeRecord = nodeRecord;

                        //if the current cost is the best cost so far
                        if(endNodeRecord.CostSoFar <= endNodeCost)
                        {
                            //exit early
                            exitEarly = true;
                            break;
                        }
                        else
                        {
                            //if it's worse then remove it from the list
                            closed.Remove(endNodeRecord);
                            break;
                        }
                    }
                }
                //check every node in the open list
                foreach (NodeRecord nodeRecord in open)
                {
                    //if the node is the end node
                    if (nodeRecord.Node == endNode)
                    {
                        //set it
                        endNodeRecord = nodeRecord;

                        //if the current cost is the best cost so far
                        if (endNodeRecord.CostSoFar <= endNodeCost)
                        {
                            //exit early
                            exitEarly = true;
                            break;
                        }
                        else
                        {
                            //if it's worse then remove it from the list
                            open.Remove(endNodeRecord);
                            break;
                        }
                    }
                }

                //if true then exit this check of the connection loop early
                if(exitEarly)
                {
                    continue;
                }
                else
                {
                    //otherwise initialize a new end node record
                    endNodeRecord = new NodeRecord();
                    endNodeRecord.Node = endNode;
                    endNodeRecord.Tile = endNode.gameObject;
                    endNodeRecord.EstimatedCostSoFar = heuristic(start, endNodeRecord.Tile, end);
                }

                //update the cost and the connection
                endNodeRecord.CostSoFar = endNodeCost;
                endNodeRecord.Connection = currentRecord.Node.Connections;

                if(displayCosts)
                {
                    endNodeRecord.Display(endNodeCost);
                }

                //check if the end node is in the open list
                bool containsEndNode = false;
                foreach (NodeRecord nodeRecord in open)
                {
                    if (nodeRecord.Node == endNode)
                    {
                        containsEndNode= true;
                    }
                }

                //if the node is not in the open list then add it
                if(!containsEndNode)
                {
                    open.Add(endNodeRecord);
                }

                if(colorTiles)
                {
                    endNodeRecord.ColorTile(openColor);
                }
                yield return new WaitForSeconds(waitTime);

            }

            //remove the current node from the open list and put it 
            //into the closed list
            open.Remove(currentRecord);
            closed.Add(currentRecord);

            //color the closed tiles
            if (colorTiles)
            {
                currentRecord.ColorTile(closedColor);
            }
        }
        // Stops the stopwatch.
        watch.Stop();

        UnityEngine.Debug.Log("Seconds Elapsed: " + (watch.ElapsedMilliseconds / 1000f).ToString());
        UnityEngine.Debug.Log("Nodes Expanded: " + closed.Count.ToString());

        // Reset the stopwatch.
        watch.Reset();

        // Determine whether A* found a path and print it here.
        if (path == null)
        {
            path = new Stack<NodeRecord>();
        }

        // Determine whether Dijkstra found a path and print it here.
        if (currentRecord == null || currentRecord.Node != end.GetComponent<Node>())
        {
            UnityEngine.Debug.Log("you fucked up");
        }
        else
        {
            //starting at the end node loop until you get to the start node
            while (currentRecord != startRecord)
            {
                //push the record to the path
                path.Push(currentRecord);

                //check the closed list
                foreach (NodeRecord nodeRecord in closed)
                {
                    //if there is a connection between nodes
                    if (nodeRecord.Node.Connections == currentRecord.Connection)
                    {
                        //step to the next node in the path
                        currentRecord = nodeRecord;
                        break;
                    }
                }


                if (colorTiles)
                {
                    currentRecord.ColorTile(pathColor);
                }

                yield return new WaitForSeconds(waitTime);
            }
            UnityEngine.Debug.Log("Path Length: " + path.Count.ToString());
        }

        yield return null;
    }

    public delegate float Heuristic(GameObject start, GameObject tile, GameObject goal);

    public static float Uniform (GameObject start, GameObject tile, GameObject goal)
    {
        return 0f;
    }

    public static float Manhattan (GameObject start, GameObject tile, GameObject goal)
    {
        //initialize values
        float distX, distY;

        //calculate the absolute distances of each
        distX = Mathf.Abs(tile.transform.position.x - goal.transform.position.x);
        distY = Mathf.Abs(tile.transform.position.y - goal.transform.position.y);

        //return the sum
        return distX + distY;
    }

    public static float CrossProduct (GameObject start, GameObject tile, GameObject goal)
    {
        //initialize values
        float distX1, distX2, distY1, distY2, cross;

        //calculate the distances
        distX1 = tile.transform.position.x - goal.transform.position.x;
        distY1 = tile.transform.position.y - goal.transform.position.y;

        distX2 = start.transform.position.x - goal.transform.position.x;
        distY2 = start.transform.position.y - goal.transform.position.y;

        //calculate the cross product of the distances
        cross = Mathf.Abs(distX1 * distY2 - distX2 * distY1);

        //plug the result into the manhattan heuristic and return the value
        return Manhattan(start, tile, goal) + cross * 0.001f;
    }
}
