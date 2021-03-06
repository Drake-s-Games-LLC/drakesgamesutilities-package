using System;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private readonly List<Action> onFinished;
    private float timeElapsed;
    private readonly TimerMode timerMode;

    private TimerComponent timerComponent = null;

    #region Constructors
    /// <summary>
    ///     Creates a new stopwatch timer
    /// </summary>
    /// <param name="timerMode"></param>
    public Timer(TimerMode timerMode)
    {
        this.timerMode = timerMode;
    }

    /// <summary>
    ///     Creates a new countdown timer
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="onTimerFinished"></param>
    public Timer(float duration, Action[] onTimerFinished = null)
    {
        timerMode = TimerMode.Countdown;
        Duration = duration;
        onFinished = new List<Action>();

        if (onTimerFinished != null)
            for (var i = 0; i < onTimerFinished.Length; i++)
                onFinished.Add(onTimerFinished[i]);
    }
    #endregion
    
    #region Properties
    public float TimeElapsed
    {
        get => timeElapsed;
        set
        {
            timeElapsed = value;
            if (timerMode == TimerMode.Countdown && timeElapsed > Duration) TimerFinished();
        }
    }

    public bool IsRunning { get; private set; }

    public bool HasStarted { get; private set; }

    public bool HasFinished { get; private set; }

    public float Duration { get; }
    #endregion

    #region Public Methods
    
    /// <summary>
    ///     Stops and resets the timer
    /// </summary>
    public void ResetTimer()
    {
        timeElapsed = 0;
        IsRunning = false;
        HasStarted = false;
        HasFinished = false;
    }

    /// <summary>
    ///     Starts or resumes a timer
    /// </summary>
    public void StartTimer(GameObject worldContextGameObject)
    {
        IsRunning = true;
        HasStarted = true;
        if (timerComponent == null)
        {
            timerComponent = worldContextGameObject.AddComponent<TimerComponent>();
        }
        timerComponent.StartTickingTimer(this);
    }

    /// <summary>
    ///     Pauses the timer
    /// </summary>
    public void PauseTimer()
    {
        IsRunning = false;
    }

    /// <summary>
    ///     Calls the "onFinished" listeners and stops the timer
    /// </summary>
    public void TimerFinished()
    {
        if (timerMode != TimerMode.Countdown)
        {
            Debug.Log("Cannot finish a timer of this type");
            return;
        }

        PauseTimer();
        foreach (var a in onFinished) a.Invoke();
    }

    /// <summary>
    ///     Add a listener to be called when the timer is finished
    /// </summary>
    /// <param name="listener"></param>
    public void AddCountdownFinishedListener(Action listener)
    {
        if (timerMode != TimerMode.Countdown)
        {
            Debug.Log("Timers of this type cannot be finished and thus have no listeners");
            return;
        }

        if (onFinished != null) onFinished.Add(listener);
    }

    public void ClearCountdownFinishedListeners()
    {
        if (onFinished == null) return;
        onFinished.Clear();
    }

    public void DeleteTimer()
    {
        if (timerComponent != null)
        {
            timerComponent.DestroyTimer();
        }
    }

    #endregion
}


