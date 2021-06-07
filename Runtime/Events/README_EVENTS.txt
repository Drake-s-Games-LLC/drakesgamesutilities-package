*** GOAL ***

This Event Management system (an implementation of the observer pattern) was designed to...
1. Make it easy to create events with payloads that are flexible / can be easily refactored
2. Simplify the process of registering / un registering listener methods
3. Integrate with UnityEvents such that users can register listeners via Unity's inspector

*** High Level Description ***

The "Event Manager" class is the core of the implementation.  It uses reflection to get a list of potential listener Types and their methods at run time via attributes.  By doing that, a listener class is able to register all of its desired methods by tagging them with an attribute and calling "RegisterMyListeners" on the Event Manager, which will automatically determine which delegates to subscribe to based on the attribute tags.  

The payload for any delegates to be used in this system must inherit from "EventInfoBase" which is essentially just a container class for any information you might want to pass.  For example, HealthEventInfo could contain a float field for damage done.

NOTE:  The scripts around unity events / editor configuration should be ignored for now.  All that stuff 
I am still cleaning up.  

*** Local vs. Global Event Scope ***

The Event Manager class is a monobehaviour meant to be attached to a persistent game object, ideally created once in a boot scene and persisted from there on.  Any events fired using this manager will call all listeners who have registered to it.

The Local Event Manager class is similar in terms of the structure of the payload and use of reflection to ease registration, but it is meant to be a local messaging system.  For example, communicating across components on a single entity like a character.  As such, it is not a singleton.

*** The Attributes ***

EventListenerClass:  Must be applied to any class that expects to register via the event listener.  The event manager uses this to filter down which types to look at for listener delegates.  

GlobalEventListenerMethod: Must be applied to any method within an EventListenerClass which you want to register as a listener in the Event Manager.  When you call EventManager.Instance.RegisterMyListeners(this), the event manager uses this attribute to determine all of the methods which it should subscribe as listeners.  Note that this implies the method you tag this with must have a signiture whose sole parameter inherits from EventInfoBase

LocalEventListenerMethod:  Same as above, but for the local event manager

*** Step By Step Setup ***

1. Add a gameobject to a persistent scene, add the EventManager class to it
2. When you want to define an event to be invoked + listened to...
    a. Create a new class that inherits from EventInfoBase.  Eg. MyEventInfo
    b. Add [EventListenerClass] to the class that wants to listen for (respond) to those events
    c. In that same class, add [GlobalEventListenerMethod] to the method that you want to be called when the event is fired
    d. Ensure that the listener class calls EventManager.Instance.RegisterMyListeners in OnEnable and .UnRegister my listeners in OnDisable.  (This only needs to be done once, regardless of how many listener methods there are)
    e. Fire the event from wherever makes sense in your project. To do so, you need to create the relevant payload (in this case, MyEventInfo eventInfo = new MyEventInfo()) and then fire the event using EventManager.Instance.FireGlobalEvent(eventInfo)

Note, the event manager will automatically know which "event" is fired + which listeners to call by the type of MyEventInfo.


