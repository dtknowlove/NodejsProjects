/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

using PTGame.Core;
namespace PTGame.Framework
{
	/// <summary>
	/// 目前暂时是用户的动作.
	/// 1.录制Action可以序列化反序列化.
	/// 2.也可以顺序执行Action,这个可能就要Node的功能了。
	/// </summary>
	public interface IAction : IExecutable,INameable
	{
		bool Finished { get; }
	}
}