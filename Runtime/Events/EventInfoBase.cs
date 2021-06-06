using DrakesGames.Factory;

namespace DrakesGames.Events
{
    /// <summary>
    ///     Need a new one of these for each new event type.  Can pass as much or as little info as you want.
    ///     The event info class that you create is the definition of an event
    /// </summary>
    public abstract class EventInfoBase : FactoryObjectBase
    {
        public string EventDescription = "Default";
    }
}