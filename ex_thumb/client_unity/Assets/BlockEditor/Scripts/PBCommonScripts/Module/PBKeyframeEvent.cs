/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using UnityEngine.Events;

namespace Putao.PaiBloks.Common
{
    public class KeyframeStart : UnityEvent<PBKeyFrame> {}
    public class KeyframeComplete : UnityEvent<PBKeyFrame> {}
    public class KeyframeItemComplete: UnityEvent<PBKeyFrameItem> {}
    
    public class KeyframeCompleteAll : UnityEvent {}
    
    public class KeyframeSectionPoint : UnityEvent<PBKeyFrame> {}
    
    public class KeyframeReset : UnityEvent<PBKeyFrame> {}
}