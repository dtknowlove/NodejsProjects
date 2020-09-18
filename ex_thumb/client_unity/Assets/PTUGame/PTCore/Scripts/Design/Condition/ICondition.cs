/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    /// <summary>
    /// 条件判断是否符合条件
    /// </summary>
    public interface ICondition
    {
        bool IsSatisfy();
    }

    /// <summary>
    /// 抽象类
    /// </summary>
    public abstract class Condition : ICondition, IDisposable
    {
        protected Func<bool> mConditionFunc;

        public bool IsSatisfy()
        {
            return mConditionFunc.InvokeGracefully();
        }

        public void Dispose()
        {
            mConditionFunc = null;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }

    /// <summary>
    /// 默认的条件
    /// </summary>
    public class DefaultCondition : Condition
    {
        public DefaultCondition(Func<bool> condition)
        {
            mConditionFunc = condition;
        }
    }
}