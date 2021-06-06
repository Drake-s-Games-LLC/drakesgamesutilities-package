using UnityEngine;

namespace DrakesGames.Events.PlaymodeTesting
{
    /// <summary>
    /// Simple class to test / showcase event firing
    /// </summary>
    public class EventInvoker : MonoBehaviour
    {
        public void FireEventType1()
        {
            Debug.Log("Firing Test Event: TestEventWithFields");
            TestEventWithFields eventInfo = new TestEventWithFields(1, 0.95673f);
            EventManager.Instance.FireGlobalEvent(eventInfo);
        }
        
        public void FireEventType2()
        {
            Debug.Log("Firing Test Event: TestEventWithGameObject");
            TestEventWithGameObject eventInfo = new TestEventWithGameObject(this.gameObject);
            EventManager.Instance.FireGlobalEvent(eventInfo);
        }
        
        public void FireEventType3()
        {
            Debug.Log("Firing Test Event: TestEvent");
            TestEvent eventInfo = new TestEvent();
            EventManager.Instance.FireGlobalEvent(eventInfo);
        }
    }
}