using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DrakesGames.Events.InspectorSupport;
using DrakesGames.Factory;
using UnityEngine;

namespace DrakesGames.Events
{
    public class EventManager : MonoBehaviour
    {
        #region Fields and Properties
        
        private static EventManager _instance;
        private Dictionary<Type, List<Tuple<MethodInfo, object>>> globalEventListeners;

        private bool isSetUp;

        private Dictionary<Type, List<MethodInfo>> typeToMethods;
        private Dictionary<Type, List<UnityEventEventInfo>> unityEventListeners;
        private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        // Note that any attempt to access the event manager before init will cause a null ref exception.  You must ensure
        // the event manager is initialized before your listeners try to register themselves. Try loading initializing 
        // the EM in a boot scene and/or setting the script execution order to execute before your other scripts
        public static EventManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<EventManager>();
                if (_instance == null)
                    Debug.LogWarning("Event Manager has not yet been initialized.  You need to ensure it is fully set up" +
                                     "before accessing");
                
                return _instance;
            }
        }
        
        #endregion
        private IEnumerator WaitForFinishLoading()
        {
            yield return waitForEndOfFrame;
            _instance = FindObjectOfType<EventManager>();
        }

        #region Public Methods
        /// <summary>
        ///     Registers all of the calling class' methods who have been tagged with the proper EventListener attributes as
        ///     listeners
        ///     to the events whose info is taken in as the method's parameter
        /// </summary>
        /// <param name="callingObject"></param>
        public void RegisterMyListeners(object callingObject)
        {
            if (!isSetUp) ReflectListenerClasses();

            if (globalEventListeners == null)
                globalEventListeners = new Dictionary<Type, List<Tuple<MethodInfo, object>>>();

            if (!typeToMethods.ContainsKey(callingObject.GetType()))
            {
                Debug.Log("NO Methods to register as listeners!:" + callingObject.GetType());
                return;
            }

            foreach (var mi in typeToMethods[callingObject.GetType()])
            {
                var parameterType = mi.GetParameters()[0].ParameterType;

                if (!globalEventListeners.ContainsKey(parameterType) || globalEventListeners[parameterType] == null)
                    globalEventListeners[parameterType] = new List<Tuple<MethodInfo, object>>();

                globalEventListeners[parameterType].Add(new Tuple<MethodInfo, object>(mi, callingObject));
            }
        }

        /// <summary>
        ///     Unregisters all of the calling class' methods who have been tagged with the proper EventListener attributes
        /// </summary>
        /// <param name="callingObject"></param>
        public void UnRegisterMyListeners(object callingObject)
        {
            if (callingObject == null) return;

            if (globalEventListeners == null || !typeToMethods.ContainsKey(callingObject.GetType()))
                //Debug.Log("Trying to remove listener that isnt registered");
                return;

            foreach (var mi in typeToMethods[callingObject.GetType()])
            {
                var parameterType = mi.GetParameters()[0].ParameterType;

                if (!globalEventListeners.ContainsKey(parameterType) || globalEventListeners[parameterType] == null)
                    //Debug.Log("Trying to remove listener that isnt registered");
                    return;

                globalEventListeners[parameterType].RemoveAll(item => item.Item2 == callingObject);
            }
        }

        public void RegisterThroughUnityEvent(UnityEventEventInfo unityEvent, string eventTypeString)
        {
            var parameterType = GenericFactory<EventInfoBase>.GetFactoryObjectType(eventTypeString);

            if (unityEventListeners == null) unityEventListeners = new Dictionary<Type, List<UnityEventEventInfo>>();

            if (!unityEventListeners.ContainsKey(parameterType) || unityEventListeners[parameterType] == null)
                unityEventListeners[parameterType] = new List<UnityEventEventInfo>();

            unityEventListeners[parameterType].Add(unityEvent);
        }

        public void UnregisterThroughUnityEvent(UnityEventEventInfo unityEvent, string eventTypeString)
        {
            var parameterType = GenericFactory<EventInfoBase>.GetFactoryObjectType(eventTypeString);
            if (!unityEventListeners.ContainsKey(parameterType) || unityEventListeners[parameterType] == null) return;
            unityEventListeners[parameterType].Remove(unityEvent);
        }

        public void FireGlobalEvent(EventInfoBase eventInfo)
        {
            var trueEventInfoClass = eventInfo.GetType();

            if ((globalEventListeners == null || !globalEventListeners.ContainsKey(trueEventInfoClass)) &&
                (unityEventListeners == null || !unityEventListeners.ContainsKey(trueEventInfoClass)))
                // No one is listening, we are done.
                return;

            StartCoroutine(FireEventRoutine(eventInfo, trueEventInfoClass));
        }

        #endregion
        
        private IEnumerator FireEventRoutine(EventInfoBase eventInfo, Type trueEventInfoClass)
        {
            var param = new object[1] {eventInfo};

            if (globalEventListeners.ContainsKey(trueEventInfoClass))
                foreach (var tuple in globalEventListeners[trueEventInfoClass].ToList())
                    tuple.Item1.Invoke(tuple.Item2, param);

            if (unityEventListeners == null) yield break;
            if (!unityEventListeners.ContainsKey(trueEventInfoClass)) yield break;

            foreach (var unityEventInfo in unityEventListeners[trueEventInfoClass].ToList()) unityEventInfo.Invoke(eventInfo);

            yield return null;
        }

        #region Manager Initialization

        private void Awake()
        {
            ReflectListenerClasses();
        }

        private void OnEnable()
        {
            _instance = this;
        }
        
        private void ReflectListenerClasses()
        {
            typeToMethods = new Dictionary<Type, List<MethodInfo>>();

            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
                if (t.GetCustomAttribute(typeof(EventListenerClass)) != null)
                    foreach (var mi in t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance |
                                                    BindingFlags.DeclaredOnly | BindingFlags.Public))
                        AddMethodsWithAttributes(t, mi);

            isSetUp = true;
        }

        private void AddMethodsWithAttributes(Type t, MethodInfo mi)
        {
            if (mi.GetCustomAttribute(typeof(GlobalEventListenerMethod)) != null)
            {
                if (!typeToMethods.ContainsKey(t)) typeToMethods.Add(t, new List<MethodInfo>());

                typeToMethods[t].Add(mi);
            }
        }

        #endregion

    }
}