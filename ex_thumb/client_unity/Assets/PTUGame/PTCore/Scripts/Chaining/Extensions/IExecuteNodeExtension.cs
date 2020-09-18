/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System.Collections;
    using UnityEngine;
    using System;
    
    
    public static class IExecuteNodeExtension
    {
        public static T ExecuteNode<T>(this T selBehaviour, IExecuteNode commandNode) where T : MonoBehaviour
        {
            selBehaviour.StartCoroutine(commandNode.Execute());
            return selBehaviour;
        }

        public static void Delay<T>(this T selfBehaviour, float seconds, Action delayEvent) where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(DelayAction.Allocate(seconds, delayEvent));
        }
        
        public static IEnumerator Execute(this IExecuteNode selfNode)
        {
            if (selfNode.Finished) selfNode.ResetState();
            
            while (!selfNode.Execute(Time.deltaTime))
            {
                yield return null;
            }
        }
    }
}