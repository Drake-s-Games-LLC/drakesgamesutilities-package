using System;
using System.Collections;
using System.Collections.Generic;
using DrakesGames.Events.InspectorSupport;
using DrakesGames.Factory;
using UnityEngine;

namespace DrakesGames.Events
{
    /// <summary>
    ///     Local event manager / messaging system.  Functions very similarly to the "EventManager" singleton class,
    /// but can be instanced / referenced locally instead of globally via a singleton.  Common use case is to use this to
    /// message across multiple components within one game "entity" like a character.  
    /// </summary>
    public class LocalEventManager : MonoBehaviour
    {
        private Dictionary<Type, List<LocalEventListener>> eventListeners;
        private Dictionary<object, Dictionary<Type, List<LocalEventListener>>> instanceEventListeners;

        private Dictionary<object, Dictionary<Type, List<UnityEventEventInfo>>> unityEventListeners;

        private void Awake()
        {
            if (instanceEventListeners == null)
                instanceEventListeners = new Dictionary<object, Dictionary<Type, List<LocalEventListener>>>();
        }

        public void RegisterLocalListener<T>(Action<T> listener, object callingObject) where T : EventInfoBase
        {
            var eventType = typeof(T);

            if (instanceEventListeners == null)
                instanceEventListeners = new Dictionary<object, Dictionary<Type, List<LocalEventListener>>>();

            if (!instanceEventListeners.ContainsKey(callingObject))
                instanceEventListeners.Add(callingObject, new Dictionary<Type, List<LocalEventListener>>());

            if (instanceEventListeners[callingObject].ContainsKey(eventType) == false ||
                instanceEventListeners[callingObject][eventType] == null)
                instanceEventListeners[callingObject][eventType] = new List<LocalEventListener>();

            // Wrap a type converstion around the event listener
            LocalEventListener wrapper = ei => { listener((T) ei); };

            instanceEventListeners[callingObject][eventType].Add(wrapper);
        }

        public void UnregisterLocalListener(object callingObject)
        {
            if (instanceEventListeners != null &&
                instanceEventListeners.ContainsKey(callingObject) &&
                instanceEventListeners[callingObject] != null)
                instanceEventListeners[callingObject].Clear();

            if (unityEventListeners != null &&
                unityEventListeners.ContainsKey(callingObject) &&
                unityEventListeners[callingObject] != null)
                unityEventListeners[callingObject].Clear();
        }

        public void RegisterByString(UnityEventEventInfo unityEvent, string eventTypeString, object callingObject)
        {
            var parameterType = GenericFactory<EventInfoBase>.GetFactoryObjectType(eventTypeString);


            if (unityEventListeners == null)
                unityEventListeners = new Dictionary<object, Dictionary<Type, List<UnityEventEventInfo>>>();

            if (!unityEventListeners.ContainsKey(callingObject))
                unityEventListeners[callingObject] = new Dictionary<Type, List<UnityEventEventInfo>>();

            if (!unityEventListeners[callingObject].ContainsKey(parameterType))
                unityEventListeners[callingObject].Add(parameterType, new List<UnityEventEventInfo>());

            unityEventListeners[callingObject][parameterType].Add(unityEvent);
        }

        /// <summary>
        ///     Fires an event of type EventInfoParameter ONLY to the components on this game object
        ///     who have subscribed
        /// </summary>
        /// <param name="eventInfo"></param>
        public void FireLocalEvent(EventInfoBase eventInfo)
        {
            if (!enabled) return;
            StartCoroutine(Fire(eventInfo));
            StartCoroutine(FireUnityEvents(eventInfo));
        }

        private IEnumerator Fire(EventInfoBase eventInfo)
        {
            var trueEventInfoClass = eventInfo.GetType();

            if (instanceEventListeners == null) yield break;

            foreach (var eventListeners in instanceEventListeners.Values)
            {
                if (!eventListeners.ContainsKey(trueEventInfoClass) || eventListeners[trueEventInfoClass] == null)
                    // No one is listening, we are done.
                    continue;

                foreach (var el in eventListeners[trueEventInfoClass]) el(eventInfo);
            }

            yield return null;
        }

        private IEnumerator FireUnityEvents(EventInfoBase eventInfo)
        {
            var trueEventInfoClass = eventInfo.GetType();

            if (unityEventListeners == null) yield break;

            foreach (var eventListeners in unityEventListeners.Values)
            {
                if (!eventListeners.ContainsKey(trueEventInfoClass) || eventListeners[trueEventInfoClass] == null)
                    // No one is listening, we are done.
                    continue;

                foreach (var el in eventListeners[trueEventInfoClass]) el.Invoke(eventInfo);
            }

            yield return null;
        }

        private delegate void LocalEventListener(EventInfoBase eventInfo);
    }
}