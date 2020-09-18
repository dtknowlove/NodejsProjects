/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine.UI;
    
    public static class SelectableExtension
    {
        public static T EnableInteract<T>(this T selfSelectable) where T :Selectable
        {
            selfSelectable.interactable = true;
            return selfSelectable;
        }

        public static T DisableInteract<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.interactable = false;
            return selfSelectable;
        }
        
        public static T CancalAllTransitions<T>(this T selfSelectable) where T :Selectable
        {
            selfSelectable.transition = Selectable.Transition.None;
            return selfSelectable;
        } 
    }
}