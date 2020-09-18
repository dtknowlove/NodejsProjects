/****************************************************************************
 * Copyright (c) 2017 ouyanggongping@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using PTGame.Core;
namespace PTGame.Framework
{
	/// <summary>
	/// 这个最后做成类似GAFPlayer或者ResLoader形式的
	/// </summary>
	public class AudioPlayer : IPoolType, IPoolable
	{
		public bool IsRecycled { get; set; }

		public void OnRecycled()
		{
		}

		public static AudioPlayer Allocate()
		{
			return SafeObjectPool<AudioPlayer>.Instance.Allocate();
		}

		public void Recycle2Cache()
		{
			SafeObjectPool<AudioPlayer>.Instance.Recycle(this);
		}
	}
}