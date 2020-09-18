/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System;

public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
public delegate void Callback<T, U, V, Y>(T arg1, U arg2, V arg3, Y arg4);

namespace PTGame.Event1
{
    /// <summary>
    /// 消息管理
    /// </summary>
    public static class MessageManager
    {
        public class BroadcastException : Exception
        {
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        public class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }

        public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
        public static List<string> permanentMessages = new List<string>();

        public static void MarkAsPermanent(string eventType)
        {
            MessageManager.permanentMessages.Add(eventType);
        }

        public static void Cleanup()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, Delegate> current in MessageManager.eventTable)
            {
                bool flag = false;
                foreach (string current2 in MessageManager.permanentMessages)
                {
                    if (current.Key == current2)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list.Add(current.Key);
                }
            }
            foreach (string current3 in list)
            {
                MessageManager.eventTable.Remove(current3);
            }
        }

        public static void PrintEventTable()
        {
            foreach (KeyValuePair<string, Delegate> current in MessageManager.eventTable)
            {
                Debug.Log( string.Concat(new object[]
			    {
				    "\t\t\t",
				    current.Key,
				    "\t\t",
				    current.Value
			    }));
            }
        }

        public static void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!MessageManager.eventTable.ContainsKey(eventType))
            {
                MessageManager.eventTable.Add(eventType, null);
            }
            Delegate @delegate = MessageManager.eventTable[eventType];
            if (@delegate != null && @delegate.GetType() != listenerBeingAdded.GetType())
            {
                throw new MessageManager.ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, @delegate.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        public static void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (MessageManager.eventTable.ContainsKey(eventType))
            {
                Delegate @delegate = MessageManager.eventTable[eventType];
                if (@delegate == null)
                {
                    throw new MessageManager.ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
                }
                if (@delegate.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new MessageManager.ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, @delegate.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
        }

        public static void OnListenerRemoved(string eventType)
        {
            if (MessageManager.eventTable.ContainsKey(eventType) && MessageManager.eventTable[eventType] == null)
            {
                MessageManager.eventTable.Remove(eventType);
            }
        }

        public static void OnBroadcasting(string eventType)
        {
        }

        public static MessageManager.BroadcastException CreateBroadcastSignatureException(string eventType)
        {
            return new MessageManager.BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
        }

        public static void AddListener(string eventType, Callback handler)
        {
            MessageManager.OnListenerAdding(eventType, handler);
            MessageManager.eventTable[eventType] = (Callback)Delegate.Combine((Callback)MessageManager.eventTable[eventType], handler);
        }

        public static void AddListener<T>(string eventType, Callback<T> handler)
        {
            MessageManager.OnListenerAdding(eventType, handler);
            MessageManager.eventTable[eventType] = (Callback<T>)Delegate.Combine((Callback<T>)MessageManager.eventTable[eventType], handler);
        }

        public static void AddListener<T, U>(string eventType, Callback<T, U> handler)
        {
            MessageManager.OnListenerAdding(eventType, handler);
            MessageManager.eventTable[eventType] = (Callback<T, U>)Delegate.Combine((Callback<T, U>)MessageManager.eventTable[eventType], handler);
        }

        public static void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler)
        {
            MessageManager.OnListenerAdding(eventType, handler);
            MessageManager.eventTable[eventType] = (Callback<T, U, V>)Delegate.Combine((Callback<T, U, V>)MessageManager.eventTable[eventType], handler);
        }

        public static void AddListener<T, U, V, Y>(string eventType, Callback<T, U, V, Y> handler)
        {
            MessageManager.OnListenerAdding(eventType, handler);
            MessageManager.eventTable[eventType] = (Callback<T, U, V, Y>)Delegate.Combine((Callback<T, U, V, Y>)MessageManager.eventTable[eventType], handler);
        }

        public static void AddListener(string eventType, Callback<object[]> handler)
        {
            MessageManager.OnListenerAdding(eventType, handler);
            MessageManager.eventTable[eventType] = (Callback<object[]>)Delegate.Combine((Callback<object[]>)MessageManager.eventTable[eventType], handler);
        }

        public static void RemoveListener(string eventType, Callback handler)
        {
            MessageManager.OnListenerRemoving(eventType, handler);
            if (MessageManager.eventTable.ContainsKey(eventType))
            {
                MessageManager.eventTable[eventType] = (Callback)Delegate.Remove((Callback)MessageManager.eventTable[eventType], handler);
            }
            MessageManager.OnListenerRemoved(eventType);
        }

        public static void RemoveListener<T>(string eventType, Callback<T> handler)
        {
            MessageManager.OnListenerRemoving(eventType, handler);
            if (MessageManager.eventTable.ContainsKey(eventType))
            {
                MessageManager.eventTable[eventType] = (Callback<T>)Delegate.Remove((Callback<T>)MessageManager.eventTable[eventType], handler);
            }
            MessageManager.OnListenerRemoved(eventType);
        }

        public static void RemoveListener<T, U>(string eventType, Callback<T, U> handler)
        {
            MessageManager.OnListenerRemoving(eventType, handler);
            if (MessageManager.eventTable.ContainsKey(eventType))
            {
                MessageManager.eventTable[eventType] = (Callback<T, U>)Delegate.Remove((Callback<T, U>)MessageManager.eventTable[eventType], handler);
            }
            MessageManager.OnListenerRemoved(eventType);
        }

        public static void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler)
        {
            MessageManager.OnListenerRemoving(eventType, handler);
            if (MessageManager.eventTable.ContainsKey(eventType))
            {
                MessageManager.eventTable[eventType] = (Callback<T, U, V>)Delegate.Remove((Callback<T, U, V>)MessageManager.eventTable[eventType], handler);
            }
            MessageManager.OnListenerRemoved(eventType);
        }

        public static void RemoveListener<T, U, V, Y>(string eventType, Callback<T, U, V, Y> handler)
        {
            MessageManager.OnListenerRemoving(eventType, handler);
            if (MessageManager.eventTable.ContainsKey(eventType))
            {
                MessageManager.eventTable[eventType] = (Callback<T, U, V, Y>)Delegate.Remove((Callback<T, U, V, Y>)MessageManager.eventTable[eventType], handler);
            }
            MessageManager.OnListenerRemoved(eventType);
        }

        public static void RemoveListener(string eventType, Callback<object[]> handler)
        {
            MessageManager.OnListenerRemoving(eventType, handler);
            if (MessageManager.eventTable.ContainsKey(eventType))
            {
                MessageManager.eventTable[eventType] = (Callback<object[]>)Delegate.Remove((Callback<object[]>)MessageManager.eventTable[eventType], handler);
            }
            MessageManager.OnListenerRemoved(eventType);
        }

        public static void Broadcast(string eventType)
        {
            MessageManager.OnBroadcasting(eventType);
            Delegate @delegate;
            if (MessageManager.eventTable.TryGetValue(eventType, out @delegate))
            {
                Callback callback = @delegate as Callback;
                if (callback == null)
                {
                    throw MessageManager.CreateBroadcastSignatureException(eventType);
                }
                callback();
            }
        }

        public static void Broadcast<T>(string eventType, T arg1)
        {
            MessageManager.OnBroadcasting(eventType);
            Delegate @delegate;
            if (MessageManager.eventTable.TryGetValue(eventType, out @delegate))
            {
                Callback<T> callback = @delegate as Callback<T>;
                if (callback == null)
                {
                    throw MessageManager.CreateBroadcastSignatureException(eventType);
                }
                callback(arg1);
            }
        }
        public static void Broadcast<T, U>(string eventType, T arg1, U arg2)
        {
            MessageManager.OnBroadcasting(eventType);
            Delegate @delegate;
            if (MessageManager.eventTable.TryGetValue(eventType, out @delegate))
            {
                Callback<T, U> callback = @delegate as Callback<T, U>;
                if (callback == null)
                {
                    throw MessageManager.CreateBroadcastSignatureException(eventType);
                }
                callback(arg1, arg2);
            }
        }

        public static void Broadcast<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            MessageManager.OnBroadcasting(eventType);
            Delegate @delegate;
            if (MessageManager.eventTable.TryGetValue(eventType, out @delegate))
            {
                Callback<T, U, V> callback = @delegate as Callback<T, U, V>;
                if (callback == null)
                {
                    throw MessageManager.CreateBroadcastSignatureException(eventType);
                }
                callback(arg1, arg2, arg3);
            }
        }

        public static void Broadcast<T, U, V, Y>(string eventType, T arg1, U arg2, V arg3, Y arg4)
        {
            MessageManager.OnBroadcasting(eventType);
            Delegate @delegate;
            if (MessageManager.eventTable.TryGetValue(eventType, out @delegate))
            {
                Callback<T, U, V, Y> callback = @delegate as Callback<T, U, V, Y>;
                if (callback == null)
                {
                    throw MessageManager.CreateBroadcastSignatureException(eventType);
                }
                callback(arg1, arg2, arg3, arg4);
            }
        }

        public static void Broadcast(string eventType, object[] args)
        {
            MessageManager.OnBroadcasting(eventType);
            Delegate @delegate;
            if (MessageManager.eventTable.TryGetValue(eventType, out @delegate))
            {
                Callback<object[]> callback = @delegate as Callback<object[]>;
                if (callback == null)
                {
                    throw MessageManager.CreateBroadcastSignatureException(eventType);
                }
                callback(args);
            }
        }
    }
}