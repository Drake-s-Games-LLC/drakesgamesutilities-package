using System;

namespace DrakesGames.Events
{

    /// <summary>
    /// This attribute identifies a class as having listener methods to the Event Manager.  By tagging
    /// a class with this attribute you tell the Event Manager to inspect the class (using reflection) for
    /// listener methods.  Those listener methods can then be quickly / easily be registered or unregistered by instances
    /// of the class when they call EventManager.Instance.Register/UnregisterMyListeners
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventListenerClass : Attribute
    {
    }
    
    /// <summary>
    /// Identifies a method within a class as a listener.  A class MUST be tagged with the "EventListenerClass"
    /// attribute for this to work as that attribute instructs the Event Manager to check for listener methods
    /// during initialization
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GlobalEventListenerMethod : Attribute
    {
    }

    /// <summary>
    /// Same as GlobalEventListenerMethod attribute, but for use with local events via the "LocalEventManager" class
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LocalEventListenerMethod : Attribute
    {
    }
}