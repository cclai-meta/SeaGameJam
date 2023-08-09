using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Animations;
using Utils;

public class AStar
{
    
    class Node
    {
        public Vector3Int loc;
        public float costToNodeFromStart;
        public float estimatedCostToGoal;

        public float totalCost
        {
            get { return costToNodeFromStart + estimatedCostToGoal; }
        }
        
        public Node parent;
        

        public Node(Vector3Int newLoc, float nextCostToNodeFromStart, float nextEstimatedCostToGoal, Node newParent)
        {
            loc = newLoc;
            parent = newParent;
            costToNodeFromStart = nextCostToNodeFromStart;
            estimatedCostToGoal = nextEstimatedCostToGoal;
        }
    }
    
    private TDGrid _tdGrid;
    
    public AStar(TDGrid newTdGrid)
    {
        _tdGrid = newTdGrid;
    }

    public List<Vector3Int> AStarAlgorithm(Vector3Int startNode, Vector3Int goalNode)
    {
        PriorityQueue<Node, float> openNodes = new PriorityQueue<Node, float>();
        HashSet<Vector3Int> openLocations = new HashSet<Vector3Int>();
        HashSet<Vector3Int> visitedLocations = new HashSet<Vector3Int>();
    
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
                // TDGrid.print(currentNode.loc);

                do
                {
                    // TDGrid.print(currentNode.loc);
                    path.Add(currentNode.loc);
                    currentNode = currentNode.parent;
                } while (currentNode != null);

                path.Reverse();
                return path;
            }

            visitedLocations.Add(currentNode.loc);
            openLocations.Remove(currentNode.loc);
            
            var neighbors = _tdGrid.GetOpenNeighbors(currentNode.loc);
            // For each neighbor of the current node:
            foreach (Vector3Int neighbor in neighbors)
            {
                // If the neighbor is in the closed list, skip it.
                if (visitedLocations.Contains(neighbor))
                {
                    //Skip because we've already evaluated this position.
                    continue;
                }
                
                if (_tdGrid.IsBlocked(neighbor, _tdGrid.navigationLayer))
                {
                    visitedLocations.Add(neighbor);
                    continue;
                }

                if (_tdGrid.IsMovementBlocked(currentNode.loc, neighbor))
                {
                    continue;
                }
                
                float costFromStartToNeighbor = currentNode.costToNodeFromStart + CalculateDistance(currentNode.loc, neighbor);

                if (openLocations.Contains(neighbor))
                {
                    // If the neighbor is already in the open list:
                    // Check if the new calculated cost is lower than the recorded cost for the neighbor.
                    foreach ((Node node, _) in openNodes.UnorderedItems)
                    {
                        if (node.loc == neighbor)
                        {
                            if (costFromStartToNeighbor < node.costToNodeFromStart)
                            {
                                // If so, update the cost and parent.
                                node.costToNodeFromStart = costFromStartToNeighbor;
                                node.parent = currentNode;
                            }
                            break;
                        }
                    }
                    continue;
                }
                
                // If the neighbor is not in the open list:
                // Add it to the open list.
                // Set the parent of the neighbor to the current node.
                // Calculate the cost from the start node to the neighbor.
                // Calculate the heuristic cost from the neighbor to the goal.
                if (!openLocations.Contains(neighbor))
                {
                    Node neighborNode = new Node(neighbor, costFromStartToNeighbor, costFromStartToNeighbor, currentNode);
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