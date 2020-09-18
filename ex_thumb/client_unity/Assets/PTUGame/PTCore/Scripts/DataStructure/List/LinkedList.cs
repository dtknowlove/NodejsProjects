/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    //链表
    public class PTLinkedList<T> : IPTList<T>, Iteratable<T>
    {
        protected ListNode<T> mHeadNode { get; private set; }

        //获取队尾
        protected ListNode<T> mTailNode
        {
            get
            {
                if (mHeadNode == null)
                {
                    return null;
                }

                var nextNode = mHeadNode;
                while (nextNode.Next != null)
                {
                    nextNode = nextNode.Next;
                }

                return nextNode;
            }
        }

        #region 公共方法

        #region 插入方法

        //插入队列头
        public void InsertHead(T data)
        {
            var preHead = mHeadNode;

            mHeadNode = new ListNode<T>();
            mHeadNode.Data = data;

            mHeadNode.Next = preHead;
        }

        //插入队列尾
        public void InsertTail(T data)
        {
            var preTail = mTailNode;

            var tail = new ListNode<T>();
            tail.Data = data;

            if (preTail == null)
            {
                mHeadNode = tail;
            }
            else
            {
                preTail.Next = tail;
            }
        }

        #endregion

        #region 删除方法

        public void RemoveHead()
        {
            if (mHeadNode == null)
            {
                return;
            }

            mHeadNode = mHeadNode.Next;
        }

        public bool RemoveAt(int index)
        {
            if (mHeadNode == null)
            {
                return false;
            }

            ListNode<T> preNode = null;
            var currentNode = mHeadNode;

            while (index-- > 0 && currentNode != null)
            {
                preNode = currentNode;
                currentNode = preNode.Next;
            }

            if (currentNode == null)
            {
                return false;
            }

            //删除的是头结点
            if (preNode == null)
            {
                mHeadNode = currentNode.Next;
            }
            else
            {
                preNode.Next = currentNode.Next;
            }
            return true;
        }

        public bool Remove(T data)
        {
            if (mHeadNode == null)
            {
                return false;
            }

            ListNode<T> preNode = null;
            var currentNode = mHeadNode;
            var hasFind = false;

            while (currentNode != null)
            {
                if (currentNode.Data.Equals(data))
                {
                    hasFind = true;
                    break;
                }

                preNode = currentNode;
                currentNode = currentNode.Next;
            }

            if (!hasFind)
            {
                return false;
            }

            //删除的是头结点
            if (preNode == null)
            {
                mHeadNode.Next = currentNode.Next;
            }
            else
            {
                preNode.Next = currentNode.Next;
            }
            return true;
        }

        #endregion

        #region 查询方法

        //查询方法，返回索引
        public int Query(T data)
        {
            if (mHeadNode == null)
            {
                return -1;
            }

            var currentNode = mHeadNode;
            var index = 0;
            while (currentNode != null)
            {
                if (currentNode.Data.Equals(data))
                {
                    return index;
                }
                currentNode = currentNode.Next;
                ++index;
            }

            return -1;
        }

        #endregion

        #region 获取对头

        public T HeadData
        {
            get
            {
                if (mHeadNode == null)
                {
                    return default(T);
                }
                return mHeadNode.Data;
            }
        }

        public T TailData
        {
            get
            {
                var tailHead = mTailNode;
                if (tailHead == null)
                {
                    return default(T);
                }
                return tailHead.Data;
            }
        }

        public bool IsEmpty
        {
            get { return mHeadNode == null; }
        }

        #endregion

        #region 访问器遍历

        public void Accept(IListVisitor<T> visitor)
        {
            var it = Iterator();
            while (it.HasNext)
            {
                visitor.Visit(it.Next);
            }
        }

        public void Accept(ListVisitorDelegate<T> visitor)
        {
            var it = Iterator();
            while (it.HasNext)
            {
                visitor(it.Next);
            }
        }

        #endregion

        #region 迭代器实现

        public class LinkedListIterator : Iterator<T>
        {
            private ListNode<T> HeadNode;
            private ListNode<T> mCurrentNode;

            public LinkedListIterator(ListNode<T> head)
            {
                HeadNode = head;
                if (HeadNode != null)
                {
                    mCurrentNode = new ListNode<T>();
                    mCurrentNode.Next = HeadNode;
                }
            }

            public bool HasNext
            {
                get { return mCurrentNode.Next != null; }
            }

            public T Next
            {
                get
                {
                    var r = mCurrentNode.Next.Data;
                    mCurrentNode = mCurrentNode.Next;
                    return r;
                }
            }
        }

        //该链表的迭代器
        public Iterator<T> Iterator()
        {
            return new LinkedListIterator(mHeadNode);
        }

        #endregion

        #endregion
    }
}