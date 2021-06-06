using UnityEngine;

namespace DrakesGames.Events.PlaymodeTesting
{
    /// <summary>
    /// Example events to be used in testing.  Note that each carries different data.  This can be defined
    /// as generically as you want / can hold as many fields as you want.
    /// </summary>
    
    
    // Example with two info fields.  Shows how you can define them as defaults or via constructor.
    public class TestEventWithFields : EventInfoBase
    {
        public int intEventInfo = 1;
        public float floatEventInfo = 0.567f;

        public TestEventWithFields()
        {
        }

        public TestEventWithFields(int intNumber, float floatNumber)
        {
            this.intEventInfo = intNumber;
            this.floatEventInfo = floatNumber;
        }
    }
    
    public class TestEventWithGameObject : EventInfoBase
    {
        public GameObject gameObjectEventInfo;

        public TestEventWithGameObject(GameObject go)
        {
            this.gameObjectEventInfo = go;
        }
    }
    
    // Very basic example that passes no data. Similar to System.Action
    public class TestEvent : EventInfoBase
    {
        // No info example
    }
}