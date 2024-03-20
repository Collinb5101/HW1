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

        float endNodeHeuristic = 0;

        startRecord.Tile = start;
        startRecord.Node = start.GetComponent<Node>();
        startRecord.Connection = null;
        startRecord.CostSoFar = 0;
        startRecord.EstimatedCostSoFar = heuristic(start, startRecord.Tile, end);

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
                //get the cost estimate for the end node
                //endNode = connection.GetComponent<NodeRecord>().Node;
                Node endNode = connection.Value.GetComponent<Node>();
                float endNodeCost = currentRecord.CostSoFar + scale;

                bool exitEarly = false;
                NodeRecord endNodeRecord = new NodeRecord();

                foreach (NodeRecord nodeRecord in closed)
                {
                    if (nodeRecord.Node == endNode)
                    {
                        endNodeRecord = nodeRecord;

                        //seems to never be greater than endnodecost
                        if(endNodeRecord.CostSoFar <= endNodeCost)
                        {
                            exitEarly = true;
                            break;
                        }
                        else
                        {
                            closed.Remove(endNodeRecord);
                            //endNodeHeuristic = endNodeRecord.EstimatedCostSoFar - endNodeRecord.CostSoFar;

                            //endNodeRecord.EstimatedCostSoFar = endNodeHeuristic;
                            break;
                        }
                    }
                }
                foreach (NodeRecord nodeRecord in open)
                {
                    
                    if (nodeRecord.Node == endNode)
                    {
                        endNodeRecord = nodeRecord;

                        if (endNodeRecord.CostSoFar <= endNodeCost)
                        {
                            exitEarly = true;
                            break;
                        }
                        else
                        {
                            open.Remove(endNodeRecord);
                            //endNodeHeuristic = endNodeRecord.EstimatedCostSoFar - endNodeRecord.CostSoFar;

                            //endNodeRecord.EstimatedCostSoFar = endNodeHeuristic;
                            break;
                        }
                    }
                }

                if(exitEarly)
                {
                    continue;
                }
                else
                {
                    endNodeRecord = new NodeRecord();
                    endNodeRecord.Node = endNode;
                    endNodeRecord.Tile = endNode.gameObject;
                    //endNodeHeuristic = heuristic(start, currentRecord.Tile, end);
                    endNodeRecord.EstimatedCostSoFar = heuristic(start, endNodeRecord.Tile, end);
                }

                endNodeRecord.CostSoFar = endNodeCost;
                endNodeRecord.Connection = currentRecord.Node.Connections;
                //endNodeHeuristic = endNodeCost + endNodeHeuristic;

                //endNodeRecord.EstimatedCostSoFar = endNodeHeuristic;

                if(displayCosts)
                {
                    endNodeRecord.Display(endNodeCost);
                }

                bool containsEndNode = false;
                foreach (NodeRecord nodeRecord in open)
                {
                    if (nodeRecord.Node == endNode)
                    {
                        containsEndNode= true;
                    }
                }
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
            while (currentRecord != startRecord)
            {
                path.Push(currentRecord);
                foreach (NodeRecord nodeRecord in closed)
                {
                    if (nodeRecord.Node.Connections == currentRecord.Connection)
                    {
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
        float distX, distY;
        distX = Mathf.Abs(tile.transform.position.x - goal.transform.position.x);
        distY = Mathf.Abs(tile.transform.position.y - goal.transform.position.y);

        return distX + distY;
    }

    public static float CrossProduct (GameObject start, GameObject tile, GameObject goal)
    {
        float distX1, distX2, distY1, distY2, cross;

        distX1 = tile.transform.position.x - goal.transform.position.x;
        distY1 = tile.transform.position.y - goal.transform.position.y;

        distX2 = start.transform.position.x - goal.transform.position.x;
        distY2 = start.transform.position.y - goal.transform.position.y;

        cross = Mathf.Abs(distX1 * distY2 - distX2 * distY1);

        return Manhattan(start, tile, goal) + cross * 0.001f;
    }
}
