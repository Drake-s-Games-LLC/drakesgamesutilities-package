using System;
using UnityEngine;

namespace DrakesGames.Events.PlaymodeTesting
{
    // Registration manually
    [EventListenerClass]
    public class EventListener2 : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Instance.RegisterMyListeners(this);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnRegisterMyListeners(this);
        }

        [GlobalEventListenerMethod]
        private void ListenerMethodExample(TestEvent eventInfo)
        {
            Debug.Log("Event Listener Called by: " + eventInfo.ToString());
        }
        
        [GlobalEventListenerMethod]
        private void ListenerMethodExample2(TestEventWithGameObject eventInfo)
        {
            Debug.Log("Event Listener Called by: " + eventInfo.ToString());
            Debug.Log("Gameobject Passed: " + eventInfo.gameObjectEventInfo.name);
        }
    }
}