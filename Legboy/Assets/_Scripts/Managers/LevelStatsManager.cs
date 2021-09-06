using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatsManager : MonoBehaviour
{
    private float curDiamonds, curTablets, curDeaths, curTime, accumulatedTime;

    public static LevelStatsManager instance;
    private void Awake()
    {
        #region Singleton

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        #endregion
    }

    public void StartRecording()
    {
        if (!ScenesManager.instance.isLevel) return;
        StartTimer();
        GameStateManager.instance.onPause += PauseTimer;
        GameStateManager.instance.onUnpause += ResumeTimer;
    }

    public void IncrDiamonds()
    {
        curDiamonds++;
    }

    public void IncrTablets()
    {
        curTablets++;
    }

    public void IncrDeaths()
    {
        curDeaths++;
    }

    public void StartTimer()
    {
        curTime = Time.realtimeSinceStartup;
        accumulatedTime = 0f;
    }

    public void StopTimer()
    {
        accumulatedTime += Time.realtimeSinceStartup - curTime;
        GameStateManager.instance.onPause -= PauseTimer;
        GameStateManager.instance.onUnpause -= ResumeTimer;
    }

    public void PauseTimer()
    {
        accumulatedTime += Time.realtimeSinceStartup - curTime;
    }

    public void ResumeTimer()
    {
        curTime = Time.realtimeSinceStartup;
    }

    public string getFormatedAccumulatedTime()
    {
        float hours = (accumulatedTime / 3600);
        float minutes = ((accumulatedTime / 60) % 60);
        float seconds = (accumulatedTime % 60);
        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }
}
