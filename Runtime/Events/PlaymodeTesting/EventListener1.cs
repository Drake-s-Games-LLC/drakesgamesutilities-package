using UnityEngine;

namespace DrakesGames.Events.PlaymodeTesting
{
    // Base class automatically handles registration
    [EventListenerClass]
    public class EventListener1 : MonobehaviourEventListenerBaseClass
    {
        [GlobalEventListenerMethod]
        private void ListenerMethodExample(TestEvent eventInfo)
        {
            Debug.Log("Event Listener Called by: " + eventInfo.ToString());
        }
        
        [GlobalEventListenerMethod]
        private void ListenerMethodExample2(TestEventWithFields eventInfo)
        {
            Debug.Log("Event Listener Called by: " + eventInfo.ToString());
            Debug.Log("Field Values: " + eventInfo.intEventInfo + ", " + eventInfo.floatEventInfo);
        }
    }
    
}