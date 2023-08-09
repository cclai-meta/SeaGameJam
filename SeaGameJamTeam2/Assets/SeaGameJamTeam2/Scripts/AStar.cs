using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Animations;
using Utils;

public class AStar
{
    
    class Node
    {
        public Vector3Int loc;
        public float costToStartFromStart;
        public float estimatedCostToGoal;

        public float totalCost
        {
            get { return costToStartFromStart + estimatedCostToGoal; }
        }
        
        public Node parent;
        

        public Node(Vector3Int newLoc, float nextCostToStart, float nextEstimatedCostToGoal, Node newParent)
        {
            loc = newLoc;
            parent = newParent;
            costToStartFromStart = nextCostToStart;
            estimatedCostToGoal = nextEstimatedCostToGoal;
        }
    }
    
    private PriorityQueue<Node, float> openNodes = new PriorityQueue<Node, float>();
    private HashSet<Vector3Int> openLocations = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> visitedLocations = new HashSet<Vector3Int>();
    private Grid grid;

    public AStar(Grid newGrid)
    {
        grid = newGrid;
    }

    public List<Vector3Int> AStarAlgorithm(Vector3Int startNode, Vector3Int goalNode)
    {
        // Add the starting node to the open list.
        // Assign a cost of zero to the starting node.
        // Assign a heuristic cost (usually the estimated distance to the goal) to the starting node.
        Node currentNode = new Node(startNode, 0, CalculateDistance(startNode, goalNode), null);
        openNodes.Enqueue(currentNode, currentNode.totalCost); // Enqueue the start node with priority 0
        openLocations.Add(startNode);

        // While the open list is not empty:
        while (openNodes.Count > 0)
        {
            // Find the node with the lowest total cost (cost + heuristic) in the open list. This node will be the current node.
            // Remove the current node from the open list and add it to the closed list.
            currentNode = openNodes.Dequeue(); // Get the node with the lowest f-cost

            // Check for Goal:
            if (currentNode.loc == goalNode)
            {        
                // Path Construction:
                //
                // Once the algorithm terminates and a path is found:
                // Traverse the parent references from the goal node to the start node, constructing the final path.
                // If the current node is the goal node:
                // Construct the path by backtracking from the goal node to the start node using parent references.
                // The path is complete. Stop the algorithm.
                // Generate Successors:
                List<Vector3Int> path = new();
                do
                {
                    path.Add(currentNode.loc);
                    currentNode = currentNode.parent;
                } while (currentNode != null);

                return path;
            }

            visitedLocations.Add(currentNode.loc);
            openLocations.Remove(currentNode.loc);

            // For each neighbor of the current node:
            foreach (Vector3Int neighbor in grid.GetOpenNeighbors(currentNode.loc))
            {
                // If the neighbor is in the closed list, skip it.
                if (visitedLocations.Contains(neighbor))
                {
                    //Skip because we've already evaluated this position.
                    continue;
                }
                
                float tentativeCostFromStart = currentNode.costToStartFromStart + CalculateDistance(currentNode.loc, neighbor);

                if (openLocations.Contains(neighbor))
                {
                    // If the neighbor is already in the open list:
                    // Check if the new calculated cost is lower than the recorded cost for the neighbor.
                    bool found = false;
                    foreach ((Node node, _) in openNodes.UnorderedItems)
                    {
                        if (node.loc == neighbor)
                        {
                            found = true;
                            if (tentativeCostFromStart < node.costToStartFromStart)
                            {
                                // If so, update the cost and parent.
                                node.costToStartFromStart = tentativeCostFromStart;
                                node.parent = currentNode;
                            }

                            break;
                        }
                    }

                    if (found)
                    {
                        continue;
                    }
                }
                
                // If the neighbor is not in the open list:
                // Add it to the open list.
                // Set the parent of the neighbor to the current node.
                // Calculate the cost from the start node to the neighbor.
                // Calculate the heuristic cost from the neighbor to the goal.
                if (!openLocations.Contains(neighbor))
                {
                    Node neighborNode = new Node(neighbor, tentativeCostFromStart, tentativeCostFromStart, currentNode);
                    openNodes.Enqueue(neighborNode, neighborNode.totalCost);
                    openLocations.Add(neighborNode.loc);
                }
            }
            //
            // Repeat:
            // Go back to the main loop.
        }
        // Termination:
        //
        // If the open list is empty and the goal node has not been reached, there is no valid path. The algorithm terminates.

        // Optimizations:
        //
        // A* can be further optimized using techniques like binary heaps for efficient priority queue management, and techniques to reduce redundant node evaluation.
        return new List<Vector3Int>();
    }

    public float CalculateDistance(Vector3Int cellA, Vector3Int cellB)
    {
        return Mathf.Abs(cellA.x - cellB.x) + Mathf.Abs(cellA.y - cellB.y) + Mathf.Abs(cellA.z - cellB.z);
    }

    // ...
}