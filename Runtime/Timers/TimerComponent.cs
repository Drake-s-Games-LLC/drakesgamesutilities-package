using System.Collections;
using UnityEngine;

/// <summary>
/// Monobehaviour responsible for ticking a timer
/// </summary>
public class TimerComponent : MonoBehaviour
{
    private Timer timer;

    public void StartTickingTimer(Timer timer)
    {
        StopAllCoroutines();
        this.timer = timer;
        StartCoroutine(TickTimer());
    }

    public void DestroyTimer()
    {
        StopAllCoroutines();
        Destroy(this);
    }

    IEnumerator TickTimer()
    {
        while (timer.IsRunning)
        {
            yield return null;
            timer.TimeElapsed += Time.deltaTime;
        }
    }
}
