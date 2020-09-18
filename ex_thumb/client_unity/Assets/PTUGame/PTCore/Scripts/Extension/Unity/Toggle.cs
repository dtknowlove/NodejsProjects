/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine.Events;
    using UnityEngine.UI;
    
    public static class ToggleExtension
    {
        public static void RegOnValueChangedEvent(this Toggle selfToggle, UnityAction<bool> onValueChangedEvent)
        {
            selfToggle.onValueChanged.AddListener(onValueChangedEvent);
        }
    }
}