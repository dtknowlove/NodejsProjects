/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * reference blog:http://blog.csdn.net/aisajiajiao/article/details/17622063
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// 类处理所有代表地图的格子的属性
    /// </summary>
    public class GridManager : PTMonoSingleton<GridManager>
    {
        public int NumOfRows;
        public int NumOfColumns;
        public float GridCellSize;
        public bool ShowGrid = true;
        public bool ShowObstacleBlocks = true;
        private Vector3 mOrigin = new Vector3();
        private GameObject[] mObstacleList;
        public AStarNode[,] Nodes { get; set; }

        public Vector3 Origin
        {
            get { return mOrigin; }
        }

        void Awake()
        {
            mObstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
            CalculateObstacles();
        }

        // Find all the obstacles on the map  
        void CalculateObstacles()
        {
            Nodes = new AStarNode[NumOfColumns, NumOfRows];
            var index = 0;
            for (var i = 0; i < NumOfColumns; i++)
            {
                for (var j = 0; j < NumOfRows; j++)
                {
                    var cellPos = GetGridCellCenter(index);
                    var node = new AStarNode(cellPos);
                    Nodes[i, j] = node;
                    index++;
                }
            }
            if (mObstacleList != null && mObstacleList.Length > 0)
            {
                //For each obstacle found on the map, record it in our list  
                foreach (var data in mObstacleList)
                {
                    var indexCell = GetGridIndex(data.transform.position);
                    var col = GetColumn(indexCell);
                    var row = GetRow(indexCell);
                    Nodes[row, col].MarkAsObstacle();
                }
            }
        }

        public int GetGridIndex(Vector3 pos)
        {
            if (!IsInBounds(pos))
            {
                return -1;
            }
            pos -= Origin;
            var col = (int) (pos.x / GridCellSize);
            var row = (int) (pos.z / GridCellSize);
            return (row * NumOfColumns + col);
        }

        public bool IsInBounds(Vector3 pos)
        {
            var width = NumOfColumns * GridCellSize;
            var height = NumOfRows * GridCellSize;
            return (pos.x >= Origin.x && pos.x <= Origin.x + width &&
                    pos.x <= Origin.z + height && pos.z >= Origin.z);
        }

        public Vector3 GetGridCellCenter(int index)
        {
            var cellPosition = GetGridCellPosition(index);
            cellPosition.x += (GridCellSize / 2.0f);
            cellPosition.z += (GridCellSize / 2.0f);
            return cellPosition;
        }

        public Vector3 GetGridCellPosition(int index)
        {
            var row = GetRow(index);
            var col = GetColumn(index);
            var xPosInGrid = col * GridCellSize;
            var zPosInGrid = row * GridCellSize;
            return Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
        }

        public int GetRow(int index)
        {
            var row = index / NumOfColumns;
            return row;
        }

        public int GetColumn(int index)
        {
            var col = index % NumOfColumns;
            return col;
        }


        public void GetNeighbours(AStarNode node, List<AStarNode> neighbors)
        {
            var neighborPos = node.Position;
            var neighborIndex = GetGridIndex(neighborPos);
            var row = GetRow(neighborIndex);
            var column = GetColumn(neighborIndex);
            //Bottom  
            var leftNodeRow = row - 1;
            var leftNodeColumn = column;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
            //Top  
            leftNodeRow = row + 1;
            leftNodeColumn = column;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
            //Right  
            leftNodeRow = row;
            leftNodeColumn = column + 1;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
            //Left  
            leftNodeRow = row;
            leftNodeColumn = column - 1;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
        }

        void AssignNeighbour(int row, int column, List<AStarNode> neighbors)
        {
            if (row != -1 && column != -1 &&
                row < NumOfRows && column < NumOfColumns)
            {
                var nodeToAdd = Nodes[row, column];
                if (!nodeToAdd.IsObstacle)
                {
                    neighbors.Add(nodeToAdd);
                }
            }
        }
        
        void OnDrawGizmos() {  
            if (ShowGrid) {  
                DebugDrawGrid(transform.position, NumOfRows, NumOfColumns,   
                    GridCellSize, Color.blue);  
            }  
            Gizmos.DrawSphere(transform.position, 0.5f);  
            if (ShowObstacleBlocks) {  
                var cellSize = new Vector3(GridCellSize, 1.0f,  
                    GridCellSize);  
                if (mObstacleList != null && mObstacleList.Length > 0) {  
                    foreach (var data in mObstacleList) {  
                        Gizmos.DrawCube(GetGridCellCenter(  
                            GetGridIndex(data.transform.position)), cellSize);  
                    }  
                }  
            }  
        }  
        public void DebugDrawGrid(Vector3 origin, int numRows, int  
            numCols,float cellSize, Color color) {  
            var width = (numCols * cellSize);  
            var height = (numRows * cellSize);  
            // Draw the horizontal grid lines  
            for (var i = 0; i < numRows + 1; i++) {  
                var startPos = origin + i * cellSize * new Vector3(0.0f,  
                                       0.0f, 1.0f);  
                var endPos = startPos + width * new Vector3(1.0f, 0.0f,  
                                     0.0f);  
                Debug.DrawLine(startPos, endPos, color);  
            }  
            // Draw the vertial grid lines  
            for (var i = 0; i < numCols + 1; i++) {  
                var startPos = origin + i * cellSize * new Vector3(1.0f,  
                                       0.0f, 0.0f);  
                var endPos = startPos + height * new Vector3(0.0f, 0.0f,  
                                     1.0f);  
                Debug.DrawLine(startPos, endPos, color);  
            }  
        }  
    }
}