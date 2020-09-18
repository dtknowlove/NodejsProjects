/****************************************************************************
 * 2017 liqingyun
 ****************************************************************************/

namespace PTGame.Framework
{
    using UnityEngine;

    /// <summary>
    /// belone to a panel 
    /// </summary>
    public abstract class UIElement : PTMonoBehaviour,IUIMark
    {
        public virtual UIMarkType GetUIMarkType()
        {
            return UIMarkType.Element;
        }

        public abstract string ComponentName { get; }

        public Transform Transform
        {
            get { return transform; }
        }

        protected override IManager mMgr
        {
            get { return PTUIManager.Instance; }
        }
    }
}