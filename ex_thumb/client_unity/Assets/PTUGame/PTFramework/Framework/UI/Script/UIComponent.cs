/****************************************************************************
 * 2018.5 liqingyun
 ****************************************************************************/

namespace PTGame.Framework
{
    /// <inheritdoc />
    /// <summary>
    /// 是一个通用的组件,和 UIElement 类似，但是存放的目录不同
    /// </summary>
    public abstract class UIComponent : UIElement
    {
        public override UIMarkType GetUIMarkType()
        {
            return UIMarkType.Component;
        }
    }
}