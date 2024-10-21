using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFindings : MonoBehaviour
{

    GridUI grid;
    PathRequestManager requestManager;

    void Awake() {
        grid = GetComponent<GridUI>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;


        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);


        if(startNode.walkable && targetNode.walkable) {

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(startNode);

            while(openSet.Count > 0) {
                //UnityEngine.Debug.Log("openSet.Count: " + openSet.Count);
                Node currentNode = openSet.RemoveFirst();
                closeSet.Add(currentNode);

                bool isEnded = currentNode == targetNode;
                if(isEnded) {
                    sw.Stop();
                    UnityEngine.Debug.Log("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach(Node neighbours in grid.GetNeighbours(currentNode)) {
                    if(!neighbours.walkable || closeSet.Contains(neighbours)) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours) + neighbours.movementPenalty;
                    if(newMovementCostToNeighbour < neighbours.gCost || !openSet.Contains(neighbours)) {
                        neighbours.gCost = newMovementCostToNeighbour;
                        neighbours.hCost = GetDistance(neighbours, targetNode);
                        neighbours.parent = currentNode;

                        if(!openSet.Contains(neighbours)) {
                            openSet.Add(neighbours);
                        }else{
                            openSet.UpdateItem(neighbours);
                        }
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess){
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        System.Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path){
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++){
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if (directionNew != directionOld){
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    /// <summary>
    /// Get the distance between two nodes. It is G cost.
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    int GetDistance(Node nodeA, Node nodeB) {
        int HorizontalCost = 10;
        int DiagonalCost = 14;//14 = 10 * sqrt(2)
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(dstX > dstY) {
            return DiagonalCost * dstY + HorizontalCost * (dstX - dstY);
        }

        return DiagonalCost * dstX + HorizontalCost * (dstY - dstX);
    }
}
