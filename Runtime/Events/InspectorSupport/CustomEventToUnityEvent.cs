using System;

namespace Events.InspectorSupport
{
    [Serializable]
    public struct CustomEventToUnityEvent
    {
        [EventType] public string eventType;
        public UnityEventEventInfo Listeners;
    }
}