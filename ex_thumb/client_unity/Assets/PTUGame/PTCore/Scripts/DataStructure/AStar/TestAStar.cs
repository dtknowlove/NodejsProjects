/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * reference blog:http://blog.csdn.net/aisajiajiao/article/details/17622063
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine;
    using System.Collections.Generic;

    public class TestAStar : MonoBehaviour
    {
        private Transform mStartPos, mEndPos;
        public AStarNode StartNode { get; set; }
        public AStarNode GoalNode { get; set; }
        public List<AStarNode> PathArray;
        GameObject mObjStartCube, mObjEndCube;

        private float mElapsedTime = 0.0f;

        //Interval time between pathfinding  
        public float mIntervalTime = 1.0f;

        void Start()
        {
            mObjStartCube = GameObject.FindGameObjectWithTag("Start");
            mObjEndCube = GameObject.FindGameObjectWithTag("End");
            PathArray = new List<AStarNode>();
            FindPath();
        }

        void Update()
        {
            mElapsedTime += Time.deltaTime;
            if (mElapsedTime >= mIntervalTime)
            {
                mElapsedTime = 0.0f;
                FindPath();
            }
        }

        void FindPath()
        {
            mStartPos = mObjStartCube.transform;
            mEndPos = mObjEndCube.transform;
            StartNode = new AStarNode(GridManager.Instance.GetGridCellCenter(
                GridManager.Instance.GetGridIndex(mStartPos.position)));
            GoalNode = new AStarNode(GridManager.Instance.GetGridCellCenter(
                GridManager.Instance.GetGridIndex(mEndPos.position)));
            PathArray = AStar.FindPath(StartNode, GoalNode);
        }

        private void OnDrawGizmos()
        {
            if (PathArray == null)
                return;
            if (PathArray.Count > 0)
            {
                var index = 1;
                foreach (var node in PathArray)
                {
                    if (index < PathArray.Count)
                    {
                        var nextNode = PathArray[index];
                        Debug.DrawLine(node.Position, nextNode.Position,
                            Color.green);
                        index++;
                    }
                }
            }
        }
    }
}