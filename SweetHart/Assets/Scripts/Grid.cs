using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public Transform startPosition;
    public LayerMask obstacleMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float gapBetweenNodes;

    Node[,] nodeArray;
    public List<Node> FinalPath;

    public int MaxSize { get { return gridSizeX + gridSizeY; } }

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Start()
    {

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        nodeArray = new Node[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                Vector3 nodeWorldPosition = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool wall = false;

                if(Physics.CheckSphere(nodeWorldPosition, nodeRadius, obstacleMask)){
                    wall = true;
                }

                nodeArray[x, y] = new Node(wall, nodeWorldPosition, x, y);

            }
        }
    }

    public List<Node> GetNeighbourNodes(Node _node)
    {
        List<Node> NeighbourNodes = new List<Node>();

        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                //if we are on the node tha was passed in, skip this iteration.
                if(x == 0 && y == 0) {
                    continue;
                }

                int checkX = _node.gridX + x;
                int checkY = _node.gridY + y;

                //Make sure the node is within the grid.
                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    NeighbourNodes.Add(nodeArray[checkX, checkY]); //Adds to the neighbours list.
                }

            }
        }

        return NeighbourNodes;
    }

    public Node NodeFromWorldPosition(Vector3 _worldPosition)
    {
        float xPosition = ((_worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float yPosition = ((_worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

        xPosition = Mathf.Clamp01(xPosition);
        yPosition = Mathf.Clamp01(yPosition);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPosition);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPosition);

        return nodeArray[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(nodeArray != null) {
            foreach(Node node in nodeArray) {
                if(node.isWall) {
                    Gizmos.color = Color.yellow;
                }
                else {
                    Gizmos.color = Color.white;
                }
                if(FinalPath != null) {
                    if(FinalPath.Contains(node)) {
                        Gizmos.color = Color.red;
                    }
                }
                Gizmos.DrawCube(node.position, Vector3.one * (nodeDiameter - gapBetweenNodes));
            }
        }
    }
}
