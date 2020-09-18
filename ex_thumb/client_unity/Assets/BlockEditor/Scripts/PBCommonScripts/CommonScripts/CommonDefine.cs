using System;
using System.Collections.Generic;

public enum PolygonType
{
	LOW = 0,
	HIGH,
}

public enum Category
{
	large, 		//Category_1
	largesuper, //Category_pbl
	fig, 		//Category_fig
	small, 		//Category_pbs
	tech, 		//Category_tech
	sticker,
	custom,
}


#region ToolClass
    
public static class ListExtension
{
	public static List<T> Sort<T>(this List<T> list,Func<T,T,int> compareFunc,bool reverse)
	{
		if (compareFunc != null)
		{
			list.Sort(new ListCompare<T>(compareFunc,reverse));
		}
		return list;
	}
}

public class ListCompare<T> : IComparer<T>
{
	private Func<T, T, int> m_Func;
	private bool m_Reverse;
        
	public ListCompare(Func<T,T,int> f,bool reverse=false)
	{
		m_Func = f;
		m_Reverse = reverse;
	}   
	public int Compare(T x, T y)
	{
		if (m_Func == null)
			return 0;

		if (m_Reverse)
			return m_Func(y, x);

		return m_Func(x, y);
	}
}

#endregion