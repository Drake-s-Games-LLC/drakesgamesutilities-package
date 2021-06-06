using System.Collections;
using UnityEngine;

namespace DrakesGames.Events
{
    /// <summary>
    /// Inheritors of this base class will automatically register and unregister listener methods when tagged by the proper attribute "EventListenerMethod".
    /// The class calls EventManager.Register and .Unregister in OnEnable and OnDisable respectively.
    /// Note that if you want to use those callbacks, you will need to override
    /// </summary>
    [EventListenerClass]
    public class MonobehaviourEventListenerBaseClass : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            if (EventManager.Instance == null)
            {
                Debug.LogWarning("Event Manager Not Found!  Attempting delayed registration");
                StartCoroutine(TryRegister());
                return;
            }

            EventManager.Instance.RegisterMyListeners(this);
        }

        protected virtual void OnDisable()
        {
            if (EventManager.Instance == null) return;
            EventManager.Instance.UnRegisterMyListeners(this);
        }

        private IEnumerator TryRegister()
        {
            while (EventManager.Instance == null) yield return null;
            EventManager.Instance.RegisterMyListeners(this);
        }
    }
}