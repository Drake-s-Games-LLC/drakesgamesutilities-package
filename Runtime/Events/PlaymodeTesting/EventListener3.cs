using UnityEngine;

namespace DrakesGames.Events.PlaymodeTesting
{
    [EventListenerClass]
    public class EventListener3 : MonobehaviourEventListenerBaseClass
    {
        // Responds to test event by moving the gameobject to the right
        [GlobalEventListenerMethod]
        private void ListenerMethodExample(TestEvent eventInfo)
        {
            gameObject.transform.position += Vector3.right;
        }
    }
}