/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public interface IPTList<T>
    {
        void Accept(IListVisitor<T> visitor);
        void Accept(ListVisitorDelegate<T> visitor);
    }

    //列表访问器
    public interface IListVisitor<T>
    {
        void Visit(T data);
    }

    public delegate void ListVisitorDelegate<T>(T data);

    public interface Iterator<T>
    {
        bool HasNext
        {
            get;
        }

        T Next
        {
            get;
        }
    }

    public interface Iteratable<T>
    {
        Iterator<T> Iterator(); 
    }

    public class ListNode<T>
    {
		private T mData;
        ListNode<T> mNext;
        
        public T Data
        {
            get { return mData; }
            set { mData = value; }
        }

        public ListNode<T> Next
        {
			get { return mNext; }
            set { mNext = value; }
        }
    }
}