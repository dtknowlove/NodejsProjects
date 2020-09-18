/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	public class KeyEventAction : EventAction
	{
		private TimelineNode mTimelineNode;
		private string mKeyEventName;

		public KeyEventAction(string keyEventName, TimelineNode timelineNode)
		{
			mTimelineNode = timelineNode;
			mKeyEventName = keyEventName;
		}

		protected override void OnExecute(float dt)
		{
			mTimelineNode.OnKeyEventsReceivedCallback(mKeyEventName);
			Finished = true;
		}

		protected override void OnDispose()
		{
			mTimelineNode = null;
			mKeyEventName = null;
		}
	}
}

