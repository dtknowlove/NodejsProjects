/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    public class SCBinarySearchTree<T> : Iteratable<T> where T : IBinarySearchTreeElement
    {
        public enum eNodeType
        {
            kLeft = 0,
            kRight = 1,
            kRoot = 2,
        }

        #region 节点类
        public class Node
        {
            public Node LeftChild;
            public Node RightChild;
            private Node mParent;
            protected eNodeType mNodeType;

            public bool IsLeaf()
            {
                if (LeftChild == null && RightChild == null)
                {
                    return true;
                }
                return false;
            }

            public Node Parent
            {
                get { return mParent; }
            }

            public eNodeType NodeType
            {
                get { return mNodeType; }
            }

            public void SetParent(Node parent, eNodeType nodeType)
            {
                mParent = parent;
                mNodeType = nodeType;
            }

            private T mData;

            public float SortScore
            {
                get { return mData.SortScore; }
            }

            public T Data
            {
                get { return mData; }
            }

            public Node(T data)
            {
                mData = data;
            }
        }

        #endregion

        protected Node m_HeadNode;


        #region 插入
        public void Insert(T[] dataArray)
        {
            if (dataArray == null)
            {
                throw new NullReferenceException("BinarySearchTree Not Support Insert Null Object");
            }

            for (var i = 0; i < dataArray.Length; ++i)
            {
                Insert(dataArray[i]);
            }
        }

        public void Insert(T data)
        {
            if (data == null)
            {
                throw new NullReferenceException("BinarySearchTree Not Support Insert Null Object");
            }

            if (m_HeadNode == null)
            {
                m_HeadNode = new Node(data);
                m_HeadNode.SetParent(null, eNodeType.kRoot);
                return;
            }

            var newNode = new Node(data);

            var score = newNode.SortScore;

            Node preNode = null;
            var currentNode = m_HeadNode;
            while (currentNode != null)
            {
                preNode = currentNode;
                if (score < currentNode.SortScore)
                {
                    currentNode = currentNode.LeftChild;
                    if (currentNode == null)
                    {
                        newNode.SetParent(preNode, eNodeType.kLeft);
                        preNode.LeftChild = newNode;
                        break;
                    }
                }
                else
                {
                    currentNode = currentNode.RightChild;
                    if (currentNode == null)
                    {
                        newNode.SetParent(preNode, eNodeType.kRight);
                        preNode.RightChild = newNode;
                        break;
                    }
                }
            }
        }

        #endregion

        #region 查找

        protected Node Find(Node head, T data)
        {
            if (data == null)
            {
                return null;
            }

            var score = data.SortScore;
            var currentNode = head;
            while (currentNode != null)
            {
                if (data.Equals(currentNode.Data))
                {
                    break;
                }
                if (score < currentNode.SortScore)
                {
                    currentNode = currentNode.LeftChild;
                }
                else
                {
                    currentNode = currentNode.RightChild;
                }
            }
            return currentNode;
        }

        #endregion

        #region 删除

        public void Remove(T data)
        {
            if (data == null)
            {
                return;
            }

            var currentNode = Find(m_HeadNode, data);

            if (currentNode == null)
            {
                Console.WriteLine("Not Find DeleteNode");
                return;
            }

            #region 左右子树都空直接删除

            if (currentNode.LeftChild == null && currentNode.RightChild == null)
            {
                switch (currentNode.NodeType)
                {
                    case eNodeType.kLeft:
                    {
                        currentNode.Parent.LeftChild = null;
                    }
                        break;
                    case eNodeType.kRight:
                    {
                        currentNode.Parent.RightChild = null;
                    }
                        break;
                    case eNodeType.kRoot:
                    {
                        m_HeadNode = null;
                    }
                        break;
                    default:
                        break;
                }
                return;
            }

            #endregion

            #region 右子树不空，接入原父节点的父节点，并绑定原子节点

            if (currentNode.RightChild != null)
            {
                var rightChild = currentNode.RightChild;
                switch (currentNode.NodeType)
                {
                    case eNodeType.kLeft:
                    {
                        currentNode.Parent.LeftChild = rightChild;
                        rightChild.SetParent(currentNode.Parent, eNodeType.kLeft);
                    }
                        break;
                    case eNodeType.kRight:
                    {
                        currentNode.Parent.RightChild = rightChild;
                        rightChild.SetParent(currentNode.Parent, eNodeType.kRight);
                    }
                        break;
                    case eNodeType.kRoot:
                        m_HeadNode = rightChild;
                        rightChild.SetParent(null, eNodeType.kRoot);
                        break;
                    default:
                        break;
                }
                //左子树的根节点是右子树的最左节点
                var minLeftNode = GetMinNode(rightChild);

                if (currentNode.LeftChild != null)
                {
                    minLeftNode.LeftChild = currentNode.LeftChild;
                    currentNode.LeftChild.SetParent(minLeftNode, eNodeType.kLeft);
                }

                return;
            }

            #endregion

            #region 左子树不空，接入原父节点的父节点

            var leftNode = currentNode.LeftChild;
            switch (currentNode.NodeType)
            {
                case eNodeType.kLeft:
                    currentNode.Parent.LeftChild = leftNode;
                    leftNode.SetParent(currentNode.Parent, eNodeType.kLeft);
                    break;
                case eNodeType.kRight:
                    currentNode.Parent.RightChild = leftNode;
                    leftNode.SetParent(currentNode.Parent, eNodeType.kRight);
                    break;
                case eNodeType.kRoot:
                    m_HeadNode = leftNode;
                    leftNode.SetParent(null, eNodeType.kRoot);
                    break;
                default:
                    break;
            }

            #endregion
        }

        #endregion

        #region 遍历&访问&迭代器

        public delegate void DataVisitor(T data);

        //遍历 通过队列实现
        public void Accept(DataVisitor visitor)
        {
            if (m_HeadNode == null)
            {
                return;
            }

            var stack = new PTStack<Node>();
            var current = m_HeadNode;
            while (current != null || !stack.IsEmpty)
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.LeftChild;
                }

                if (!stack.IsEmpty)
                {
                    current = stack.Pop();
                    visitor(current.Data);

                    current = current.RightChild;
                }
            }
        }

        public Iterator<T> Iterator()
        {
            return new BinarySearchTreeIterator(m_HeadNode);
        }

        public class BinarySearchTreeIterator : Iterator<T>
        {
            private Node m_HeadNode;
            private Node m_Current;
            PTStack<Node> m_Stack = new PTStack<Node>();

            public BinarySearchTreeIterator(Node headNode)
            {
                m_HeadNode = headNode;
                m_Current = m_HeadNode;
            }

            public bool HasNext
            {
                get
                {
                    if (m_Current != null || !m_Stack.IsEmpty)
                    {
                        return true;
                    }
                    return false;
                }
            }

            public T Next
            {
                get
                {
                    while (m_Current != null)
                    {
                        m_Stack.Push(m_Current);
                        m_Current = m_Current.LeftChild;
                    }

                    if (!m_Stack.IsEmpty)
                    {
                        m_Current = m_Stack.Pop();
                        var result = m_Current.Data;
                        m_Current = m_Current.RightChild;
                        return result;
                    }
                    return default(T);
                }
            }
        }

        protected Node GetMinNode(Node head)
        {
            var current = head;
            while (current.LeftChild != null)
            {
                current = current.LeftChild;
            }
            return current;
        }

        #endregion
    }
}