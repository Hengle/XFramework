using System;
using System.Collections.Generic;

/// <summary>
/// 消息类型
/// </summary>
public enum MessageEventType
{    
    A,
    B,
}

namespace XDEDZL
{
    //=====================================================================================/
    /// <summary>
    /// 消息类 全局类消息
    /// </summary>
    //=====================================================================================.
    public class Messenger : IGameModule
    {
        public delegate void Callback();

        public delegate void Callback<T>(T arg1);

        public delegate void Callback<T, U>(T arg1, U arg2);

        public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

        public Dictionary<MessageEventType, Delegate> m_eventDictionary = new Dictionary<MessageEventType, Delegate>();

        public int Priority { get { return 100; } }

        ~Messenger()
        {
            Cleanup();
        }

        #region AddEventListener

        public void AddEventListener(MessageEventType eventType, Callback handler)
        {
            OnListenerAdding(eventType, handler);
            m_eventDictionary[eventType] = (Callback)m_eventDictionary[eventType] + handler;
        }

        //一个参数 parameter
        public void AddEventListener<T>(MessageEventType eventType, Callback<T> handler)
        {
            OnListenerAdding(eventType, handler);
            m_eventDictionary[eventType] = (Callback<T>)m_eventDictionary[eventType] + handler;
        }

        //两个参数 parameter
        public void AddEventListener<T, U>(MessageEventType eventType, Callback<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            m_eventDictionary[eventType] = (Callback<T, U>)m_eventDictionary[eventType] + handler;
        }

        //三个参数 parameter
        public void AddEventListener<T, U, V>(MessageEventType eventType, Callback<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            m_eventDictionary[eventType] = (Callback<T, U, V>)m_eventDictionary[eventType] + handler;
        }

        #endregion AddEventListener

        #region RemoveEventListener

        public void RemoveEventListener(MessageEventType eventType, Callback handler)
        {
            OnListenerRemoving(eventType, handler);
            m_eventDictionary[eventType] = (Callback)m_eventDictionary[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        public void RemoveEventListener<T>(MessageEventType eventType, Callback<T> handler)
        {
            OnListenerRemoving(eventType, handler);
            m_eventDictionary[eventType] = (Callback<T>)m_eventDictionary[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        public void RemoveEventListener<T, U>(MessageEventType eventType, Callback<T, U> handler)
        {
            OnListenerRemoving(eventType, handler);
            m_eventDictionary[eventType] = (Callback<T, U>)m_eventDictionary[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        public void RemoveEventListener<T, U, V>(MessageEventType eventType, Callback<T, U, V> handler)
        {
            OnListenerRemoving(eventType, handler);
            m_eventDictionary[eventType] = (Callback<T, U, V>)m_eventDictionary[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        #endregion RemoveEventListener

        #region OnListenerAdding OnListenerRemoving

        private void OnListenerAdding(MessageEventType eventType, Delegate listenerBeingAdded)
        {
            if (!m_eventDictionary.ContainsKey(eventType))
            {
                m_eventDictionary.Add(eventType, null);
            }

            Delegate d = m_eventDictionary[eventType];

            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new Exception(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        private void OnListenerRemoving(MessageEventType eventType, Delegate listenerBeingRemoved)
        {
            if (m_eventDictionary.ContainsKey(eventType))
            {
                Delegate d = m_eventDictionary[eventType];

                if (d == null)
                {
                    throw new Exception(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
                }
                else if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new Exception(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
            else
            {
                throw new Exception(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
            }
        }

        private void OnListenerRemoved(MessageEventType eventType)
        {
            if (m_eventDictionary[eventType] == null)
            {
                m_eventDictionary.Remove(eventType);
            }
        }

        #endregion OnListenerAdding OnListenerRemoving

        #region BroadCastEventMsg

        public void BroadCastEventMsg(MessageEventType eventType)
        {
            Delegate d;
            if (m_eventDictionary.TryGetValue(eventType, out d))
            {
                Callback callback = d as Callback;

                if (callback != null)
                {
                    callback();
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        public void BroadCastEventMsg<T>(MessageEventType eventType, T arg1)
        {
            Delegate d;
            if (m_eventDictionary.TryGetValue(eventType, out d))
            {
                Callback<T> callback = d as Callback<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        public void BroadCastEventMsg<T, U>(MessageEventType eventType, T arg1, U arg2)
        {
            Delegate d;
            if (m_eventDictionary.TryGetValue(eventType, out d))
            {
                Callback<T, U> callback = d as Callback<T, U>;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        public void BroadCastEventMsg<T, U, V>(MessageEventType eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (m_eventDictionary.TryGetValue(eventType, out d))
            {
                Callback<T, U, V> callback = d as Callback<T, U, V>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        #endregion BroadCastEventMsg

        #region CheckEventListener

        public bool CheckEventListener(MessageEventType eventType, Callback handler)
        {
            if (m_eventDictionary.ContainsKey(eventType))
            {
                Delegate d = m_eventDictionary[eventType];

                if (d == null)
                {
                    return false;
                }
                else if (d.GetType() != handler.GetType())
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        //Single parameter
        public bool CheckEventListener<T>(MessageEventType eventType, Callback<T> handler)
        {
            if (m_eventDictionary.ContainsKey(eventType))
            {
                Delegate d = m_eventDictionary[eventType];

                if (d == null)
                {
                    return false;
                }
                else if (d.GetType() != handler.GetType())
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        //Two parameters
        public bool CheckEventListener<T, U>(MessageEventType eventType, Callback<T, U> handler)
        {
            if (m_eventDictionary.ContainsKey(eventType))
            {
                Delegate d = m_eventDictionary[eventType];

                if (d == null)
                {
                    return false;
                }
                else if (d.GetType() != handler.GetType())
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        //Three parameters
        public bool CheckEventListener<T, U, V>(MessageEventType eventType, Callback<T, U, V> handler)
        {
            if (m_eventDictionary.ContainsKey(eventType))
            {
                Delegate d = m_eventDictionary[eventType];

                if (d == null)
                {
                    return false;
                }
                else if (d.GetType() != handler.GetType())
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public Exception CreateBroadcastSignatureException(MessageEventType eventType)
        {
            return new System.Exception(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
        }

        #endregion CheckEventListener

        public void Cleanup()
        {
            m_eventDictionary.Clear();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        public void Shutdown()
        {

        }
    }
}