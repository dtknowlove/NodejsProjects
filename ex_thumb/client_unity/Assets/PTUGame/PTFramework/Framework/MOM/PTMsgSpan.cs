/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/
namespace PTGame.Framework
{
	public class PTMsgSpan
	{
		public const int Count = 3000;
	}

	public class PTMgrID
	{
		public const int Framework = 0;
		public const int UI = Framework + PTMsgSpan.Count; // 3000
		public const int Audio = UI + PTMsgSpan.Count; // 6000
		public const int Network = Audio + PTMsgSpan.Count;
		public const int UIFilter = Network + PTMsgSpan.Count;
		public const int Game = UIFilter + PTMsgSpan.Count;
		public const int PCConnectMobile = Game + PTMsgSpan.Count;
		public const int Action = PCConnectMobile + PTMsgSpan.Count;
		public const int FrameworkEnded = PCConnectMobile + PTMsgSpan.Count;
		public const int FrameworkMsgModuleCount = 8;
	}
}