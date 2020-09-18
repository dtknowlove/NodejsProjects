/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    /// <summary>
    /// 启动执行节点
    /// </summary>
    public class ProcessNode : ExecuteNode
    {
        protected string mTips = "Default";

        public virtual float Progress { get; set; }

        public virtual string Tips
        {
            get { return mTips; }
            set { mTips = value; }
        }
    }
}