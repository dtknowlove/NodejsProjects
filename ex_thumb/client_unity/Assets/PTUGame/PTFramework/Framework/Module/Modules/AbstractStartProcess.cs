﻿/****************************************************************************
 * Copyright (c) 2017 ouyanggongping@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using PTGame.Core;

namespace PTGame.Framework
{
    using System;

    public class AbstractStartProcess : AbstractMonoModule
    {
        private SequenceNode mSequenceNode;
        private Action mOnProcessFinish;

        public void SetFinishListener(Action listener)
        {
            mOnProcessFinish = listener;
        }

        public float TotalCount
        {
            get
            {
                if (mSequenceNode == null)
                {
                    return 0;
                }

                return mSequenceNode.TotalCount;
            }
        }

        public void Append(ProcessNode node)
        {   
            if (null == mSequenceNode)
                mSequenceNode = new SequenceNode();
            mSequenceNode.Append(node);
        }

        protected override void OnAwakeCom()
        {
            InitExecuteContainer();
        }

        public override void OnComponentStart()
        {
            if (null == mSequenceNode) return;
            mSequenceNode.OnEndedCallback += OnAllExecuteNodeEnd;
        }

        public override void OnComponentUpdate(float dt)
        {
            if (!mSequenceNode.Finished && mSequenceNode.Execute(dt))
            {
                
            }
        }

        protected virtual void InitExecuteContainer()
        {
        }

        protected virtual void OnAllExecuteNodeEnd()
        {
            PTDebug.Log("#BaseStartProcess: OnAllExecuteNodeEnd");
            mSequenceNode.OnEndedCallback -= OnAllExecuteNodeEnd;
            mSequenceNode.Dispose();
            mSequenceNode = null;
            
            mOnProcessFinish.InvokeGracefully();
            Actor.RemoveComponent(this);
        }
    }
}