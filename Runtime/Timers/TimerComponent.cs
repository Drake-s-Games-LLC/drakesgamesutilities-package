using System.Collections;
using UnityEngine;

// Note to self:  This shold be controlled by the TIMER, not by the user.  The timer is the thing that should be public!!!
public class TimerComponent : MonoBehaviour
{
    private Coroutine timerHandle;
    private Timer.TimerMode timerMode = Timer.TimerMode.Countdown;
    private Timer timer;
    
    private void Awake()
    {
        timer = new Timer(timerMode);
    }

    public void StartTimer()
    {
        timer.StartTimer();
        timerHandle = StartCoroutine(TickTimer());
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
